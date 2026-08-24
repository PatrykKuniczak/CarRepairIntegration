# CarRepairIntegration

Minimalna aplikacja demonstracyjna w .NET 10 pokazująca integrację danych z dwóch zewnętrznych serwisów samochodowych.

## Struktura

```text
src/
├── Api/            # GraphQL i composition root
├── Application/    # CQRS, pipeline, adapter contracts, reguły aplikacyjne
├── Domain/         # encje i Specification DSL
└── Infrastructure/ # EF Core, Dapper, adaptery i seeder
```

## Workflow

### Query

```text
GraphQL Query
    ↓
CarQueries
    ↓
CarReadStore (Dapper)
    ↓
GetCar DTO
    ↓
GraphQL response
```

Dapper korzysta z tego samego `DbConnection`, który posiada `AppDbContext`.

### CreateCarRepair

```text
GraphQL Mutation
    ↓
CarCommands
    ↓
CreateCarRepairPipeline
    ↓
Receive
    ↓
Adapter (Service_A / Service_B)
    ↓
Normalization (np. kW → km)
    ↓
Specification / RuleSet
    ↓
Prepare repair
    ↓
CarWriteStore (EF Core)
    ↓
Database
```

Pipeline nie zna konkretnych serwisów. `CarAdapterFactory` wybiera adapter na podstawie `source`.

### EditCarRepair

```text
GraphQL Mutation
    ↓
CarCommands
    ↓
CarWriteStore
    ↓
EF Core
```

## Dodanie trzeciego serwisu

Dodaj nową implementację `ICarAdapter` i zarejestruj ją w DI. `CreateCarRepairPipeline`, `CarCommands` i reguły nie wymagają zmian.

## Dodanie nowego zestawu reguł

Dodaj implementację `ICarRuleSet` z nazwą i `Specification<Car>`. Factory znajdzie ją po nazwie.

## Przykładowe requesty (Nitro)

Każdy przykład składa się z sekcji **Request** oraz **Variables** — skopiuj obie do odpowiednich zakładek w Nitro (`http://localhost:5000/graphql`).

Uwaga: obsługiwane jednostki mocy to `kW` i `km`, a reguły walidacji wymagają koloru `black` lub `white` (`ruleSet: "standard"`) albo mocy ≥ 200 km i koloru innego niż `white` (`ruleSet: "sport"`).

### CreateCar — Service_A (payload JSON)

Request:

```graphql
mutation CreateCar(
  $source: String!
  $payload: String!
  $ruleSet: String
  $repair: RepairInputDtoInput!
) {
  createCarRepair(
    input: {
      source: $source
      payload: $payload
      ruleSet: $ruleSet
      repair: $repair
    }
  ) {
    carId
    repairId
  }
}
```

Variables:

```json
{
  "source": "Service_A",
  "payload": "{\"brand\":\"Audi\",\"model\":\"A4\",\"powerKw\":110,\"color\":\"Black\"}",
  "ruleSet": "standard",
  "repair": {
    "description": "Wymiana rozrządu",
    "repairDate": "2026-08-23T14:30:00Z",
    "cost": 1200.50,
    "serviceName": "ASO"
  }
}
```

### CreateCar — Service_B (payload CSV `KLUCZ:WARTOŚĆ;...`)

Request:

```graphql
mutation CreateCar(
  $source: String!
  $payload: String!
  $ruleSet: String
  $repair: RepairInputDtoInput!
) {
  createCarRepair(
    input: {
      source: $source
      payload: $payload
      ruleSet: $ruleSet
      repair: $repair
    }
  ) {
    carId
    repairId
  }
}
```

Variables:

```json
{
  "source": "Service_B",
  "payload": "CAR_BRAND:BMW;CAR_MODEL:X5;ENGINE_POWER:250;ENGINE_POWER_UNIT:kW;COLOR:black",
  "ruleSet": "sport",
  "repair": {
    "description": "Wymiana klocków hamulcowych",
    "repairDate": "2026-08-24T10:00:00Z",
    "cost": 890.00,
    "serviceName": "BMW Serwis"
  }
}
```

### EditCar — edycja naprawy

Jako `id` podaj `repairId` zwrócone przez `createCarRepair`.

Request:

```graphql
mutation EditCar(
  $id: UUID!
  $description: String!
  $repairDate: DateTime
  $cost: Decimal!
  $serviceName: String!
) {
  editCarRepair(
    input: {
      id: $id
      description: $description
      repairDate: $repairDate
      cost: $cost
      serviceName: $serviceName
    }
  ) {
    repairId
  }
}
```

Variables:

```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "description": "Wymiana rozrządu + pompa wody",
  "repairDate": "2026-08-25T09:00:00Z",
  "cost": 1650.00,
  "serviceName": "ASO"
}
```

### Query — lista samochodów

Request:

```graphql
query GetCars($take: Int!) {
  cars(input: { take: $take }) {
    id
    brand
    model
    enginePower
    color
  }
}
```

Variables:

```json
{
  "take": 50
}
```

### Query — pojedynczy samochód po id

Jako `id` podaj `carId` zwrócone przez `createCarRepair` (lub `id` z listy `cars`).

Request:

```graphql
query GetCar($id: UUID!) {
  car(id: $id) {
    id
    brand
    model
    enginePower
    color
  }
}
```

Variables:

```json
{
  "id": "00000000-0000-0000-0000-000000000000"
}
```

## Uruchomienie

```bash
dotnet watch --non-interactive run --project src/Api/Api.csproj
```

Przy starcie aplikacja wykonuje migracje EF Core i seeduje bazę, jeżeli jest pusta.

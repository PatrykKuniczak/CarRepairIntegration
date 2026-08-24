namespace Domain.Entities;

public record CarRepair
{
    public Guid Id { get; init; }
    public Guid CarId { get; init; }
    public string Description { get; set; } = "";
    public DateTime RepairDate { get; set; }
    public decimal Cost { get; set; }
    public string ServiceName { get; set; } = "";
}

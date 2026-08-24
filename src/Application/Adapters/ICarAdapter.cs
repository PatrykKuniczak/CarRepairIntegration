using Application.Pipeline;

namespace Application.Adapters;

// Adapter hides the external format from the rest of the application.
public interface ICarAdapter
{
    bool CanHandle(string source);
    IncomingCarData Adapt(string payload);
}

public sealed class CarAdapterFactory(IEnumerable<ICarAdapter> adapters)
{
    public ICarAdapter Create(string source) =>
        adapters.FirstOrDefault(x => x.CanHandle(source))
        ?? throw new InvalidOperationException($"Unsupported source: {source}");
}

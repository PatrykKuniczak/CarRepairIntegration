using Application.Pipeline;

namespace Application.Adapters;

// Adapter hides the external format from the rest of the application.
public interface ICarAdapter
{
    bool CanHandle(string source);
    bool TryAdapt(string payload, out IncomingCarData? data, out string? error);
}

public sealed class CarAdapterFactory(IEnumerable<ICarAdapter> adapters)
{
    public ICarAdapter? GetAdapter(string source) =>
        adapters.FirstOrDefault(x => x.CanHandle(source));

    public ICarAdapter? Create(string source) => GetAdapter(source);
}

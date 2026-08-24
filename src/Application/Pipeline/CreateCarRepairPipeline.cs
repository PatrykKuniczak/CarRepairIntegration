namespace Application.Pipeline;

// Pipe & Filter: the filters are registered in order in Infrastructure.
// The pipeline itself knows nothing about Service_A, Service_B or rule sets.
public sealed class CreateCarRepairPipeline(IEnumerable<ICreateCarRepairFilter> filters)
{
    public async Task<CreateCarRepairContext> RunAsync(
        CreateCarRepairContext context,
        CancellationToken cancellationToken)
    {
        foreach (var filter in filters)
            await filter.ApplyAsync(context, cancellationToken);

        return context;
    }
}

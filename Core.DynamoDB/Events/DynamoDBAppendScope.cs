using Core.Events;
using Core.DynamoDbEventStore.OptimisticConcurrency;
using Core.Tracing;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DynamoDbEventStore.Events;

public interface IDynamoDBAppendScope: IAppendScope<long>
{
}

public class DynamoDBAppendScope: AppendScope<long>, IDynamoDBAppendScope
{
    public DynamoDBAppendScope(
        Func<long?> getExpectedVersion,
        Action<long> setNextExpectedVersion,
        Func<TraceMetadata?> getEventMetadata
    ): base(getExpectedVersion, setNextExpectedVersion, getEventMetadata)
    {
    }
}

public static class DynamoDBAppendScopeExtensions
{
    public static IServiceCollection AddDynamoDBAppendScope(this IServiceCollection services) =>
        services
            .AddScoped<DynamoDBExpectedStreamVersionProvider, DynamoDBExpectedStreamVersionProvider>()
            .AddScoped<DynamoDBNextStreamVersionProvider, DynamoDBNextStreamVersionProvider>()
            .AddScoped<IDynamoDBAppendScope, DynamoDBAppendScope>(
                sp =>
                {
                    var expectedStreamVersionProvider = sp.GetRequiredService<DynamoDBExpectedStreamVersionProvider>();
                    var nextStreamVersionProvider = sp.GetRequiredService<DynamoDBNextStreamVersionProvider>();
                    var traceMetadataProvider = sp.GetRequiredService<ITraceMetadataProvider>();

                    return new DynamoDBAppendScope(
                        () => expectedStreamVersionProvider.Value,
                        nextStreamVersionProvider.Set,
                        traceMetadataProvider.Get
                    );
                });
}

using Core.Aggregates;
using Core.Exceptions;
using Core.Projections;
using Core.Tracing;

namespace Core.DynamoDbEventStore.Repository;

public static class DynamoDBRepositoryExtensions
{
    public static async Task<T> Get<T>(
        this IDynamoDBRepository<T> repository,
        Guid id,
        CancellationToken cancellationToken = default
    ) where T : class, IAggregate
    {
        var entity = await repository.Find(id, cancellationToken);

        return entity ?? throw AggregateNotFoundException.For<T>(id);
    }

    public static async Task<long> GetAndUpdate<T>(
        this IDynamoDBRepository<T> repository,
        Guid id,
        Action<T> action,
        long? expectedVersion = null,
        TraceMetadata? traceMetadata = null,
        CancellationToken cancellationToken = default
    ) where T : class, IAggregate
    {
        var entity = await repository.Get(id, cancellationToken);

        action(entity);

        return await repository.Update(entity, expectedVersion, traceMetadata, cancellationToken);
    }
}

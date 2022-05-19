
using Core.Aggregates;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Models;
using Core.Events;
using Core.Tracing;
using EfficientDynamoDb;
using EfficientDynamoDb.Operations.TransactWriteItems.Builders;
using Newtonsoft.Json;

namespace Core.DynamoDbEventStore.Repository;

public interface IDynamoDBRepository<T> where T : class, IAggregate
{
    Task<T?> Find(Guid id, CancellationToken cancellationToken);

    Task<long> Add(
        T aggregate,
        TraceMetadata? traceMetadata = null,
        CancellationToken ct = default
    );

    Task<long> Update(
        T aggregate,
        long? expectedRevision = null,
        TraceMetadata? traceMetadata = null,
        CancellationToken ct = default
    );

    Task<long> Delete(
        T aggregate,
        long? expectedRevision = null,
        TraceMetadata? traceMetadata = null,
        CancellationToken ct = default
    );
}

public class DynamoDBRepository<T>: IDynamoDBRepository<T> where T : class, IAggregate
{
    private readonly DynamoDbContext eventStore;
    public DynamoDBRepository(DynamoDbContext eventStore)
    {
        this.eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
    }

    public async Task<long> Add(T aggregate, TraceMetadata? traceMetadata = null, CancellationToken ct = default)
    {
        var events = aggregate.DequeueUncommittedEvents();

        //Create New Stream
        var stream = new StreamRecord {
            Id = aggregate.Id,
            Version = aggregate.Version,
            StreamType = "aggregate",
            CreatedAt = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            SnapshotVersion = 0
        };

        //Save Stream Events
        var eventsSerialized = events.Select(@event => {
            return new EventRecord{
                Id = Guid.NewGuid(),
                EventType = EventTypeMapper.ToName(@event.GetType()),
                Data = JsonConvert.SerializeObject(@event),
                Metadata = @traceMetadata != null ? JsonConvert.SerializeObject(@traceMetadata) : String.Empty,
                CreatedAt = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                StreamId = aggregate.Id,
                Version = (ulong)aggregate.Version
            };
        }).ToList();

        var toWrite = new List<ITransactWriteItemBuilder>();
        toWrite.Add(Transact.PutItem<StreamRecord>(stream)
            .WithCondition(Condition<StreamRecord>.On(x => x.Id).NotExists()));
        toWrite.AddRange(eventsSerialized.Select( e => Transact.PutItem<EventRecord>(e)));

        await eventStore.TransactWrite()
                .WithItems(toWrite)
                .ExecuteAsync();

        return events.Length;
    }

    public Task<long> Delete(T aggregate, long? expectedRevision = null, TraceMetadata? traceMetadata = null, CancellationToken ct = default) =>
    Update(aggregate, expectedRevision, traceMetadata, ct);

    public Task<T?> Find(Guid id, CancellationToken cancellationToken) =>
    eventStore.AggregateStream<T>(
        id,
        cancellationToken
    );

    public async Task<long> Update(T aggregate, long? expectedRevision = null, TraceMetadata? traceMetadata = null, CancellationToken ct = default)
    {
        var events = aggregate.DequeueUncommittedEvents();

        var nextVersion = expectedRevision.HasValue ?
            expectedRevision.Value + events.Length
            : aggregate.Version;

        //Save Stream Events
        var eventsSerialized = events.Select(@event => {
            var rec = new EventRecord{
                Id = Guid.NewGuid(),
                EventType = EventTypeMapper.ToName(@event.GetType()),
                Data = JsonConvert.SerializeObject(@event),
                CreatedAt = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                StreamId = aggregate.Id,
                Version = (ulong)nextVersion
            };
            return rec;
        }).ToList();

        var toWrite = new List<ITransactWriteItemBuilder>();
        //Ensure we have the expected version of the stream
        //And update with nextVersion
        //NOTE, in dynamodb you cannot update the PK or SK once written
        toWrite.Add(Transact.UpdateItem<StreamRecord>()
            .WithPrimaryKey(aggregate.Id)
            .On(x => x.Version).Assign(nextVersion)
            .WithCondition(Condition<StreamRecord>.On(x => x.Version).EqualTo(nextVersion - events.Length)) );
        
        //Add our dequeued events and ensure their version does not already exist
        toWrite.AddRange(eventsSerialized.Select( e => Transact.PutItem<EventRecord>(e)
            .WithCondition(Condition<StreamRecord>.On(x => x.Version).NotEqualTo(nextVersion)) ));

        await eventStore.TransactWrite()
                .WithItems(toWrite)
                .ExecuteAsync();

        return nextVersion;
    }
}
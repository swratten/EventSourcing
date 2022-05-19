using Core.Events;
using Core.Projections;
using EfficientDynamoDb;
using EfficientDynamoDb.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DynamoDbEventStore.ExternalProjections;

public class DynamoDBExternalProjection<TEvent, TView>: IEventHandler<EventEnvelope<TEvent>>
    where TView : class, IVersionedProjection
    where TEvent : notnull
{
    private readonly DynamoDbContext session;
    private readonly Func<TEvent, Guid> getId;

    public DynamoDBExternalProjection(
        DynamoDbContext session,
        Func<TEvent, Guid> getId
    )
    {
        this.session = session;
        this.getId = getId;
    }

    public async Task Handle(EventEnvelope<TEvent> eventEnvelope, CancellationToken ct)
    {
        var (@event, eventMetadata) = eventEnvelope;
        var id = getId(@event);
        var exists = true;
        var entity = (TView)Activator.CreateInstance(typeof(TView), true)!;
        try
        {
            var getEntity = await session.GetItemAsync<TView>(id, ct);
            if(getEntity != null)
            {
                entity = getEntity;
            }
            else
            {
                exists = false;
            }
        }
        catch (NullReferenceException)
        {
            exists = false;
            Console.WriteLine("View didnt exist...");
        }
        //I think this can essentially be treated as version in a per stream basis...
        //The entity retrieved is a projection (as current state)
        //And its position (version) should be less than current *stream* version - 1
        var eventLogPosition = eventMetadata.LogPosition;
        var lastProcessedPosition = entity.LastProcessedPosition;

        if (entity.LastProcessedPosition >= eventLogPosition)
            return;

        entity.When(@event);

        entity.LastProcessedPosition = eventLogPosition;
        try
        {
            if(exists)
            {
                var conditionBuilder = Condition.ForEntity<TView>()
                    .On(x => x.LastProcessedPosition).EqualTo(lastProcessedPosition);

                await session.PutItem().WithItem<TView>(entity)
                        .WithCondition(conditionBuilder)
                        .ExecuteAsync(ct);
            }
            else
            {
                var conditionBuilder = Condition.ForEntity<TView>()
                    .On(x => x.LastProcessedPosition).NotExists();

                await session.PutItem().WithItem<TView>(entity)
                        .WithCondition(conditionBuilder)
                        .ExecuteAsync(ct);
            }
        }
        catch(ConditionalCheckFailedException ex)
        {
            Console.WriteLine("Conditional key constraint failed: " + ex.ToString());
        }
    }
}

public class DynamoDBheckpoint
{
    public string Id { get; set; } = default!;

    public ulong? Position { get; set; }

    public DateTime CheckpointedAt { get; set; } = default!;
}

public static class DynamoDBExternalProjectionConfig
{
    public static IServiceCollection Project<TEvent, TView>(this IServiceCollection services,
        Func<TEvent, Guid> getId)
        where TView : class, IVersionedProjection
        where TEvent : notnull
    {
        services
            .AddTransient<IEventHandler<EventEnvelope<TEvent>>>(sp =>
            {
                var session = sp.GetRequiredService<DynamoDbContext>();

                return new DynamoDBExternalProjection<TEvent, TView>(session, getId);
            });

        return services;
    }
}

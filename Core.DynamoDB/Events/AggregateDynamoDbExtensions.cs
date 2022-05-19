
using System.Text.Json;
using Core.Projections;
using Core.DynamoDbEventStore.Models;
using Core.DynamoDbEventStore.Serialization;
using EfficientDynamoDb;
using Core.Aggregates;

namespace Core.DynamoDbEventStore.Events;
public static class AggregateDynamoDbExtensions
{
    public static async Task<T?> AggregateStream<T>(
        this DynamoDbContext eventStore,
        Guid id,
        CancellationToken cancellationToken,
        ulong? fromVersion = null
    ) where T : class, IProjection
    {
        // var queryParams = new QueryRequest{
        //     TableName = "esEvents",
        //     ConsistentRead = true,
        //     KeyConditionExpression = "streamId = :a AND version >= :v"
        // };
        // queryParams.ExpressionAttributeValues.Add(":a", new AttributeValue{ N = id.ToString() });
        // queryParams.ExpressionAttributeValues.Add(":v", new AttributeValue{ N = fromVersion.ToString() });
        // var readResult = await eventStore.QueryAsync(queryParams);

        var conditionBuilder = Condition.ForEntity<EventRecord>();
        var conditions = Joiner.And(
            conditionBuilder.On(x => x.StreamId).EqualTo(id),
            conditionBuilder.On(x => x.Version).GreaterThanOrEqualTo(fromVersion ?? (ulong)0)
        );

        //DynamoDB can only return up to 1 MB of data per response. 
        //If your query contains more, DynamoDB will paginate the response. 
        //In this case, ToListAsync() makes multiple calls until all the data is fetched and put into a single resulting array.
        var readResult = await eventStore.Query<EventRecord>()
            .WithKeyExpression(conditions)
            .WithConsistentRead(true)
            .ToListAsync();


        // TODO: consider adding extension method for the aggregation and deserialisation
        var aggregate = (T)Activator.CreateInstance(typeof(T), true)!;

        foreach (var @event in readResult)
        {
            var eventData = DynamoDBSerializer.Deserialize(@event);
            aggregate.When(eventData!);
        }

        return aggregate;
    }
}
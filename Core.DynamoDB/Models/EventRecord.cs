
using EfficientDynamoDb.Attributes;

namespace Core.DynamoDbEventStore.Models;
[DynamoDbTable("write_events")]
public class EventRecord
{
    [DynamoDbProperty("id")]
    public Guid Id { get; set; }
    [DynamoDbProperty("stream_id", DynamoDbAttributeType.PartitionKey)]
    public Guid StreamId { get; set; }
    [DynamoDbProperty("version", DynamoDbAttributeType.SortKey)]
    public ulong Version { get; set; }
    [DynamoDbProperty("event_type")]
    public string EventType { get; set; } = String.Empty;
    [DynamoDbProperty("created_at")]
    public ulong CreatedAt { get;set; }
    [DynamoDbProperty("data")]
    public string Data { get; set; } = String.Empty;
    [DynamoDbProperty("metadata")]
    public string Metadata { get; set; } = String.Empty;
}
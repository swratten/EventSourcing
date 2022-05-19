
using EfficientDynamoDb.Attributes;

namespace Core.DynamoDbEventStore.Models;
[DynamoDbTable("write_stream")]
public class StreamRecord
{
    [DynamoDbProperty("id", DynamoDbAttributeType.PartitionKey)]
    public Guid Id { get; set; }
    [DynamoDbProperty("version")]
    public long Version { get; set; }
    [DynamoDbProperty("stream_type")]
    public string StreamType { get; set; } = String.Empty;
    [DynamoDbProperty("created_at")]
    public ulong CreatedAt { get;set; }
    [DynamoDbProperty("snapshot")]
    public string Snapshot { get; set; } = String.Empty;
    [DynamoDbProperty("snapshot_version")]
    public uint SnapshotVersion { get; set; }
}
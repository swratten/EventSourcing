using System.Text;
using Core.DynamoDbEventStore.Models;
using Core.Events;
using Core.Serialization.Newtonsoft;
using Core.Tracing;
using Newtonsoft.Json;

namespace Core.DynamoDbEventStore.Serialization;

public static class DynamoDBSerializer
{

    public static T? Deserialize<T>(this EventRecord resolvedEvent) where T : class =>
        Deserialize(resolvedEvent) as T;

    public static object? Deserialize(this EventRecord resolvedEvent)
    {
        // get type
        var eventType = EventTypeMapper.ToType(resolvedEvent.EventType);

        if (eventType == null)
            return null;

        // deserialize event
        return JsonConvert.DeserializeObject(
            resolvedEvent.Data,
            eventType
        )!;
    }

    public static TraceMetadata? DeserializeMetadata(this EventRecord resolvedEvent)
    {
        // get type
        var eventType = EventTypeMapper.ToType(resolvedEvent.EventType);

        if (eventType == null)
            return null;

        // deserialize event
        return JsonConvert.DeserializeObject<TraceMetadata>(
            resolvedEvent.Metadata
        )!;
    }

    // public static EventData ToJsonEventData(this object @event, object? metadata = null) =>
    //     new(
    //         Uuid.NewUuid(),
    //         EventTypeMapper.ToName(@event.GetType()),
    //         Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event, SerializerSettings)),
    //         Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metadata ?? new { }, SerializerSettings))
    //     );
}

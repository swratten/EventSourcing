using Core.Events;
using Core.DynamoDbEventStore.Serialization;
using Core.DynamoDbEventStore.Models;

namespace Core.DynamoDbEventStore.Events;

public static class DynamoDBStreamEventExtensions
{
    public static EventEnvelope? ToStreamEvent(this EventRecord resolvedEvent)
    {
        var eventData = resolvedEvent.Deserialize();
        var eventMetadata = resolvedEvent.DeserializeMetadata();

        if (eventData == null)
            return null;

        var metaData = new EventMetadata(
            resolvedEvent.Id.ToString(),
            resolvedEvent.Version, 
            resolvedEvent.Version,
            eventMetadata
        );
        var type = typeof(EventEnvelope<>).MakeGenericType(eventData.GetType());
        return (EventEnvelope)Activator.CreateInstance(type, eventData, metaData)!;
    }
}

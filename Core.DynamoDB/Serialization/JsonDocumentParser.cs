
using System.Buffers;
using System.Text.Json;

namespace Core.DynamoDbEventStore.Serialization;
public static class JsonDocumentParser
{
    public static JsonElement Parse(string input)
    {
        try
        {
            using var doc = JsonDocument.Parse(input);
            return doc.RootElement.Clone();
        }
        catch (JsonException jsonEx)
        {
            throw jsonEx;
        }            
    }

    public static T ToObject<T>(this JsonElement element, JsonSerializerOptions? options = null)
    {
       var bufferWriter = new ArrayBufferWriter<byte>();
       using (var writer = new Utf8JsonWriter(bufferWriter))
            element.WriteTo(writer);
       return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options)!;
    }
}
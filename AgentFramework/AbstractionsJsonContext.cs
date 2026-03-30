using System.Text.Json.Serialization;

[JsonSerializable(typeof(Microsoft.Extensions.AI.TextContent))]
// Add other relevant types here, especially those derived from AIContent
[JsonSerializable(typeof(Microsoft.Extensions.AI.AIContent))] 
// Add any other types that might be serialized polymorphically
public partial class AbstractionsJsonContext : JsonSerializerContext
{
}

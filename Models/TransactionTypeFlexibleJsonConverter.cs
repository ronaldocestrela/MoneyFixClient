using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoneyFixClient.Models;

/// <summary>
/// Aceita <c>type</c> como texto ("Entrada"/"Saida") ou número (1 = saída, 2 = entrada), como em APIs legadas.
/// </summary>
public sealed class TransactionTypeFlexibleJsonConverter : JsonConverter<string>
{
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString() ?? string.Empty;
            case JsonTokenType.Number:
                if (reader.TryGetInt32(out var n))
                {
                    return n switch
                    {
                        1 => "Saida",
                        2 => "Entrada",
                        _ => string.Empty
                    };
                }

                return string.Empty;
            case JsonTokenType.Null:
                return string.Empty;
            default:
                return string.Empty;
        }
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value ?? string.Empty);
    }
}

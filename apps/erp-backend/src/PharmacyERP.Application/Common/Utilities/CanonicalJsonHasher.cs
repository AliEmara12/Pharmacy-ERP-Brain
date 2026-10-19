using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PharmacyERP.Application.Common.Utilities;

public static class CanonicalJsonHasher
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public static string SerializeToCanonicalJson<T>(T payload)
    {
        if (payload == null) return "null";

        var jsonNode = JsonSerializer.SerializeToNode(payload, Options);
        SortNode(jsonNode);

        return jsonNode?.ToJsonString(Options) ?? "null";
    }

    public static string ComputeSha256Hex<T>(T payload)
    {
        var canonicalJson = SerializeToCanonicalJson(payload);
        var bytes = Encoding.UTF8.GetBytes(canonicalJson);
        var hashBytes = SHA256.HashData(bytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    private static void SortNode(JsonNode? node)
    {
        if (node is JsonObject jsonObject)
        {
            var properties = jsonObject.ToList();
            jsonObject.Clear();

            foreach (var property in properties.OrderBy(p => p.Key, StringComparer.Ordinal))
            {
                SortNode(property.Value);
                jsonObject.Add(property.Key, property.Value);
            }
        }
        else if (node is JsonArray jsonArray)
        {
            foreach (var item in jsonArray)
            {
                SortNode(item);
            }
        }
    }
}
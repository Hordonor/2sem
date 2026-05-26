using System.Text.Json;
using System.Text.Json.Serialization;
using AsyncDataLibrary.Interfaces;

namespace AsyncDataLibrary.Infrastructure;

/// <summary>
/// Реалізація IDataSerializer на базі System.Text.Json.
/// Підтримує: відступи (WriteIndented), серіалізацію enum як рядка,
/// ігнорування null-значень, нечутливість до регістру ключів.
/// </summary>
public class JsonDataSerializer : IDataSerializer
{
    private readonly JsonSerializerOptions _options;

    public JsonDataSerializer()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public string Serialize<T>(T obj) =>
        JsonSerializer.Serialize(obj, _options);

    public T? Deserialize<T>(string json) =>
        JsonSerializer.Deserialize<T>(json, _options);
}

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using IcePanel.Api.Models;
using Microsoft.Kiota.Abstractions.Serialization;

namespace IcePanel.Api;

/// <summary>
/// Helper to transform import JSON where items are objects keyed by id into a LandscapeImportData
/// where items are lists. Input is a JSON string, output a LandscapeImportData instance.
/// </summary>
public static class LandscapeImportMapper
{
    /// <summary>
    /// Converts an import JSON (with object maps) into a LandscapeImportData instance.
    /// </summary>
    /// <param name="json">The input JSON string.</param>
    /// <returns>LandscapeImportData populated from the JSON.</returns>
    public static async Task<LandscapeImportData> FromJson(string json)
    {
        if (json == null) throw new ArgumentNullException(nameof(json));

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            // Convert enum string values like "live" -> ModelObjectStatus.Live (camelCase)
        };
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

        var result = new LandscapeImportData();

        // Helper to deserialize object-of-objects into list<T>
        static async Task<List<T>?> MapObjectOfObjectsToList<T>(JsonElement root, string propertyName, JsonSerializerOptions options) where T : IParsable
        {
            if (!root.TryGetProperty(propertyName, out var prop)) return null;
            if (prop.ValueKind != JsonValueKind.Object) return null;

            var list = new List<T>();
            foreach (var item in prop.EnumerateObject())
            {
                try
                {
                    var raw = item.Value.GetRawText();
                    var obj = await KiotaJsonSerializer.DeserializeAsync<T>(raw);
                    //var obj = JsonSerializer.Deserialize<T>(raw, options);
                    if (obj != null) list.Add(obj);
                }
                catch
                {
                    // ignore individual parse errors and continue with others
                }
            }
            return list;
        }

        // Map known sections
        result.ModelConnections = await MapObjectOfObjectsToList<ModelConnectionImport>(root, "modelConnections", options);
        result.ModelObjects = await MapObjectOfObjectsToList<ModelObjectImport>(root, "modelObjects", options);
        result.TagGroups = await MapObjectOfObjectsToList<TagGroupImport>(root, "tagGroups", options);
        result.Tags = await MapObjectOfObjectsToList<TagImport>(root, "tags", options);

        // Also accept arrays directly if JSON already contains arrays
        static List<T>? MapArrayIfPresent<T>(JsonElement root, string propertyName, JsonSerializerOptions options)
        {
            if (!root.TryGetProperty(propertyName, out var prop)) return null;
            if (prop.ValueKind != JsonValueKind.Array) return null;
            try
            {
                return JsonSerializer.Deserialize<List<T>>(prop.GetRawText(), options);
            }
            catch
            {
                return null;
            }
        }

        // Prefer arrays if provided
        var arr = MapArrayIfPresent<ModelConnectionImport>(root, "modelConnections", options);
        if (arr != null) result.ModelConnections = arr;
        var arrModelObjects = MapArrayIfPresent<ModelObjectImport>(root, "modelObjects", options);
        if (arrModelObjects != null) result.ModelObjects = arrModelObjects;
        var arrTagGroups = MapArrayIfPresent<TagGroupImport>(root, "tagGroups", options);
        if (arrTagGroups != null) result.TagGroups = arrTagGroups;
        var arrTags = MapArrayIfPresent<TagImport>(root, "tags", options);
        if (arrTags != null) result.Tags = arrTags;

        // Optional namespace property
        if (root.TryGetProperty("namespace", out var nsProp) && nsProp.ValueKind == JsonValueKind.String)
        {
            result.Namespace = nsProp.GetString();
        }

        return result;
    }
}

using System.Text.Json;

namespace App.Core.Json;

public class JsonHelper : IJsonHelper
{
    public string? File_Path { get; set; }
    public string? Json_Content { get; set; }

    public JsonHelper(string? file_path = null)
    {
        File_Path = file_path;
    }

    #region Public Methods

    #region Reader

    /// <summary>
    /// Loads the JSON content from the specified file path to the memory.
    /// </summary>
    /// <returns>The loaded JSON content as a string.</returns>
    /// <exception cref="FileNotFoundException"></exception>
    public string LoadJsonContent()
    {
        if (string.IsNullOrWhiteSpace(File_Path) || !File.Exists(File_Path))
            throw new FileNotFoundException("The specified JSON file was not found.", File_Path);

        Json_Content = File.ReadAllText(File_Path);
        return Json_Content;
    }

    /// <summary>
    /// Deserializes the JSON content into an object of type T.
    /// </summary>
    /// <typeparam name="T">Target type to deserialize the JSON content into.</typeparam>
    /// <param name="json_content">The JSON content to deserialize. If null, the content will be loaded from the file.</param>
    /// <returns>An object of type T deserialized from the JSON content.</returns>
    /// <exception cref="ArgumentException"></exception>
    public T DeserializeJsonContent<T>(string? json_content = null)
    {
        if (string.IsNullOrWhiteSpace(json_content))
        {
            if (string.IsNullOrWhiteSpace(Json_Content))
                LoadJsonContent();
            json_content = Json_Content;
        }

        if (string.IsNullOrWhiteSpace(json_content))
            throw new ArgumentException("JSON content is empty.", nameof(json_content));

        return JsonSerializer.Deserialize<T>(json_content)!;
    }

    #endregion

    #region Writer

    /// <summary>
    /// Saves the JSON content to the specified file path. If no content is provided, the current in-memory JSON content will be used.
    /// </summary>
    /// <param name="json_content">The JSON content to save. If null, the current in-memory JSON content will be used.</param>
    /// <exception cref="ArgumentException"></exception>
    public void SaveJsonContent(string? json_content = null)
    {
        if (json_content == null)
        {
            if (string.IsNullOrWhiteSpace(Json_Content))
                LoadJsonContent();
            json_content = Json_Content;
        }

        if (string.IsNullOrWhiteSpace(json_content))
            throw new ArgumentException("JSON content is empty.", nameof(json_content));

        if (string.IsNullOrWhiteSpace(File_Path))
            throw new ArgumentException("File path is not specified.", nameof(File_Path));

        File.WriteAllText(File_Path, json_content);
    }

    /// <summary>
    /// Serializes an object to JSON content and optionally saves it to the file.
    /// </summary>
    /// <param name="content">The object to serialize.</param>
    /// <param name="save">Indicates whether to save the serialized JSON content to the file. Defaults to true.</param>
    /// <returns>The serialized JSON content as a string.</returns>
    public string SerializeJsonContent(object content, bool save = true)
    {
        Json_Content = JsonSerializer.Serialize(content);
        if (save)
            SaveJsonContent(Json_Content);
        return Json_Content;
    }

    #endregion

    #region Helper

    /// <summary>
    /// Unwraps the value from a JSON element, converting it to a native C# type if possible.
    /// </summary>
    /// <param name="value">The JSON value to unwrap.</param>
    /// <returns>The unwrapped value as a native C# type, or the original value if it is not a JSON element.</returns>
    /// <exception cref="ArgumentException"></exception>
    public object? UnwrapJsonValue(object? value)
    {
        if (value is not JsonElement element)
            return value;

        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Number when element.TryGetInt32(out int number) => number,
            JsonValueKind.Number when element.TryGetDouble(out double doubleNumber) => doubleNumber,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => throw new ArgumentException($"Unsupported JSON value: {element.ValueKind}")
        };
    }

    #endregion

    #endregion
}

public interface IJsonHelper
{
    string? File_Path { get; set; }
    string? Json_Content { get; set; }
    string LoadJsonContent();
    T DeserializeJsonContent<T>(string? json_content = null);
    object? UnwrapJsonValue(object? value);
}
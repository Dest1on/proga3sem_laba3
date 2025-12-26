using System.Text.Json;
using PhotoEditor.Core.Project;

namespace PhotoEditor.Infrastructure;

public sealed class JsonProjectSerializer : IProjectSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public string Serialize(PhotoProject project)
    {
        return JsonSerializer.Serialize(project, Options);
    }

    public PhotoProject Deserialize(string data)
    {
        return JsonSerializer.Deserialize<PhotoProject>(data, Options)
               ?? throw new InvalidOperationException("Failed to deserialize project");
    }
}

using System.IO;
using PhotoEditor.Core.Project;

namespace PhotoEditor.Infrastructure;

public sealed class ProjectFileStorage
{
    private readonly IProjectSerializer _serializer;

    public ProjectFileStorage(IProjectSerializer serializer)
    {
        _serializer = serializer;
    }

    public void Save(PhotoProject project, string path)
    {
        var json = _serializer.Serialize(project);
        File.WriteAllText(path, json);
    }

    public PhotoProject Load(string path)
    {
        var json = File.ReadAllText(path);
        return _serializer.Deserialize(json);
    }
}

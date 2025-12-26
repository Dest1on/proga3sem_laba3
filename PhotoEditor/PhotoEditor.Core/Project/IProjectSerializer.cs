namespace PhotoEditor.Core.Project;

public interface IProjectSerializer
{
    string Serialize(PhotoProject project);
    PhotoProject Deserialize(string data);
}

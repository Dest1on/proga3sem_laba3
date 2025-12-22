using System;
namespace PhotoEditor.Core.Project;

public class PhotoProject
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
using System;
namespace PhotoEditor.Core;

public sealed class ImageAsset
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string FileName { get; init; } = string.Empty;
    public string SourcePath { get; init; } = string.Empty;

    public int Width { get; init; }
    public int Height { get; init; }

    public DateTimeOffset ImportedAt { get; init; } = DateTimeOffset.UtcNow;
}

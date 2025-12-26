using System;
using System.Collections.Generic;

namespace PhotoEditor.Core.Project;

public class PhotoProject
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public List<ImageAsset> Assets { get; init; } = new();
    public List<OperationRecord> Operations { get; init; } = new();

    public Guid? ActiveAssetId { get; set; }
}

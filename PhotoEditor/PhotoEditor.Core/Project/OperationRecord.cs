using System;

namespace PhotoEditor.Core.Project;

public sealed class OperationRecord
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid? AssetId { get; init; }

    public string OperationType { get; init; } = string.Empty;
    public string Parameters { get; init; } = string.Empty;

    public DateTimeOffset AppliedAt { get; init; } = DateTimeOffset.UtcNow;
}

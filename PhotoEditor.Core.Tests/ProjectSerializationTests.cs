using System;
using PhotoEditor.Core;
using PhotoEditor.Core.Project;
using PhotoEditor.Infrastructure;
using Xunit;

public class ProjectSerializationTests
{
    [Fact]
    public void RoundTrip_PreservesBasicProjectState()
    {
        // Arrange
        var project = new PhotoProject
        {
            ActiveAssetId = Guid.NewGuid()
        };

        project.Assets.Add(new ImageAsset
        {
            FileName = "test.png",
            SourcePath = "C:\\images\\test.png",
            Width = 100,
            Height = 80
        });

        var serializer = new JsonProjectSerializer();

        // Act
        var json = serializer.Serialize(project);
        var restored = serializer.Deserialize(json);

        // Assert
        Assert.NotNull(restored);
        Assert.Equal(project.ActiveAssetId, restored.ActiveAssetId);
        Assert.Single(restored.Assets);
        Assert.Equal(project.Assets[0].FileName, restored.Assets[0].FileName);
        Assert.Equal(project.Assets[0].Width, restored.Assets[0].Width);
        Assert.Equal(project.Assets[0].Height, restored.Assets[0].Height);
    }

    [Fact]
    public void RoundTrip_PreservesOperations()
    {
        // Arrange
        var project = new PhotoProject();

        var operation = new OperationRecord
        {
            AssetId = Guid.NewGuid(),
            OperationType = "Rotate",
            Parameters = "angle=90"
        };

        project.Operations.Add(operation);

        var serializer = new JsonProjectSerializer();

        // Act
        var json = serializer.Serialize(project);
        var restored = serializer.Deserialize(json);

        // Assert
        Assert.Single(restored.Operations);

        var restoredOperation = restored.Operations[0];

        Assert.Equal(operation.AssetId, restoredOperation.AssetId);
        Assert.Equal(operation.OperationType, restoredOperation.OperationType);
        Assert.Equal(operation.Parameters, restoredOperation.Parameters);
    }



}

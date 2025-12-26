using System;
using System.IO;
using PhotoEditor.Core;
using PhotoEditor.Core.Project;
using PhotoEditor.Infrastructure;
using Xunit;

public class ProjectFileStorageTests
{
    [Fact]
    public void SaveLoad_PreservesProjectState()
    {
        // Arrange
        var project = new PhotoProject();

        var asset = new ImageAsset
        {
            FileName = "test.png",
            SourcePath = "C:\\images\\test.png",
            Width = 100,
            Height = 80
        };

        project.Assets.Add(asset);
        project.ActiveAssetId = asset.Id;

        project.Operations.Add(new OperationRecord
        {
            AssetId = asset.Id,
            OperationType = "Brightness",
            Parameters = "delta=50"
        });

        project.Operations.Add(new OperationRecord
        {
            AssetId = asset.Id,
            OperationType = "Crop",
            Parameters = "template=Center"
        });

        var storage = new ProjectFileStorage(new JsonProjectSerializer());

        var path = Path.Combine(Path.GetTempPath(), $"project_{Guid.NewGuid()}.json");

        try
        {
            // Act
            storage.Save(project, path);
            var restored = storage.Load(path);

            // Assert
            Assert.Equal(project.ActiveAssetId, restored.ActiveAssetId);

            Assert.Single(restored.Assets);
            Assert.Equal(asset.SourcePath, restored.Assets[0].SourcePath);
            Assert.Equal(asset.FileName, restored.Assets[0].FileName);
            Assert.Equal(asset.Width, restored.Assets[0].Width);
            Assert.Equal(asset.Height, restored.Assets[0].Height);

            Assert.Equal(2, restored.Operations.Count);
            Assert.Equal(project.Operations[0].OperationType, restored.Operations[0].OperationType);
            Assert.Equal(project.Operations[0].Parameters, restored.Operations[0].Parameters);
            Assert.Equal(project.Operations[1].OperationType, restored.Operations[1].OperationType);
            Assert.Equal(project.Operations[1].Parameters, restored.Operations[1].Parameters);
        }
        finally
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}

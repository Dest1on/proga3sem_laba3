
using PhotoEditor.Core.Images;
using PhotoEditor.Core.Operations;
using Xunit;

namespace PhotoEditor.Core.Tests.Operations
{
    public class BrightnessOperationTests
    {
        [Fact]
        public void Apply_IncreasesPixelBrightness()
        {
            // Arrange
            var image = new InMemoryImage(1, 1);
            image.SetPixel(0, 0, new PixelColor(100, 100, 100));

            var operation = new BrightnessOperation(50);

            // Act
            var result = operation.Apply(image);
            var pixel = result.GetPixel(0, 0);

            // Assert
            Assert.Equal(150, pixel.R);
            Assert.Equal(150, pixel.G);
            Assert.Equal(150, pixel.B);
        }

        [Fact]
        public void Apply_ClampsPixelValuesTo255()
        {
            // Arrange
            var image = new InMemoryImage(1, 1);
            image.SetPixel(0, 0, new PixelColor(240, 240, 240));

            var operation = new BrightnessOperation(50);

            // Act
            var result = operation.Apply(image);
            var pixel = result.GetPixel(0, 0);

            // Assert
            Assert.Equal(255, pixel.R);
            Assert.Equal(255, pixel.G);
            Assert.Equal(255, pixel.B);
        }

        [Fact]
        public void Apply_DoesNotModifySourceImage()
        {
            // Arrange
            var image = new InMemoryImage(1, 1);
            image.SetPixel(0, 0, new PixelColor(100, 100, 100));

            var operation = new BrightnessOperation(50);

            // Act
            operation.Apply(image);
            var originalPixel = image.GetPixel(0, 0);

            // Assert
            Assert.Equal(100, originalPixel.R);
            Assert.Equal(100, originalPixel.G);
            Assert.Equal(100, originalPixel.B);
        }
    }
}

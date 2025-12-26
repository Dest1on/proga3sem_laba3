using PhotoEditor.Core.Images;
using PhotoEditor.Core.Operations;
using Xunit;

namespace PhotoEditor.Core.Tests.Operations
{
    public class CropOperationTests
    {
        [Fact]
        public void Apply_ReturnsCroppedImageWithCorrectSize()
        {
            var image = new InMemoryImage(4, 4);

            var crop = new CropOperation(1, 1, 2, 2);

            var result = crop.Apply(image);

            Assert.Equal(2, result.Width);
            Assert.Equal(2, result.Height);
        }

        [Fact]
        public void Apply_CopiesCorrectPixels()
        {
            var image = new InMemoryImage(3, 3);

            image.SetPixel(1, 1, new PixelColor(10, 20, 30));

            var crop = new CropOperation(1, 1, 1, 1);

            var result = crop.Apply(image);
            var pixel = result.GetPixel(0, 0);

            Assert.Equal(10, pixel.R);
            Assert.Equal(20, pixel.G);
            Assert.Equal(30, pixel.B);
        }

        [Fact]
        public void Apply_DoesNotModifySourceImage()
        {
            var image = new InMemoryImage(3, 3);
            image.SetPixel(1, 1, new PixelColor(50, 60, 70));

            var crop = new CropOperation(1, 1, 1, 1);
            crop.Apply(image);

            var originalPixel = image.GetPixel(1, 1);

            Assert.Equal(50, originalPixel.R);
            Assert.Equal(60, originalPixel.G);
            Assert.Equal(70, originalPixel.B);
        }
    }
}

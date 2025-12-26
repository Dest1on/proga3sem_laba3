using System.Collections.Generic;
using PhotoEditor.Core.Images;
using PhotoEditor.Core.Operations;
using Xunit;

namespace PhotoEditor.Core.Tests.Operations
{
    public class ComposeOperationTests
    {
        [Fact]
        public void Apply_DrawsSingleLayerCorrectly()
        {
            var image = new InMemoryImage(1, 1);
            image.SetPixel(0, 0, new PixelColor(255, 0, 0));

            var layers = new List<ImageLayer>
            {
                new ImageLayer(image, 1, 1)
            };

            var compose = new ComposeOperation(layers, 3, 3);

            var result = compose.Apply(null);

            var pixel = result.GetPixel(1, 1);

            Assert.Equal(255, pixel.R);
            Assert.Equal(0, pixel.G);
            Assert.Equal(0, pixel.B);
        }

        [Fact]
        public void Apply_LayersAreDrawnInOrder()
        {
            var bottom = new InMemoryImage(1, 1);
            bottom.SetPixel(0, 0, new PixelColor(0, 0, 255));

            var top = new InMemoryImage(1, 1);
            top.SetPixel(0, 0, new PixelColor(255, 0, 0));

            var layers = new List<ImageLayer>
            {
                new ImageLayer(bottom, 0, 0),
                new ImageLayer(top, 0, 0)
            };

            var compose = new ComposeOperation(layers, 1, 1);

            var result = compose.Apply(null);
            var pixel = result.GetPixel(0, 0);

            Assert.Equal(255, pixel.R);
            Assert.Equal(0, pixel.G);
            Assert.Equal(0, pixel.B);
        }
    }
}

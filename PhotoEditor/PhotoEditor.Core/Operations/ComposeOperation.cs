using System;
using System.Collections.Generic;
using PhotoEditor.Core.Images;

namespace PhotoEditor.Core.Operations
{
    /// Операция композиции (коллажа) нескольких изображений.
    public sealed class ComposeOperation : IImageOperation
    {
        private readonly IReadOnlyList<ImageLayer> _layers;
        private readonly int _width;
        private readonly int _height;

        public string Name => "Compose";

        public ComposeOperation(
            IReadOnlyList<ImageLayer> layers,
            int width,
            int height)
        {
            if (layers == null || layers.Count == 0)
                throw new ArgumentException("At least one layer is required.", nameof(layers));

            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            _layers = layers;
            _width = width;
            _height = height;
        }

        public IImage Apply(IImage _)
        {
            var result = new InMemoryImage(_width, _height);

            foreach (var layer in _layers)
            {
                DrawLayer(result, layer);
            }

            return result;
        }

        private static void DrawLayer(InMemoryImage canvas, ImageLayer layer)
        {
            for (int x = 0; x < layer.Image.Width; x++)
            {
                for (int y = 0; y < layer.Image.Height; y++)
                {
                    int targetX = layer.X + x;
                    int targetY = layer.Y + y;

                    if (targetX < 0 || targetX >= canvas.Width)
                        continue;

                    if (targetY < 0 || targetY >= canvas.Height)
                        continue;

                    var pixel = layer.Image.GetPixel(x, y);
                    canvas.SetPixel(targetX, targetY, pixel);
                }
            }
        }
    }
}

using System.Drawing;
using System;

namespace PhotoEditor.Core.Images
{
    /// Реализация изображения, хранящая пиксели в памяти.
    /// Используется в доменном слое для обработки изображений.

    public sealed class InMemoryImage : IImage
    {
        private readonly Color[,] _pixels;

        public int Width { get; }
        public int Height { get; }

        public InMemoryImage(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            Width = width;
            Height = height;

            _pixels = new Color[width, height];
        }

        public Color GetPixel(int x, int y)
        {
            ValidateCoordinates(x, y);
            return _pixels[x, y];
        }

        public void SetPixel(int x, int y, Color color)
        {
            ValidateCoordinates(x, y);
            _pixels[x, y] = color;
        }

        private void ValidateCoordinates(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException(nameof(y));
        }
    }
}

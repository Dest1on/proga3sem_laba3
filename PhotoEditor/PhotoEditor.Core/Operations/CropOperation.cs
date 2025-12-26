using System;
using PhotoEditor.Core.Images;

namespace PhotoEditor.Core.Operations
{
    /// <summary>
    /// Операция обрезки изображения по прямоугольной области.
    /// </summary>
    public sealed class CropOperation : IImageOperation
    {
        private readonly int _x;
        private readonly int _y;
        private readonly int _width;
        private readonly int _height;

        public string Name => "Crop";

        public CropOperation(int x, int y, int width, int height)
        {
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));

            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));

            if (x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));

            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        public IImage Apply(IImage source)
        {
            if (_x + _width > source.Width)
                throw new InvalidOperationException("Crop area exceeds image width.");

            if (_y + _height > source.Height)
                throw new InvalidOperationException("Crop area exceeds image height.");

            var result = new InMemoryImage(_width, _height);

            for (int dx = 0; dx < _width; dx++)
            {
                for (int dy = 0; dy < _height; dy++)
                {
                    var pixel = source.GetPixel(_x + dx, _y + dy);
                    result.SetPixel(dx, dy, pixel);
                }
            }

            return result;
        }
    }
}

using System;

namespace PhotoEditor.Core.Images
{
    /// <summary>
    /// Описывает слой изображения в композиции.
    /// </summary>
    public sealed class ImageLayer
    {
        public IImage Image { get; }
        public int X { get; }
        public int Y { get; }

        public ImageLayer(IImage image, int x, int y)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));

            if (x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));

            if (y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));

            X = x;
            Y = y;
        }
    }
}

using System;

using PhotoEditor.Core.Images;

namespace PhotoEditor.Core.Operations
{
    /// Операция изменения яркости изображения
    public sealed class BrightnessOperation : IImageOperation
    {
        private readonly int _delta;

        public string Name => "Brightness";

        /// Положительное значение - осветление, отрицательное - затемнение [-255, 255]
        public BrightnessOperation(int delta)
        {
            if (delta < -255 || delta > 255)
                throw new ArgumentOutOfRangeException(nameof(delta));

            _delta = delta;
        }

        public IImage Apply(IImage source)
        {
            var result = new InMemoryImage(source.Width, source.Height);

            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    var pixel = source.GetPixel(x, y);

                    var r = Clamp(pixel.R + _delta);
                    var g = Clamp(pixel.G + _delta);
                    var b = Clamp(pixel.B + _delta);

                    result.SetPixel(
                    x,
                    y,
                    new PixelColor((byte)r, (byte)g, (byte)b, pixel.A)
                    );

                }
            }

            return result;
        }

        private static int Clamp(int value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;
            return value;
        }
    }
}

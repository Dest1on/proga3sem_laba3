namespace PhotoEditor.Core.Images
{
    /// Доменное представление цвета пикселя.
    /// Не зависит от платформы или UI.
    public readonly struct PixelColor
    {
        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }

        public PixelColor(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public PixelColor WithRgb(byte r, byte g, byte b)
            => new PixelColor(r, g, b, A);
    }
}

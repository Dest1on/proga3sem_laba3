using PhotoEditor.Core.Images;

namespace PhotoEditor.Core.Operations
{
    public sealed class DrawTextOperation : IImageOperation
    {
        private readonly int _x;
        private readonly int _y;
        private readonly int _width;
        private readonly int _height;
        private readonly PixelColor _color;

        public string Name => "DrawText";

        public DrawTextOperation(
            int x,
            int y,
            int width,
            int height,
            PixelColor color)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _color = color;
        }

        public IImage Apply(IImage source)
        {
            var result = new InMemoryImage(source.Width, source.Height);

            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    result.SetPixel(x, y, source.GetPixel(x, y));
                }
            }

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    int px = _x + x;
                    int py = _y + y;

                    if (px < source.Width && py < source.Height)
                        result.SetPixel(px, py, _color);
                }
            }

            return result;
        }
    }
}

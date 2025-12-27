using PhotoEditor.Core.Images;

namespace PhotoEditor.Core.Operations
{
    public sealed class RotateOperation : IImageOperation
    {
        public string Name => "Rotate90";

        public IImage Apply(IImage source)
        {
            var result = new InMemoryImage(source.Height, source.Width);

            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    var pixel = source.GetPixel(x, y);
                    result.SetPixel(
                        source.Height - y - 1,
                        x,
                        pixel
                    );
                }
            }

            return result;
        }
    }
}

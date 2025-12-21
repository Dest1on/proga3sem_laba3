using System.Drawing;

namespace PhotoEditor.Core.Images
{
    /// Абстракция изображения.
    public interface IImage
    {
        int Width { get; }
        int Height { get; }

        Color GetPixel(int x, int y);
        void SetPixel(int x, int y, Color color);

        Bitmap ToBitmap();
    }
}

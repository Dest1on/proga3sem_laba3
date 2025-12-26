using PhotoEditor.Core.Images;

namespace PhotoEditor.Core.Images
{
    /// Абстракция изображения.
public interface IImage
{
    int Width { get; }
    int Height { get; }

    PixelColor GetPixel(int x, int y);
    void SetPixel(int x, int y, PixelColor color);
}

}

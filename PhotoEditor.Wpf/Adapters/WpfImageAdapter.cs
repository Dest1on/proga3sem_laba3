using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PhotoEditor.Core.Images;

namespace PhotoEditor.Wpf.Adapters
{
    /// Адаптер для преобразования изображений в формат Wpf(BitmapSource).
    public class WpfImageAdapter:IWpfImageAdapter
    {
        public BitmapSource Convert(IImage image)
        {
            var bitmap = new WriteableBitmap(image.Width, image.Height, 96, 96, PixelFormats.Bgra32, null);

            var pixels = new byte[image.Width * image.Height * 4]; // 4 байта на пиксель
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    var color = image.GetPixel(x, y);
                    int index = (y * image.Width + x) * 4;

                    pixels[index + 0] = color.B;
                    pixels[index + 1] = color.G;
                    pixels[index + 2] = color.R;
                    pixels[index + 3] = color.A;
                }
            }

            bitmap.WritePixels(new Int32Rect(0, 0, image.Width, image.Height),pixels, image.Width * 4, 0);

            return bitmap;
        }
 
    }
}
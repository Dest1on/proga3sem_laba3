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
            var bitmap=new WriteableBitmap(image.Width,image.Height,96,96,PixelFormats.Bra32,null);
            bitmap.Lock();

            /// Используем небезопасный код для прямого доступа к памяти(через указатели)
            unsafe
            {
                var buffer=(byte*)bitmap.BackBuffer;
                for (int y=0;y<image.Height;y++)
                {
                    for (int x=0;x<image.Width;x++)
                    {
                        var color=image.GetPixel(x,y);

                        int index=y*bitmap.BackBufferStride+x*4;

                        buffer[index+0]=color.B;
                        buffer[index+1]=color.G;
                        buffer[index+2]=color.R;
                        buffer[index+3]=color.A;
                    }
                }
            }

            bitmap.AddDirtyRect(new Int32Rect(0,0,image.Width,image.Height));
            bitmap.Unlock();

            return bitmap;


        }
    }
}
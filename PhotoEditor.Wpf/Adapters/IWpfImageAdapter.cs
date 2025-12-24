using System.Windows.Media.Imaging;
using PhotoEditor.Core.Images;

namespace PhotoEditor.Wpf.Adapters
{
    /// Адаптер для преобразования изображений из Core в формат Wpf(BitmapSource).
    public interface IWpfImageAdapter
    {
        BitmapSource Convert(IImage image);
    }
}


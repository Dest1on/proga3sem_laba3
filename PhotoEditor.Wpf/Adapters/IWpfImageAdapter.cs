using System.Windows.Media.Imaging;
using PhotoEditor.Core.Images;

namespace PhotoEditor.Wpf.Adapters
{
    public interface IWpfImageAdapter
    {
        BitmapSource Convert(IImage image);
    }
}
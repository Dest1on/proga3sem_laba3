using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PhotoEditor.Core.Images;
using PhotoEditor.Wpf.Adapters;

namespace PhotoEditor.Wpf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    { 
        private readonly IWpfImageAdapter _imageAdapter;
        private BitmapSource? _currentBitmap;
        private IImage? _currentImage;

        public MainViewModel(IwpfImageAdapter imageAdapter)
        {
            _imageAdapter=imageAdapter;
            OpenImageCommand=new RelayCommand(OpenImage);
        }

        public BitmapSource? CurrentBitmap
        {
            get => _currentBitmap;
            private set
            {
                _currentBitmap=value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenImageCommand { get; }

        private void OpenImage()
        {

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName=null)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));

        }
    }
}
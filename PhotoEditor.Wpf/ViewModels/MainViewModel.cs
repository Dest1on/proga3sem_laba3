using Microsoft.Win32;
using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PhotoEditor.Core.Images;
using PhotoEditor.Wpf.Adapters;
using PhotoEditor.Wpf.Commands;

namespace PhotoEditor.Wpf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    { 
        private readonly IWpfImageAdapter _imageAdapter;
        private BitmapSource? _currentBitmap;
        private IImage? _currentImage;

        public MainViewModel(IWpfImageAdapter imageAdapter)
        {
            _imageAdapter = imageAdapter;
            OpenImageCommand=new RelayCommand(OpenImage);
        }

        public MainViewModel()
        {
            _imageAdapter = new WpfImageAdapter();
            OpenImageCommand = new RelayCommand(OpenImage);
        }

        public BitmapSource? CurrentBitmap
        {
            get => _currentBitmap;
            private set
            {
                _currentBitmap = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenImageCommand { get; }

        private void OpenImage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Images(*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            bool? result = dialog.ShowDialog();

            if (result!=true) return;

            BitmapImage bitmap = new BitmapImage();

            using (FileStream stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            bitmap.Freeze();

            CurrentBitmap = bitmap;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName=null)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));

        }
    }
}
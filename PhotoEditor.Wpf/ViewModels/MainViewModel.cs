using Microsoft.Win32;
using System;
using System.Drawing;
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
            CropCommand = new RelayCommand(Crop);
            RotateCommand = new RelayCommand(Rotate);
            ApplyFilterCommand = new RelayCommand(ApplyFilter);
            AddTextCommand = new RelayCommand(AddText);
            CreateCollageCommand = new RelayCommand(CreateCollage);
            EditPixelCommand = new RelayCommand(EditPixel);
            SaveCommand = new RelayCommand(Save);
            DeleteImageCommand = new RelayCommand(DeleteImage);
        }

        public MainViewModel()
        {
            _imageAdapter = new WpfImageAdapter();
            OpenImageCommand = new RelayCommand(OpenImage);
            CropCommand = new RelayCommand(Crop);
            RotateCommand = new RelayCommand(Rotate);
            ApplyFilterCommand = new RelayCommand(ApplyFilter);
            AddTextCommand = new RelayCommand(AddText);
            CreateCollageCommand = new RelayCommand(CreateCollage);
            EditPixelCommand = new RelayCommand(EditPixel);
            SaveCommand = new RelayCommand(Save);
            DeleteImageCommand = new RelayCommand(DeleteImage);
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
        public ICommand CropCommand { get; }
        public ICommand RotateCommand { get; }
        public ICommand ApplyFilterCommand { get; }
        public ICommand AddTextCommand { get; }
        public ICommand CreateCollageCommand { get; }
        public ICommand EditPixelCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteImageCommand { get; }
    

        private void OpenImage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Images(*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            bool? result = dialog.ShowDialog();
            if (result != true) return;

            BitmapImage bitmap = new BitmapImage();

            using (FileStream stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            bitmap.Freeze();

            InMemoryImage image = new InMemoryImage(bitmap.PixelWidth, bitmap.PixelHeight);

            int stride = bitmap.PixelWidth * 4;
            byte[] pixels = new byte[bitmap.PixelHeight * stride];
            bitmap.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < bitmap.PixelHeight; y++)
            {
                for (int x = 0; x < bitmap.PixelWidth; x++)
                {
                    int index = y * stride + x * 4;

                    byte b = pixels[index];
                    byte g = pixels[index + 1];
                    byte r = pixels[index + 2];
                    byte a = pixels[index + 3];

                    image.SetPixel(x, y, new PixelColor(r, g, b, a));
                  }
           }

            _currentImage = image;
            CurrentBitmap = _imageAdapter.Convert(_currentImage);
        }

        private void Save()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "PNG Image (*.png)|*.png",
                FileName = "image"
            };

            bool? result = dialog.ShowDialog();
            if (result != true)  return;

            //...
        }

        private void DeleteImage()
        {
             _currentImage = null;
            CurrentBitmap = null;

        }


        private void Crop()
        {
            
        }

        private void Rotate()
        {
            
        }

        private void ApplyFilter()
        {
            
        }

        private void AddText()
        {
            
        }

        private void CreateCollage()
        {
            
        }

        private void EditPixel()
        {
            
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName=null)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));

        }
    }
}
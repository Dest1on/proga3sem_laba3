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
using PhotoEditor.Core.Operations;
using PhotoEditor.Wpf.Views;


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

        public bool HasImage
        {
            get
            {
                return _currentImage != null;
            }
        }


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
            OnPropertyChanged(nameof(HasImage));
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
            OnPropertyChanged(nameof(HasImage));

        }


        private void Crop()
        {
            if (_currentImage == null) return;
            var dialog = new CropDialog();

            if (dialog.ShowDialog() != true) return;

            var operation = CreateCropOperation(dialog.SelectedTemplate);
            _currentImage = operation.Apply(_currentImage);
            CurrentBitmap = _imageAdapter.Convert(_currentImage);
            
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

        private CropOperation CreateCropOperation(CropTemplate template)
        {
            int width = _currentImage!.Width;
            int height = _currentImage!.Height;
    
            switch (template)
            {
                case CropTemplate.Square:
                    if (width > height)
                    {
                        return new CropOperation((width - height) / 2, 0, height, height);
                     }
                    else
                    {
                        return new CropOperation(0, (height - width) / 2, width, width);
                    }
            
                case CropTemplate.Ratio16x9:
                    return new CropOperation(0, 0, width, width * 9 / 16);
            
                case CropTemplate.Ratio4x3:
                    return new CropOperation(0, 0, width, width * 3 / 4);
            
                case CropTemplate.Center:
                    return new CropOperation(width / 4, height / 4, width / 2, height / 2);
            
                default:
                    return new CropOperation(0, 0, width, height);
    }
}
    }

    

}
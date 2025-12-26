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
using PhotoEditor.Core;
using PhotoEditor.Core.Project;
using PhotoEditor.Infrastructure;



namespace PhotoEditor.Wpf.ViewModels
{
     public enum FilterType
        {
            None,
            BrightnessPlus50,
            BrightnessMinus50
        }

    public class MainViewModel : INotifyPropertyChanged
    { 
        private readonly IWpfImageAdapter _imageAdapter;
        private BitmapSource? _currentBitmap;
        private IImage? _currentImage;
        private PhotoProject _project = new PhotoProject();
        private ImageAsset? _activeAsset;


        public MainViewModel(IWpfImageAdapter imageAdapter)
        {
            _imageAdapter = imageAdapter;
            OpenImageCommand=new RelayCommand(OpenImage);
            OpenProjectCommand = new RelayCommand(OpenProject);
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
            OpenProjectCommand = new RelayCommand(OpenProject);
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
        public ICommand OpenProjectCommand { get; }


        public bool HasImage
        {
            get
            {
                return _currentImage != null;
            }
        }

        private FilterType _selectedFilter = FilterType.None;
        public FilterType SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged();
                ApplySelectedFilter();
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

            _activeAsset = new PhotoEditor.Core.ImageAsset
            {
                FileName = System.IO.Path.GetFileName(dialog.FileName),
                SourcePath = dialog.FileName,
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight
            };

            _project.Assets.Add(_activeAsset);
            _project.ActiveAssetId = _activeAsset.Id;


            CurrentBitmap = _imageAdapter.Convert(_currentImage);
            OnPropertyChanged(nameof(HasImage));
        }

        private void OpenProject()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Project (*.json)|*.json"
            };

            if (dialog.ShowDialog() != true)
                return;

            var storage = new ProjectFileStorage(new JsonProjectSerializer());

            _project = storage.Load(dialog.FileName);

            RestoreActiveImage();
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

            // Сохраняем BitmapSource в PNG
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(_currentBitmap));

            using (var stream = new FileStream(dialog.FileName, FileMode.Create))
            {
                encoder.Save(stream);
            }

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
            if (_activeAsset != null)
{
            _project.Operations.Add(new OperationRecord
            {
                AssetId = _activeAsset.Id,
                OperationType = "Crop",
                Parameters = $"template={dialog.SelectedTemplate}"
            });
}

            CurrentBitmap = _imageAdapter.Convert(_currentImage);
            
        }

        private void Rotate()
        {
            
        }

        private void ApplyFilter()
        {
            ApplySelectedFilter();
            
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

        ///Обрезка по шаблону
       
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

        ///Применение фильтра

        private void ApplySelectedFilter()
        {
            if (_currentImage == null) return;

            var dialog = new FilterDialog();
            if (dialog.ShowDialog() != true) return;

            switch (dialog.SelectedFilter)
            {
                case FilterType.BrightnessPlus50:
                    _currentImage = new BrightnessOperation(50).Apply(_currentImage);

                    if (_activeAsset != null)
                    {
                        _project.Operations.Add(new OperationRecord
                        {
                            AssetId = _activeAsset.Id,
                            OperationType = "Brightness",
                            Parameters = "delta=50"
                        });
                    }
                    break;

                case FilterType.BrightnessMinus50:
                    _currentImage = new BrightnessOperation(-50).Apply(_currentImage);

                    if (_activeAsset != null)
                    {
                        _project.Operations.Add(new OperationRecord
                        {
                            AssetId = _activeAsset.Id,
                            OperationType = "Brightness",
                            Parameters = "delta=-50"
                        });
                    }
                    break;

                case FilterType.None:
                    return;
            }


            CurrentBitmap = _imageAdapter.Convert(_currentImage);
        }
        
        private void RestoreActiveImage()
        {
            if (_project == null) return;
            if (_project.ActiveAssetId == Guid.Empty) return;

            _activeAsset = _project.Assets
                .FirstOrDefault(a => a.Id == _project.ActiveAssetId);

            if (_activeAsset == null) return;
            if (!File.Exists(_activeAsset.SourcePath)) return;

            // 1. Загружаем исходное изображение
            BitmapImage bitmap = new BitmapImage();
            using (FileStream stream = new FileStream(_activeAsset.SourcePath, FileMode.Open, FileAccess.Read))
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
                    image.SetPixel(
                        x, y,
                        new PixelColor(
                            pixels[index + 2],
                            pixels[index + 1],
                            pixels[index],
                            pixels[index + 3]
                        )
                    );
                }
            }

            IImage current = image;

            // 2. Применяем операции по порядку
            var operations = _project.Operations
                .Where(o => o.AssetId == _activeAsset.Id)
                .OrderBy(o => o.AppliedAt);

            foreach (var op in operations)
            {
                current = ApplyOperation(current, op);
            }

            _currentImage = current;

            CurrentBitmap = _imageAdapter.Convert(_currentImage);
            OnPropertyChanged(nameof(HasImage));
        }


        private IImage ApplyOperation(IImage image, OperationRecord record)
        {
            switch (record.OperationType)
            {
                case "Brightness":
                    int delta = int.Parse(record.Parameters.Split('=')[1]);
                    return new BrightnessOperation(delta).Apply(image);

                case "Crop":
                    var template = Enum.Parse<CropTemplate>(
                        record.Parameters.Split('=')[1]
                    );
                    var cropOp = CreateCropOperation(template);
                    return cropOp.Apply(image);

                default:
                    return image;
            }
        }


    }

    

}
using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;

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
            SaveProjectCommand = new RelayCommand(SaveProject);
            
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
            SaveProjectCommand = new RelayCommand(SaveProject);
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
        public ICommand SaveProjectCommand { get; }



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

             _currentImage = LoadInMemoryImage(dialog.FileName);

            _activeAsset = new PhotoEditor.Core.ImageAsset
            {
                FileName = Path.GetFileName(dialog.FileName),
                SourcePath = dialog.FileName,
                Width = _currentImage.Width,
                Height = _currentImage.Height
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

        try
        {
            var storage = new ProjectFileStorage(new JsonProjectSerializer());
            var loaded = storage.Load(dialog.FileName);

            if (loaded == null)
                throw new InvalidOperationException("Project deserialization returned null.");

            _project = loaded;

            RestoreActiveImage();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                ex.ToString(),
                "Ошибка при открытии проекта",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error
            );
        }
    }

        private void SaveProject()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Project (*.json)|*.json",
                FileName = "project"
            };

            if (dialog.ShowDialog() != true)
                return;

            var storage = new ProjectFileStorage(new JsonProjectSerializer());
            storage.Save(_project, dialog.FileName);
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

            // Находим последнюю операцию коллажа
            var lastCollage = _project.Operations
                .Where(o => o.OperationType == "Collage")
                .LastOrDefault();

            if (lastCollage != null && !string.IsNullOrWhiteSpace(lastCollage.Parameters))
            {
                var usedIds = lastCollage.Parameters.Split(',')
                    .Select(Guid.Parse)
                    .ToList();
                
                _project.Assets.RemoveAll(a => usedIds.Contains(a.Id));
            }

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
            var activeAssets = _project.Assets.Where(a => File.Exists(a.SourcePath)).ToList();
            if (activeAssets.Count == 0) return;


            int targetSize = 600; // размер коллажа
            int count = _project.Assets.Count;
            int perRow = (int)Math.Ceiling(Math.Sqrt(count));
            int cellSize = targetSize / perRow;

    
            var collage = new RenderTargetBitmap(targetSize, targetSize, 96, 96, PixelFormats.Pbgra32);

            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                for (int i = 0; i < count; i++)
                {
                    var asset = activeAssets[i];
                    if (!File.Exists(asset.SourcePath)) continue;

                    BitmapImage bmp = new BitmapImage(new Uri(asset.SourcePath));

                    int row = i / perRow;
                    int col = i % perRow;

                    Rect rect = new Rect(col * cellSize, row * cellSize, cellSize, cellSize);
                    dc.DrawImage(bmp, rect);
                }
            }

            collage.Render(dv);

            var image = new InMemoryImage(targetSize, targetSize);
            int stride = targetSize * 4;
            byte[] pixels = new byte[targetSize * stride];
            collage.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < targetSize; y++)
            {
                for (int x = 0; x < targetSize; x++)
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

        private CropOperation CreateCropOperation(CropTemplate template, IImage image)
        {
            int width = image.Width;
            int height = image.Height;

            switch (template)
            {
                case CropTemplate.Square:
                    if (width > height)
                        return new CropOperation((width - height) / 2, 0, height, height);
                    else
                        return new CropOperation(0, (height - width) / 2, width, width);

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
        private void ApplySelectedFilter()
        {
            if (_currentImage == null) return;

            var dialog = new FilterDialog
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (dialog.ShowDialog() != true) return;

            // Пока у нас только яркость
            int delta = dialog.BrightnessValue;

            var operation = new BrightnessOperation(delta);
            _currentImage = operation.Apply(_currentImage);

            if (_activeAsset != null)
            {
                 _project.Operations.Add(new OperationRecord
                {
                    AssetId = _activeAsset.Id,
                    OperationType = "Brightness",
                    Parameters = $"delta={delta}"
                });
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
            _currentImage = LoadInMemoryImage(_activeAsset.SourcePath);

            // 2. Применяем операции по порядку
            var operations = _project.Operations
                .Where(o => o.AssetId == _activeAsset.Id)
                .OrderBy(o => o.AppliedAt);

            foreach (var op in operations)
            {
                _currentImage = ApplyOperation(_currentImage, op);
            }

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
                    var cropOp = CreateCropOperation(template, image);
                    return cropOp.Apply(image);


                default:
                    return image;
            }
        }




        private InMemoryImage LoadInMemoryImage(string path)
        {
            BitmapImage bitmap = new BitmapImage();
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }
            bitmap.Freeze();

            var image = new InMemoryImage(bitmap.PixelWidth, bitmap.PixelHeight);
            int stride = bitmap.PixelWidth * 4;
            byte[] pixels = new byte[bitmap.PixelHeight * stride];
            bitmap.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < bitmap.PixelHeight; y++)
            {
                for (int x = 0; x < bitmap.PixelWidth; x++)
                {
                    int i = y * stride + x * 4;
                    image.SetPixel(x, y,new PixelColor(
                    pixels[i + 2],
                    pixels[i + 1],
                    pixels[i],
                    pixels[i + 3]));
                }
            }

            return image;
        }

        





    }

    

}
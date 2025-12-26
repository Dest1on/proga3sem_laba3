using System.Windows;
using PhotoEditor.Wpf.ViewModels;

namespace PhotoEditor.Wpf.Views
{
    /// <summary>
    /// Диалоговое окно применения фильтра
    /// </summary>
    public partial class FilterDialog : Window
    {
        public FilterType SelectedFilter { get; private set; }

        public FilterDialog()
        {
            InitializeComponent();
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            SelectedFilter = FilterBox.SelectedIndex switch
            {
                0 => FilterType.None,
                1 => FilterType.BrightnessPlus50,
                2 => FilterType.BrightnessMinus50,
                _ => FilterType.None
            };
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }

}

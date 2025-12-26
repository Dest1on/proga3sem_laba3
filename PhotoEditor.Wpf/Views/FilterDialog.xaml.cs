using System.Windows;
using PhotoEditor.Wpf.ViewModels;

namespace PhotoEditor.Wpf.Views
{
    /// <summary>
    /// Диалоговое окно применения фильтра
    /// </summary>
    public partial class FilterDialog : Window
    {
        public int BrightnessValue { get; private set; }

        public FilterDialog()
        {
            InitializeComponent();
            FilterCombo.SelectedIndex = 0;
        }

        private void FilterCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Пока один фильтр — яркость
            BrightnessPanel.Visibility = Visibility.Visible;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            BrightnessValue = (int)BrightnessSlider.Value;
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    
    }
        

}

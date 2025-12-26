using System.Windows;

namespace PhotoEditor.Wpf.Views
{
    public partial class CropDialog : Window
    {
        public CropTemplate SelectedTemplate { get; private set; }

        public CropDialog()
        {
            InitializeComponent();
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            SelectedTemplate = (CropTemplate)TemplateBox.SelectedIndex;
            DialogResult = true;
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }

    public enum CropTemplate
    {
        Square,
        Ratio16x9,
        Ratio4x3,
        Center
    }
}

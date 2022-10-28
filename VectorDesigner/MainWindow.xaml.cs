using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VectorDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly float[] ObfuscationMinimizations = { 1, 0.3f };
        private static readonly float ObfuscationMinimization = ObfuscationMinimizations[0];

        private const int WindowSize = 800;

        public MainWindow()
        {
            InitializeComponent();

            Width = WindowSize * ObfuscationMinimization;
            Height = WindowSize * ObfuscationMinimization;
            Top = SystemParameters.PrimaryScreenHeight - Height - 45;

            PreviewImgInWpf(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\1.jpg");
        }

        private void PreviewImgInWpf(string imgFilePath)
        {
            var bitmapImg = new BitmapImage(new Uri(imgFilePath));
            Image1.Source = bitmapImg;
        }
    }
}

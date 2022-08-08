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
using ImageMagick;

namespace MagickPhotoAnimationLib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WindowSizeCategory = 1;
        private readonly int[] WindowWidthCategories = new int[] { 200, 400 };

        private const float VideoRatio = 1.5f;

        public MainWindow()
        {
            InitializeComponent();

            Image1.Width = WindowWidthCategories[WindowSizeCategory];
            Image1.Height = Image1.Width / VideoRatio;
            Top = SystemParameters.PrimaryScreenHeight - Image1.Height - 85;

            var img = new MagickImage(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\1.jpg");

            var croppedWidth = 2000;
            img.Crop(new MagickGeometry(2000, 1000, croppedWidth, (int)(croppedWidth / VideoRatio)));
            
            
            
            
            
            
            
            
            
            
            
            img.Write(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\2.jpg");
            Image1.Source = new BitmapImage(new Uri(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\2.jpg"));
        }
    }
}

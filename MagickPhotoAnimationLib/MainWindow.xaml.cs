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
        private const int WindowSizeCategory = 0;
        
        private readonly int[] WindowWidthCategories = new int[] { 200, 400 };

        public MainWindow()
        {
            InitializeComponent();

            Image1.Width = WindowWidthCategories[WindowSizeCategory];
            Image1.Height = Image1.Width / 1.5;
            Top = SystemParameters.PrimaryScreenHeight - Image1.Height - 80;

            var img = new MagickImage(@"C:\Users\ondrej\materialy\IT\MagickPhotoAnimationLib\images\1.jpg");
            img.Flip();
            img.Write(@"C:\Users\ondrej\materialy\IT\MagickPhotoAnimationLib\images\2.jpg");
            Image1.Source = new BitmapImage(new Uri(@"C:\Users\ondrej\materialy\IT\MagickPhotoAnimationLib\images\2.jpg"));
        }
    }
}

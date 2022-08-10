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
        private readonly int[] WindowWidthCategories = new int[] { 10, 200, 400 };
                                                                // 0   1    2
        private const float OutputScreenRatio = 1.5f;
        private const int OutputFrameRate = 1;

        public MainWindow()
        {
            InitializeComponent();

            Image1.Width = WindowWidthCategories[WindowSizeCategory];
            Image1.Height = Image1.Width / OutputScreenRatio;
            Top = SystemParameters.PrimaryScreenHeight - Image1.Height - 85;

            var img = new MagickImage(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\1.jpg");



            const float animTimeInSec = 3.572f;

            var startCropWidth = img.Width;
            var startCropHeight = img.Height;
            var startCropX = 0;
            var startCropY = 0;

            var endCropWidth = 2000;
            var endCropHeight = (int)(endCropWidth / OutputScreenRatio);
            var endCropX = (startCropWidth - endCropWidth) / 2;
            var endCropY = (startCropHeight - endCropHeight) / 2;

            img.Write(@"C:\Users\ondrej\MagickPhotoAnimationLib\out\sequence\start.jpg");
            
            var img2 = img.Clone();
            img.Dispose();
            
            img2.Crop(new MagickGeometry(endCropX, endCropY, endCropWidth, endCropHeight));
            img2.Write(@"C:\Users\ondrej\MagickPhotoAnimationLib\out\sequence\end.jpg");
            
            



            //img.Crop(new MagickGeometry(2000, 1000, croppedWidth, (int)(croppedWidth / OutputScreenRatio)));
            //img.Crop(croppedWidth, (int)(croppedWidth / OutputScreenRatio));



        }
    }
}

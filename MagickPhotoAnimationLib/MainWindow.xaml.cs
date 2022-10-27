using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
    public partial class MainWindow : Window
    {
        private static float[] ObfuscationMinimizations = { 1, 0.2f };
        private static float ObfuscationMinimization = ObfuscationMinimizations[0];

        private const int OutputScreenWidth = 1000;
        private const float OutputScreenRatio = 1.5f;
        private const int OutputFrameRate = 24;

        private int _outputIndex;

        public MainWindow()
        {
            InitializeComponent();

            foreach (var outFile in new DirectoryInfo(@"C:\Users\ondrej\MagickPhotoAnimationLib\out\sequence").EnumerateFiles())
            {
                outFile.Delete();
            }

            var startImg = new MagickImage(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\1.jpg");

            var startCropWidth = startImg.Width;
            var startCropHeight = startImg.Height;

            var endCropWidth = 3000;
            var endCropHeight = (int)(endCropWidth / OutputScreenRatio);

            startImg.Write(@"C:\Users\ondrej\MagickPhotoAnimationLib\out\start_end\1_start.jpg");

            var endImg = startImg.Clone();

            endImg.Crop(endCropWidth, endCropHeight, Gravity.Center);
            endImg.Write(@"C:\Users\ondrej\MagickPhotoAnimationLib\out\start_end\2_end.jpg");

            IMagickImage currentImg;

            var animStartTime = 10.3;
            //var animEndTime = 13.4;
            var animEndTime = 13.4;

            var animFrameCount = Math.Ceiling((animEndTime - animStartTime) * OutputFrameRate);

            for (int i = 0; i < animFrameCount; i++)
            {
                currentImg = startImg.Clone();

                var relativePositionWithinAnimation = i / animFrameCount;

                var currentCropWidth = startCropWidth + (int)((endCropWidth - startCropWidth) * relativePositionWithinAnimation);
                var currentCropHeight = startCropHeight + (int)((endCropHeight - startCropHeight) * relativePositionWithinAnimation);

                currentImg.Crop(currentCropWidth, currentCropHeight, Gravity.Center);

                WriteAndDispose(currentImg);
            }

            //PreviewImgInWpf(startImg);

            startImg.Dispose();
            Application.Current.Shutdown();
        }

        private void WriteAndDispose(IMagickImage img)
        {
            img.Resize(OutputScreenWidth, (int)(OutputScreenWidth / OutputScreenRatio));
            img.Resize(new Percentage(ObfuscationMinimization * 100));
            img.Write($@"C:\Users\ondrej\MagickPhotoAnimationLib\out\sequence\{_outputIndex.ToString("00")}.jpg");
            img.Dispose();
            _outputIndex++;
        }

        private void PreviewImgInWpf(MagickImage img)
        {
            var previewImage = img.Clone();
            previewImage.Resize(OutputScreenWidth, (int)(OutputScreenWidth / OutputScreenRatio));
            previewImage.Resize(new Percentage(ObfuscationMinimization * 100));
            Width = previewImage.Width;
            Height = previewImage.Height;
            Top = SystemParameters.PrimaryScreenHeight - previewImage.Height - 45;
            previewImage.Write($@"C:\Users\ondrej\MagickPhotoAnimationLib\out\preview_image.jpg");
            previewImage.Dispose();
            Image1.Source = new BitmapImage(new Uri(@"C:\Users\ondrej\MagickPhotoAnimationLib\out\preview_image.jpg"));
        }
    }
}

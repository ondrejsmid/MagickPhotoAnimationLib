using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using static System.Net.Mime.MediaTypeNames;

namespace MagickPhotoAnimationLib
{
    public partial class MainWindow : Window
    {
        private static readonly float[] ObfuscationMinimizations = { 1, 0.2f };
        private static readonly float ObfuscationMinimization = ObfuscationMinimizations[0];

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

            var image1 = new MagickImage(MagickColor.FromRgb(200, 255, 255), OutputScreenWidth, (int)(OutputScreenWidth / OutputScreenRatio));
            var image2 = startImg.Clone();
            const int image2Width = 300;
            image2.Resize(image2Width, (int)(image2Width / OutputScreenRatio));
            image1.Composite(image2, 100, 100);
            PreviewImgInWpf(image1);
            WriteAndDispose(image1);

#if false
            var startCropWidth = startImg.Width;
            var startCropHeight = startImg.Height;

            var endCropWidth = 3000;
            var endCropHeight = (int)(endCropWidth / OutputScreenRatio);

            IMagickImage currentImg = null;
            
            var animStartTime = 10.3;
            var animEndTime = 10.6;

            var animFrameCount = Math.Ceiling((animEndTime - animStartTime) * OutputFrameRate);

            var allImagesStopwatch = new Stopwatch();
            allImagesStopwatch.Start();

            for (int i = 0; i < animFrameCount; i++)
            {
                MeasureTime(() =>
                {
                    currentImg = startImg.Clone();

                }, nameof(startImg.Clone));

                var relativePositionWithinAnimation = i / animFrameCount;

                var currentCropWidth = startCropWidth + (int)((endCropWidth - startCropWidth) * relativePositionWithinAnimation);
                var currentCropHeight = startCropHeight + (int)((endCropHeight - startCropHeight) * relativePositionWithinAnimation);

                MeasureTime(() =>
                {
                    currentImg.Crop(currentCropWidth, currentCropHeight, Gravity.Center);

                }, nameof(startImg.Clone));

                WriteAndDispose(currentImg);
            }

            allImagesStopwatch.Stop();
            Debug.WriteLine($"Elapsed Time of all images: {(float)allImagesStopwatch.ElapsedMilliseconds / 1000} s");
            File.WriteAllText(@"C:\Users\ondrej\MagickPhotoAnimationLib\out\log.txt", ((float)allImagesStopwatch.ElapsedMilliseconds / 1000).ToString());
#endif
            startImg.Dispose();
            //Application.Current.Shutdown();
        }

        private static void MeasureTime(Action action, string nameOfMeasurement)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            Debug.WriteLine($"Elapsed Time of {nameOfMeasurement}: {(float)stopwatch.ElapsedMilliseconds / 1000} s");
        }

        private void WriteAndDispose(IMagickImage img)
        {
            MeasureTime(() =>
            {
                img.Resize(OutputScreenWidth, (int)(OutputScreenWidth / OutputScreenRatio));

            }, nameof(img.Resize));

            MeasureTime(() =>
            {
                img.Write($@"C:\Users\ondrej\MagickPhotoAnimationLib\out\sequence\{_outputIndex.ToString("00")}.jpg");

            }, nameof(img.Write));

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

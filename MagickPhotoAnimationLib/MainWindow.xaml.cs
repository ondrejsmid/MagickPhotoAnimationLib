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
using MagickPhotoAnimationLib.Extensions;
using VectorDesigner;
using static System.Net.Mime.MediaTypeNames;
using Point = System.Windows.Point;

namespace MagickPhotoAnimationLib
{
    public partial class MainWindow : Window
    {
        private static readonly float[] ObfuscationMinimizations = { 1, 0.3f };
        private static readonly float ObfuscationMinimization = ObfuscationMinimizations[1];

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

            var camel = new MagickImageAndVector(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\camel.png");

            var camelPivot = camel.GetPoint("Pivot");
            var camelTail = camel.GetPoint("Tail");

            var compositeImage = new MagickImage(MagickColor.FromRgb(200, 255, 255), OutputScreenWidth, (int)(OutputScreenWidth / OutputScreenRatio));

            const int degrees = 90;
            var camelRotated = camel.GetRotated(degrees, camelPivot);
            compositeImage.Composite(camel.MagickImage, Gravity.Center, CompositeOperator.Over);
            compositeImage.Composite(camelRotated.MagickImage, Gravity.Center, CompositeOperator.Over);

            var camelRotatedPivot = camelRotated.GetPoint("Pivot");
            var camelRotatedTail = camelRotated.GetPoint("Tail");

            var camelRotatedPivotShift = camelPivot.Subtract(camelRotatedPivot);

            var part2 = new MagickImageAndVector(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\part2.png");
            var part2Pivot = part2.GetPoint("Pivot");

            compositeImage.Composite(part2.MagickImage, Gravity.Center,
                camelRotatedTail.Add(camelRotatedPivotShift).Subtract(part2Pivot),
                CompositeOperator.Over);


            const int widthOfCanvasForHuman = 2000;

            var background = new MagickImage(MagickColor.FromRgb(200, 255, 255), widthOfCanvasForHuman, (int)(widthOfCanvasForHuman / OutputScreenRatio));

            var human = new Human(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\ondra0", new Point(widthOfCanvasForHuman, widthOfCanvasForHuman / OutputScreenRatio));

            background.Composite(human.MagickImage, Gravity.Center, CompositeOperator.Over);
            PreviewImgInWpf(background);
#if false
            const int testPointBlueSize = 40;
            const int testPointRedSize = 25;
            var testPointBlue = new MagickImage(MagickColor.FromRgb(0, 0, 255), testPointBlueSize, testPointBlueSize);
            var testPointRed = new MagickImage(MagickColor.FromRgb(255, 0, 0), testPointRedSize, testPointRedSize);

            compositeImage.Composite(testPointBlue, Gravity.Center, camelPivot, CompositeOperator.Over);
            compositeImage.Composite(testPointBlue, Gravity.Center, camelTail, CompositeOperator.Over);
            compositeImage.Composite(testPointRed, Gravity.Center, camelRotatedPivot.Add(camelRotatedPivotShift), CompositeOperator.Over);
            compositeImage.Composite(testPointRed, Gravity.Center, camelRotatedTail.Add(camelRotatedPivotShift), CompositeOperator.Over);
#endif
#if false
            var startImg = new MagickImage(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\1.jpg");
            var compositeImage = new MagickImage(MagickColor.FromRgb(200, 255, 255), OutputScreenWidth, (int)(OutputScreenWidth / OutputScreenRatio));
            var image1 = startImg.Clone();
            var image2 = startImg.Clone();
            const int image2Width = 300;
            image1.Resize(image2Width, (int)(image2Width / OutputScreenRatio));
            image2.Resize(image2Width, (int)(image2Width / OutputScreenRatio));
            compositeImage.Composite(image1, -150, -50);
            compositeImage.Composite(image2, 100, 100);
            PreviewImgInWpf(compositeImage);
            startImg.Dispose();
#endif
#if false
            var startImg = new MagickImage(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\1.jpg");
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
            startImg.Dispose();
#endif
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

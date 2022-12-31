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
using MagickPhotoAnimationLib.Animation;
using MagickPhotoAnimationLib.Extensions;
using VectorDesigner;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;
using Path = System.IO.Path;
using Point = System.Windows.Point;

namespace MagickPhotoAnimationLib
{
    public partial class MainWindow : Window
    {
        const int InputImagesPercentageSize = 25; /* supported values: 100, 50, 25 */
        private static string _imagesSizeDirName = InputImagesPercentageSize == 100 ? "images" : $"images_size_{InputImagesPercentageSize}_percent";
        private static string _imagesDirPath = $@"C:\Users\ondrej\MagickPhotoAnimationLib\{_imagesSizeDirName}";

        private static readonly float[] ObfuscationMinimizations = { 1, 0.5f };
        private static readonly float ObfuscationMinimization = ObfuscationMinimizations[1];
        private const int OutputScreenWidth = (int)(1000 * ((float)InputImagesPercentageSize / 100));
        private const float OutputScreenRatio = 1.5f;
        private const int OutputFrameRate = 24;

        private int _outputIndex;
        private GraphicsCache _graphicsCache = new GraphicsCache();

        public MainWindow()
        {
            InitializeComponent();

            foreach (var outFile in new DirectoryInfo(@"C:\Users\ondrej\MagickPhotoAnimationLib\out\sequence").EnumerateFiles())
            {
                outFile.Delete();
            }
            File.WriteAllText(@"C:\Users\ondrej\MagickPhotoAnimationLib\out\log.txt", string.Empty);
#if false
            var head = new MagickImageAndVector(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\ondra0\head.png");

            var headPivot = head.GetPoint("Pivot");

            const int compositeImageSize = 2000;
            var compositeImage = new MagickImage(MagickColor.FromRgb(200, 255, 255), compositeImageSize, (int)(compositeImageSize / OutputScreenRatio));

            const double degrees = -75;
            var headRotated = head.GetRotated(degrees, headPivot);
            compositeImage.Composite(head.MagickImage, Gravity.Center, CompositeOperator.Over);
            compositeImage.Composite(headRotated.MagickImage, Gravity.Center, CompositeOperator.Over);

            var headRotatedPivot = headRotated.GetPoint("Pivot");

            const int testPointBlueSize = 40;
            const int testPointRedSize = 25;
            var testPointBlue = new MagickImage(MagickColor.FromRgb(0, 0, 255), testPointBlueSize, testPointBlueSize);
            var testPointRed = new MagickImage(MagickColor.FromRgb(255, 0, 0), testPointRedSize, testPointRedSize);

            var rotatedPivotShift = headPivot.Subtract(headRotatedPivot);

            compositeImage.Composite(testPointBlue, Gravity.Center, headPivot, CompositeOperator.Over);
            compositeImage.Composite(testPointRed, Gravity.Center, headRotatedPivot.Add(rotatedPivotShift), CompositeOperator.Over);
            
            PreviewImgInWpf(compositeImage);
#endif
#if false
            var drawing1 = new MagickImageAndVector(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\drawing1.png");

            var drawing1Pivot = drawing1.GetPoint("Pivot");
            var drawing1Tail = drawing1.GetPoint("Tail");

            var compositeImage = new MagickImage(MagickColor.FromRgb(200, 255, 255), OutputScreenWidth, (int)(OutputScreenWidth / OutputScreenRatio));

            const double degrees = -75;
            var drawing1Rotated = drawing1.GetRotated(degrees, drawing1Pivot);
            compositeImage.Composite(drawing1.MagickImage, Gravity.Center, CompositeOperator.Over);
            compositeImage.Composite(drawing1Rotated.MagickImage, Gravity.Center, CompositeOperator.Over);

            var drawing1RotatedPivot = drawing1Rotated.GetPoint("Pivot");
            var drawing1RotatedTail = drawing1Rotated.GetPoint("Tail");

            const int testPointBlueSize = 40;
            const int testPointRedSize = 25;
            var testPointBlue = new MagickImage(MagickColor.FromRgb(0, 0, 255), testPointBlueSize, testPointBlueSize);
            var testPointRed = new MagickImage(MagickColor.FromRgb(255, 0, 0), testPointRedSize, testPointRedSize);

            var rotatedPivotShift = drawing1RotatedPivot.Subtract(drawing1RotatedPivot);

            //compositeImage.Composite(testPointBlue, Gravity.Center, drawing1Pivot, CompositeOperator.Over);
            //compositeImage.Composite(testPointBlue, Gravity.Center, drawing1Tail, CompositeOperator.Over);
            //compositeImage.Composite(testPointRed, Gravity.Center, drawing1RotatedPivot.Add(rotatedPivotShift), CompositeOperator.Over);
            //compositeImage.Composite(testPointRed, Gravity.Center, drawing1RotatedTail.Add(rotatedPivotShift), CompositeOperator.Over);
            
            PreviewImgInWpf(compositeImage);
#endif
#if true
            const int widthOfCanvasForHuman = 2 * OutputScreenWidth;

            const int legTopAngle = 70;

            const string human0Name = "ondra0";

            var human0DirPath = Path.Combine(_imagesDirPath, human0Name);
           
            var animationPercentageState = new AnimationPercentageState();

            var bodyAngleAnimParam = new AnimationParameter(0, 50, animationPercentageState);

            MeasureTime(() =>
                Animate(0, 0.2, animationPercentageState, () =>
                {
                    MagickImage oneCycleRetVal = default;
                    MeasureTime(() =>
                    {
                        var human = new Human(
                            human0DirPath,
                            human0Name,
                            _graphicsCache,
                            new Point(widthOfCanvasForHuman, widthOfCanvasForHuman / OutputScreenRatio),
                            new Dictionary<HumanSkeletonPartName, double>
                            {
                            { HumanSkeletonPartName.Body, bodyAngleAnimParam.CurrentValue },
                            { HumanSkeletonPartName.Head, 0 },
                            { HumanSkeletonPartName.LegLTop, legTopAngle },
                            { HumanSkeletonPartName.LegLBottom, legTopAngle },
                            }
                        );
                        var background = new MagickImage(MagickColor.FromRgb(200, 255, 255), widthOfCanvasForHuman, (int)(widthOfCanvasForHuman / OutputScreenRatio));
                        background.Composite(human.MagickImage, Gravity.Center, CompositeOperator.Over);
                        oneCycleRetVal = background;
                    },
                    "one cycle");
                    return oneCycleRetVal;
                }),
                "whole animation");

            PreviewImgInWpf($@"C:\Users\ondrej\MagickPhotoAnimationLib\out\sequence\{(_outputIndex - 1).ToString("00")}.jpg");
            //Application.Current.Shutdown();
#endif
#if false
            var startImg = new MagickImage(Path.Combine(_imagesDirPath, "1.jpg"));
            
            var endCropWidth = (int)(startImg.Width * 0.7);

            var animationPercentageState = new AnimationPercentageState();

            var cropWidthAnimParam = new AnimationParameter(startImg.Width, endCropWidth, animationPercentageState);
            var cropHeightAnimParam = new AnimationParameter(startImg.Height, endCropWidth / OutputScreenRatio, animationPercentageState);

            Animate(10.3, 10.6, animationPercentageState, () =>
            {
                var newImage = startImg.Clone();
                newImage.Crop((int)cropWidthAnimParam.CurrentValue, (int)cropHeightAnimParam.CurrentValue, Gravity.Center);
                return newImage;
            });

            startImg.Dispose();
            
            Application.Current.Shutdown();
#endif
        }

        private static void MeasureTime(Action action, string nameOfMeasurement)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            File.AppendAllText(@"C:\Users\ondrej\MagickPhotoAnimationLib\out\log.txt", $"Elapsed Time of {nameOfMeasurement}: {(float)stopwatch.ElapsedMilliseconds / 1000} s{Environment.NewLine}");
        }

        private void WriteAndDispose(IMagickImage img)
        {
            img.Resize(OutputScreenWidth, (int)(OutputScreenWidth / OutputScreenRatio));
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

        private void PreviewImgInWpf(string imgPath)
        {
            var imgOnDisk = new MagickImage(imgPath);
            var previewImgWidth = imgOnDisk.Width * ObfuscationMinimization;
            var previewImgHeight = imgOnDisk.Height * ObfuscationMinimization;
            Width = previewImgWidth;
            Height = previewImgHeight;
            Top = SystemParameters.PrimaryScreenHeight - previewImgHeight - 45;
            Image1.Source = new BitmapImage(new Uri(imgPath));
        }

        private void Animate(double animStartTime, double animEndTime, AnimationPercentageState animationPercentageState,
            Func<IMagickImage> newImageCreator)
        {
            var animFrameCount = Math.Ceiling((animEndTime - animStartTime) * OutputFrameRate);

            for (int i = 0; i < animFrameCount; i++)
            {
                animationPercentageState.Value = i / animFrameCount;
                var newIMage = newImageCreator();
                WriteAndDispose(newIMage);
            }
        }
    }
}

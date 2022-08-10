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
        private static float[] ObfuscationMinimizations = new float[] { 1, 0.1f };
        private static float ObfuscationMinimization = ObfuscationMinimizations[1];

        private const float OutputScreenRatio = 1.5f;
        private const int OutputFrameRate = 1;

        private int _outputIndex = 0;

        public MainWindow()
        {
            InitializeComponent();

            Top = SystemParameters.PrimaryScreenHeight - Image1.Height - 85;

            var startImg = new MagickImage(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\1.jpg");

            const float animTimeInSec = 3.572f;

            var startCropWidth = startImg.Width;
            var startCropHeight = startImg.Height;

            var endCropWidth = 3000;
            var endCropHeight = (int)(endCropWidth / OutputScreenRatio);

            startImg.Write(@"C:\Users\ondrej\MagickPhotoAnimationLib\out\start_end\1_start.jpg");

            var endImg = startImg.Clone();

            endImg.Crop(endCropWidth, endCropHeight, Gravity.Center);
            endImg.Write(@"C:\Users\ondrej\MagickPhotoAnimationLib\out\start_end\2_end.jpg");

            var animFrameCount = Math.Ceiling(animTimeInSec * OutputFrameRate);

            IMagickImage currentImg;

            for (int i = 0; i < animFrameCount; i++)
            {
                currentImg = startImg.Clone();

                const float OutputFrameTimeInSec = 1 / OutputFrameRate;
                var animTimePositionInSec = (i + 0.5) * OutputFrameTimeInSec;
                var animTimePositionRatio = animTimePositionInSec / animTimeInSec;

                var currentCropWidth = startCropWidth + (int)((endCropWidth - startCropWidth) * animTimePositionRatio);
                var currentCropHeight = startCropHeight + (int)((endCropHeight - startCropHeight) * animTimePositionRatio);

                currentImg.Crop(currentCropWidth, currentCropHeight, Gravity.Center);

                WriteAndDispose(currentImg);
            }

            startImg.Dispose();
        }

        private void WriteAndDispose(IMagickImage img)
        {
            img.Resize(new Percentage(ObfuscationMinimization * 100));
            img.Write($@"C:\Users\ondrej\MagickPhotoAnimationLib\out\sequence\{_outputIndex.ToString("00")}.jpg");
            img.Dispose();
            _outputIndex++;
        }
    }
}

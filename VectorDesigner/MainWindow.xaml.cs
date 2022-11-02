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
using static System.Net.Mime.MediaTypeNames;

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
            const int circleSize = 40;
            var elipse = new Ellipse() { Height = circleSize, Width = circleSize, Fill = new SolidColorBrush(Colors.Red) };
            var canvas1 = new Canvas();
            Content = canvas1;
#if true
            PreviewImgInWpf(canvas1, @"C:\Users\ondrej\MagickPhotoAnimationLib\images\1.jpg");
#else
            PreviewImgInWpf(canvas1, @"C:\Users\ondrej\MagickPhotoAnimationLib\images\2.jpg");
#endif
            canvas1.Children.Add(elipse);
            Canvas.SetTop(elipse, 100);
            Canvas.SetLeft(elipse, 100);
        }

        private void PreviewImgInWpf(Canvas canvas, string imgFilePath)
        {
            var bitmapImg = new BitmapImage(new Uri(imgFilePath));
            var image = new System.Windows.Controls.Image();
            image.Source = bitmapImg;
            var imageRatio = bitmapImg.Width / bitmapImg.Height;
            if (bitmapImg.Width > bitmapImg.Height)
            {
                image.Width = WindowSize;
                Canvas.SetTop(image, (WindowSize - WindowSize * 1 / imageRatio) / 2);
            }
            else
            {
                image.Height = WindowSize;
                Canvas.SetLeft(image, (WindowSize - WindowSize * imageRatio) / 2);
            }
            canvas.Children.Add(image);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = PointToScreen(Mouse.GetPosition(this));
        }
    }
}

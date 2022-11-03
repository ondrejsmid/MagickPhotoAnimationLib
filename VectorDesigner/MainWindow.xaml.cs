using System;
using System.Collections.Generic;
using System.Drawing;
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
using Image = System.Windows.Controls.Image;
using Point = System.Windows.Point;

namespace VectorDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly float[] ObfuscationMinimizations = { 1, 0.3f };
        private static readonly float ObfuscationMinimization = ObfuscationMinimizations[0];

        private static readonly int WindowSize = (int)(800 * ObfuscationMinimization);
        private static readonly int WindowClientSize = WindowSize - 140;
        const int CircleSize = 20;

        private Ellipse _selectedCircle;
        private TextBlock _selectedCircleText;

        private System.Windows.Point _selectedPercentagePositionWithinImage;
        Image _wpfImage;
        int _wpfImageWidth;
        int _wpfImageHeight;
        public MainWindow()
        {
            InitializeComponent();

            Width = WindowSize;
            Height = WindowSize;
            Top = SystemParameters.PrimaryScreenHeight - Height - 45;

            var canvas1 = new Canvas();
            Content = canvas1;

            var circle = new Ellipse() { Height = CircleSize, Width = CircleSize, Fill = new SolidColorBrush(Colors.Red) };

            _selectedCircle = circle;

#if true
            var bitmapImg = new BitmapImage(new Uri(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\1.jpg"));
#else
            var bitmapImg = new BitmapImage(new Uri(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\2.jpg"));
#endif
            PreviewImgInWpf(canvas1, bitmapImg);

            canvas1.Children.Add(circle);

            var circleText = new TextBlock { Text = "abcgggg", FontSize = 20 };
            circleText.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            canvas1.Children.Add(circleText);
            _selectedCircleText = circleText;
        }

        private void PreviewImgInWpf(Canvas canvas, BitmapImage bitmapImg)
        {
            _wpfImage = new Image();
            _wpfImage.Source = bitmapImg;
            
            var imageRatio = _wpfImage.Source.Width / _wpfImage.Source.Height;
            
            if (bitmapImg.Width > bitmapImg.Height)
            {
                _wpfImage.Width = WindowClientSize;
                _wpfImageWidth = (int)_wpfImage.Width;
                _wpfImageHeight = (int)(_wpfImageWidth / imageRatio);
            }
            else
            {
                _wpfImage.Height = WindowClientSize;
                _wpfImageHeight = (int)_wpfImage.Height;
                _wpfImageWidth = (int)(_wpfImageHeight / imageRatio);
            }
            Canvas.SetTop(_wpfImage, 60);
            Canvas.SetLeft(_wpfImage, 60);
            canvas.Children.Add(_wpfImage);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = PointToScreen(Mouse.GetPosition(this));
            
            var circleCanvasLeft = mousePos.X - Left - CircleSize / 2 - 8;
            var circleCanvasTop = mousePos.Y - Top - CircleSize / 2 - 31;

            var selectedPointWithinWpfImage = new Point(circleCanvasLeft - 50, circleCanvasTop - 50);
          
            if (selectedPointWithinWpfImage.X > 0 && selectedPointWithinWpfImage.X < _wpfImageWidth &&
                selectedPointWithinWpfImage.Y > 0 && selectedPointWithinWpfImage.Y < _wpfImageHeight)
            {
                _selectedPercentagePositionWithinImage = new Point(
                    selectedPointWithinWpfImage.X / _wpfImageWidth,
                    selectedPointWithinWpfImage.Y / _wpfImageHeight);
                Canvas.SetLeft(_selectedCircle, circleCanvasLeft);
                Canvas.SetTop(_selectedCircle, circleCanvasTop);
                Canvas.SetLeft(_selectedCircleText, circleCanvasLeft + 20);
                Canvas.SetTop(_selectedCircleText, circleCanvasTop);
            }
        }
    }
}

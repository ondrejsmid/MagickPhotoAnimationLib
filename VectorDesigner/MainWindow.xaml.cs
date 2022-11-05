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
using VectorDesigner.Models;
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
        const int CircleSize = 20;

        private static readonly float[] ObfuscationMinimizations = { 1, 0.3f };
        private static readonly float ObfuscationMinimization = ObfuscationMinimizations[0];

        private static readonly int WindowSize = (int)(800 * ObfuscationMinimization);
        private static readonly int WindowClientSize = WindowSize - 140;

        private static readonly Dictionary<string, string[]> VectorTypes = new Dictionary<string, string[]>
        {
            { "Limb", new string[] { "Top", "Bottom" } }
        };


        private int _wpfImageWidth;
        private int _wpfImageHeight;
        private string _vectorTypeKey;
        private Point?[] _vectorPoints;
        private int _vectorPointIndex;

        private Canvas _canvas;
        private Image _wpfImage;
        private CircleWithTextBlock[] _insideNavigation;
        private TextBlock[] _bottomNavigation;

        private CanvasPositionToImagePercentagePositionConvertor _canvasPositionToImagePercentageConvertor;

        public MainWindow()
        {

            InitializeComponent();

            Width = WindowSize;
            Height = WindowSize;
            Top = SystemParameters.PrimaryScreenHeight - Height - 45;

            var canvas1 = new Canvas();
            Content = canvas1;
            _canvas = canvas1;
#if true
            var bitmapImg = new BitmapImage(new Uri(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\1.jpg"));
#else
            var bitmapImg = new BitmapImage(new Uri(@"C:\Users\ondrej\MagickPhotoAnimationLib\images\2.jpg"));
#endif
            _vectorTypeKey = "Limb";
            _vectorPoints = new Point?[VectorTypes[_vectorTypeKey].Count()];
            _vectorPointIndex = 0;
            _vectorPoints[0] = new Point(0, 0);
            _vectorPoints[1] = new Point(0.5, 0.5);

            _insideNavigation = VectorTypes[_vectorTypeKey]
                .Select(x => new CircleWithTextBlock
                {
                    Circle = new Ellipse() { Height = CircleSize, Width = CircleSize, Fill = new SolidColorBrush(Colors.Blue) },
                    TextBlock = new TextBlock { Text = x, FontSize = 20, Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0)) }
                })
                .ToArray();
            
            DrawBottomNavigation();
            PreviewImgInWpf(canvas1, bitmapImg);
            RedrawInsideNavigation();
            RedrawHighlightingOfNavigationSelection();

            var testCircle = new Ellipse() { Height = 5, Width = 5, Fill = new SolidColorBrush(Colors.Red) };
            _canvas.Children.Add(testCircle);
            Canvas.SetLeft(testCircle, 988);
            Canvas.SetTop(testCircle, 0);
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
                _wpfImageWidth = (int)(_wpfImageHeight / (1 / imageRatio));
            }
            Canvas.SetTop(_wpfImage, 60);
            Canvas.SetLeft(_wpfImage, 60);
            canvas.Children.Add(_wpfImage);
            _canvasPositionToImagePercentageConvertor = new CanvasPositionToImagePercentagePositionConvertor(_wpfImageWidth, _wpfImageHeight);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = PointToScreen(Mouse.GetPosition(this));
            
            var circleCanvasLeft = mousePos.X - Left - CircleSize / 2 - 8;
            var circleCanvasTop = mousePos.Y - Top - CircleSize / 2 - 31;

            var imagePercentagePosition = _canvasPositionToImagePercentageConvertor
                .ToImagePercentagePosition(new Point(circleCanvasLeft, circleCanvasTop));

            if (imagePercentagePosition != null)
            {
                _vectorPoints[_vectorPointIndex] = imagePercentagePosition;
                RedrawInsideNavigation();
            }
        }

        private void RedrawInsideNavigation()
        {
            for (int i = 0; i < VectorTypes[_vectorTypeKey].Count(); i++)
            {
                _canvas.Children.Remove(_insideNavigation[i].Circle);
                _canvas.Children.Remove(_insideNavigation[i].TextBlock);
            }
            for (int i = 0; i < VectorTypes[_vectorTypeKey].Count(); i++)
            {
                if (_vectorPoints[i] != null)
                {
                    _canvas.Children.Add(_insideNavigation[i].Circle);
                    _canvas.Children.Add(_insideNavigation[i].TextBlock);
                    var vectorPointCanvasPosition = _canvasPositionToImagePercentageConvertor.ToCanvasPosition(_vectorPoints[i].Value);
                    Canvas.SetLeft(_insideNavigation[i].Circle, vectorPointCanvasPosition.X);
                    Canvas.SetTop(_insideNavigation[i].Circle, vectorPointCanvasPosition.Y);
                    Canvas.SetLeft(_insideNavigation[i].TextBlock, vectorPointCanvasPosition.X + 20);
                    Canvas.SetTop(_insideNavigation[i].TextBlock, vectorPointCanvasPosition.Y - 5);
                }
            }
        }

        private void DrawBottomNavigation()
        {
            _bottomNavigation = VectorTypes[_vectorTypeKey]
                .Select(x => new TextBlock { Text = x, FontSize = 20, Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0)) })
                .ToArray();

            for (int i = 0; i < VectorTypes[_vectorTypeKey].Count(); i++)
            {
                _canvas.Children.Add(_bottomNavigation[i]);
                Canvas.SetLeft(_bottomNavigation[i], i * 80 + 5);
                Canvas.SetTop(_bottomNavigation[i], WindowClientSize + 70);
            }
        }

        private void RedrawHighlightingOfNavigationSelection()
        {
            for (int i = 0; i < VectorTypes[_vectorTypeKey].Count(); i++)
            {
                if (i == _vectorPointIndex)
                {
                    _insideNavigation[i].TextBlock.FontWeight = FontWeights.Bold;
                    _bottomNavigation[i].FontWeight = FontWeights.Bold;
                }
                else
                {
                    _insideNavigation[i].TextBlock.FontWeight = FontWeights.Normal;
                    _bottomNavigation[i].FontWeight = FontWeights.Normal;
                }
            }
        }
    }
}

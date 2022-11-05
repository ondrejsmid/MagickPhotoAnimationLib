using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
            { "Limb", new string[] { "Top", "Bottom" } },
            { "Bbb", new string[] { "p1", "p2", "p3" } }
        };

        private string _imagePath;
        private int _wpfImageWidth;
        private int _wpfImageHeight;
        private string _vectorTypeKey;
        private Point?[] _vectorPoints;
        private int _vectorPointIndex;

        private Canvas _canvas;
        private Image _wpfImage;
        private CircleWithTextBlock[] _insideNavigation;
        private TextBlock[] _bottomNavigation;
        private TextBlock _vectorTypeKeyTextBlock;

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
            
           

            var testCircle = new Ellipse() { Height = 5, Width = 5, Fill = new SolidColorBrush(Colors.Red) };
            _canvas.Children.Add(testCircle);
            Canvas.SetLeft(testCircle, 988);
            Canvas.SetTop(testCircle, 0);
        }

        private void DrawWpfImage()
        {
            _canvas.Children.Remove(_wpfImage);

            var bitmapImg = new BitmapImage(new Uri(_imagePath));

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
            _canvas.Children.Add(_wpfImage);
            _canvasPositionToImagePercentageConvertor = new CanvasPositionToImagePercentagePositionConvertor(_wpfImageWidth, _wpfImageHeight);
        }

        private void RedrawInsideNavigation()
        {
            UndrawInsideNavigation();
            DrawInsideNavigation();
        }

        private void UndrawInsideNavigation()
        {
            for (int i = 0; i < VectorTypes[_vectorTypeKey].Count(); i++)
            {
                _canvas.Children.Remove(_insideNavigation[i].Circle);
                _canvas.Children.Remove(_insideNavigation[i].TextBlock);
            }
        }

        private void DrawInsideNavigation()
        {
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

        private void RedrawBottomNavigation()
        {
            UndrawBottomNavigation();
            DrawBottomNavigation();
        }

        private void UndrawBottomNavigation()
        {
            if (_bottomNavigation == null)
            {
                return;
            }
            for (int i = 0; i < VectorTypes[_vectorTypeKey].Count(); i++)
            {
                _canvas.Children.Remove(_bottomNavigation[i]);
            }
            _canvas.Children.Remove(_vectorTypeKeyTextBlock);
        }

        private void DrawBottomNavigation()
        {
            _bottomNavigation = VectorTypes[_vectorTypeKey]
                .Select(x => new TextBlock { Text = x, FontSize = 20, Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0)) })
                .ToArray();
            _vectorTypeKeyTextBlock = new TextBlock { Text = _vectorTypeKey, FontSize = 20, Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0)) };

            for (int i = 0; i < VectorTypes[_vectorTypeKey].Count(); i++)
            {
                _canvas.Children.Add(_bottomNavigation[i]);
                Canvas.SetLeft(_bottomNavigation[i], i * 80 + 5);
                Canvas.SetTop(_bottomNavigation[i], WindowClientSize + 70);
            }
            _canvas.Children.Add(_vectorTypeKeyTextBlock);
            Canvas.SetLeft(_vectorTypeKeyTextBlock, 5);
            Canvas.SetTop(_vectorTypeKeyTextBlock, 0);
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_imagePath == null)
            {
                return;
            }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Right:
                    if (_imagePath == null)
                    {
                        return;
                    }
                    _vectorPointIndex = (_vectorPointIndex + 1) % VectorTypes[_vectorTypeKey].Count();
                    RedrawHighlightingOfNavigationSelection();
                    break;
                case Key.Left:
                    if (_imagePath == null)
                    {
                        return;
                    }
                    _vectorPointIndex = _vectorPointIndex == 0 ? VectorTypes[_vectorTypeKey].Count() - 1 : _vectorPointIndex - 1;
                    RedrawHighlightingOfNavigationSelection();
                    break;
                case Key.S:
                    if (_imagePath == null)
                    {
                        return;
                    }
                    if (_vectorPoints.All(x => x != null))
                    {
                        Save();
                    }
                    break;
                case Key.O:
                    Open();
                    break;
            }
        }

        private void Save()
        {
            var dirPath = System.IO.Path.GetDirectoryName(_imagePath);
            var imageFileName = System.IO.Path.GetFileNameWithoutExtension(_imagePath);
            var content = new StringBuilder();
            content.AppendLine(_vectorTypeKey);
            content.Append(string.Join(Environment.NewLine, _vectorPoints));
            File.WriteAllText(System.IO.Path.Combine(dirPath, $"{imageFileName}{VectorLoader.VectorFileExtension}"), content.ToString());
        }

        private void Open()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\Users\ondrej\MagickPhotoAnimationLib\images";
            openFileDialog.Filter = "Image Files(*.jpg;*.jpeg)|*.jpg;*.jpeg;";
            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }
            _imagePath = openFileDialog.FileName;
            var vector = VectorLoader.Load(_imagePath);
            if (vector == null)
            {
                _vectorTypeKey = VectorTypes.Keys.First();
                _vectorPoints = new Point?[VectorTypes[_vectorTypeKey].Count()];
            }
            else
            {
                _vectorTypeKey = vector.TypeKey;
                _vectorPoints = vector.Points.Cast<Point?>().ToArray();
            }
            
            _vectorPointIndex = 0;

            _insideNavigation = VectorTypes[_vectorTypeKey]
                .Select(x => new CircleWithTextBlock
                {
                    Circle = new Ellipse() { Height = CircleSize, Width = CircleSize, Fill = new SolidColorBrush(Colors.Blue) },
                    TextBlock = new TextBlock { Text = x, FontSize = 20, Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0)) }
                })
                .ToArray();

            RedrawBottomNavigation();
            DrawWpfImage();
            RedrawInsideNavigation();
            RedrawHighlightingOfNavigationSelection();
        }
    }
}

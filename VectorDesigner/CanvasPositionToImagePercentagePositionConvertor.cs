using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = System.Windows.Point;

namespace VectorDesigner
{
    internal class CanvasPositionToImagePercentagePositionConvertor
    {
        private const int XCorrection = 50;
        private const int YCorrection = 50;

        private int _wpfImageWidth;
        private int _wpfImageHeight;

        public CanvasPositionToImagePercentagePositionConvertor(int wpfImageWidth, int wpfImageHeight)
        {
            _wpfImageWidth = wpfImageWidth;
            _wpfImageHeight = wpfImageHeight;
        }

        public Point ToCanvasPosition(Point imagePercentage)
        {
            var x = imagePercentage.X * _wpfImageWidth + XCorrection;
            var y = imagePercentage.Y * _wpfImageHeight + YCorrection;
            return new Point(x, y);
        }

        public Point? ToImagePercentagePosition(Point canvasPosition)
        {
            var selectedPointWithinWpfImage = new Point(canvasPosition.X - XCorrection, canvasPosition.Y - YCorrection);

            if (selectedPointWithinWpfImage.X > 0 && selectedPointWithinWpfImage.X < _wpfImageWidth &&
                selectedPointWithinWpfImage.Y > 0 && selectedPointWithinWpfImage.Y < _wpfImageHeight)
            {
                return new Point(
                    selectedPointWithinWpfImage.X / _wpfImageWidth,
                    selectedPointWithinWpfImage.Y / _wpfImageHeight);
            }
            else
            {
                return null;
            }

        }
    }
}

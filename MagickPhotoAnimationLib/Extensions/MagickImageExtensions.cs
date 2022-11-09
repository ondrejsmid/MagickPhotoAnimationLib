using System;
using System.Collections.Generic;
using System.Text;
using ImageMagick;

namespace MagickPhotoAnimationLib.Extensions
{
    internal static class MagickImageExtensions
    {
        internal static MagickImage GetRotated(this MagickImage magickImage, double degrees, System.Windows.Point pivotPoint)
        {
            var rotated = magickImage.Clone();

            var centerPoint = new System.Windows.Point(rotated.Width / 2, rotated.Height / 2);

            rotated.Rotate(degrees);

            var rotationAngleInRad = degrees * (Math.PI / 180);
            var aPoint = pivotPoint;
            var a = centerPoint.Y - aPoint.Y;
            var b = centerPoint.X - aPoint.X;
            var r = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            var alpha = Math.Asin(a / r);
            var aRotated = r * Math.Sin(alpha + rotationAngleInRad);
            var bRotated = r * Math.Cos(alpha + rotationAngleInRad);
            var yShift = aRotated - a;
            var xShift = bRotated - b;

            var canvasImage = new MagickImage(MagickColor.FromRgba(0, 0, 0, 0), 800, 800);
            canvasImage.Composite(rotated, Gravity.Center, (int)xShift, (int)yShift, CompositeOperator.Over);
            return canvasImage;
        }
    }
}

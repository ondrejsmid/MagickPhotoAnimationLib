using ImageMagick;
using MagickPhotoAnimationLib.MathUtils;
using Point = System.Windows.Point;

namespace MagickPhotoAnimationLib.Extensions
{
    internal static class MagickImageExtensions
    {
        internal static MagickImage GetRotated(this MagickImage magickImage, double degrees, Point pivotPoint, int targetCanvasImageSize)
        {
            var centerPoint = new Point(magickImage.Width / 2, magickImage.Height / 2);

            var rotated = magickImage.Clone();
            rotated.Rotate(degrees);

            var pivotPointShift = Rotation.GetRotatedPointShift(centerPoint.Add(pivotPoint), degrees, centerPoint);

            var canvasImage = new MagickImage(MagickColor.FromRgba(0, 0, 0, 0), targetCanvasImageSize, targetCanvasImageSize);
            canvasImage.Composite(rotated, Gravity.Center, (int)pivotPointShift.X, (int)pivotPointShift.Y, CompositeOperator.Over);

            return canvasImage;
        }
    }
}

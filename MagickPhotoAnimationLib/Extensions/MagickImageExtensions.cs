using ImageMagick;
using MagickPhotoAnimationLib.MathUtils;
using Point = System.Windows.Point;

namespace MagickPhotoAnimationLib.Extensions
{
    internal static class MagickImageExtensions
    {
        internal static void Composite(this MagickImage thisImage, IMagickImage otherImage, Gravity gravity, Point offset, CompositeOperator compose)
        {
            thisImage.Composite(otherImage, gravity, (int)offset.X, (int)offset.Y, compose);
        }

        internal static MagickImage GetRotated(this MagickImage thisImage, double degrees, Point pivotPoint, int targetCanvasImageSize)
        {
            var centerPoint = new Point(thisImage.Width / 2, thisImage.Height / 2);

            var rotated = thisImage.Clone();
            rotated.Rotate(degrees);

            var pivotPointShift = Rotation.GetRotatedPointShift(centerPoint.Add(pivotPoint), degrees, centerPoint);

            var canvasImage = new MagickImage(MagickColor.FromRgba(0, 0, 0, 0), targetCanvasImageSize, targetCanvasImageSize);
            canvasImage.Composite(rotated, Gravity.Center, (int)pivotPointShift.X, (int)pivotPointShift.Y, CompositeOperator.Over);

            return canvasImage;
        }
    }
}

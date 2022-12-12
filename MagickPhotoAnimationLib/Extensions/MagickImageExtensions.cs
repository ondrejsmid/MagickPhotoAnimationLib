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
            var rotated = thisImage.Clone();
            rotated.Rotate(-degrees);

            var rotatedPivotShift = pivotPoint.Subtract(Rotation.GetPointRotatedByCenter(pivotPoint, degrees));

            var canvasImage = new MagickImage(MagickColor.FromRgba(0, 0, 0, 0), targetCanvasImageSize, targetCanvasImageSize);
            canvasImage.Composite(rotated, Gravity.Center, (int)rotatedPivotShift.X, (int)rotatedPivotShift.Y, CompositeOperator.Over);

            return canvasImage;
        }
    }
}

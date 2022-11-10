using Point = System.Windows.Point;

namespace MagickPhotoAnimationLib.Extensions
{
    internal static class PointExtensions
    {
        internal static Point Add(this Point thisPoint, Point otherPoint)
        {
            return new Point(thisPoint.X + otherPoint.X, thisPoint.Y + otherPoint.Y);
        }

        internal static Point Subtract(this Point thisPoint, Point otherPoint)
        {
            return new Point(thisPoint.X - otherPoint.X, thisPoint.Y - otherPoint.Y);
        }
    }
}

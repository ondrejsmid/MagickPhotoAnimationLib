using System;
using System.Collections.Generic;
using System.Text;
using Point = System.Windows.Point;

namespace MagickPhotoAnimationLib.MathUtils
{
    internal static class Rotation
    {
        internal static Point GetRotatedPointShift(Point pointToRotate, double degrees, Point center)
        {
            if (pointToRotate == center)
            {
                return new Point(0, 0);
            }

            var rotationAngleInRad = degrees * (Math.PI / 180);
            var a = center.Y - pointToRotate.Y;
            var b = center.X - pointToRotate.X;
            var r = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            var alpha = Math.Asin(a / r);
            var aRotated = r * Math.Sin(alpha + rotationAngleInRad);
            var bRotated = r * Math.Cos(alpha + rotationAngleInRad);
            var yShift = aRotated - a;
            var xShift = bRotated - b;

            if (pointToRotate.X > 162 && pointToRotate.X < 163)
            {
            }

            return new Point(xShift, yShift);
        }
    }
}

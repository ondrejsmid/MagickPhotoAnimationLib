using System;
using Point = System.Windows.Point;

namespace MagickPhotoAnimationLib.MathUtils
{
    internal static class Rotation
    {
        /*
         pointToRotate must be expressed in a form of relative pixels' distance from image's center.
         In such model, the center point is considered to be (0, 0).
         For example if pointToRotate is (71, -106), it means it is a point which is 71 pixels to the right from
         image center and 106 pixels above image center.
        */
        internal static Point GetPointRotatedByCenter(Point pointToRotate, double degrees)
        {
            var rotationAngleInRad = degrees * (Math.PI / 180);
            var xCenterOffset = pointToRotate.X;
            var yCenterOffset = -pointToRotate.Y; /* The minus is because points, which are above center, have positive Sin */
            var r = Math.Sqrt(Math.Pow(yCenterOffset, 2) + Math.Pow(xCenterOffset, 2));
            
            var mathAsin = Math.Asin(yCenterOffset / r); /* Represent possible 2 solutions for an initial angle of pointToRotate */
            double alpha;
            if (xCenterOffset > 0)
            {
                alpha = mathAsin;
            }
            else
            {
                alpha = Math.PI - mathAsin;
            }

            var xCenterOffsetRotated = r * Math.Cos(alpha + rotationAngleInRad);
            var yCenterOffsetRotated = r * Math.Sin(alpha + rotationAngleInRad);

            return new Point(xCenterOffsetRotated, -yCenterOffsetRotated);  /* The minus is because points, which are above center, have negative center offset. */
        }
    }
}

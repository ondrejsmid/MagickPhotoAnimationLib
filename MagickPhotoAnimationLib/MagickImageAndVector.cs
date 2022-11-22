using System;
using System.Linq;
using ImageMagick;
using MagickPhotoAnimationLib.Extensions;
using MagickPhotoAnimationLib.MathUtils;
using VectorDesigner;
using Point = System.Windows.Point;

namespace MagickPhotoAnimationLib
{
    internal class MagickImageAndVector
    {
        public MagickImage MagickImage;
        public Vector Vector;

        public MagickImageAndVector(string imgPath)
        {
            MagickImage = new MagickImage(imgPath);
            MagickImage.BackgroundColor = MagickColors.Transparent;
            Vector = VectorLoader.LoadAsPixelShiftFromCenter(imgPath, MagickImage.Width, MagickImage.Height);
        }

        private MagickImageAndVector(){}

        public Point GetPoint(string name)
        {
            return Vector.Points[VectorLoader.VectorTypes[Vector.TypeKey].TakeWhile(x => x != name).Count()];
        }

        public Point Pivot
        {
            get => GetPoint("Pivot");
        }

        public MagickImageAndVector GetRotated(double degrees, Point pivotPoint)
        {
            var canvasImageSize = 2 * (int)Math.Sqrt(Math.Pow(MagickImage.Width, 2) + Math.Pow(MagickImage.Height, 2));
            var canvasCenter = new Point(canvasImageSize / 2, canvasImageSize / 2);

            var vectorRotated = new Vector
            {
                TypeKey = Vector.TypeKey,
                Points = Vector.Points.Select(p =>
                {
                    var newPWithoutPivotShift = p.Subtract(Rotation.GetRotatedPointShift(p, degrees, new Point(0, 0)));
                    return newPWithoutPivotShift;
                }).ToArray()
            };

            var newMagickImage = MagickImage.GetRotated(degrees, pivotPoint, canvasImageSize);

            return new MagickImageAndVector
            {
                MagickImage = newMagickImage,
                Vector = vectorRotated
            };
        }

        public void Rotate(double degrees, Point pivotPoint)
        {
            var rotated = GetRotated(degrees, pivotPoint);
            MagickImage = rotated.MagickImage;
            Vector = rotated.Vector;
        }
    }
}

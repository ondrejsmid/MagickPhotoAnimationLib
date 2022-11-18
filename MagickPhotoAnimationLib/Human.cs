using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImageMagick;
using MagickPhotoAnimationLib.Extensions;
using Point = System.Windows.Point;

namespace MagickPhotoAnimationLib
{
    internal class Human
    {
        private readonly Point _canvasSize;
        private readonly MagickImageAndVector _head;
        private readonly MagickImageAndVector _body;
        private readonly MagickImageAndVector _armRTop;
        private readonly MagickImageAndVector _armLTop;
        private readonly MagickImageAndVector _armRBottom;
        private readonly MagickImageAndVector _armLBottom;
        private readonly MagickImageAndVector _legRTop;
        private readonly MagickImageAndVector _legLTop;
        private readonly MagickImageAndVector _legRBottom;
        private readonly MagickImageAndVector _legLBottom;

        public Human(string imgDirPath, Point canvasSize)
        {
            _canvasSize = canvasSize;
            _head = new MagickImageAndVector(Path.Combine(imgDirPath, "head.png"));
            _body = new MagickImageAndVector(Path.Combine(imgDirPath, "body.png"));
            _armRTop = new MagickImageAndVector(Path.Combine(imgDirPath, "arm-R-top.png"));
            _armRBottom = new MagickImageAndVector(Path.Combine(imgDirPath, "arm-R-bottom.png"));
            _armLTop = new MagickImageAndVector(Path.Combine(imgDirPath, "arm-L-top.png"));
            _armLBottom = new MagickImageAndVector(Path.Combine(imgDirPath, "arm-L-bottom.png"));
            _legRTop = new MagickImageAndVector(Path.Combine(imgDirPath, "leg-R-top.png"));
            _legRBottom = new MagickImageAndVector(Path.Combine(imgDirPath, "leg-R-bottom.png"));
            _legLTop = new MagickImageAndVector(Path.Combine(imgDirPath, "leg-L-top.png"));
            _legLBottom = new MagickImageAndVector(Path.Combine(imgDirPath, "leg-L-bottom.png"));
        }

        public MagickImage MagickImage
        {
            get
            {
                var mainPivotPointShift = Pivot.Negate();
                var canvasImage = new MagickImage(MagickColor.FromRgba(0, 0, 0, 0), (int)_canvasSize.X, (int)_canvasSize.Y);
                
                canvasImage.Composite(_body.MagickImage, Gravity.Center, mainPivotPointShift, CompositeOperator.Over);
                
                canvasImage.Composite(_head.MagickImage, Gravity.Center, mainPivotPointShift
                    .Add(_body.GetPoint("Head"))
                    .Subtract(_head.GetPoint("Pivot")), CompositeOperator.Over);
                
                canvasImage.Composite(_armRTop.MagickImage, Gravity.Center,
                    mainPivotPointShift
                    .Add(_body.GetPoint("ArmR"))
                    .Subtract(_armRTop.GetPoint("Pivot")), CompositeOperator.Over);

                canvasImage.Composite(_armRBottom.MagickImage, Gravity.Center,
                    mainPivotPointShift
                    .Add(_body.GetPoint("ArmR"))
                    .Subtract(_armRTop.GetPoint("Pivot"))
                    .Add(_armRTop.GetPoint("Tail"))
                    .Subtract(_armRBottom.GetPoint("Pivot")), CompositeOperator.Over);

                canvasImage.Composite(_armLTop.MagickImage, Gravity.Center,
                    mainPivotPointShift
                    .Add(_body.GetPoint("ArmL"))
                    .Subtract(_armLTop.GetPoint("Pivot")), CompositeOperator.Over);

                canvasImage.Composite(_armLBottom.MagickImage, Gravity.Center,
                    mainPivotPointShift
                    .Add(_body.GetPoint("ArmL"))
                    .Subtract(_armLTop.GetPoint("Pivot"))
                    .Add(_armLTop.GetPoint("Tail"))
                    .Subtract(_armLBottom.GetPoint("Pivot")), CompositeOperator.Over);

                canvasImage.Composite(_legRTop.MagickImage, Gravity.Center,
                    mainPivotPointShift
                    .Add(_body.GetPoint("LegR"))
                    .Subtract(_legRTop.GetPoint("Pivot")), CompositeOperator.Over);

                canvasImage.Composite(_legRBottom.MagickImage, Gravity.Center,
                    mainPivotPointShift
                    .Add(_body.GetPoint("LegR"))
                    .Subtract(_legRTop.GetPoint("Pivot"))
                    .Add(_legRTop.GetPoint("Tail"))
                    .Subtract(_legRBottom.GetPoint("Pivot")), CompositeOperator.Over);

                canvasImage.Composite(_legLTop.MagickImage, Gravity.Center,
                    mainPivotPointShift
                    .Add(_body.GetPoint("LegL"))
                    .Subtract(_legLTop.GetPoint("Pivot")), CompositeOperator.Over);

                canvasImage.Composite(_legLBottom.MagickImage, Gravity.Center,
                    mainPivotPointShift
                    .Add(_body.GetPoint("LegL"))
                    .Subtract(_legLTop.GetPoint("Pivot"))
                    .Add(_legLTop.GetPoint("Tail"))
                    .Subtract(_legLBottom.GetPoint("Pivot")), CompositeOperator.Over);

                return canvasImage;
            }
        }

        public Point Pivot
        {
            get => _body.GetPoint("Pivot");
        }
    }
}

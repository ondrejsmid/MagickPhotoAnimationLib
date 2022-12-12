using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private readonly SkeletonPart _skeleton;
        private readonly Dictionary<HumanSkeletonPartName, double> _skeletonRotations = new Dictionary<HumanSkeletonPartName, double>();
        private readonly Dictionary<HumanSkeletonPartName, Point> _skeletonRotationShifts = new Dictionary<HumanSkeletonPartName, Point>();

        public Human(string imgDirPath, Point canvasSize)
        {
            _canvasSize = canvasSize;
            _body = new MagickImageAndVector(Path.Combine(imgDirPath, "body.png"));
            _head = new MagickImageAndVector(Path.Combine(imgDirPath, "head.png"));
            _armRTop = new MagickImageAndVector(Path.Combine(imgDirPath, "arm-R-top.png"));
            _armRBottom = new MagickImageAndVector(Path.Combine(imgDirPath, "arm-R-bottom.png"));
            _armLTop = new MagickImageAndVector(Path.Combine(imgDirPath, "arm-L-top.png"));
            _armLBottom = new MagickImageAndVector(Path.Combine(imgDirPath, "arm-L-bottom.png"));
            _legRTop = new MagickImageAndVector(Path.Combine(imgDirPath, "leg-R-top.png"));
            _legRBottom = new MagickImageAndVector(Path.Combine(imgDirPath, "leg-R-bottom.png"));
            _legLTop = new MagickImageAndVector(Path.Combine(imgDirPath, "leg-L-top.png"));
            _legLBottom = new MagickImageAndVector(Path.Combine(imgDirPath, "leg-L-bottom.png"));

            _skeleton = new SkeletonPart
            {
                HumanSkeletonPartName = HumanSkeletonPartName.Body,
                MagickImageAndVector = _body,
                Children = new[]
                {
                    new SkeletonPart
                    {
                       HumanSkeletonPartName = HumanSkeletonPartName.Head,
                       ParentTailPointName = "Head",
                       MagickImageAndVector = _head
                    },
                    new SkeletonPart
                    {
                       HumanSkeletonPartName = HumanSkeletonPartName.ArmRTop,
                       ParentTailPointName = "ArmR",
                       MagickImageAndVector = _armRTop,
                       Children = new[]
                       {
                           new SkeletonPart
                           {
                               HumanSkeletonPartName = HumanSkeletonPartName.ArmRBottom,
                               ParentTailPointName = "Tail",
                               MagickImageAndVector = _armRBottom,
                           }
                       }
                    },
                    new SkeletonPart
                    {
                       HumanSkeletonPartName = HumanSkeletonPartName.ArmLTop,
                       ParentTailPointName = "ArmL",
                       MagickImageAndVector = _armLTop,
                       Children = new[]
                       {
                           new SkeletonPart
                           {
                               HumanSkeletonPartName = HumanSkeletonPartName.ArmLBottom,
                               ParentTailPointName = "Tail",
                               MagickImageAndVector = _armLBottom,
                           }
                       }
                    },
                    new SkeletonPart
                    {
                       HumanSkeletonPartName = HumanSkeletonPartName.LegRTop,
                       ParentTailPointName = "LegR",
                       MagickImageAndVector = _legRTop,
                       Children = new[]
                       {
                           new SkeletonPart
                           {
                               HumanSkeletonPartName = HumanSkeletonPartName.LegRBottom,
                               ParentTailPointName = "Tail",
                               MagickImageAndVector = _legRBottom,
                           }
                       }
                    },
                    new SkeletonPart
                    {
                       HumanSkeletonPartName = HumanSkeletonPartName.LegLTop,
                       ParentTailPointName = "LegL",
                       MagickImageAndVector = _legLTop,
                       Children = new[]
                       {
                           new SkeletonPart
                           {
                               HumanSkeletonPartName = HumanSkeletonPartName.LegLBottom,
                               ParentTailPointName = "Tail",
                               MagickImageAndVector = _legLBottom,
                           }
                       }
                    },
                }
            };
        }

        public Human(string imgDirPath, Point canvasSize, Dictionary<HumanSkeletonPartName, double> skeletonRotations)
            :
            this(imgDirPath, canvasSize)
        {
            _skeletonRotations = skeletonRotations;
        }
        public MagickImage MagickImage
        {
            get
            {
                ComputeRotationShift(HumanSkeletonPartName.Body, _body);
                ComputeRotationShift(HumanSkeletonPartName.Head, _head);
                ComputeRotationShift(HumanSkeletonPartName.ArmRTop, _armRTop);
                ComputeRotationShift(HumanSkeletonPartName.ArmRBottom, _armRBottom);
                ComputeRotationShift(HumanSkeletonPartName.ArmLTop, _armLTop);
                ComputeRotationShift(HumanSkeletonPartName.ArmLBottom, _armLBottom);
                ComputeRotationShift(HumanSkeletonPartName.LegRTop, _legRTop);
                ComputeRotationShift(HumanSkeletonPartName.LegRBottom, _legRBottom);
                ComputeRotationShift(HumanSkeletonPartName.LegLTop, _legLTop);
                ComputeRotationShift(HumanSkeletonPartName.LegLBottom, _legLBottom);

                var canvasImage = new MagickImage(MagickColor.FromRgba(0, 0, 0, 0), (int)_canvasSize.X, (int)_canvasSize.Y);

                var shiftsForComposition = GetShiftsForComposition();

                foreach (var child in new[] {
                    _head,
                    _armRTop,
                    _armRBottom,
                    _armLTop,
                    _armLBottom,
                    _legRTop,
                    _legRBottom,
                    _legLTop,
                    _legLBottom,
                    _body
                }.Reverse())
                {
                    canvasImage.Composite(child.MagickImage, Gravity.Center, shiftsForComposition[child], CompositeOperator.Over);
                }

                return canvasImage;
            }
        }

        public Point Pivot
        {
            get => _body.GetPoint("Pivot");
        }

        private Dictionary<MagickImageAndVector, Point> GetShiftsForComposition()
        {
            var shiftsForComposition = new Dictionary<MagickImageAndVector, Point>();
            GetShiftsForComposition(_skeleton, ref shiftsForComposition, Pivot.Negate());
            return shiftsForComposition;
        }

        private void GetShiftsForComposition(SkeletonPart skeletonPart, ref Dictionary<MagickImageAndVector, Point> shiftsForComposition, Point accumulatedShift)
        {
            var rotatedShift =
                _skeletonRotationShifts.ContainsKey(skeletonPart.HumanSkeletonPartName)
                ? _skeletonRotationShifts[skeletonPart.HumanSkeletonPartName]
                : new Point(0, 0);

            shiftsForComposition[skeletonPart.MagickImageAndVector] = accumulatedShift.Add(rotatedShift);

            if (skeletonPart.Children != null)
            {
                foreach (var child in skeletonPart.Children)
                {
                    if (child.HumanSkeletonPartName == HumanSkeletonPartName.ArmLTop)
                    {

                    }

                    var parentTailPointName = child.ParentTailPointName == null ? new Point(0, 0) : skeletonPart.MagickImageAndVector.GetPoint(child.ParentTailPointName);
                    GetShiftsForComposition(child, ref shiftsForComposition,
                        accumulatedShift.Add(parentTailPointName).Subtract(child.MagickImageAndVector.GetPoint("Pivot")));
                }
            }
        }

        private void ComputeRotationShift(HumanSkeletonPartName humanSkeletonPartName, MagickImageAndVector skeletonPartMagickImageAndVector)
        {
            if (!_skeletonRotations.ContainsKey(humanSkeletonPartName))
            {
                return;
            }
            var pivotBefore = skeletonPartMagickImageAndVector.Pivot;
            skeletonPartMagickImageAndVector.Rotate(_skeletonRotations[humanSkeletonPartName], skeletonPartMagickImageAndVector.Pivot);
            var pivotAfter = skeletonPartMagickImageAndVector.Pivot;
            var rotationShift = pivotAfter.Subtract(pivotBefore);
            _skeletonRotationShifts[humanSkeletonPartName] = rotationShift;
        }
    }
}

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
        private MagickImage _finalMagickImage;
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

        private readonly Dictionary<HumanSkeletonPartName, MagickImageAndVector> _partNameToMagickImageAndVector;
        private readonly Dictionary<HumanSkeletonPartName, SkeletonPart> _partNameToSkeletonPart = new Dictionary<HumanSkeletonPartName, SkeletonPart>();

        private readonly SkeletonPart _skeleton;
        private readonly Dictionary<HumanSkeletonPartName, double> _skeletonRotations = new Dictionary<HumanSkeletonPartName, double>();
        private readonly Dictionary<HumanSkeletonPartName, Point> _skeletonRotationShifts = new Dictionary<HumanSkeletonPartName, Point>();
        private Dictionary<HumanSkeletonPartName, Point> _shiftsForComposition = new Dictionary<HumanSkeletonPartName, Point>();
       
        public Human(string imgDirPath, Point canvasSize, Dictionary<HumanSkeletonPartName, double> skeletonRotations)
        {
            _skeletonRotations = skeletonRotations;

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

            _partNameToMagickImageAndVector = new Dictionary<HumanSkeletonPartName, MagickImageAndVector>
            {
                { HumanSkeletonPartName.Body, _body },
                { HumanSkeletonPartName.Head, _head },
                { HumanSkeletonPartName.ArmRTop, _armRTop },
                { HumanSkeletonPartName.ArmRBottom, _armRBottom },
                { HumanSkeletonPartName.ArmLTop, _armLTop },
                { HumanSkeletonPartName.ArmLBottom, _armLBottom },
                { HumanSkeletonPartName.LegRTop, _legRTop },
                { HumanSkeletonPartName.LegRBottom, _legRBottom },
                { HumanSkeletonPartName.LegLTop, _legLTop },
                { HumanSkeletonPartName.LegLBottom, _legLBottom }
            };

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
                               MagickImageAndVector = _armRBottom
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

            ComputeMagickImage();
        }

        public MagickImage MagickImage => _finalMagickImage;

        public Point Pivot
        {
            get => _body.GetPoint("Pivot");
        }

        /*
        public static void StoreGraphicsToCache(GraphicsCache graphicsCache, string humanName, string humanDirPath)
        {
            graphicsCache.Set($"{humanName}-body", new MagickImageAndVector(Path.Combine(humanDirPath, "body.png")));
            graphicsCache.Set($"{humanName}-head", new MagickImageAndVector(Path.Combine(humanDirPath, "head.png")));
            graphicsCache.Set($"{humanName}-arm-R-top", new MagickImageAndVector(Path.Combine(humanDirPath, "arm-R-top.png")));
            graphicsCache.Set($"{humanName}-arm-R-bottom", new MagickImageAndVector(Path.Combine(humanDirPath, "arm-R-bottom.png")));
            graphicsCache.Set($"{humanName}-arm-L-top", new MagickImageAndVector(Path.Combine(humanDirPath, "arm-L-top.png")));
            graphicsCache.Set($"{humanName}-arm-L-bottom", new MagickImageAndVector(Path.Combine(humanDirPath, "arm-L-bottom.png")));
            graphicsCache.Set($"{humanName}-leg-R-top", new MagickImageAndVector(Path.Combine(humanDirPath, "leg-R-top.png")));
            graphicsCache.Set($"{humanName}-leg-R-bottom", new MagickImageAndVector(Path.Combine(humanDirPath, "leg-R-bottom.png")));
            graphicsCache.Set($"{humanName}-leg-L-top", new MagickImageAndVector(Path.Combine(humanDirPath, "leg-L-top.png")));
            graphicsCache.Set($"{humanName}-leg-L-bottom", new MagickImageAndVector(Path.Combine(humanDirPath, "leg-L-bottom.png")));
        }
        */

        public Point GetShiftForCompositionWithExternalImage(HumanSkeletonPartName humanPartNameToJoinWith, Point externalImagePivot)
        {
            var accumulatedShift = _shiftsForComposition[humanPartNameToJoinWith];
            var humanPartToJoinWithRotatedShift =
                _skeletonRotationShifts.ContainsKey(humanPartNameToJoinWith)
                ? _skeletonRotationShifts[humanPartNameToJoinWith]
                : new Point(0, 0);

            var humanPartToJoinWith = _partNameToSkeletonPart[humanPartNameToJoinWith];
            var parentTailPoint = humanPartToJoinWith.MagickImageAndVector.GetPoint("Tail");

            return accumulatedShift.Subtract(humanPartToJoinWithRotatedShift).Add(parentTailPoint).Subtract(externalImagePivot);
        }

        private void ComputeMagickImage()
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

                ComputeShiftsForComposition();

                var canvasImage = new MagickImage(MagickColor.FromRgba(0, 0, 0, 0), (int)_canvasSize.X, (int)_canvasSize.Y);

                foreach (var partName in new[] {
                    HumanSkeletonPartName.Head,
                    HumanSkeletonPartName.ArmRTop,
                    HumanSkeletonPartName.ArmRBottom,
                    HumanSkeletonPartName.ArmLTop,
                    HumanSkeletonPartName.ArmLBottom,
                    HumanSkeletonPartName.LegRTop,
                    HumanSkeletonPartName.LegRBottom,
                    HumanSkeletonPartName.LegLTop,
                    HumanSkeletonPartName.LegLBottom,
                    HumanSkeletonPartName.Body
                }.Reverse())
                {
                    canvasImage.Composite(_partNameToMagickImageAndVector[partName].MagickImage, Gravity.Center, _shiftsForComposition[partName], CompositeOperator.Over);
                }

                _finalMagickImage = canvasImage;
        }

        private void ComputeShiftsForComposition()
        {
            ComputeShiftsForComposition(_skeleton, Pivot.Negate());
        }

        private void ComputeShiftsForComposition(SkeletonPart skeletonPart, Point accumulatedShift)
        {
            var rotatedShift =
                _skeletonRotationShifts.ContainsKey(skeletonPart.HumanSkeletonPartName)
                ? _skeletonRotationShifts[skeletonPart.HumanSkeletonPartName]
                : new Point(0, 0);

            _shiftsForComposition[skeletonPart.HumanSkeletonPartName] = accumulatedShift.Add(rotatedShift);

            if (skeletonPart.Children != null)
            {
                foreach (var child in skeletonPart.Children)
                {
                    var parentTailPoint = skeletonPart.MagickImageAndVector.GetPoint(child.ParentTailPointName);
                    ComputeShiftsForComposition(
                        child,
                        accumulatedShift.Add(parentTailPoint).Subtract(child.MagickImageAndVector.Pivot));
                }
            }

            _partNameToSkeletonPart[skeletonPart.HumanSkeletonPartName] = skeletonPart;
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

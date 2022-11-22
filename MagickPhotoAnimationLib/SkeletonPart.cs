using System;
using System.Collections.Generic;
using System.Text;

namespace MagickPhotoAnimationLib
{
    internal class SkeletonPart
    {
        public HumanSkeletonPartName HumanSkeletonPartName;
        public string ParentTailPointName;
        public MagickImageAndVector MagickImageAndVector;
        public SkeletonPart[] Children;
    }
}

namespace MagickPhotoAnimationLib.Animation
{
    internal class AnimationParameter
    {
        private double _startValue;
        private double _endValue;
        private AnimationPercentageState _animationPercentageState;

        public AnimationParameter(double startValue, double endValue, AnimationPercentageState animationPercentageState)
        {
            _startValue = startValue;
            _endValue = endValue;
            _animationPercentageState = animationPercentageState;
        }

        public double CurrentValue => _startValue + (_endValue - _startValue) * _animationPercentageState.Value;
    }
}

namespace Mentula.MathExtensions
{
    public static class Math
    {
        public static float Lerp(float Min, float Max, float amount)
        {
            return Min + (Max - Min) * amount;
        }

        public static float InvLerp(float Min, float Max, float value)
        {
            return (value - Min) / (Max - Min);
        }
    }
}
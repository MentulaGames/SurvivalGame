namespace Mentula.MathExtensions
{
    public static class Math
    {
        public static float Lerp(float Min, float Max, float a)
        {
            float v = Min + (Max - Min) * a;
            return v;
        }

        public static float InvLerp(float Min, float Max, float v)
        {
            float a = (v - Min) / (Max - Min);
            return a;
        }
    }
}
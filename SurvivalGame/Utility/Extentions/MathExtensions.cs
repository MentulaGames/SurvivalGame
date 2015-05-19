using System;
using Microsoft.Xna.Framework;

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
        public static float GetMaxDiff(Vector2 a, Vector2 b)
        {
            return System.Math.Max(System.Math.Abs(a.X - b.X), System.Math.Abs(a.Y - b.Y));
        }
    }
}
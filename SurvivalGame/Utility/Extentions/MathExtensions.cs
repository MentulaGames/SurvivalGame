using System;
using Microsoft.Xna.Framework;
using math = System.Math;
using Mentula.General;

namespace Mentula.General.MathExtensions
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
            return math.Max(math.Abs(a.X - b.X), math.Abs(a.Y - b.Y));
        }
        public static float GetMaxDiff(IntVector2 a, IntVector2 b)
        {
            return math.Max(math.Abs(a.X - b.X), math.Abs(a.Y - b.Y));
        }
        public static float GetMinDiff(Vector2 a, Vector2 b)
        {
            return math.Min(math.Abs(a.X - b.X), math.Abs(a.Y - b.Y));
        }
        public static float GetMinDiff(IntVector2 a, IntVector2 b)
        {
            return math.Min(math.Abs(a.X - b.X), math.Abs(a.Y - b.Y));
        }

        public static float DifferenceBetweenDegrees(float a, float b)
        {
            float x = math.Max(a, b);
            float y = math.Min(a, b);
            float c = math.Abs(x - y);
            if (c < 180) return c;
            else return 360 - c;
        }

        public static float VectorToDegrees(Vector2 v)
        {
            v.Normalize(); return (float)math.Atan2(v.Y, v.X) * (float)(180f / math.PI);
        }
    }
}
using Mentula.General;
using System;
using System.Collections.Generic;
using MEx = Mentula.MathExtensions.Math;

namespace Mentula.SurvivalGameServer
{
    public static class PerlinNoise
    {
        private static Dictionary<string, float> NoiseDict = new Dictionary<string, float>();
        private static string seed = "2";
        public static float Generate(float weight, float frequency, float x, float y)
        {
            float xLow = (float)Math.Floor(x / frequency) * frequency;
            float xHigh = (float)Math.Ceiling(x / frequency) * frequency;
            float yLow = (float)Math.Floor(y / frequency) * frequency;
            float yHigh = (float)Math.Ceiling(y / frequency) * frequency;
            float yLowNoise;
            float yHighNoise;

            if (float.IsNaN((x - xLow) / (xHigh - xLow)))
            {
                yLowNoise = MEx.Lerp((RNG.RFloatFromString(xLow + yLow)), (RNG.RFloatFromString(xHigh + yLow)), 0);
                yHighNoise = MEx.Lerp((RNG.RFloatFromString(xLow + yHigh)), (RNG.RFloatFromString(xHigh + yHigh)), 0);
            }
            else
            {
                yLowNoise = MEx.Lerp((GetNoise(xLow, yLow)), (GetNoise(xHigh, yLow)), MEx.InvLerp(xLow, xHigh, x));
                yHighNoise = MEx.Lerp((GetNoise(xLow, yHigh)), (GetNoise(xHigh, yHigh)), MEx.InvLerp(xLow, xHigh, x));
            }

            if (float.IsNaN((y - yLow) / (yHigh - yLow)))
            {
                return MEx.Lerp(yLowNoise, yHighNoise, 0) * weight;
            }
            else
            {
                return MEx.Lerp(yLowNoise, yHighNoise, MEx.InvLerp(yLow, yHigh, y)) * weight;
            }
        }

        private static float GetNoise(float x, float y)
        {
            float n;
            string s = x.ToString() + x + y.ToString();

            if (NoiseDict.Count > 4096)
            {
                NoiseDict = new Dictionary<string, float>();
            }

            if (NoiseDict.ContainsKey(s))
            {
                NoiseDict.TryGetValue(s, out n);
                return n;
            }
            else
            {
                n = RNG.RFloatFromString(s + seed);
                NoiseDict.Add(s, n);
                return n;
            }
        }
    }
}

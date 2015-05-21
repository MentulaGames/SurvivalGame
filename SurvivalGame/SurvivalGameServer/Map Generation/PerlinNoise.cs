using Mentula.General;
using System;
using System.Collections.Generic;
using MEx = Mentula.General.MathExtensions.Math;

namespace Mentula.SurvivalGameServer
{
    public static class PerlinNoise
    {
        private static Dictionary<string, float> NoiseDict = new Dictionary<string, float>();
        private static string SEED = "1";

        public static float Generate(float weight, float frequency, float x, float y, string seed)
        {
            float xLow = (float)Math.Floor(x / frequency) * frequency;
            float xHigh = (float)Math.Ceiling(x / frequency) * frequency;
            float yLow = (float)Math.Floor(y / frequency) * frequency;
            float yHigh = (float)Math.Ceiling(y / frequency) * frequency;
            float x0y0 = GetNoise(xLow, yLow, seed);
            float x0y1 = GetNoise(xLow, yHigh, seed);
            float x1y0 = GetNoise(xHigh, yLow, seed);
            float x1y1 = GetNoise(xHigh, yHigh, seed);
            float x0Noise;
            float x1Noise;
            if (yLow == yHigh)
            {
                x0Noise = x0y0;
                x1Noise = x1y0;
            }
            else
            {
                x0Noise = MEx.Lerp(x0y0, x0y1, MEx.InvLerp(yLow, yHigh, y));
                x1Noise = MEx.Lerp(x1y0, x1y1, MEx.InvLerp(yLow, yHigh, y));
            }
            if (x0Noise == x1Noise)
            {
                return x0Noise * weight;
            }
            else
            {
                return MEx.Lerp(x0Noise, x1Noise, MEx.InvLerp(xLow, xHigh, x)) * weight;
            }
        }

        private static float GetNoise(float x, float y, string seed)
        {
            float n;
            string s = x.ToString() + x + y.ToString() + seed + SEED;

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
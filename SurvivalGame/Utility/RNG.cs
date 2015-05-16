using System;

namespace Mentula.General
{
    public static class RNG
    {
        public static float RFloatFromString(string s)
        {
            float result = 0;

            for (int i = 0; i < s.Length; i++)
            {
                int byteVal = Convert.ToInt16(s[i]);
                Random rnd = new Random(byteVal + (int)result * 100);
                result += (float)rnd.NextDouble();
            }

            return result % 1;
        }

        public static float RFloatFromString(params object[] arg)
        {
            string s = string.Empty;
            float result = 0;

            for (int i = 0; i < arg.Length; i++)
            {
                s += arg[i].ToString();
            }

            for (int i = 0; i < s.Length; i++)
            {
                int byteVal = Convert.ToInt16(s[i]);
                Random rnd = new Random(byteVal + (int)result * 100);
                result += (float)rnd.NextDouble();
            }

            return result % 1;
        }
    }
}

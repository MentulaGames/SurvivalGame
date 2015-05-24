using System;

namespace Mentula.General
{
    public static class RNG
    {
        public static float RFloatFromString(string s)
        {
            double result = 0;

            for (int i = 0; i < s.Length; i++)
            {
                int byteVal = Convert.ToInt16(s[i]);
                Random rnd = new Random(byteVal + (int)result * 100);
                result += rnd.NextDouble();
            }

            return (float)(result % 1);
        }

        public static float RFloatFromString(params object[] arg)
        {
            string s = string.Empty;
            double result = 0;

            for (int i = 0; i < arg.Length; i++)
            {
                s += arg[i].ToString();
            }

            for (int i = 0; i < s.Length; i++)
            {
                int byteVal = Convert.ToInt16(s[i]);
                Random rnd = new Random(byteVal + (int)result * 100);
                result += rnd.NextDouble();
            }
            return (float)(result % 1);
        }

        public static int RIntFromString(string s)
        {
            double result = 0;

            for (int i = 0; i < s.Length; i++)
            {
                int byteVal = Convert.ToInt16(s[i]);
                Random rnd = new Random(byteVal + (int)result * 100);
                result += rnd.NextDouble();
            }

            return (int)((result % 1) * 1000000);
        }

        public static int RIntFromString(params object[] arg)
        {
            string s = string.Empty;
            double result = 0;

            for (int i = 0; i < arg.Length; i++)
            {
                s += arg[i].ToString();
            }

            for (int i = 0; i < s.Length; i++)
            {
                int byteVal = Convert.ToInt16(s[i]);
                Random rnd = new Random(byteVal + (int)result * 100);
                result += rnd.NextDouble();
            }
            return (int)((result % 1)*1000000);
        }
    }
}

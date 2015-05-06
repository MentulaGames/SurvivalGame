using System;

namespace Mentula.General
{
    public static class RNG
    {

        public static float RFloatFromString(string s)
        {
            float returnVal = 0;

            for (int i = 0; i < s.Length; i++)
            {
                int byteVal = Convert.ToInt16(s[i]);
                Random r = new Random(byteVal + (int)returnVal * 100);
                returnVal += (float)r.NextDouble();
            }

            return returnVal % 1;
        }

        public static float RFloatFromString(params object[] arg)
        {
            string s = string.Empty;
            float returnVal = 0;

            for (int i = 0; i < arg.Length; i++)
            {
                s += arg[i].ToString();
            }

            for (int i = 0; i < s.Length; i++)
            {
                int byteVal = Convert.ToInt16(s[i]);
                Random r = new Random(byteVal + (int)returnVal * 100);
                returnVal += (float)r.NextDouble();
            }

            return returnVal % 1;
        }
    }
}

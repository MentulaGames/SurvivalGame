using System.Diagnostics;

namespace Mentula.Content
{
    [DebuggerDisplay("{ToString()}")]
    public class Stats
    {
        public float Str;
        public float Dex;
        public float Int;
        public float Per;
        public float End;

        public Stats()
        {
            Str = 0;
            Dex = 0;
            Int = 0;
            Per = 0;
            End = 0;
        }

        public Stats(float value)
        {
            Str = value;
            Dex = value;
            Int = value;
            Per = value;
            End = value;
        }

        public Stats(float strength, float dexterity, float inteligence, float perception, float endurance)
        {
            Str = strength;
            Dex = dexterity;
            Int = inteligence;
            Per = perception;
            End = endurance;
        }

        public static Stats operator +(Stats s, Stats c)
        {
            return new Stats(s.Str + c.Str, s.Dex + c.Dex, s.Int + c.Int, s.Per + c.Per, s.End + c.End);
        }

        public static Stats operator -(Stats s, Stats c)
        {
            return new Stats(s.Str - c.Str, s.Dex - c.Dex, s.Int - c.Int, s.Per - c.Per, s.End - c.End);
        }

        public static Stats operator /(Stats s, float c)
        {
            return new Stats(s.Str / c, s.Dex / c, s.Int / c, s.Per / c, s.End / c);
        }

        public static Stats operator *(Stats s, float c)
        {
            return new Stats(s.Str * c, s.Dex * c, s.Int * c, s.Per * c, s.End * c);
        }

        public override string ToString()
        {
            return "Str=" + Str.ToString() + " Dex=" + Dex.ToString() + " Int=" + Int.ToString() + " Per=" + Per.ToString() + " End=" + End.ToString();
        }
    }
}
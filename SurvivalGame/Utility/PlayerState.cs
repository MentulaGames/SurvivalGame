using Microsoft.Xna.Framework;
using System;
using System.Collections;

namespace Mentula.Network.Xna
{
    public struct PlayerState
    {
        public readonly UInt3[] States;
        public readonly Color[] Colors;

        public PlayerState(UInt3[] parts)
        {
            States = parts;
            Colors = new Color[States.Length];

            for (int i = 0; i < States.Length; i++)
            {
                Color temp = Color.Transparent;

                switch (States[i])
                {
                    case (0):
                        temp = Color.DarkGreen;
                        break;
                    case (1):
                        temp = Color.LightGreen;
                        break;
                    case (2):
                        temp = Color.Yellow;
                        break;
                    case (3):
                        temp = Color.Orange;
                        break;
                    case (4):
                        temp = Color.Red;
                        break;
                    case (5):
                        temp = Color.DarkRed;
                        break;
                    default:
                        temp = Color.Gray;
                        break;
                }

                Colors[i] = temp;
            }
        }

        public override string ToString()
        {
            string v = "";

            for (int i = 0; i < States.Length; i++)
            {
                v += States[i].ToString() + "|";
            }

            return v;
        }

        public struct UInt3 : IComparable, IFormattable, IComparable<UInt3>, IEquatable<UInt3>
        {
            public const int MaxValue = 7;
            public const int MinValue = 0;

            private readonly BitArray C;
            private int Value
            {
                get
                {
                    int v = 0;

                    for (int i = 0; i < C.Length; i++)
                    {
                        v += C[i] ? 1 << i : 0;
                    }

                    return v;
                }
            }

            public UInt3(bool[] b)
            {
                if (b.Length > 3) throw new OverflowException();

                C = new BitArray(b);
            }

            public BitArray GetRaw()
            {
                return C;
            }

            public static implicit operator UInt3(uint d)
            {
                if (d > MaxValue) throw new OverflowException();

                bool[] b = new bool[3];

                for (int i = 0; i < b.Length; i++)
                {
                    b[i] = d % 2 == 1;
                    d = d >> 1;
                }

                return new UInt3(b);
            }

            public static implicit operator int(UInt3 i)
            {
                return i.Value;
            }

            public int CompareTo(object obj)
            {
                try
                {
                    UInt16 dec = (UInt16)obj;
                    return dec == Value ? 0 : (dec < Value ? -1 : 1);
                }
                catch (InvalidCastException) { return 0; }
            }

            public int CompareTo(UInt3 other)
            {
                int hashCode = Value.GetHashCode();
                int oHashCode = other.Value.GetHashCode();

                return hashCode == oHashCode ? 0 : (hashCode > oHashCode ? -1 : 1);
            }

            public override int GetHashCode()
            {
                return Value;
            }

            public bool Equals(UInt3 other)
            {
                for (int i = 0; i < C.Length; i++)
                {
                    if (C[i] != other.C[i]) return false;
                }

                return true;
            }

            public override bool Equals(object obj)
            {
                try
                {
                    return Equals((UInt3)obj);
                }
                catch (InvalidCastException) { return false; }
            }

            public string ToString(string format, IFormatProvider formatProvider)
            {
                return Value.ToString();
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }
    }
}
using Mentula.General;
using System.Diagnostics;
using System.Globalization;

namespace Mentula.General
{
    public struct BytePoint
    {
        public byte X;
        public byte Y;

        public BytePoint(byte x, byte y)
        {
            X = x;
            Y = y;
        }

        public static explicit operator BytePoint(IntVector2 vec2)
        {
            return new BytePoint((byte)vec2.X, (byte)vec2.Y);
        }

        public static explicit operator IntVector2(BytePoint p2)
        {
            return new IntVector2(p2.X, p2.Y);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X:{0} Y:{1}}}", new object[2] { X, Y });
        }
    }
}
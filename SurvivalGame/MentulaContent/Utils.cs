using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using System.Globalization;
using System.Text;

namespace Mentula.Content
{
    internal static class Utils
    {
        private static readonly CultureInfo usInfo;

        static Utils()
        {
            usInfo = CultureInfo.CreateSpecificCulture("en-US");
        }

        public static void WriteString(this ContentWriter cw, string value)
        {
            byte[] enc = Encoding.UTF8.GetBytes(value);
            cw.Write(enc.Length);
            cw.Write(enc);
        }

        public static string ReadCString(this ContentReader cr)
        {
            int length = cr.ReadInt32();
            byte[] enc = cr.ReadBytes(length);
            return Encoding.UTF8.GetString(enc);
        }

        public static void Write(this ContentWriter cw, Stats value)
        {
            cw.Write(value.Str);
            cw.Write(value.Dex);
            cw.Write(value.Int);
            cw.Write(value.Per);
            cw.Write(value.End);
        }

        public static Stats ReadStats(this ContentReader cr)
        {
            Stats result = new Stats();

            result.Str = cr.ReadSingle();
            result.Dex = cr.ReadSingle();
            result.Int = cr.ReadSingle();
            result.Per = cr.ReadSingle();
            result.End = cr.ReadSingle();

            return result;
        }

        public static void CheckProcessorType(string type, string request)
        {
            if (type.ToUpper() != request.ToUpper()) throw new ArgumentException("Wrong processor type selected this= '" + type + "', needed='" + request + "'");
        }

        public static bool TryParse(string s, out float result)
        {
            return float.TryParse(s, NumberStyles.Number, usInfo, out result);
        }
    }
}
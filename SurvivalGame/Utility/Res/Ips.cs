using System.Net;

namespace Mentula.General.Res
{
    public static class Ips
    {
        public const int PORT = 25565;

        public static byte[] IpJoëll;
        public static byte[] IpFrank;

        public static IPEndPoint EndJoëll;
        public static IPEndPoint EndFrank;

        static Ips()
        {
            IpJoëll = new byte[4] { 83, 82, 128, 64 };
            IpFrank = new byte[4] { 83, 82, 180, 172 };

            EndJoëll = new IPEndPoint(new IPAddress(IpJoëll), PORT);
            EndFrank = new IPEndPoint(new IPAddress(IpFrank), PORT);
        }
    }
}
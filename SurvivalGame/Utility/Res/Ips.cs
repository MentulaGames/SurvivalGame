using System.Net;

namespace Mentula.General.Resources
{
    public static class Ips
    {
        public const int PORT = 7777;

        public static IPEndPoint EndJoëll;
        public static IPEndPoint EndShitPc;
        public static IPEndPoint EndFrank;

        static Ips()
        {
            EndJoëll = new IPEndPoint(new IPAddress(new byte[4] { 83, 82, 128, 64 }), PORT);
            EndFrank = new IPEndPoint(new IPAddress(new byte[4] { 83, 82, 180, 172 }), PORT);
            EndShitPc = new IPEndPoint(new IPAddress(new byte[4] { 83, 82, 128, 64 }), 25566);
        }
    }
}
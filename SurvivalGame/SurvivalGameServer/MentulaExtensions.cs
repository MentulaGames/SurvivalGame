using System;
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer
{
    public static class MentulaExtensions
    {
        public static long GetId(this NIM message)
        {
            return message.SenderConnection.RemoteUniqueIdentifier;
        }

        public static AStar.Node[] PropperClone(this AStar.Node[] arr)
        {
            AStar.Node[] result = new AStar.Node[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                AStar.Node cur = arr[i];
                result[i] = new AStar.Node(cur.Position, cur.GValue, !cur.wall);
            }

            return result;
        }

        public static void WriteLine(this NIMT nimt, string format, params object[] arg)
        {
            string mode = "";

            switch (nimt)
            {
                case (NIMT.ConnectionApproval):
                    Console.ForegroundColor = ConsoleColor.Green;
                    mode = "Approval";
                    break;
                case (NIMT.ConnectionLatencyUpdated):
                    mode = "LatencyUpdate";
                    break;
                case (NIMT.Data):
                case (NIMT.UnconnectedData):
                    mode = "Data";
                    break;
                case (NIMT.DebugMessage):
                case (NIMT.VerboseDebugMessage):
                    mode = "Debug";
                    break;
                case (NIMT.DiscoveryRequest):
                case (NIMT.DiscoveryResponse):
                    mode = "Discovery";
                    break;
                case (NIMT.Error):
                case (NIMT.ErrorMessage):
                    Console.ForegroundColor = ConsoleColor.Red;
                    mode = "Error";
                    break;
                case (NIMT.NatIntroductionSuccess):
                    mode = "NAT";
                    break;
                case (NIMT.Receipt):
                    mode = "Receipt";
                    break;
                case (NIMT.StatusChanged):
                    Console.ForegroundColor = ConsoleColor.Green;
                    mode = "Status";
                    break;
                case (NIMT.WarningMessage):
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    mode = "Warning";
                    break;
            }

            Console.WriteLine(string.Format("[{0}][{1}] {2}", string.Format("{0:H:mm:ss}", DateTime.Now), mode, format), arg);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}

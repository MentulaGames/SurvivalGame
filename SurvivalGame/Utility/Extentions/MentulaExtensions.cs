using Lidgren.Network;
using Mentula.General;
using System;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.Network.Xna
{
    public static class MentulaExtensions
    {
        public static void Write(this NetBuffer message, CTile value)
        {
            message.Write(value.Pos.X);
            message.Write(value.Pos.Y);
            message.Write(value.ChunkPos.X);
            message.Write(value.ChunkPos.Y);
            message.Write(value.TextureId);
            message.Write(value.Layer);
            message.Write(value.Walkable);
        }

        public static void Write(this NetBuffer message, CTile[] value)
        {
            message.Write(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                message.Write(value[i]);
            }
        }

        public static CTile ReadTile(this NetBuffer message)
        {
            IntVector2 pos = new IntVector2(message.ReadInt32(), message.ReadInt32());
            IntVector2 chunkPos = new IntVector2(message.ReadInt32(), message.ReadInt32());
            return new CTile(chunkPos, pos, message.ReadInt32(), message.ReadInt32(), message.ReadBoolean());
        }

        public static CTile[] ReadTileArr(this NetBuffer message)
        {
            int length = message.ReadInt32();
            CTile[] result = new CTile[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = message.ReadTile();
            }

            return result;
        }

        public static void Write(this NetBuffer message, IntVector2 value)
        {
            message.Write(value.X);
            message.Write(value.Y);
        }

        public static IntVector2 ReadVector(this NetBuffer message)
        {
            return new IntVector2(message.ReadInt32(), message.ReadInt32());
        }

        public static void WriteLine(NIMT nimt, string format, params object[] arg)
        {
            string mode = "";

            switch(nimt)
            {
                case(NIMT.ConnectionApproval):
                    mode = "Approval";
                    break;
                case(NIMT.ConnectionLatencyUpdated):
                    mode = "LatencyUpdate";
                    break;
                case(NIMT.Data):
                case (NIMT.UnconnectedData):
                    mode = "Data";
                    break;
                case(NIMT.DebugMessage):
                case(NIMT.VerboseDebugMessage):
                    mode = "Debug";
                    break;
                case(NIMT.DiscoveryRequest):
                case (NIMT.DiscoveryResponse):
                    mode = "Discovery";
                    break;
                case(NIMT.Error):
                case(NIMT.ErrorMessage):
                    Console.ForegroundColor = ConsoleColor.Red;
                    mode = "Error";
                    break;
                case(NIMT.NatIntroductionSuccess):
                    mode = "NAT";
                    break;
                case(NIMT.Receipt):
                    mode = "Receipt";
                    break;
                case(NIMT.StatusChanged):
                    Console.ForegroundColor = ConsoleColor.Green;
                    mode = "Status";
                    break;
                case(NIMT.WarningMessage):
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    mode = "Warning";
                    break;
            }

            Console.WriteLine(string.Format("[{0}][{1}] {2}", string.Format("{0:H:mm:ss}", DateTime.Now), mode, format), arg);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
using Lidgren.Network;
using Mentula.General;
using Mentula.General.Res;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.Network.Xna
{
    public static class MentulaExtensions
    {
        public static void Write(this NetBuffer message, C_Tile value)
        {
            message.Write(value.Pos.X);
            message.Write(value.Pos.Y);
            message.Write(value.ChunkPos.X);
            message.Write(value.ChunkPos.Y);
            message.Write(value.TextureId);
            message.Write(value.Layer);
            message.Write(value.Walkable);
        }

        public static C_Tile ReadTile(this NetBuffer message)
        {
            IntVector2 pos = new IntVector2(message.ReadInt32(), message.ReadInt32());
            IntVector2 chunkPos = new IntVector2(message.ReadInt32(), message.ReadInt32());
            return new C_Tile(chunkPos, pos, message.ReadByte(), message.ReadByte(), message.ReadBoolean());
        }

        public static void Write(this NetBuffer message, C_Tile[] value)
        {
            message.Write(value.Length);

            C_Tile baseT = value[0];
            message.Write(baseT.ChunkPos);      // chunk pos.
            message.Write(baseT.Pos);           // Initial tile pos.
            message.Write(baseT.Layer);

            for (int i = 0; i < value.Length; i++)
            {
                C_Tile curT = value[i];
                message.Write(curT.TextureId);
                message.Write(curT.Walkable);
            }
        }

        public static C_Tile[] ReadTileArr(this NetBuffer message)
        {
            int length = message.ReadInt32();
            int chunkLength = int.Parse(Resources.ChunkSize);
            C_Tile[] result = new C_Tile[length];

            IntVector2 chunkPos = message.ReadVector();
            IntVector2 pos = message.ReadVector();
            byte layer = message.ReadByte();

            for (int i = 0; i < length; i++)
            {
                C_Tile newTile = new C_Tile(chunkPos, pos, message.ReadByte(), layer, message.ReadBoolean());
                result[i] = newTile;

                pos.X++;

                if (pos.X >= chunkLength)
                {
                    pos.X = 0;
                    pos.Y++;
                }
            }

            return result;
        }

        public static void Write(this NetBuffer message, C_Destrucible[] value)
        {
            message.Write(value.Length);

            C_Destrucible baseD = value[0];
            message.Write(baseD.ChunkPos);
            message.Write(baseD.Layer);

            for (int i = 0; i < value.Length; i++)
            {
                C_Destrucible curD = value[i];
                message.Write(curD.Pos);
                message.Write(curD.TextureId);
                message.Write(curD.Walkable);
                message.Write(curD.Health);
            }
        }

        public static C_Destrucible[] ReadDesArr(this NetBuffer message)
        {
            int length = message.ReadInt32();
            C_Destrucible[] result = new C_Destrucible[length];

            IntVector2 chunkPos = message.ReadVector();
            byte layer = message.ReadByte();

            for (int i = 0; i < length; i++)
            {
                result[i] = new C_Destrucible(chunkPos, message.ReadVector(), message.ReadByte(), layer, message.ReadBoolean(), message.ReadFloat());
            }

            return result;
        }

        public static IntVector2 ReadVector(this NetBuffer message)
        {
            return new IntVector2(message.ReadInt32(), message.ReadInt32());
        }

        public static void Write(this NetBuffer message, IntVector2 value)
        {
            message.Write(value.X);
            message.Write(value.Y);
        }

        public static TEnum ReadEnum<TEnum>(this NetBuffer message)
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), message.ReadByte());
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
        
        public static void Draw(this SpriteBatch batch, Texture2D texture, Vector2 position, Color color, byte layer)
        {
            batch.Draw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f / (layer + 1));
        }
    }
}
using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.General;
using Mentula.General.Res;
using Microsoft.Xna.Framework;
using System;

namespace Mentula.Network.Xna
{
    public static class MentulaExtensions
    {
        private static int ChunkSize;

        static MentulaExtensions()
        {
            ChunkSize = int.Parse(Resources.ChunkSize);
        }

        public static void Write(this NetBuffer msg, BytePoint value)
        {
            msg.Write(value.X);
            msg.Write(value.Y);
        }

        public static void Write(this NetBuffer msg, IntVector2 value)
        {
            msg.Write(value.X);
            msg.Write(value.Y);
        }

        public static void Write(this NetBuffer msg, C_Tile value)
        {
            msg.Write(value.ChunkPos.X);
            msg.Write(value.ChunkPos.Y);
            msg.Write((BytePoint)value.Pos);
            msg.Write(value.TextureId);
            msg.Write(value.Layer);
            msg.Write(value.Walkable);
        }

        public static void Write(this NetBuffer msg, C_Tile[] value)
        {
            msg.Write((Int16)value.Length);

            C_Tile baseT = value[0];
            msg.Write(baseT.ChunkPos);                  // chunk pos.
            msg.Write((BytePoint)baseT.Pos);            // Initial tile pos.
            msg.Write(baseT.Layer);

            for (int i = 0; i < value.Length; i++)
            {
                C_Tile curT = value[i];
                msg.Write(curT.TextureId);
                msg.Write(curT.Walkable);
            }
        }

        public static void Write(this NetBuffer msg, C_Destrucible[] value)
        {
            msg.Write((Int16)value.Length);

            if (value.Length == 0) return;

            C_Destrucible baseD = value[0];
            msg.Write(baseD.ChunkPos);
            msg.Write(baseD.Layer);

            for (int i = 0; i < value.Length; i++)
            {
                C_Destrucible curD = value[i];
                msg.Write((BytePoint)curD.Pos);
                msg.Write(curD.TextureId);
                msg.Write(curD.Walkable);
                msg.Write(curD.Health);
            }
        }

        public static void Write(this NetBuffer msg, C_Creature value)
        {
            msg.Write(value.ChunkPos);
            msg.Write(value.Pos);
            msg.Write(value.Color.PackedValue);
            msg.Write(value.TextureId);
        }

        public static void Write(this NetBuffer msg, C_Creature[] value)
        {
            msg.Write((Int16)value.Length);

            if (value.Length == 0) return;

            msg.Write(value[0].ChunkPos);

            for (int i = 0; i < value.Length; i++)
            {
                C_Creature curC = value[i];
                msg.Write(curC.Pos);
                msg.Write(curC.Color.PackedValue);
                msg.Write(curC.TextureId);
            }
        }

        public static void Write(this NetBuffer msg, Player value)
        {
            msg.Write(value.ChunkPos);
            msg.Write(value.GetTilePos());
        }

        public static void Write(this NetBuffer msg, Player[] value)
        {
            msg.Write((Int16)value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                Player p = value[i];

                msg.Write(p.Name);
                msg.Write(p.ChunkPos);
                msg.Write(p.GetTilePos());
            }
        }

        public static BytePoint ReadPoint(this NetBuffer msg)
        {
            return new BytePoint(msg.ReadByte(), msg.ReadByte());
        }

        public static IntVector2 ReadVector(this NetBuffer msg)
        {
            return new IntVector2(msg.ReadInt32(), msg.ReadInt32());
        }

        public static C_Tile ReadTile(this NetBuffer msg)
        {
            IntVector2 chunkPos = new IntVector2(msg.ReadInt32(), msg.ReadInt32());
            IntVector2 pos = (IntVector2)msg.ReadPoint();
            return new C_Tile(chunkPos, pos, msg.ReadByte(), msg.ReadByte(), msg.ReadBoolean());
        }

        public static C_Tile[] ReadTileArr(this NetBuffer msg)
        {
            int length = msg.ReadInt16();
            C_Tile[] result = new C_Tile[length];

            IntVector2 chunkPos = msg.ReadVector();
            IntVector2 pos = (IntVector2)msg.ReadPoint();
            byte layer = msg.ReadByte();

            for (int i = 0; i < length; i++)
            {
                C_Tile newTile = new C_Tile(chunkPos, pos, msg.ReadByte(), layer, msg.ReadBoolean());
                result[i] = newTile;

                pos.X++;

                if (pos.X >= ChunkSize)
                {
                    pos.X = 0;
                    pos.Y++;
                }
            }

            return result;
        }

        public static C_Destrucible[] ReadDesArr(this NetBuffer msg)
        {
            int length = msg.ReadInt16();

            C_Destrucible[] result = new C_Destrucible[length];

            if (length == 0) return result;

            IntVector2 chunkPos = msg.ReadVector();
            byte layer = msg.ReadByte();

            for (int i = 0; i < length; i++)
            {
                result[i] = new C_Destrucible(chunkPos, (IntVector2)msg.ReadPoint(), msg.ReadByte(), layer, msg.ReadBoolean(), msg.ReadFloat());
            }

            return result;
        }

        public static C_Creature ReadCreature(this NetBuffer msg)
        {
            return new C_Creature(msg.ReadVector(), msg.ReadVector2(), new Color() { PackedValue = msg.ReadUInt32() }, msg.PeekInt32());
        }

        public static C_Creature[] ReadCreatureArr(this NetBuffer msg)
        {
            int length = msg.ReadUInt16();

            C_Creature[] result = new C_Creature[length];

            if (length == 0) return result;

            IntVector2 chunkPos = msg.ReadVector();

            for (int i = 0; i < length; i++)
            {
                result[i] = new C_Creature(chunkPos, msg.ReadVector2(), new Color() { PackedValue = msg.ReadUInt32() }, msg.ReadInt32());
            }

            return result;
        }

        public static void ReadReSetPlayer(this NetBuffer msg, ref Player player)
        {
            player.ReSet(msg.ReadVector(), msg.ReadVector2());
        }

        public static Player[] ReadPlayers(this NetBuffer msg)
        {
            int length = msg.ReadInt16();
            Player[] result = new Player[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = new Player(msg.ReadString(), msg.ReadVector(), msg.ReadVector2());
            }

            return result;
        }

        public static TEnum ReadEnum<TEnum>(this NetBuffer msg)
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), msg.ReadByte());
        }
    }
}
using Lidgren.Network;
using Lidgren.Network.Xna;
using Mentula.General;
using Mentula.General.Resources;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mentula.Network.Xna
{
    public static class MentulaExtensions
    {
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
            msg.Write(value.State);
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
                msg.Write(curC.State);
            }
        }

        public static void Write(this NetBuffer msg, C_Player[] value)
        {
            msg.Write((Int16)value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                C_Player p = value[i];

                msg.Write(p.Name);
                msg.Write(p.ChunkPos);
                msg.Write(p.GetTilePos());
                msg.Write(p.State);
            }
        }

        public static void Write(this NetBuffer msg, CreatureState value)
        {
            msg.Write((Int16)value.States.Length);

            for (int i = 0; i < value.States.Length; i++)
            {
                BitArray c = value.States[i].Value.GetRaw();
                msg.Write(value.States[i].Key);
                msg.Write((byte)c.Length);

                for (int j = 0; j < c.Length; j++)
                {
                    msg.Write(c[j]);
                }
            }
        }

        public static BytePoint ReadPoint(this NetBuffer msg)
        {
            byte x = msg.ReadByte();
            byte y = msg.ReadByte();
            return new BytePoint(x, y);
        }

        public static IntVector2 ReadVector(this NetBuffer msg)
        {
            int x = msg.ReadInt32();
            int y = msg.ReadInt32();
            return new IntVector2(x, y);
        }

        public static C_Tile ReadTile(this NetBuffer msg)
        {
            int x = msg.ReadInt32();
            int y = msg.ReadInt32();
            IntVector2 chunkPos = new IntVector2(x, y);

            IntVector2 pos = (IntVector2)msg.ReadPoint();
            byte texture = msg.ReadByte();
            byte layer = msg.ReadByte();
            bool walkable = msg.ReadBoolean();

            return new C_Tile(chunkPos, pos, texture, layer, walkable);
        }

        public static C_Tile[] ReadTileArr(this NetBuffer msg)
        {
            int length = msg.ReadInt16();
            C_Tile[] result = new C_Tile[length];

            if (length == 0) return result;

            IntVector2 chunkPos = msg.ReadVector();
            IntVector2 pos = (IntVector2)msg.ReadPoint();
            byte layer = msg.ReadByte();

            for (int i = 0; i < length; i++)
            {
                byte texture = msg.ReadByte();
                bool walkable = msg.ReadBoolean();

                C_Tile newTile = new C_Tile(chunkPos, pos, texture, layer, walkable);
                result[i] = newTile;

                pos.X++;

                if (pos.X >= Res.ChunkSize)
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
            IntVector2 chunkPos = msg.ReadVector();
            Vector2 tilePos = msg.ReadVector2();
            Color color = new Color() { PackedValue = msg.ReadUInt32() };
            int id = msg.ReadInt32();
            CreatureState state = msg.ReadState();

            return new C_Creature(chunkPos, tilePos, color, id, state);
        }

        public static C_Creature[] ReadCreatureArr(this NetBuffer msg)
        {
            int length = msg.ReadUInt16();

            C_Creature[] result = new C_Creature[length];

            if (length == 0) return result;

            IntVector2 chunkPos = msg.ReadVector();

            for (int i = 0; i < length; i++)
            {
                Vector2 tilePos = msg.ReadVector2();
                Color color = new Color() { PackedValue = msg.ReadUInt32() };
                int id = msg.ReadInt32();
                CreatureState state = msg.ReadState();

                result[i] = new C_Creature(chunkPos, tilePos, color, id, state);
            }

            return result;
        }

        public static void ReadReSetPlayer(this NetBuffer msg, ref C_Player player)
        {
            IntVector2 chunkPos = msg.ReadVector();
            Vector2 tilePos = msg.ReadVector2();

            player.ReSet(chunkPos, tilePos);
        }

        public static C_Player[] ReadPlayers(this NetBuffer msg)
        {
            int length = msg.ReadInt16();
            C_Player[] result = new C_Player[length];

            for (int i = 0; i < length; i++)
            {
                string name = msg.ReadString();
                IntVector2 chunkPos = msg.ReadVector();
                Vector2 tilePos = msg.ReadVector2();
                CreatureState state = msg.ReadState();

                result[i] = new C_Player(name, chunkPos, tilePos, state);
            }

            return result;
        }

        public static TEnum ReadEnum<TEnum>(this NetBuffer msg)
        {
            return (TEnum)Enum.ToObject(typeof(TEnum), msg.ReadByte());
        }

        public static CreatureState ReadState(this NetBuffer msg)
        {
            int length = msg.ReadInt16();
            KeyValuePair<string, CreatureState.UInt3>[] a_U = new KeyValuePair<string, CreatureState.UInt3>[length];

            for (int i = 0; i < length; i++)
            {
                byte l = msg.ReadByte();
                string name = msg.ReadString();
                bool[] values = new bool[l];

                for (int j = 0; j < l; j++)
                {
                    values[j] = msg.ReadBoolean();
                }

                a_U[i] = new KeyValuePair<string, CreatureState.UInt3>(name, new CreatureState.UInt3(values));
            }

            return new CreatureState(a_U);
        }
    }
}
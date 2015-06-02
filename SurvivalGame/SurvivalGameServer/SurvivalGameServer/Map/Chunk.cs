using Mentula.General;
using Mentula.General.Res;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.SurvivalGameServer
{
    public class Chunk
    {
        public IntVector2 Pos;
        public Tile[] Tiles;
        public List<Destructible> Destructibles;
        public List<Creature> Creatures;

        private readonly int CS;

        public Chunk(IntVector2 pos)
        {
            Pos = pos;
            CS = int.Parse(Resources.ChunkSize);
            Tiles = new Tile[CS * CS];
            Destructibles = new List<Destructible>();
            Creatures = new List<Creature>();
        }

        public Chunk(IntVector2 pos, Tile[] tiles)
        {
            Pos = pos;
            Tiles = tiles;
            Destructibles = new List<Destructible>();
            Creatures = new List<Creature>();
        }

        public Chunk(IntVector2 pos, Tile[] tiles, List<Destructible> destructibles)
        {
            Pos = pos;
            Tiles = tiles;
            Destructibles = destructibles;
            Creatures = new List<Creature>();
        }

        public Chunk(IntVector2 pos, Tile[] tiles, List<Destructible> destructibles, List<Creature> creatures)
        {
            Pos = pos;
            Tiles = tiles;
            Destructibles = destructibles;
            Creatures = creatures;
        }

        public void Generate(byte id)
        {
            for (int y = 0; y < CS; y++)
            {
                for (int x = 0; x < CS; x++)
                {
                    Tiles[x + (y * CS)] = new Tile(new IntVector2(x, y), id);
                }
            }
        }

        public static explicit operator C_Tile[](Chunk chunk)
        {
            return chunk.Tiles.Select(t => new C_Tile(chunk.Pos, t)).ToArray();
        }

        public static explicit operator C_Destrucible[](Chunk chunk)
        {
            return chunk.Destructibles.Select(d => new C_Destrucible(chunk.Pos, d)).ToArray();
        }

        public static explicit operator C_Creature[](Chunk chunk)
        {
            return chunk.Creatures.Select(c => new C_Creature(chunk.Pos, c.GetTilePos(), c.SkinColor, c.Health, c.Texture)).ToArray();
        }
    }
}
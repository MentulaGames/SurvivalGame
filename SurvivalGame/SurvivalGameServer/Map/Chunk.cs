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

        private readonly int CS;

        public Chunk(IntVector2 pos)
        {
            Pos = pos;
            CS = int.Parse(Resources.ChunkSize);
            Tiles = new Tile[CS * CS];
        }

        public Chunk(IntVector2 pos, Tile[] Tiles)
        {
            Pos = pos;
            this.Tiles = Tiles;
        }

        public Chunk(IntVector2 pos, Tile[] Tiles, List<Destructible> Destructibles)
        {
            Pos = pos;
            this.Tiles = Tiles;
            this.Destructibles = Destructibles;
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
    }
}
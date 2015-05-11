using Mentula.General;
using Mentula.General.Res;

namespace Mentula.SurvivalGameServer
{
    public class Chunk
    {
        public int ChunkType;
        public IntVector2 Pos;
        public Tile[] Tiles;

        public Chunk(IntVector2 pos)
        {
            Pos = pos;
            int size = int.Parse(Resources.ChunkSize);
            Tiles = new Tile[size * size];
        }
        public Chunk(IntVector2 pos, Tile[] Tiles)
        {
            Pos = pos;
            this.Tiles = Tiles;
        }

        public void Generate(byte id)
        {
            int size = int.Parse(Resources.ChunkSize);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Tiles[x + (y * size)] = new Tile(new IntVector2(x, y), id);
                }
            }
        }
    }
}
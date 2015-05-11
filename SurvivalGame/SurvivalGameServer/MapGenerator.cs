using Mentula.General;
using Mentula.General.Res;
using System;

namespace Mentula.SurvivalGameServer
{
    public static class MapGenerator
    {
        public static Chunk GenerateTerrain(IntVector2 pos)
        {
            int cSize = int.Parse(Resources.ChunkSize);
            Tile[] Tiles = new Tile[cSize * cSize];
            int x;
            int y;
            for (int i = 0; i < cSize * cSize; i++)
            {
                x = i % cSize + pos.X * cSize;
                y = i / cSize + pos.Y * cSize;
                float rain = 0;
                rain += PerlinNoise.Generate(75, cSize * 4, x, y);
                rain += PerlinNoise.Generate(25, cSize / 2, x, y);
                Tiles[i] = new Tile(new IntVector2(x, y), (byte)Math.Round((rain / 25)));
            }
            return new Chunk(pos, Tiles);
        }
    }
}
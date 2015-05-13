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
                rain += PerlinNoise.Generate(70, cSize * 16, x, y);
                rain += PerlinNoise.Generate(20, cSize, x, y);
                rain += PerlinNoise.Generate(10, cSize/4, x, y);
                int textureid=-1;
                if (rain>=0&&rain<25)
                {
                    textureid = 0;
                }
                else if (rain>=25&&rain<50)
                {
                    textureid = 1;
                }
                else if (rain>=50&&rain <75)
                {
                    textureid = 2;
                }
                else if (rain >=75 && rain <=100)
                {
                    textureid = 3;
                }
                Tiles[i] = new Tile(new IntVector2(i%cSize, i/cSize), (byte)textureid);
            }
            return new Chunk(pos, Tiles);
        }
    }
}
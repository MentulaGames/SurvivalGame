using Mentula.General;
using Mentula.General.Res;
using System.Collections.Generic;
using System;

namespace Mentula.SurvivalGameServer
{
    public static class MapGenerator
    {
        public static Chunk GenerateTerrain(IntVector2 pos)
        {
            int cSize = int.Parse(Resources.ChunkSize);
            Tile[] Tiles = new Tile[cSize * cSize];
            List<Destructible> destructibles = new List<Destructible>();
            int x;
            int y;
            for (int i = 0; i < cSize * cSize; i++)
            {
                x = i % cSize + pos.X * cSize;
                y = i / cSize + pos.Y * cSize;
                float rain = 0;
                rain += PerlinNoise.Generate(70, cSize * 4, x, y, "1");
                rain += PerlinNoise.Generate(20, cSize, x, y, "2");
                rain += PerlinNoise.Generate(10, cSize / 4, x, y, "3");
                float lakeyness = 0;
                lakeyness += PerlinNoise.Generate(100, cSize / 4, x, y, "lakey");
                float chanceToSpawnTree = Math.Max(rain - 15, 0) / 10;

                int textureid = -1;
                if (rain >= 0 & rain < 25)
                {
                    textureid = 0;
                }
                else if (rain >= 25 & rain < 50)
                {
                    textureid = 1;
                }
                else if (rain >= 50 & rain < 75)
                {
                    textureid = 2;
                }
                else if (rain >= 75 & rain <= 100)
                {
                    textureid = 3;
                }

                if (lakeyness > 80)
                {
                    destructibles.Add(new Destructible(100, new Tile(new IntVector2(i % cSize, i / cSize), 5, 1, true)));
                }
                else if (RNG.RFloatFromString(x, y)*100 <= chanceToSpawnTree)
                {
                    destructibles.Add(new Destructible(100, new Tile(new IntVector2(i % cSize, i / cSize), 4, 1, true)));
                }
                Tiles[i] = new Tile(new IntVector2(i % cSize, i / cSize), (byte)textureid);
            }
            return new Chunk(pos, Tiles, destructibles);
        }
    }
}
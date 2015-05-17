using Mentula.General;
using Mentula.General.Res;
using System;
using System.Collections.Generic;

namespace Mentula.SurvivalGameServer
{
    public static class MapGenerator
    {
        public static Chunk GenerateTerrain(IntVector2 pos)
        {
            int rnum = (int)(RNG.RFloatFromString(pos.X, pos.Y) * 1000000);
            Random r = new Random(rnum);
            Random r2 = new Random(rnum + 1);
            int cSize = int.Parse(Resources.ChunkSize);
            Tile[] Tiles = new Tile[cSize * cSize];
            List<Destructible> destructibles = new List<Destructible>();
            List<Creature> creatures = new List<Creature>();
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
                lakeyness += PerlinNoise.Generate(100, cSize / 2, x, y, "lakey");
                float chanceToSpawnTree = (rain - 30) / 5;
                float chanceToSpawnForestCreature = (rain - 50) / 5;
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

                if (lakeyness > 90)
                {
                    destructibles.Add(new Destructible(100, new Tile(new IntVector2(i % cSize, i / cSize), 5, 1, false)));
                }

                else if ((float)r.NextDouble() * 100 <= chanceToSpawnTree)
                {
                    destructibles.Add(new Destructible(100, new Tile(new IntVector2(i % cSize, i / cSize), 4, 1, true)));
                }
                else if ((float)r2.NextDouble()*100<= chanceToSpawnTree)
                {
                    creatures.Add(ForestWildLife.CreatureList[0]);
                }
                else if ((float)r2.NextDouble()*100<=chanceToSpawnForestCreature)
                {
                    int a = (int)Math.Min(1+r2.NextDouble()*2,2);
                    creatures.Add(ForestWildLife.CreatureList[a]);
                }

                Tiles[i] = new Tile(new IntVector2(i % cSize, i / cSize), (byte)textureid);
            }
            return new Chunk(pos, Tiles, destructibles,creatures);
        }
    }
}
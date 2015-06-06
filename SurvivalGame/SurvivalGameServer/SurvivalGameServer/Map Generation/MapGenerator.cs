using Mentula.General;
using Mentula.General.Resources;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Mentula.SurvivalGameServer
{
    public static class MapGenerator
    {
        public static Chunk GenerateTerrain(IntVector2 pos)
        {
            int rnum = (RNG.RIntFromString(pos.X,pos.Y));
            Random r = new Random(rnum);
            Random r2 = new Random(rnum + 1);
            Tile[] Tiles = new Tile[Res.ChunkSize * Res.ChunkSize];
            List<Destructible> destructibles = new List<Destructible>();
            List<Creature> creatures = new List<Creature>();
            int x;
            int y;

            for (int i = 0; i < Res.ChunkSize * Res.ChunkSize; i++)
            {
                x = i % Res.ChunkSize + pos.X * Res.ChunkSize;
                y = i / Res.ChunkSize + pos.Y * Res.ChunkSize;
                float rain = 0;
                rain += PerlinNoise.Generate(70, Res.ChunkSize * 4, x, y, "1");
                rain += PerlinNoise.Generate(20, Res.ChunkSize, x, y, "2");
                rain += PerlinNoise.Generate(10, Res.ChunkSize / 4, x, y, "3");

                float lakeyness = 0;
                lakeyness += PerlinNoise.Generate(50, Res.ChunkSize / 2, x, y, "lakey");
                lakeyness += PerlinNoise.Generate(50, Res.ChunkSize / 4, x, y, "lakey2");

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

                if (lakeyness > 80)
                {
                    destructibles.Add(new Destructible(100, new Tile(new IntVector2(i % Res.ChunkSize, i / Res.ChunkSize), 5, 1, false)));
                }

                else if ((float)r.NextDouble() * 100 <= chanceToSpawnTree)
                {
                    destructibles.Add(new Destructible(100, new Tile(new IntVector2(i % Res.ChunkSize, i / Res.ChunkSize), 4, 1, true)));
                }
                else if ((float)r2.NextDouble() * 100 <= chanceToSpawnTree / 10)
                {
                    creatures.Add(new Creature(ForestWildLife.CreatureList[0], pos, new Vector2(i % Res.ChunkSize, i / Res.ChunkSize)));
                }
                else if ((float)r2.NextDouble() * 100 <= chanceToSpawnForestCreature / 10)
                {
                    int a = (int)Math.Min(1 + r2.NextDouble() * 2, 2);
                    creatures.Add(new Creature(ForestWildLife.CreatureList[a], pos, new Vector2(i % Res.ChunkSize, i / Res.ChunkSize)));
                }

                Tiles[i] = new Tile(new IntVector2(i % Res.ChunkSize, i / Res.ChunkSize), (byte)textureid);
            }
            return new Chunk(pos, Tiles, destructibles, creatures);
        }
    }
}
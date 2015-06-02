using Mentula.General;
using Mentula.General.Res;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Mentula.SurvivalGameServer
{
    public static class TerrainGenerator
    {
        private static Chunk chunk;
        private static float[] rainArray;
        private static int cSize;
        private static IntVector2 _pos;
        private static Random r;

        static TerrainGenerator()
        {
            cSize = int.Parse(Resources.ChunkSize);
        }

        public static Chunk GenerateAll(IntVector2 pos)
        {
            Init(pos);
            GenerateRain();
            GenerateTerrain();
            GenerateLakes();
            GenerateTrees();
            GenerateCreatures();
            return chunk;
        }
        public static Chunk GenerateTer(IntVector2 pos)
        {
            Init(pos);
            GenerateRain();
            GenerateTerrain();
            GenerateTrees();
            return chunk;
        }

        public static Chunk GenerateTer(Chunk c)
        {
            Init(c.Pos);
            chunk = c;
            GenerateRain();
            GenerateTerrain();
            return chunk;
        }
        private static void Init(IntVector2 pos)
        {
            _pos = pos;
            chunk = new Chunk(pos);
            r = new Random(RNG.RIntFromString(_pos.X, "x", _pos.Y));
        }

        private static void GenerateRain()
        {
            rainArray = new float[cSize * cSize];
            for (int i = 0; i < rainArray.Length; i++)
            {
                int x = _pos.X * cSize + i % cSize;
                int y = _pos.Y * cSize + i / cSize;
                rainArray[i] += PerlinNoise.Generate(70, cSize * 4, x, y, "1");
                rainArray[i] += PerlinNoise.Generate(20, cSize, x, y, "2");
                rainArray[i] += PerlinNoise.Generate(10, cSize / 4, x, y, "3");
            }
        }

        private static void GenerateTerrain()
        {
            for (int i = 0; i < rainArray.Length; i++)
            {
                IntVector2 tPos = new IntVector2(i % cSize, i / cSize);
                byte t = byte.MaxValue;
                if (rainArray[i] > 0 & rainArray[i] <= 25)
                {
                    t = 0;
                }
                else if (rainArray[i] > 25 & rainArray[i] <= 50)
                {
                    t = 1;
                }
                else if (rainArray[i] > 50 & rainArray[i] <= 75)
                {
                    t = 2;
                }
                else if (rainArray[i] > 75 & rainArray[i] <= 100)
                {
                    t = 3;
                }
                chunk.Tiles[i] = new Tile(tPos, t);
            }
        }

        private static void GenerateTrees()
        {
            int destructibleC = chunk.Destructibles.Count;
            for (int i = 0; i < cSize * cSize; i++)
            {
                bool isinuse = false;
                IntVector2 p = new IntVector2(i % cSize, i / cSize);
                for (int j = 0; j < destructibleC; j++)
                {
                    if (chunk.Destructibles[j].Pos == p)
                    {
                        isinuse = true;
                    }
                }
                if (!isinuse)
                {
                    float chanceToSpawnTrees = (rainArray[i] - 30) / 5;
                    if (r.NextDouble() * 100 <= chanceToSpawnTrees)
                    {
                        chunk.Destructibles.Add(new Destructible(100, p, 4, 2, false));
                    }
                }
            }

        }
        private static void GenerateLakes()
        {
            for (int i = 0; i < cSize * cSize; i++)
            {
                IntVector2 p = new IntVector2(i % cSize, i / cSize);
                float lake = PerlinNoise.Generate(50, cSize / 2, p.X + _pos.X * cSize, p.Y + _pos.Y * cSize, "lakey");
                lake += PerlinNoise.Generate(50, cSize / 4, p.X + _pos.X * cSize, p.Y + _pos.Y * cSize, "lakey2");
                if (lake > 80)
                {
                    chunk.Destructibles.Add(new Destructible(100, p, 5, 2, false));
                }
            }
        }

        private static void GenerateCreatures()
        {
            for (int i = 0; i < cSize * cSize; i++)
            {
                bool isInUse = false;
                IntVector2 p = new IntVector2(i % cSize, i / cSize);
                for (int j = 0; j < chunk.Destructibles.Count; j++)
                {
                    if (chunk.Destructibles[j].Pos == p)
                    {
                        isInUse = true;
                    }
                }
                if (!isInUse)
                {
                    float chanceToSpawnRabbits = (rainArray[i] - 30) / 50;
                    float chanceToSpawnDif = (rainArray[i] - 50) / 50;
                    if (r.NextDouble() * 100 <= chanceToSpawnRabbits)
                    {
                        chunk.Creatures.Add(new Creature(ForestWildLife.CreatureList[0], _pos, p.ToVector2()));
                    }
                    else if (r.NextDouble()*100<=chanceToSpawnDif)
                    {
                        int index = (int)Math.Min(1 + r.NextDouble() * 2, 2);
                        chunk.Creatures.Add(new Creature(ForestWildLife.CreatureList[index], _pos, p.ToVector2()));
                    }
                }
            }
        }
    }
}
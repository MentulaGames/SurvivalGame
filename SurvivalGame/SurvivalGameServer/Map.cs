using Mentula.General;
using Mentula.General.Res;
using System;
using System.Collections.Generic;

namespace Mentula.SurvivalGameServer
{
    public class Map
    {
        public List<Chunk> ChunkList = new List<Chunk>();
        public List<Chunk> LoadedChunks = new List<Chunk>();
        public int RTL = 2;
        public int RTL_C = 1;

        public bool Generate(IntVector2 pos)
        {
            bool gen = false;

            for (int y = -RTL; y <= RTL; y++)
            {
                for (int x = -RTL; x <= RTL; x++)
                {
                    bool chunkexists = false;

                    for (int i = 0; i < ChunkList.Count; i++)
                    {
                        if (ChunkList[i].Pos.X == x + pos.X && ChunkList[i].Pos.Y == y + pos.Y)
                        {
                            chunkexists = true;
                            break;
                        }
                    }
                    if (!chunkexists)
                    {
                        gen = true;
                        Chunk generatedChunk = MapGenerator.GenerateTerrain(new IntVector2(x + pos.X, y + pos.Y));
                        ChunkList.Add(generatedChunk);
                    }
                }
            }

            return gen;
        }

        public void LoadChunks(IntVector2 pos)
        {
            for (int y = -RTL; y <= RTL; y++)
            {
                for (int x = -RTL; x <= RTL; x++)
                {
                    for (int i = 0; i < ChunkList.Count; i++)
                    {
                        if (ChunkList[i].Pos.X == x + pos.X && ChunkList[i].Pos.Y == y + pos.Y)
                        {
                            bool chunkisloaded = false;

                            for (int j = 0; j < LoadedChunks.Count; j++)
                            {
                                if (LoadedChunks[j] == ChunkList[i])
                                {
                                    chunkisloaded = true;
                                }
                            }

                            if (!chunkisloaded)
                            {
                                LoadedChunks.Add(ChunkList[i]);
                            }

                        }
                    }
                }
            }
        }

        public Chunk[] GetChunks(IntVector2 pos)
        {
            int cSize = int.Parse(Resources.ChunkSize);
            int c = 0;
            Chunk[] result = new Chunk[(RTL_C * 2 + 1) * (RTL_C * 2 + 1)];

            for (int y = -RTL_C; y <= RTL_C; y++)
            {
                for (int x = -RTL_C; x <= RTL_C; x++)
                {
                    for (int i = 0; i < LoadedChunks.Count; i++)
                    {
                        if (LoadedChunks[i].Pos.X == x + pos.X && LoadedChunks[i].Pos.Y == y + pos.Y)
                        {
                            result[c] = LoadedChunks[i];
                            c++;
                        }
                    }
                }
            }

            return result;
        }

        public List<Chunk> GetChunks(IntVector2 oldPos, IntVector2 newPos)
        {
            List<Chunk> r = new List<Chunk>();
            for (int x = RTL_C * -1; x <= RTL_C; x++)
            {
                for (int y = RTL_C * -1; y <= RTL_C; y++)
                {
                    for (int i = 0; i < ChunkList.Count; i++)
                    {
                        if (Math.Abs(newPos.X + x - oldPos.X) < RTL_C | Math.Abs(newPos.Y + y - oldPos.Y) < RTL_C)
                        {
                            r.Add(ChunkList[i]);
                        }
                    }
                }
            }
            return r;
        }

        public void UnloadChunks()
        {
            LoadedChunks = new List<Chunk>();
        }

        public void UnloadChunks(IntVector2 pos)
        {
            for (int i = 0; i < LoadedChunks.Count; )
            {
                if (Math.Abs(LoadedChunks[i].Pos.X - pos.X) > RTL | Math.Abs(LoadedChunks[i].Pos.Y - pos.Y) > RTL)
                {
                    LoadedChunks.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }

        public void UnloadChunks(IntVector2[] pos)
        {
            for (int i = 0; i < LoadedChunks.Count; )
            {
                bool isnearplayer = false;
                for (int p = 0; p < pos.Length; p++)
                {
                    if (Math.Abs(LoadedChunks[i].Pos.X - pos[p].X) <= RTL & Math.Abs(LoadedChunks[i].Pos.Y - pos[p].Y) <= RTL)
                    {
                        isnearplayer = true;
                    }
                }
                if (!isnearplayer)
                {
                    LoadedChunks.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
using Mentula.General;
using System;
using System.Collections.Generic;
using Mentula.General.Res;

namespace Mentula.SurvivalGameServer
{
    public class Map
    {
        public List<Chunk> ChunkList = new List<Chunk>();
        public List<Chunk> LoadedChunks = new List<Chunk>();
        public int RTL = 2;
        public int RTLC = 1;

        public void Generate(IntVector2 pos)
        {
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
                        Chunk generatedChunk = MapGenerator.GenerateTerrain(new IntVector2(x + pos.X, y + pos.Y));
                        ChunkList.Add(generatedChunk);
                    }
                }
            }
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
                            if (LoadedChunks.Find(c => c.Pos == ChunkList[i].Pos) == null)
                            {
                                LoadedChunks.Add(ChunkList[i]);
                            }
                        }
                    }
                }
            }
        }
        public CTile[] GetChunks(IntVector2 pos)
        {
            int cSize = int.Parse(Resources.ChunkSize);
            CTile[] Ctilearray = new CTile[(RTLC * 2 + 1) * (RTLC * 2 + 1) * cSize * cSize];
            int c = 0;
            int length = LoadedChunks.Count;

            for (int i = 0; i < length; i++)
            {
                if (Math.Abs(LoadedChunks[i].Pos.X - pos.X) <= RTLC && Math.Abs(LoadedChunks[i].Pos.Y - pos.Y) <= RTLC)
                {
                    for (int j = 0; j < cSize * cSize; j++)
                    {
                        Ctilearray[c] = new CTile(LoadedChunks[i].Pos, LoadedChunks[i].Tiles[j]);
                        c++;
                    }
                }
            }

            return Ctilearray;
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
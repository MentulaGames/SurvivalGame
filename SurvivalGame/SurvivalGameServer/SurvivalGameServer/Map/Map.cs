using Mentula.General;
using Mentula.General.Resources;
using System;
using System.Collections.Generic;

namespace Mentula.SurvivalGameServer
{
    public class Map
    {
        public List<MegaChunk> MegaChunkList;
        public List<Chunk> ChunkList;
        public List<Chunk> LoadedChunks;
        public const int RTL = 2;
        public const int RTL_C = 1;
        public const int MCS = MegaChunk.MEGACHUNKSIZE;

        public Map()
        {
            MegaChunkList = new List<MegaChunk>();
            MegaChunkList.Add(new MegaChunk(IntVector2.Zero));
            ChunkList = new List<Chunk>();
            LoadedChunks = new List<Chunk>();

        }

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
                        if (ChunkList[i].Pos.X == x + pos.X & ChunkList[i].Pos.Y == y + pos.Y)
                        {
                            chunkexists = true;
                            break;
                        }
                    }
                    if (!chunkexists)
                    {
                        int mchunkx = 0;
                        int mchunky = 0;
                        int xinmchunk = x + pos.X;
                        int yinmchunk = y + pos.Y;
                        while (xinmchunk < 0 | xinmchunk > MCS | yinmchunk < 0 | yinmchunk > MCS)
                        {
                            if (xinmchunk < 0)
                            {
                                mchunkx--;
                                xinmchunk += MCS;
                            }
                            if (xinmchunk > MCS)
                            {
                                mchunkx++;
                                xinmchunk -= MCS;
                            }
                            if (yinmchunk < 0)
                            {
                                mchunky--;
                                yinmchunk += MCS;
                            }
                            if (yinmchunk > MCS)
                            {
                                mchunky++;
                                yinmchunk -= MCS;
                            }
                        }
                        int mchunknum = 0;
                        bool mchunkexists = false;
                        for (int i = 0; i < MegaChunkList.Count; i++)
                        {
                            if (MegaChunkList[i].Pos.X == mchunkx & MegaChunkList[i].Pos.Y == mchunky)
                            {
                                mchunknum = i;
                                mchunkexists = true;
                            }
                        }
                        if (!mchunkexists)
                        {
                            MegaChunkList.Add(new MegaChunk(new IntVector2(mchunkx, mchunky)));

                        }
                        if (MegaChunkList[mchunknum].ChunkData[xinmchunk % 128 + (yinmchunk * MCS)].ChunkType == 1)
                        {
                            gen = true;
                            Chunk generatedChunk = HouseGenerator.GenerateHouse(new IntVector2(pos.X + x, pos.Y + y));
                            ChunkList.Add(generatedChunk);
                        }
                        else if (MegaChunkList[mchunknum].ChunkData[xinmchunk % 128 + (yinmchunk * MCS)].ChunkType == 2)
                        {
                            gen = true;
                            Chunk generatedChunk = new Chunk(new IntVector2(pos.X + x, pos.Y + y));
                            generatedChunk.Generate(9);
                            ChunkList.Add(generatedChunk);
                        }
                        else
                        {
                            gen = true;
                            Chunk generatedChunk = TerrainGenerator.GenerateAll(new IntVector2(x + pos.X, y + pos.Y));
                            ChunkList.Add(generatedChunk);
                        }
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
                        if (ChunkList[i].Pos.X == x + pos.X & ChunkList[i].Pos.Y == y + pos.Y)
                        {
                            bool chunkisloaded = false;

                            for (int j = 0; j < LoadedChunks.Count; j++)
                            {
                                if (LoadedChunks[j] == ChunkList[i]) chunkisloaded = true;
                            }

                            if (!chunkisloaded) LoadedChunks.Add(ChunkList[i]);
                        }
                    }
                }
            }
        }

        public Chunk[] GetChunks(IntVector2 pos)
        {
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
                        bool isloaded = false;
                        for (int j = 0; j < r.Count; j++)
                        {
                            if (r[j] == ChunkList[i]) isloaded = true;
                        }
                        //if its not next to the old tilePos
                        //and it is next to the new tilePos
                        //and it is not already loaded
                        if ((Math.Abs(ChunkList[i].Pos.X - oldPos.X) > RTL_C | Math.Abs(ChunkList[i].Pos.Y - oldPos.Y) > RTL_C) &
                            (Math.Abs(ChunkList[i].Pos.X - newPos.X) <= RTL_C & Math.Abs(ChunkList[i].Pos.Y - newPos.Y) <= RTL_C) &
                            !isloaded)
                        {
                            r.Add(ChunkList[i]);
                        }
                    }
                }
            }
            return r;
        }

        public void UnloadChunks(IntVector2 pos)
        {
            for (int i = 0; i < LoadedChunks.Count; )
            {
                if (Math.Abs(LoadedChunks[i].Pos.X - pos.X) > RTL | Math.Abs(LoadedChunks[i].Pos.Y - pos.Y) > RTL) LoadedChunks.RemoveAt(i);
                else i++;
            }
        }

        public void UnloadChunks(IntVector2[] pos)
        {
            for (int i = 0; i < LoadedChunks.Count; )
            {
                bool isnearplayer = false;
                for (int p = 0; p < pos.Length; p++)
                {
                    if (Math.Abs(LoadedChunks[i].Pos.X - pos[p].X) <= RTL & Math.Abs(LoadedChunks[i].Pos.Y - pos[p].Y) <= RTL) isnearplayer = true;
                }

                if (!isnearplayer) LoadedChunks.RemoveAt(i);
                else i++;
            }
        }
    }
}
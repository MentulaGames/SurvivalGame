using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.General;
using MEx = Mentula.General.MathExtensions.Math;
using Microsoft.Xna.Framework;

namespace Mentula.SurvivalGameServer
{
    public static class MegaChunkGenerator
    {
        private static ChunkData[] ChunkDataArray;
        private static Random r;
        private static City[] cities;
        private const int MCS = MegaChunk.MEGACHUNKSIZE;
        private const int CITYSIZE = 8;
        public static ChunkData[] GenerateMegaChunk(IntVector2 pos)
        {
            r = new Random(RNG.RIntFromString(pos.X, "index", pos.Y));
            ChunkDataArray = new ChunkData[MCS * MCS];
            for (int i = 0; i < ChunkDataArray.Length; i++)
            {
                ChunkDataArray[i] = new ChunkData(new IntVector2(i % MCS, i / MCS));
            }
            GenerateCities();

            return ChunkDataArray;
        }
        private static void GenerateCities()
        {
            int numOfCities = (int)(r.NextDouble() * 5 + 1);
            cities = new City[numOfCities];
            for (int i = 0; i < cities.Length; i++)
            {
                cities[i] = new City(IntVector2.Zero, 0);

            }
            //
            for (int i = 0; i < numOfCities; i++)
            {
                bool isnearothercity = true;
                do
                {
                    isnearothercity = false;
                    float cs = CITYSIZE + (float)(CITYSIZE * r.NextDouble());
                    cities[i] = new City(new IntVector2((int)(cs + (MCS - cs * 2) * r.NextDouble()), (int)(cs + (MCS - cs * 2) * r.NextDouble())), cs);
                    for (int j = 0; j < numOfCities; j++)
                    {
                        if (i != j)
                        {
                            if (MEx.GetMaxDiff(cities[i].Pos, cities[j].Pos) < cities[i].CitySize + cities[j].CitySize)
                            {
                                isnearothercity = true;
                            }
                        }
                    }
                } while (isnearothercity);
            }
            //
            for (int i = 0; i < cities.Length; i++)
            {
                int citySize = (int)cities[i].CitySize;
                int numOfBuildings = (int)(cities[i].CitySize * cities[i].CitySize) / 2;
                int numOfBuildingsToPlace = numOfBuildings;
                int numOfHousesToPlace = numOfBuildings;
                cities[i].CityChunks.Add(new ChunkData(IntVector2.Zero, 1));
                bool allBuildingsAccesible = false;
                do
                {
                    allBuildingsAccesible = true;
                    while (numOfBuildingsToPlace > 0)
                    {
                        IntVector2 posToPlace = IntVector2.Zero;
                        Vector2 CheckingPos = Vector2.Zero;
                        bool FinnishedRayTracing = false;
                        Vector2 dir = new Vector2((float)(r.NextDouble() - 0.5f), (float)(r.NextDouble() - 0.5f));
                        dir.Normalize();
                        while (!FinnishedRayTracing & ((Math.Abs(posToPlace.X) <= cities[i].CitySize) & Math.Abs(posToPlace.Y) <= cities[i].CitySize))
                        {
                            bool buildingIsOnPos = false;
                            while ((int)CheckingPos.X == posToPlace.X & (int)CheckingPos.Y == posToPlace.Y)
                            {
                                CheckingPos += (dir / 10);
                            }
                            posToPlace = new IntVector2(CheckingPos);
                            for (int j = 0; j < cities[i].CityChunks.Count; j++)
                            {
                                if (cities[i].CityChunks[j].ChunkPos == posToPlace)
                                {
                                    buildingIsOnPos = true;
                                }
                            }
                            if (!buildingIsOnPos)
                            {
                                cities[i].CityChunks.Add(new ChunkData(posToPlace, 1));
                                numOfBuildingsToPlace--;
                                FinnishedRayTracing = true;
                            }
                        }
                    }
                    AStar.Node[] NodeArray = new AStar.Node[citySize * citySize * 4];
                    IntVector2 offset = new IntVector2(citySize);
                    for (int j = 0; j < NodeArray.Length; j++)
                    {
                        IntVector2 NodePos = new IntVector2(j % (citySize * 2), j / (citySize * 2));
                        NodeArray[j] = new AStar.Node(NodePos, 2000);
                    }

                    for (int j = 0; j < cities[i].CityChunks.Count; j++)
                    {
                        IntVector2 pos = cities[i].CityChunks[j].ChunkPos+offset;
                        int index = (citySize + pos.X) + (citySize + pos.Y) * citySize;
                        if (cities[i].CityChunks[j].ChunkType != 2)
                        {
                            NodeArray[index] = new AStar.Node(cities[i].CityChunks[j].ChunkPos, 8000 + (int)(r.NextDouble() * 8000));
                        }
                        else
                        {
                            NodeArray[index] = new AStar.Node(pos, -10);
                        }
                    }

                    int count = cities[i].CityChunks.Count;
                    List<ChunkData> alist = new List<ChunkData>(cities[i].CityChunks);
                    List<ChunkData> blist = new List<ChunkData>();

                    for (int j = 0; j < alist.Count; )
                    {
                        int rand = (int)(r.NextDouble() * alist.Count);
                        blist.Add(alist[rand]);
                        alist.RemoveAt(rand);
                    }

                    for (int j = 0; j < count - 1; j++)
                    {
                        if (blist[j + 1].ChunkType != 2)
                        {
                            AStar.Map asdfg = new AStar.Map(citySize << 1, blist[j].ChunkPos+offset, blist[j + 1].ChunkPos+offset, NodeArray);
                            AStar.Node[] path = AStar.GetRoute(asdfg);
                            for (int k = 0; k < path.Length; k++)
                            {
                                int index=0;
                                ChunkData c = new ChunkData();
                                for (int l = 0; l < count; l++)
                                {
                                    if (blist[l].ChunkPos == path[k].Position-offset)
                                    {
                                        index = l;
                                        c = blist[l];
                                    }
                                }
                                if (c.ChunkType == 1)
                                {
                                    allBuildingsAccesible = false;
                                    cities[i].CityChunks[index].ChunkType = 2;
                                    numOfBuildingsToPlace++;
                                    numOfHousesToPlace++;
                                }
                            }
                        }
                    }
                } while (!allBuildingsAccesible);
            }
            //endfor
            for (int i = 0; i < cities.Length; i++)
            {
                for (int j = 0; j < cities[i].CityChunks.Count; j++)
                {
                    int index = ((cities[i].Pos.X + cities[i].CityChunks[j].ChunkPos.X)) + ((cities[i].Pos.Y + cities[i].CityChunks[j].ChunkPos.Y) * MCS);
                    ChunkDataArray[index] = cities[i].CityChunks[j];
                }
            }
        }
    }
}

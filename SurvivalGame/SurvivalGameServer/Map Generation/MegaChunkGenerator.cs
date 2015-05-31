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
                //int numOfHousesToPlace = numOfBuildings;
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
                    //generate a nodearray
                    IntVector2 offset = new IntVector2(citySize);
                    AStar.Node[] nodeArray = new AStar.Node[citySize * citySize * 4];
                    for (int j= 0; j < nodeArray.Length; j++)
                    {
                        IntVector2 pos = new IntVector2(j%(citySize*2),j/(citySize*2));
                        nodeArray[j]= new AStar.Node(pos,2000,false);
                    }
                    for (int j = 0; j < cities[i].CityChunks.Count; j++)
                    {
                        int index = (cities[i].CityChunks[j].ChunkPos.X + citySize) + (cities[i].CityChunks[j].ChunkPos.Y + citySize) * (citySize+2);
                        if (cities[i].CityChunks[j].ChunkType!=2)
                        {
                            nodeArray[index] = new AStar.Node(cities[i].CityChunks[j].ChunkPos + offset,4000+(int)(r.NextDouble()*4000),true);
                        }
                        else
                        {
                            nodeArray[index] = new AStar.Node(cities[i].CityChunks[j].ChunkPos + offset, -10,true);
                        }
                    }
                    //end generating a nodearray
                    // generate streets between all houses
                    List<ChunkData> alist =new List<ChunkData>( cities[i].CityChunks);
                    List<ChunkData> blist = new List<ChunkData>();
                    for (int j = 0; j < alist.Count;)
                    {
                        int index = (int)(r.NextDouble() * alist.Count);
                        blist.Add(alist[index]);
                        alist.RemoveAt(index);
                    }
                    int count = blist.Count - 1;
                    for (int j = 0; j < count; j++)
                    {
                        AStar.Map cityMap = new AStar.Map(citySize << 1, blist[j].ChunkPos+offset, blist[j + 1].ChunkPos+offset, nodeArray);
                        AStar.Node[] path = AStar.GetRoute(cityMap);
                        for (int k = 0; k < path.Length; k++)
                        {
                            ChunkData c=new ChunkData();
                            int chunkIndex = -1;
                            for (int l = 0; l < blist.Count; l++)
                            {
                                if (blist[l].ChunkPos==path[k].Position-offset)
                                {
                                    chunkIndex = l;
                                    c = blist[l];
                                    break;
                                }
                            }
                            if (c.ChunkType!=2)
                            {
                                int nodeIndex=(c.ChunkPos.X+offset.X)+(c.ChunkPos.Y+offset.Y)*citySize*2;
                                nodeArray[nodeIndex]= new AStar.Node(cities[i].CityChunks[j].ChunkPos + offset, 0,true);
                                blist[chunkIndex].ChunkType = 2;
                            }
                        }
                    }
                    cities[i].CityChunks=blist;
                } while (!allBuildingsAccesible);
            }
            //end generating all cities
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

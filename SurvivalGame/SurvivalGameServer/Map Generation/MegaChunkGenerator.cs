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
            r = new Random((int)(RNG.RFloatFromString(pos.X, "a", pos.Y) * 1000000));
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
                int numOfBuildings = (int)(cities[i].CitySize * cities[i].CitySize) / 2;
                int numOfBuildingsToPlace = numOfBuildings;
                int numOfHousesToPlace = numOfBuildings;
                cities[i].CityChunks.Add(new ChunkData(IntVector2.Zero, 1));
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

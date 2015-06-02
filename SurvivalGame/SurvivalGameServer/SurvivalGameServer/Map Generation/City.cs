using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.General;

namespace Mentula.SurvivalGameServer
{
    public class City
    {
        public IntVector2 Pos;
        public List<ChunkData> CityChunks;
        public float CitySize;
        public City(IntVector2 pos,float citySize)
        {
            Pos = pos;
            CitySize = citySize;
            CityChunks = new List<ChunkData>();
        }
    }
}

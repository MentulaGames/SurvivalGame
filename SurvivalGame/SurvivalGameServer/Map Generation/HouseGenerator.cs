using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.General;


namespace Mentula.SurvivalGameServer
{
    public static class HouseGenerator
    {
        public static Chunk GenerateHouse(IntVector2 pos)
        {
            Chunk result = new Chunk(pos);
            result.Generate(2);
            result.Destructibles = BuildingGenerator.GenerateBuilding(new IntVector2(4, 4), new IntVector2(24, 16), new IntVector2(3, 3), new IntVector2(7, 7), string.Format("{0} x {1}", pos.X, pos.Y));
            return result;
        }
    }
}

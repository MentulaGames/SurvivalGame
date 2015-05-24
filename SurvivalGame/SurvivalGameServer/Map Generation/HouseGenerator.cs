using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.General;
using MEx = Mentula.General.MathExtensions.Math;


namespace Mentula.SurvivalGameServer
{
    public static class HouseGenerator
    {
        public static Chunk GenerateHouse(IntVector2 pos)
        {
            Random r = new Random(RNG.RIntFromString(pos.X, pos.Y));
            Chunk result = new Chunk(pos);
            IntVector2 posToPlace = new IntVector2(MEx.Round(2 + r.NextDouble() * 4), MEx.Round(2 + r.NextDouble() * 4));
            IntVector2 size = new IntVector2(MEx.Round(22 + r.NextDouble() * 4), MEx.Round(14 + r.NextDouble() * 4));
            result.Generate(2);
            result.Destructibles = BuildingGenerator.GenerateBuilding(posToPlace, size, new IntVector2(3, 3), new IntVector2(7, 7), string.Format("{0} x {1}", pos.X, pos.Y));
            return result;
        }
    }
}

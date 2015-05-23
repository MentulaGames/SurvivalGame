using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.General;
using Microsoft.Xna.Framework;

namespace Mentula.SurvivalGameServer
{
    public static class BuildingGenerator
    {
        public static List<Destructible> GenerateBuilding(IntVector2 pos, IntVector2 size, IntVector2 minRoomSize, IntVector2 maxRoomSize)
        {
            List<Destructible> dList = new List<Destructible>();
            List<Rectangle> binaryMap = BinarySplitGenerator.GenerateBinarySplitMap(size - new IntVector2(1), minRoomSize, maxRoomSize, "a");
            for (int i = 0; i < binaryMap.Count; i++)
            {
                for (int x = 0; x < binaryMap[i].Width; x++)
                {
                    for (int y = 0; y < binaryMap[i].Height; y++)
                    {
                        IntVector2 desPos = new IntVector2(binaryMap[i].X + x , binaryMap[i].Y + y ) + pos;
                        if (x == 0 | y == 0)
                        {
                            dList.Add(new Destructible(100, desPos, 4, 2, false));
                        }
                        else
                        {
                            dList.Add(new Destructible(100, desPos, 8, 2, true));
                        }
                    }
                }
            }
            for (int i = 0; i < size.X; i++)
            {
                dList.Add(new Destructible(100, new IntVector2(pos.X + i, pos.Y+size.Y-1), 4, 2, false));
            }
            for (int i = 0; i < size.Y-1; i++)
            {
                dList.Add(new Destructible(100, new IntVector2(pos.X+size.X-1, pos.Y + i), 4, 2, false));
            }
            return dList;
        }
    }
}

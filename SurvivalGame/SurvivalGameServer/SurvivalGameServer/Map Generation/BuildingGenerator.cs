﻿using System;
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
        public static List<Destructible> GenerateBuilding(IntVector2 pos, IntVector2 size, IntVector2 minRoomSize, IntVector2 maxRoomSize, string seed)
        {
            Random r = new Random(RNG.RIntFromString(pos.X, "b", pos.Y));
            Destructible[] dList = new Destructible[size.X * size.Y];
            List<Rectangle> binaryMap = BinarySplitGenerator.GenerateBinarySplitMap(size - new IntVector2(1), minRoomSize, seed);
            for (int i = 0; i < binaryMap.Count; i++)
            {
                for (int x = 0; x < binaryMap[i].Width; x++)
                {
                    for (int y = 0; y < binaryMap[i].Height; y++)
                    {
                        IntVector2 desPos = new IntVector2(binaryMap[i].X + x, binaryMap[i].Y + y);
                        if (x == 0 | y == 0)
                        {
                            dList[desPos.X % size.X + desPos.Y * size.X] = new Destructible(100, desPos + pos, 11, 2, false);
                        }
                        else
                        {
                            dList[desPos.X % size.X + desPos.Y * size.X] = new Destructible(100, desPos + pos, 12, 2, true);
                        }
                    }
                }
            }
            for (int i = 0; i < size.X; i++)
            {
                dList[i % size.X + (size.Y - 1) * size.X] = new Destructible(100, new IntVector2(pos.X + i, pos.Y + size.Y - 1), 11, 2, false);

                //dList.Add(new Destructible(100, new IntVector2(pos.X + i, pos.Y + size.Y - 1), 11, 2, false));
            }
            for (int i = 0; i < size.Y - 1; i++)
            {
                dList[size.X - 1 + (i) * size.X] = new Destructible(100, new IntVector2(pos.X + size.X - 1, pos.Y + i), 11, 2, false);
            }
            List<Rectangle> rl= new List<Rectangle>(binaryMap); 
            List<Rectangle> a = new List<Rectangle>();
            for (int i = 0; i < rl.Count; )
            {
                int f = (int)(r.NextDouble() * rl.Count);
                a.Add(rl[f]);
                rl.RemoveAt(f);
            }
            for (int i = 0; i < a.Count - 1; i++)
            {
                IntVector2 startpos = new IntVector2(a[i].X + a[i].Width / 2, a[i].Y + a[i].Height / 2);
                IntVector2 endpos = new IntVector2(a[i + 1].X + a[i + 1].Width / 2, a[i + 1].Y + a[i + 1].Height / 2);
                AStar.Node[] NodeArray = new AStar.Node[size.X * size.Y];
                for (int j = 0; j < NodeArray.Length; j++)
                {
                    IntVector2 p = new IntVector2(j % size.X, j / size.X);
                    if (dList[j].Walkable)
                    {
                        NodeArray[j] = new AStar.Node(p, -10);
                        //NodeArray[j].GValue = 10;
                    }
                    else
                    {
                        int sWalls = 0;
                        if (j - 1 > 0)
                        {
                            if (!dList[j - 1].Walkable)
                            {
                                sWalls++;
                            }
                        }
                        if (j + 1 < dList.Length)
                        {
                            if (!dList[j + 1].Walkable)
                            {
                                sWalls++;
                            }
                        }
                        if (j - size.X > 0)
                        {
                            if (!dList[j - size.X].Walkable)
                            {
                                sWalls++;
                            }
                        }
                        if (j + size.X < dList.Length)
                        {
                            if (!dList[j + size.X].Walkable)
                            {
                                sWalls++;
                            }
                        }
                        if (sWalls > 2)
                        {
                            NodeArray[j] = new AStar.Node(p, 0, false);
                        }
                        else
                        {
                            NodeArray[j] = new AStar.Node(p, 2001 + r.Next(0, 20000));
                        }

                    }
                }
                AStar.Node[] path = AStar.GetRoute(new AStar.Map(size.X, startpos, endpos, NodeArray));
                for (int j = 0; j < path.Length; j++)
                {
                    int index = (int)(path[j].Position.X % size.X + path[j].Position.Y * size.X);
                    if (!dList[index].Walkable)
                    {

                        dList[index] = new Destructible(100, path[j].Position + pos, 12, 2, true);
                    }
                }
            }
            List<Rectangle> y0Rec = new List<Rectangle>();
            for (int i = 0; i < binaryMap.Count; i++)
            {
                if (binaryMap[i].Y == 0)
                {
                    y0Rec.Add(binaryMap[i]);
                }
            }
            int asdf = (int)(r.NextDouble() * y0Rec.Count);
            int g = y0Rec[asdf].X + 1 + (int)(r.NextDouble() * (y0Rec[asdf].Width - 2));
            dList[g] = new Destructible(100, new IntVector2(g, 0) + pos, 12, 2, true);
            return dList.ToList();
        }
    }
}

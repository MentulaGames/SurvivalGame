using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.General;
using Microsoft.Xna.Framework;

namespace Mentula.SurvivalGameServer
{
    public static class BinarySplitGenerator
    {
        public static List<Rectangle> GenerateBinarySplitMap(IntVector2 bounds, IntVector2 minSize, string seed)
        {
            Random r = new Random(RNG.RIntFromString(seed));
            List<Rectangle> rectangles = new List<Rectangle>();
            rectangles.Add(new Rectangle(0, 0, bounds.X, bounds.Y));

            while (true)
            {
                List<Rectangle> rectanglesToSplit = new List<Rectangle>();
                for (int i = 0; i < rectangles.Count; i++)
                {
                    if (rectangles[i].Width > minSize.X * 2 + 1 | rectangles[i].Height > minSize.Y * 2 + 1)
                    {
                        rectanglesToSplit.Add(rectangles[i]);
                    }
                }
                if (rectanglesToSplit.Count == 0)
                {
                    break;
                }
                int ra = (int)(r.NextDouble() * rectanglesToSplit.Count);
                float p = (float)r.NextDouble();

                Rectangle splitting = rectanglesToSplit[ra];
                bool dir;
                if (splitting.Width < minSize.X * 2)
                {
                    dir = false;
                }
                else if (splitting.Height < minSize.Y * 2)
                {
                    dir = true;
                }
                else
                {
                    dir = Convert.ToBoolean(Math.Round(r.NextDouble()));
                }

                Rectangle[] sp = new Rectangle[2];
                if (dir)
                {
                    sp[0] = new Rectangle(splitting.X, splitting.Y, (int)Math.Round(splitting.Width * p), splitting.Height);
                    sp[1] = new Rectangle(splitting.X + sp[0].Width, splitting.Y, splitting.Width - sp[0].Width, splitting.Height);
                }
                else
                {
                    sp[0] = new Rectangle(splitting.X, splitting.Y, splitting.Width, (int)Math.Round( splitting.Height * p));
                    sp[1] = new Rectangle(splitting.X, splitting.Y + sp[0].Height, splitting.Width, splitting.Height - sp[0].Height);
                }
                if (sp[0].Width > minSize.X & sp[0].Height > minSize.Y & sp[1].Width > minSize.X & sp[1].Height > minSize.Y)
                {
                    for (int i = 0; i < rectangles.Count; i++)
                    {
                        if (rectangles[i].X == sp[0].X & rectangles[i].Y == sp[0].Y)
                        {
                            rectangles.RemoveAt(i);
                        }
                    }

                    rectangles.Add(sp[0]);
                    rectangles.Add(sp[1]);

                }
            }
            return rectangles;
        }
    }
}

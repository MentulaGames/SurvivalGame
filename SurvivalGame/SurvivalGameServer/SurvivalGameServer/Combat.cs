using Mentula.Content;
using Mentula.General.Resources;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using MEx = Mentula.General.MathExtensions.Math;


namespace Mentula.SurvivalGameServer
{
    public static class Combat
    {
        private static Random r;

        static Combat()
        {
            r = new Random();
        }

        public static List<Creature> AttackCreatures(Creature attacker,ImpactObject im, Creature[] creatures, float degrees, float arc, float range)
        {
            Vector2 apos = attacker.GetTotalPos();
            List<Creature> creatureArray = creatures.ToList();
            for (int i = 0; i < creatureArray.Count; )
            {
                bool remain = true;

                if (creatureArray[i] != attacker)
                {
                    Vector2 bpos = creatureArray[i].GetTotalPos();
                    float dist = (apos - bpos).Length();
                    if (dist < range)
                    {
                        float bdeg = MEx.VectorToDegrees(bpos - apos+new Vector2(0.5f,0.5f));
                        if (MEx.DifferenceBetweenDegrees(degrees, bdeg) < arc / 2)
                        {

                            int b =(int)(r.NextDouble()*creatures[i].Parts.Length);
                            TissueLayer[] a = creatures[i].Parts[b].Layers;
                            MaterialLayer[] ml = a;
                            ImpactSimulator.OnHit(ref ml,ref im);
                            for (int j = 0; j < a.Length; j++)
                            {
                                if (a[j].essential&a[j].BiggestHoleSize>0)
                                {
                                    creatureArray[i].Alive = false;
                                }
                            }
                        }
                    }
                }

                if (remain) i++;
            }
            return creatureArray;
        }
    }
}

﻿using Mentula.General.Res;
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
        private static int cSize = int.Parse(Resources.ChunkSize);
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
                            int b =(int)(r.NextDouble()*creatures[i].bhe.Length);
                            MaterialLayer[] ml = creatures[i].bhe[b].Layers;
                            ImpactSimulator.OnHit(ref ml,ref im);
                        }
                    }
                }

                if (remain) i++;
            }
            return creatureArray;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Mentula.General.Res;
using Mentula.General;

using MEx = Mentula.MathExtensions.Math;

namespace Mentula.SurvivalGameServer
{
    public static class Combat
    {
        private static int cSize = int.Parse(Resources.ChunkSize);
        public static Creature[] AttackCreatures(Creature attacker, Creature[] creatures, float degrees,float arc,float range)
        {
            Vector2 apos= attacker.GetTotalPos();
            Creature[] creatureArray = creatures;
            for (int i = 0; i < creatures.Length; i++)
            {
                if (creatureArray[i]!=attacker)
                {
                    Vector2 bpos=creatureArray[i].GetTotalPos();
                    float dist = Vector2.Distance(apos, bpos);
                    
                    if (dist<range)
                    {
                        float bdeg=MEx.VectorToDegrees(bpos-apos);
                        if (MEx.DifferenceBetweenDegrees(degrees,bdeg)<arc/2)
                        {
                            creatureArray[i].Health -= attacker.Stats.Str;
                        }
                    }
                }
            }
            return creatureArray;
        }
    }
}

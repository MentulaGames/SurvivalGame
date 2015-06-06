using Mentula.General.Resources;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using MEx = Mentula.General.MathExtensions.Math;


namespace Mentula.SurvivalGameServer
{
    public static class Combat
    {
        public static List<Creature> AttackCreatures(Creature attacker, Creature[] creatures, float degrees, float arc, float range)
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
                            creatureArray[i].Health -= attacker.Stats.Str;
                            if (creatureArray[i].Health <= 0)
                            {
                                creatureArray.RemoveAt(i);
                                remain = false;
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

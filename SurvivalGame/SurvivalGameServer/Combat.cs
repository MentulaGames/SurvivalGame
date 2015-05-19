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
        public static List<Creature> AttackCreatures(Creature attacker, List<Creature> creatures, Vector2 dir, float range)
        {
            Vector2 posToAttack = attacker.GetTilePos() + attacker.ChunkPos.ToVector2() * cSize + dir;
            List<Creature> creatureList = creatures;
            for (int i = 0; i < creatureList.Count; i++)
            {
                if (creatureList[i] != attacker)
                {
                    if (MEx.GetMaxDiff(posToAttack, creatureList[i].GetTotalPos()) <= range & MEx.GetMaxDiff(attacker.GetTotalPos(), creatureList[i].GetTotalPos()) < range)
                    {
                        creatureList[i].Health -= attacker.Stats.Str;
                        if (creatureList[i].Health <= 0) creatureList.RemoveAt(i);
                    }
                }
            }
            return creatureList;
        }
    }
}

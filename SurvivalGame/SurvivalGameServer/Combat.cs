using Mentula.General.Res;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using MEx = Mentula.General.MathExtensions.Math;


namespace Mentula.SurvivalGameServer
{
    public static class Combat
    {
        private static int cSize = int.Parse(Resources.ChunkSize);
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
                        float bdeg = MEx.VectorToDegrees(bpos - apos);
                        if (MEx.DifferenceBetweenDegrees(degrees, bdeg) < arc / 2)
                        {
                            creatureArray[i].Health -= attacker.Stats.Str;
                            Lidgren.Network.NetIncomingMessageType.Data.WriteLine("{0} has taken {1} damage and now has {2} hp", creatureArray[i].Name, attacker.Stats.Str, creatureArray[i].Health);
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

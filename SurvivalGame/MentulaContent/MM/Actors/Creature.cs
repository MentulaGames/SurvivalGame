using Mentula.General;
using Mentula.Network.Xna;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mentula.Content
{
    [DebuggerDisplay("Name={Name}, State={GetState()}")]
    public class Creature : Actor
    {
        public readonly int Id;
        public readonly Color SkinColor;
        public readonly int Texture;
        public bool Alive;
        public string Name;
        public Stats Stats;
        public BodyParts[] Parts;

        internal Creature(int id, string name, Stats stats, BodyParts[] bodyParts, Color skinColor, int texture)
            : base()
        {
            Alive = true;
            Id = id;
            Name = name;
            Stats = stats;
            Parts = bodyParts;
            SkinColor = skinColor;
            Texture = texture;
        }

        public Creature(Creature c, IntVector2 chunkPos, Vector2 tilePos)
            : base(chunkPos, tilePos)
        {
            Alive = true;
            Name = c.Name;
            Stats = c.Stats;
            Parts = c.Parts;
            SkinColor = c.SkinColor;
            Texture = c.Texture;
        }

        public C_Player ToPlayer()
        {
            return new C_Player(Name, ChunkPos, tilePos, GetState());
        }

        public float GetTotalWeight()
        {
            float result = 0;

            for (int i = 0; i < Parts.Length; i++)
            {
                result += Parts[i].GetTotalWeight();
            }

            return result;
        }

        public PlayerState GetState()
        {
            KeyValuePair<string, PlayerState.UInt3>[] states = new KeyValuePair<string,PlayerState.UInt3>[Parts.Length];

            for (int i = 0; i < Parts.Length; i++)
            {
                states[i] = Parts[i].GetState();
            }

            return new PlayerState(states);
        }

        public override string ToString()
        {
            return "Name=" + Name + " Actor=" + base.ToString();
        }
    }
}
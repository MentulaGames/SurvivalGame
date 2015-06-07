﻿using Mentula.General;
using Microsoft.Xna.Framework;

namespace Mentula.Content
{
    public class Creature : Actor
    {
        public readonly int Id;
        public readonly Color SkinColor;
        public readonly int Texture;
        public string Name;
        public Stats Stats;
        public BodyParts[] Parts;

        internal Creature(int id, string name, Stats stats, BodyParts[] bodyParts, Color skinColor, int texture)
            : base()
        {
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
            Name = c.Name;
            Stats = c.Stats;
            Parts = c.Parts;
            SkinColor = c.SkinColor;
            Texture = c.Texture;
        }

        public C_Player ToPlayer()
        {
            return new C_Player(Name, ChunkPos, tilePos);
        }

        public override string ToString()
        {
            return "Name=" + Name + " Actor=" + base.ToString();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.General;
using Microsoft.Xna.Framework;

namespace Mentula.SurvivalGameServer
{
    public class Creature : Actor
    {
        public Stats Stats;
        public float Health;
        public Color SkinColor;
        public float Height;
        public float Weight;
        public int Texture;

        public Creature()
            :base()
        {
            Stats = new Stats();
            Health = 0;
            SkinColor = Color.White;
            Height = 0;
            Weight = 0;
            Texture = 0;
        }

        public Creature(Stats stats,float health,Color skinColor,float height, float weight, int texture)
            : base()
        {
            Stats = stats;
            Health = health;
            SkinColor = skinColor;
            Height = height;
            Weight = weight;
            Texture = texture;
        }

        public Creature(Creature c, IntVector2 chunkPos, Vector2 tilePos)
            :base(chunkPos,tilePos)
        {
            Stats = c.Stats;
            Health = c.Health;
            SkinColor = c.SkinColor;
            Height = c.Height;
            Weight = c.Weight;
            Texture = c.Texture;
        }

    }
}

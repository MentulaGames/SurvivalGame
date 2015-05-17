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
        public string Name;
        public Stats Stats;
        public float Health;
        public Color SkinColor;
        public int Texture;

        public Creature()
            :base()
        {
            Name = "The nameless";
            Stats = new Stats();
            Health = 0;
            SkinColor = Color.White;
            Texture = 0;
        }

        public Creature(string name,Stats stats,float health,Color skinColor, int texture)
            : base()
        {
            Name = name;
            Stats = stats;
            Health = health;
            SkinColor = skinColor;

            Texture = texture;
        }

        public Creature(Creature c, IntVector2 chunkPos, Vector2 tilePos)
            :base(chunkPos,tilePos)
        {
            Name = c.Name;
            Stats = c.Stats;
            Health = c.Health;
            SkinColor = c.SkinColor;
            Texture = c.Texture;
        }

    }
}

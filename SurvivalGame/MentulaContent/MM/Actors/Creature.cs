using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.General;
using Microsoft.Xna.Framework;

namespace Mentula.Content
{
    public class Creature : Actor
    {
        public int Id;
        public string Name;
        public Stats Stats;
        public BodyParts[] Parts;
        public Color SkinColor;
        public int Texture;

        public Creature(int id, string name, Stats stats, BodyParts[] bodyParts, Color skinColor, int texture)
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

        public static BodyParts[] GenerateHumanoid(float weight,TissueLayer[] m, TissueLayer guts)
        {
            BodyParts[] result = new BodyParts[6];
            BodyParts head = new BodyParts("Head",guts , m);
            BodyParts torso = new BodyParts("Torso", guts, m);
            BodyParts legs = new BodyParts("Legs", m);
            BodyParts arms = new BodyParts("Arms", m);

            head.Setweight(weight / 10);
            torso.Setweight(weight / 10 * 5);
            legs.Setweight(weight / 10 * 3);
            arms.Setweight(weight / 10);

            result[0] = head;
            result[1] = torso;
            result[2] = legs;
            result[3] = legs;
            result[4] = arms;
            result[5] = arms;

            return result;
        }

        public static BodyParts[] GenerateBeast(float weight, TissueLayer[] m, TissueLayer guts)
        {
            BodyParts[] result = new BodyParts[6];
            BodyParts head = new BodyParts("Head", guts, m);
            BodyParts torso = new BodyParts("Torso", guts, m);
            BodyParts legs = new BodyParts("Legs", m);

            head.Setweight(weight / 10);
            torso.Setweight(weight / 10 * 7);
            legs.Setweight(weight / 10);

            result[0] = head;
            result[1] = torso;
            result[2] = legs;
            result[3] = legs;
            result[4] = legs;
            result[5] = legs;

            return result;
        }

        public C_Player ToPlayer()
        {
            return new C_Player(Name, ChunkPos, tilePos);
        }
    }
}

using Mentula.General;
using Microsoft.Xna.Framework;

namespace Mentula.General
{
    public class Player : Actor
    {
        public string Name;

        public Player()
            : base()
        {
            Name = "NameLess";
        }

        public Player(string name, IntVector2 chunkPos, Vector2 tilePos)
            : base(chunkPos, tilePos)
        {
            Name = name;
        }
    }
}
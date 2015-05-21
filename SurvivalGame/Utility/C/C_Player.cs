using Microsoft.Xna.Framework;

namespace Mentula.General
{
    public class C_Player : Actor
    {
        public string Name;

        public C_Player()
            : base()
        {
            Name = "NameLess";
        }

        public C_Player(string name, IntVector2 chunkPos, Vector2 tilePos)
            : base(chunkPos, tilePos)
        {
            Name = name;
        }
    }
}
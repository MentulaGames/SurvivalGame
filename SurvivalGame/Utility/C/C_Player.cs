using Mentula.Network.Xna;
using Microsoft.Xna.Framework;

namespace Mentula.General
{
    public class C_Player : Actor
    {
        public const float DIFF = .8f;
        public const float MOVEMENT = 10f;

        public string Name;
        public CreatureState State;

        public C_Player()
            : base()
        {
            Name = "NameLess";
        }

        public C_Player(string name, IntVector2 chunkPos, Vector2 tilePos, CreatureState state)
            : base(chunkPos, tilePos)
        {
            Name = name;
            State = state;
        }
    }
}
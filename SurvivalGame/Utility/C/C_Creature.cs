using Mentula.General;
using Mentula.Network.Xna;
using Microsoft.Xna.Framework;

namespace Mentula.General
{
    public class C_Creature
    {
        public IntVector2 ChunkPos;
        public Vector2 Pos;
        public CreatureState State;
        public Color Color;
        public int TextureId;

        public C_Creature(IntVector2 chunkPos, Vector2 pos, Color color, int id, CreatureState state)
        {
            ChunkPos = chunkPos;
            Pos = pos;
            Color = color;
            TextureId = id;
            State = state;
        }
    }
}
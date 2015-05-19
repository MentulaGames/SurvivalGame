using Mentula.General;
using Microsoft.Xna.Framework;

namespace Mentula.General
{
    public class C_Creature
    {
        public IntVector2 ChunkPos;
        public Vector2 Pos;
        public Color Color;
        public float Health;
        public int TextureId;

        public C_Creature(IntVector2 chunkPos, Vector2 pos, Color color, float health, int id)
        {
            ChunkPos = chunkPos;
            Pos = pos;
            Color = color;
            Health = health;
            TextureId = id;
        }
    }
}
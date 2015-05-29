using Mentula.Network.Xna;
using Microsoft.Xna.Framework;

namespace Mentula.General
{
    public class C_Destrucible : Destructible
    {
        public IntVector2 ChunkPos;

        public C_Destrucible(IntVector2 chunkPos, Destructible d)
            : base(d.Health, new Tile(d.Pos, d.TextureId, d.Layer, d.Walkable))
        {
            ChunkPos = chunkPos;
        }

        public C_Destrucible(IntVector2 chunkPos, IntVector2 pos, byte texture, byte layer, bool walkable, float health)
            : base(health, pos, texture, layer, walkable)
        {
            ChunkPos = chunkPos;
        }

        public static implicit operator bool(C_Destrucible d)
        {
            return d != null && !d.Walkable;
        }
    }
}
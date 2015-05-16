using Mentula.General.Res;

namespace Mentula.General
{
    public class C_Tile : Tile
    {
        public IntVector2 ChunkPos;

        public C_Tile(IntVector2 chunkPos, Tile tile)
            : base(tile.Pos, tile.TextureId, tile.Layer, tile.Walkable)
        {
            ChunkPos = chunkPos;
        }

        public C_Tile(IntVector2 chunkPos, IntVector2 pos, byte texture, byte layer, bool walkable)
            : base(pos, texture, layer, walkable)
        {
            ChunkPos = chunkPos;
        }
    }
}
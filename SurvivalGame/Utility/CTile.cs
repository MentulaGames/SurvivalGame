namespace Mentula.General
{
    public class CTile : Tile
    {
        public IntVector2 ChunkPos;

        public CTile(IntVector2 chunkPos, IntVector2 pos, byte texture)
            : base(pos, texture)
        {
            ChunkPos = chunkPos;
        }

        public CTile(IntVector2 chunkPos, Tile tile)
            : base(tile.Pos, tile.TextureId, tile.Layer, tile.Walkable)
        {
            ChunkPos = chunkPos;
        }

        public CTile(IntVector2 chunkPos, IntVector2 pos, byte texture, byte layer, bool walkable)
            : base(pos, texture, layer, walkable)
        {
            ChunkPos = chunkPos;
        }
    }
}
namespace Mentula.General
{
    public class Tile
    {
        public IntVector2 Pos;
        public byte TextureId;
        public byte Layer;
        public bool Walkable;

        public Tile()
        {
            Pos = new IntVector2();
            TextureId = 0;
            Layer = 0;
            Walkable = false;
        }

        public Tile(IntVector2 pos, byte texture)
        {
            Pos = pos;
            TextureId = texture;
            Layer = 0;
            Walkable = true;
        }

        public Tile(IntVector2 pos, byte texture, byte layer, bool walkable)
        {
            Pos = pos;
            TextureId = texture;
            Layer = layer;
            Walkable = walkable;
        }

    }
}
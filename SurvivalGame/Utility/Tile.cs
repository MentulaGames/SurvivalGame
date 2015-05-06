namespace Mentula.General
{
    public class Tile
    {
        public IntVector2 Pos;
        public int TextureId;
        public int Layer;
        public bool Walkable;

        public Tile()
        {
            Pos = new IntVector2();
            TextureId = -1;
            Layer = 0;
            Walkable = false;
        }

        public Tile(IntVector2 pos, int texture)
        {
            Pos = pos;
            TextureId = texture;
            Layer = 0;
            Walkable = true;
        }

        public Tile(IntVector2 pos, int texture, int layer, bool walkable)
        {
            Pos = pos;
            TextureId = texture;
            Layer = layer;
            Walkable = walkable;
        }
    }
}
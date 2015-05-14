using Mentula.General;

namespace Mentula.General
{
    public class Destructible : Tile
    {
        public float Health;
        public Destructible()
            : base()
        {
            this.Health = 0;
        }

        public Destructible(float health, IntVector2 pos, byte texture, byte layer, bool walkable)
            :base(pos, texture, layer, walkable)
        {
            Health = health;
        }

        public Destructible(float Health, Tile t)
            : base(t.Pos, t.TextureId, t.Layer, t.Walkable)
        {
            this.Health = Health;
        }
    }
}
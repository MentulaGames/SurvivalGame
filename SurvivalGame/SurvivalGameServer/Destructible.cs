using Mentula.General;

namespace Mentula.SurvivalGameServer
{
    class Destructible:Tile
    {
        public float Health;
        public Destructible()
            :base()
        {
            this.Health = 0;
        }
        public Destructible(float Health, Tile t)
            : base(t.Pos,t.TextureId,t.Layer,t.Walkable)
        {
            this.Health = Health;
        }
    }
}

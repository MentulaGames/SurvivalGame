using Mentula.General;

namespace Mentula.SurvivalGameServer
{
<<<<<<< HEAD
    public class Destructible : Tile
=======
    public class Destructible:Tile
>>>>>>> origin/master
    {
        public float Health;
        public Destructible()
            : base()
        {
            this.Health = 0;
        }
        public Destructible(float Health, Tile t)
            : base(t.Pos, t.TextureId, t.Layer, t.Walkable)
        {
            this.Health = Health;
        }
    }
}

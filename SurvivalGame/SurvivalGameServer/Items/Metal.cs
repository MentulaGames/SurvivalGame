using Microsoft.Xna.Framework;

namespace Mentula.SurvivalGameServer.Items
{
    public class Metal : Material
    {
        public readonly StateOfMatter States;

        public Metal()
        {
            States = new StateOfMatter(new Vector4(float.PositiveInfinity));
        }

        public Metal(StateOfMatter states, int id, string name, float UTS, float TSAY, float density)
            : base(id, name, UTS, TSAY, density)
        {
            States = states;
        }
    }
}

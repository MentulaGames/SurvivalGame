using Microsoft.Xna.Framework;

namespace Mentula.Content
{
    public class Metal : Material
    {
        public readonly StateOfMatter States;

        internal Metal()
        {
            States = new StateOfMatter(new Vector3(float.PositiveInfinity));
        }

        internal Metal(StateOfMatter states, int id, string name, Vector3 values)
            : base(id, name, values.X, values.Y, values.Z)
        {
            States = states;
        }

        public override string ToString()
        {
            return "States=" + States.ToString() + " Material=" + base.ToString();
        }
    }
}

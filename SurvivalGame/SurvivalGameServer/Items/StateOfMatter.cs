using Microsoft.Xna.Framework;

namespace Mentula.SurvivalGameServer.Items
{
    public struct StateOfMatter
    {
        public readonly float MeltingPoint;
        public readonly float VaporizationPoint;
        public readonly float SublimationPoint;
        public readonly float IonizationPoint;

        public StateOfMatter(float m, float v, float s, float i)
        {
            MeltingPoint = m;
            VaporizationPoint = v;
            SublimationPoint = s;
            IonizationPoint = i;
        }

        public StateOfMatter(Vector4 vec)
        {
            MeltingPoint = vec.X;
            VaporizationPoint = vec.Y;
            SublimationPoint = vec.Z;
            IonizationPoint = vec.W;
        }

        public Vector4 ToVector4()
        {
            return new Vector4(MeltingPoint, VaporizationPoint, SublimationPoint, IonizationPoint);
        }
    }
}
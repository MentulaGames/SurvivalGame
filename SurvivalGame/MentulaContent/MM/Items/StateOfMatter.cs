using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Mentula.Content
{
    [DebuggerDisplay("{ToString()}")]
    public struct StateOfMatter
    {
        public readonly float MeltingPoint;
        public readonly float VaporizationPoint;
        public readonly float IonizationPoint;

        public StateOfMatter(float m, float v, float i)
        {
            MeltingPoint = m;
            VaporizationPoint = v;
            IonizationPoint = i;
        }

        public StateOfMatter(Vector3 vec)
        {
            MeltingPoint = vec.X;
            VaporizationPoint = vec.Y;
            IonizationPoint = vec.Z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(MeltingPoint, VaporizationPoint, IonizationPoint);
        }

        public override string ToString()
        {
            return "Melt=" + MeltingPoint.ToString() + " Vap=" + VaporizationPoint.ToString() + " Ion=" + IonizationPoint.ToString();
        }
    }
}
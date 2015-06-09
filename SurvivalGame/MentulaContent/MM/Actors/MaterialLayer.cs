using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Mentula.Content
{
    [DebuggerDisplay("Id={Id}, Name={Name}, Thickness={Thickness}, Weight={GetWeight()}")]
    public class MaterialLayer : Material
    {
        public readonly float MaxArea;
        public float Thickness;
        public float CurrArea;
        public float BiggestHoleSize;
        private float Weight;

        internal MaterialLayer()
        {
            Thickness = 0;
            MaxArea = 0;
            CurrArea = 0;
            BiggestHoleSize = 0;
            Weight = 0;
        }

        internal MaterialLayer(int id, string name, Vector3 stats, float thickness, float area, bool client = false)
            : base(id, name, stats.X, stats.Y, stats.Z, client)
        {
            Thickness = client ? thickness : thickness / 10;
            MaxArea = area;
            CurrArea = area;
            BiggestHoleSize = 0;
            Weight = CurrArea * Thickness;
        }

        internal MaterialLayer(MaterialLayer m)
            : base(m)
        {
            Thickness = m.Thickness;
            MaxArea = m.MaxArea;
            CurrArea = MaxArea;
            BiggestHoleSize = m.BiggestHoleSize;
            Weight = m.Weight;
        }

        public float GetWeight()
        {
            Weight = Thickness * CurrArea * Density;
            return Weight;
        }
    }
}
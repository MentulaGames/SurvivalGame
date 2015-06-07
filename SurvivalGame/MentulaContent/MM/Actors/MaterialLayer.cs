using Microsoft.Xna.Framework;

namespace Mentula.Content
{
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

        internal MaterialLayer(int id, string name, Vector3 stats, float thickness, float area)
            : base(id, name, stats.X, stats.Y, stats.Z)
        {
            Thickness = thickness;
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
using Mentula.Content;

namespace Mentula.SurvivalGameServer
{
    public class MaterialLayer
    {
        public Material Matter;
        public float Thickness;
        public float MaxArea;
        public float CurrArea;
        public float BiggestHoleSize;
        private float Weight;

        /// <param name="m">Matter of which the object is made</param>
        /// <param name="thickness">Thickness is in cm</param>
        /// <param name="area">area is in cm^2</param>
        public MaterialLayer(Material m, float thickness, float area)
        {
            Matter = m;
            Thickness = thickness;
            MaxArea = area;
            CurrArea = area;
            BiggestHoleSize = 0;
            Weight = CurrArea * Thickness;
        }
        
        public MaterialLayer(MaterialLayer m)
        {
            Matter = m.Matter;
            Thickness = m.Thickness;
            MaxArea = m.MaxArea;
            CurrArea = MaxArea;
            BiggestHoleSize = m.BiggestHoleSize;
            Weight = m.Weight;
        }
        public float GetWeight()
        {
            Weight = Thickness * CurrArea * Matter.Density;
            return Weight;
        }
    }
}
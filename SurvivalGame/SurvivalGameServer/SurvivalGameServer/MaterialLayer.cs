using Mentula.Content;

namespace Mentula.SurvivalGameServer
{
    public class MaterialLayer
    {
        public Material Matter;
        public float Thickness;
        public readonly float Area;
        public float CurrArea;

        /// <param name="m">Matter of which the object is made</param>
        /// <param name="thickness">Thickness is in cm</param>
        /// <param name="area">area is in cm^2</param>
        public MaterialLayer(Material m, float thickness, float area)
        {
            Matter = m;
            Thickness = thickness;
            Area = area;
            CurrArea = area;
        }
    }
}
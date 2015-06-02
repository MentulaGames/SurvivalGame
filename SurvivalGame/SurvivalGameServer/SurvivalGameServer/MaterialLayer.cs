using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.Content;

namespace Mentula.SurvivalGameServer
{
    public class MaterialLayer
    {
        public Material Matter;
        public float Thickness;
        public readonly float Area;
        public float CurrArea;
        public MaterialLayer(Material m, float thickness, float area)
        {
            Matter = m;
            Thickness = thickness;
            Area = area;
            CurrArea = area;
        }
    }
}

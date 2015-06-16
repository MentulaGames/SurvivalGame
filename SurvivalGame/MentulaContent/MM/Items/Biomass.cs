using Microsoft.Xna.Framework;

namespace Mentula.Content
{
    public class Biomass : Material
    {
        public readonly float BurnTemperature;
        public readonly float NutritiousValue;

        internal Biomass()
        {
            BurnTemperature = float.PositiveInfinity;
            NutritiousValue = float.PositiveInfinity;
        }

        internal Biomass(float burn, float nutr, int id, string name, Vector3 values)
            : base(id, name, values.X, values.Y, values.Z)
        {
            BurnTemperature = burn;
            NutritiousValue = nutr;
        }

        public override string ToString()
        {
            return "BurnT=" + BurnTemperature.ToString() + " NutrV=" + NutritiousValue.ToString() + " Material=" + base.ToString();
        }
    }
}

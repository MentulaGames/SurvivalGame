using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Mentula.Content.MM
{
    [ContentProcessor(DisplayName = "Mentula Biomass Processor")]
    internal class MBioProcessor : ContentProcessor<MMSource, Biomass[]>
    {
        public override Biomass[] Process(MMSource input, ContentProcessorContext context)
        {
            Utils.CheckProcessorType("Biomass", input.Container.Values["DEFAULT"]);

            Biomass[] result = new Biomass[input.Container.Childs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                Container curr = input.Container.Childs[i];
                Manifest mani = new Manifest();
                string rawValue = "";

                if (curr.TryGetValue("Id", out rawValue)) mani.Id = int.Parse(rawValue);
                if (curr.TryGetValue("Name", out rawValue)) mani.Name = rawValue;

                if (curr.TryGetValue("UTS", out rawValue)) mani.Values.X = float.Parse(rawValue);
                if (curr.TryGetValue("TSAY", out rawValue)) mani.Values.Y = float.Parse(rawValue);
                if (curr.TryGetValue("Density", out rawValue)) mani.Values.Z = float.Parse(rawValue);

                if (curr.TryGetValue("BurnTemperature", out rawValue)) mani.Burn = float.Parse(rawValue);
                if (curr.TryGetValue("NutritiousValue", out rawValue)) mani.Nutr = float.Parse(rawValue);

                result[i] = new Biomass(mani.Burn, mani.Nutr, mani.Id, mani.Name, mani.Values);
            }

            return result;
        }

        internal struct Manifest
        {
            public int Id;
            public string Name;
            public Vector3 Values;
            public float Burn;
            public float Nutr;

            public Manifest(int id, string name)
            {
                Id = id;
                Name = name;
                Values = default(Vector3);
                Burn = 0;
                Nutr = 0;
            }
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Collections.Generic;

namespace Mentula.Content.MM
{
    [ContentProcessor(DisplayName = "Mentula Creatures Processor")]
    internal class MCreatureProcessor : ContentProcessor<MMSource, Creature[]>
    {
        public override Creature[] Process(MMSource input, ContentProcessorContext context)
        {
            Utils.CheckProcessorType("Creatures", input.Container.Values["DEFAULT"]);

            Creature[] result = new Creature[input.Container.Childs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                Container curr = input.Container.Childs[i];
                Manifest mani = new Manifest();
                mani.BodyParts = new List<BodyParts>();
                mani.Stats = new Stats();

                string rawValue = "";

                if (curr.TryGetValue("Id", out rawValue)) mani.Id = int.Parse(rawValue);
                if (curr.TryGetValue("Name", out rawValue)) mani.Name = rawValue;

                if (curr.TryGetValue("Texture", out rawValue)) mani.TextureId = int.Parse(rawValue);
                if (curr.TryGetValue("Color.R", out rawValue)) mani.Color.R = byte.Parse(rawValue);
                if (curr.TryGetValue("Color.G", out rawValue)) mani.Color.G = byte.Parse(rawValue);
                if (curr.TryGetValue("Color.B", out rawValue)) mani.Color.B = byte.Parse(rawValue);
                if (curr.TryGetValue("Color.A", out rawValue)) mani.Color.A = byte.Parse(rawValue);

                if (curr.TryGetValue("Strength", out rawValue)) mani.Stats.Str = int.Parse(rawValue);
                if (curr.TryGetValue("Dexterity", out rawValue)) mani.Stats.Dex = int.Parse(rawValue);
                if (curr.TryGetValue("Intelect", out rawValue)) mani.Stats.Int = int.Parse(rawValue);
                if (curr.TryGetValue("Perception", out rawValue)) mani.Stats.Per = int.Parse(rawValue);
                if (curr.TryGetValue("Endurence", out rawValue)) mani.Stats.End = int.Parse(rawValue);

                for (int j = 0; j < curr.Childs.Length; j++)
                {
                    Container child = curr.Childs[j];

                    string name = "";

                    if (child.TryGetValue("Default", out rawValue)) name = rawValue;

                    TissueLayer[] layers = new TissueLayer[child.Childs.Length];
                    for (int k = 0; k < child.Childs.Length; k++)
                    {
                        Container tissueContainer = child.Childs[k];
                        TissueManifest tMani = new TissueManifest();

                        if (tissueContainer.TryGetValue("Default", out rawValue)) tMani.Essential = bool.Parse(rawValue);
                        if (tissueContainer.TryGetValue("Type", out rawValue)) tMani.Name = rawValue;
                        if (tissueContainer.TryGetValue("Id", out rawValue)) tMani.Id = int.Parse(rawValue);
                        if (tissueContainer.TryGetValue("InfluencesEffectiveness", out rawValue)) tMani.InfluencesEffectiveness = bool.Parse(rawValue);
                        if (tissueContainer.TryGetValue("Thickness", out rawValue)) tMani.Thickness = float.Parse(rawValue);
                        if (tissueContainer.TryGetValue("Area", out rawValue)) tMani.MaxArea = float.Parse(rawValue);

                        layers[i] = new TissueLayer(new MaterialLayer(tMani.Id, tMani.Name, Vector3.Zero, tMani.Thickness, tMani.MaxArea), tMani.Essential, tMani.InfluencesEffectiveness);
                    }

                    mani.BodyParts.Add(new BodyParts(name, layers));
                }

                result[i] = new Creature(mani.Id, mani.Name, mani.Stats, mani.BodyParts.ToArray(), mani.Color, mani.TextureId);
            }

            return result;
        }

        internal struct Manifest
        {
            public int Id;
            public string Name;
            public int TextureId;
            public Color Color;
            public Stats Stats;
            public List<BodyParts> BodyParts;

            public Manifest(int id, string name)
            {
                Id = id;
                Name = name;
                TextureId = 0;
                Color = Color.Transparent;
                Stats = new Stats();
                BodyParts = new List<BodyParts>();
            }
        }

        private struct TissueManifest
        {
            public bool Essential;
            public string Name;
            public int Id;
            public bool InfluencesEffectiveness;
            public float Thickness;
            public float MaxArea;
        }
    }
}
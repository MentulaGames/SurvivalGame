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
                Container creatureContainer = input.Container.Childs[i];
                Manifest mani = new Manifest()
                    {
                        BodyParts = new List<BodyParts>(),
                        Stats = new Stats()
                    };

                string rawValue = "";

                const string ID = "Id";
                if (creatureContainer.TryGetValue(ID, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.Id = raw;
                    else throw new ParameterException(ID, rawValue, typeof(int));
                }
                else throw new ParameterNullException(ID);

                const string NAME = "Name";
                if (creatureContainer.TryGetValue(NAME, out rawValue)) mani.Name = rawValue;
                else throw new ParameterNullException(NAME);

                const string TEXTURE = "Texture";
                if (creatureContainer.TryGetValue(TEXTURE, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.TextureId = raw;
                    else throw new ParameterException(TEXTURE, rawValue, typeof(int));
                }
                else throw new ParameterNullException(TEXTURE);

                const string R = "Color.R";
                if (creatureContainer.TryGetValue(R, out rawValue))
                {
                    byte raw = 0;

                    if (byte.TryParse(rawValue, out raw)) mani.Color.R = raw;
                    else throw new ParameterException(R, rawValue, typeof(byte));
                }
                else throw new ParameterNullException(R);

                const string G = "Color.G";
                if (creatureContainer.TryGetValue(G, out rawValue))
                {
                    byte raw = 0;

                    if (byte.TryParse(rawValue, out raw)) mani.Color.G = raw;
                    else throw new ParameterException(G, rawValue, typeof(byte));
                }
                else throw new ParameterNullException(G);

                const string B = "Color.B";
                if (creatureContainer.TryGetValue(B, out rawValue))
                {
                    byte raw = 0;

                    if (byte.TryParse(rawValue, out raw)) mani.Color.B = raw;
                    else throw new ParameterException(B, rawValue, typeof(byte));
                }
                else throw new ParameterNullException(B);

                const string A = "Color.A";
                if (creatureContainer.TryGetValue(A, out rawValue))
                {
                    byte raw = 0;

                    if (byte.TryParse(rawValue, out raw)) mani.Color.A = raw;
                    else throw new ParameterException(A, rawValue, typeof(byte));
                }
                else throw new ParameterNullException(A);

                const string STR = "Strength";
                if (creatureContainer.TryGetValue(STR, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.Stats.Str = raw;
                    else throw new ParameterException(STR, rawValue, typeof(int));
                }
                else throw new ParameterNullException(STR);

                const string DEX = "Dexterity";
                if (creatureContainer.TryGetValue(DEX, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.Stats.Dex = raw;
                    else throw new ParameterException(DEX, rawValue, typeof(int));
                }
                else throw new ParameterNullException(DEX);

                const string INT = "Intelect";
                if (creatureContainer.TryGetValue(INT, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.Stats.Int = raw;
                    else throw new ParameterException(INT, rawValue, typeof(int));
                }
                else throw new ParameterNullException(INT);

                const string PERC = "Perception";
                if (creatureContainer.TryGetValue(PERC, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.Stats.Per = raw;
                    else throw new ParameterException(PERC, rawValue, typeof(int));
                }
                else throw new ParameterNullException(PERC);

                const string END = "Endurence";
                if (creatureContainer.TryGetValue(END, out rawValue))
                {
                    int raw = 0;

                    if (int.TryParse(rawValue, out raw)) mani.Stats.End = raw;
                    else throw new ParameterException(END, rawValue, typeof(int));
                }
                else throw new ParameterNullException(END);

                for (int j = 0; j < creatureContainer.Childs.Length; j++)
                {
                    Container bodyContainer = creatureContainer.Childs[j];

                    string name = "";

                    if (bodyContainer.TryGetValue("Default", out rawValue)) name = rawValue;
                    else throw new ParameterNullException("Name");

                    TissueLayer[] layers = new TissueLayer[bodyContainer.Childs.Length];
                    for (int k = 0; k < bodyContainer.Childs.Length; k++)
                    {
                        Container tissueContainer = bodyContainer.Childs[k];
                        TissueManifest tMani = new TissueManifest();

                        const string ESS = "Essential";
                        if (tissueContainer.TryGetValue("Default", out rawValue))
                        {
                            bool raw = false;

                            if (bool.TryParse(rawValue, out raw)) tMani.Essential = raw;
                            else throw new ParameterException(ESS, rawValue, typeof(bool));
                        }
                        else throw new ParameterNullException(ESS);

                        const string TYPE = "Type";
                        if (tissueContainer.TryGetValue("Type", out rawValue)) tMani.Name = rawValue;
                        else throw new ParameterNullException(TYPE);

                        if (tissueContainer.TryGetValue(ID, out rawValue))
                        {
                            int raw = 0;

                            if (int.TryParse(rawValue, out raw)) tMani.Id = raw;
                            else throw new ParameterException(ID, rawValue, typeof(int));
                        }
                        else throw new ParameterNullException(ID);

                        const string INF = "InfluencesEffectiveness";
                        if (tissueContainer.TryGetValue(INF, out rawValue))
                        {
                            bool raw = false;

                            if (bool.TryParse(rawValue, out raw)) tMani.InfluencesEffectiveness = raw;
                            else throw new ParameterException(INF, rawValue, typeof(bool));
                        }
                        else throw new ParameterNullException(INF);

                        const string THICK = "Thickness";
                        if (tissueContainer.TryGetValue(THICK, out rawValue))
                        {
                            float raw = 0;

                            if (Utils.TryParse(rawValue, out raw)) tMani.Thickness = raw;
                            else throw new ParameterException(THICK, rawValue, typeof(float));
                        }
                        else throw new ParameterNullException(THICK);

                        const string AREA = "Area";
                        if (tissueContainer.TryGetValue(AREA, out rawValue))
                        {
                            float raw = 0;

                            if (Utils.TryParse(rawValue, out raw)) tMani.MaxArea = raw;
                            else throw new ParameterException(AREA, rawValue, typeof(float));
                        }
                        else throw new ParameterNullException(AREA);

                        layers[k] = new TissueLayer(new MaterialLayer(tMani.Id, tMani.Name, Vector3.Zero, tMani.Thickness, tMani.MaxArea), tMani.Essential, tMani.InfluencesEffectiveness);
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
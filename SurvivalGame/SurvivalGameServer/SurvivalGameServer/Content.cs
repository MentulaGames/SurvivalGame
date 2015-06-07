using Mentula.Content;
using Microsoft.Xna.Framework.Content;

namespace Mentula.SurvivalGameServer
{
    public class Content
    {
        public readonly Metal[] Metals;
        public readonly Biomass[] BioMasses;

        public readonly Creature[] Creatures;

        public Content(ref ContentManager content)
        {
            const string METALS = "Metals";
            const string BIO = "Biomass";
            const string CREATURES = "Creatures";

            Metals = content.Load<Metal[]>(METALS);
            BioMasses = content.Load<Biomass[]>(BIO);
            Creatures = content.Load<Creature[]>(CREATURES);

            for (int i = 0; i < Creatures.Length; i++)
            {
                Creature cur = Creatures[i];
                for (int j = 0; j < cur.Parts.Length; j++)
                {
                    BodyParts part = cur.Parts[j];
                    for (int k = 0; k < part.Layers.Length; k++)
                    {
                        TissueLayer t = part.Layers[k];
                        t.InitRefrence(BioMasses);
                        t.Thickness /= 10;
                    }
                }
            }
        }
    }
}
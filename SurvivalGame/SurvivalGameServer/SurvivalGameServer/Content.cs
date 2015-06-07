using Mentula.Content;
using Microsoft.Xna.Framework.Content;

namespace Mentula.SurvivalGameServer
{
    public class Content
    {
        public readonly Metal[] Metals;
        public readonly Creature[] Creatures;

        public Content(ref ContentManager content, string metals, string creatures)
        {
            Metals = content.Load<Metal[]>(metals);
            Creatures = content.Load<Creature[]>(creatures);

            for (int i = 0; i < Creatures.Length; i++)
            {
                Creature cur = Creatures[i];
                for (int j = 0; j < cur.Parts.Length; j++)
                {
                    BodyParts part = cur.Parts[j];
                    for (int k = 0; k < part.Layers.Length; k++)
                    {
                        TissueLayer t = part.Layers[k];
                        t.InitRefrence(Metals);
                        t.Thickness /= 10;
                    }
                }
            }
        }
    }
}
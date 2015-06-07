using System;

namespace Mentula.Content
{
    public class BodyParts
    {
        public readonly string Name;
        public TissueLayer[] Layers;

        internal BodyParts()
        {
            Name = "";
            Layers = new TissueLayer[0];
        }

        internal BodyParts(string name)
        {
            Name = name;
            Layers = new TissueLayer[0];
        }

        internal BodyParts(string name, TissueLayer[] tissueLayers)
        {
            Name = name;
            Layers = tissueLayers;
        }

        public float GetTotalWeight()
        {
            float result = 0;

            for (int i = 0; i < Layers.Length; i++)
            {
                result += Layers[i].GetWeight();
            }

            return result;
        }
    }
}
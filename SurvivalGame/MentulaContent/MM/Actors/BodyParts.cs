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
    }
}
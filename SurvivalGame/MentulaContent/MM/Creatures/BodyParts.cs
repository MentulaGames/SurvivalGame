using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentula.Content
{
    public class BodyParts
    {
        public string Name;
        public TissueLayer[] Layers;

        public BodyParts()
        {
            Name = "";
            Layers = new TissueLayer[0];
        }

        public BodyParts(string name,TissueLayer[] tissueLayers)
        {
            Name = name;
            Layers = tissueLayers;
        }
        public BodyParts(string name,TissueLayer guts, TissueLayer[] tissueLayers)
        {
            Name = name;
            TissueLayer[] a = new TissueLayer[1+tissueLayers.Length];
            a[0]=guts;
            Array.Copy(tissueLayers, a, tissueLayers.Length);
            Layers = a;
        }

        public void Setweight(float weight)
        {
            float cweight = 0;
            for (int i = 0; i < Layers.Length; i++)
            {
                cweight += Layers[i].GetWeight();
            }
            float scale3 = (float)Math.Pow(weight / cweight, 1 / 3);
            float scale2 = (float)Math.Pow(weight / cweight, 2 / 3);
            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i].MaxArea = Layers[i].MaxArea * scale2;
                Layers[i].CurrArea = Layers[i].CurrArea * scale2;
                Layers[i].Thickness = Layers[i].Thickness * scale3;
            }
        }
    }
}
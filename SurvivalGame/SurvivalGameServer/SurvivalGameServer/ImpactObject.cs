using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mentula.Content;


namespace Mentula.SurvivalGameServer
{
    public class ImpactObject
    {
        public Material Matter;
        public float Velocity { get; private set; }
        public float Weight { get; private set; }
        public float Volume { get; private set; }
        public float ContactArea { get; private set; }
        public float E_k { get; private set; }
        public float MPa { get; private set; }

        public ImpactObject(Material m, float velocity, float volume,float contactArea)
        {
            Matter = m;
            Velocity = velocity;
            Weight = m.Density * volume;
            Volume = volume;
            ContactArea = contactArea;
            E_k = Weight * velocity * velocity / 2;
            MPa = E_k / contactArea /100;
        }
        public void SetMPa(float MPa)
        {
            this.MPa = MPa;
            E_k = MPa * ContactArea * 100;
        }
    }
}

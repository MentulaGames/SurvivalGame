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
        public float Velocity;
        public float Weight;
        public float Volume;
        public float ContactArea;

        public ImpactObject(Material m, float velocity, float volume,float contactArea)
        {
            Velocity = velocity;
            Weight = m.Density * volume;
            Volume = volume;
            ContactArea = contactArea;
        }
    }
}

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

        /// <param name="m">Matter of which the object is made</param>
        /// <param name="velocity">Velocity is in m/s</param>
        /// <param name="volume">Volume is in cm^3</param>
        /// <param name="contactArea">Contact Area is in cm^2</param>
        public ImpactObject(Material m, float velocity, float volume, float contactArea)
        {
            Matter = m;
            Velocity = velocity;
            Weight = m.Density * volume;
            Volume = volume;
            ContactArea = contactArea;
            E_k = Weight * velocity * velocity / 2;
            MPa = E_k / contactArea / 100;
        }

        public void SetMPa(float MPa)
        {
            this.MPa = MPa;
            E_k = MPa * ContactArea * 100;
        }
    }
}
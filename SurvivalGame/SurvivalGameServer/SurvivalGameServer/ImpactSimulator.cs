using System;
using MEx = Mentula.General.MathExtensions.Math;

namespace Mentula.SurvivalGameServer
{
    public static class ImpactSimulator
    {
        private static Random r;

        static ImpactSimulator()
        {
            r = new Random();
        }

        public static void OnHit(ref MaterialLayer[] layers,ref  ImpactObject impacter)
        {
            for (int i = layers.Length-1; i >=0 ; i--)
            {
                float lintr = layers[i].CurrArea / layers[i].MaxArea;
                float dNeeded = layers[i].Thickness * MEx.Lerp(lintr * lintr, (float)Math.Sqrt(lintr), (float)r.NextDouble());
                float d = impacter.MPa / layers[i].Matter.Tensile_Strain_At_Yield;
                if (d > dNeeded)
                {
                    layers[i].BiggestHoleSize = impacter.ContactArea;
                    layers[i].CurrArea -= impacter.ContactArea;
                    impacter.SetMPa(impacter.MPa - dNeeded * layers[i].Matter.Tensile_Strain_At_Yield);
                }
                else
                {
                    layers[i].CurrArea -= impacter.ContactArea * d / dNeeded;
                    impacter.SetMPa(0);
                    break;
                }
            }
        }
    }
}
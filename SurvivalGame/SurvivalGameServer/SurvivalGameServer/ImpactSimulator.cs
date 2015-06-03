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

        public static MaterialLayer[] OnHit(MaterialLayer[] layers, ImpactObject impacter)
        {
            MaterialLayer[] result = layers;
            ImpactObject im = impacter;

            for (int i = 0; i < result.Length; i++)
            {
                float lintr = result[i].CurrArea / result[i].Area;
                float dNeeded = result[i].Thickness * MEx.Lerp(lintr * lintr, (float)Math.Sqrt(lintr), (float)r.NextDouble());
                float d = im.MPa / result[i].Matter.Tensile_Strain_At_Yield;

                if (d > dNeeded)
                {
                    result[i].CurrArea -= im.ContactArea;
                    im.SetMPa(im.MPa - dNeeded * result[i].Matter.Tensile_Strain_At_Yield);
                }
                else
                {
                    result[i].CurrArea -= im.ContactArea * d / dNeeded;
                    im.SetMPa(0);
                    break;
                }
            }

            return result;
        }
    }
}
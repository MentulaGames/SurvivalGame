using Mentula.Network.Xna;
using System;
using System.Diagnostics;

namespace Mentula.Content
{
    [DebuggerDisplay("Name={Name}, State={GetState()}")]
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

        public PlayerState.UInt3 GetState()
        {
            const int NUM_OF_STATES = 6;
            float maxarea = 0;
            float currarea = 0;

            for (int i = 0; i < Layers.Length; i++)
            {
                TissueLayer t = Layers[i];

                maxarea += t.MaxArea;
                currarea += t.CurrArea;
            }
            uint u = (uint)(NUM_OF_STATES - (currarea / maxarea * NUM_OF_STATES));
            return u > PlayerState.UInt3.MaxValue ? PlayerState.UInt3.MaxValue : u;
        }
    }
}
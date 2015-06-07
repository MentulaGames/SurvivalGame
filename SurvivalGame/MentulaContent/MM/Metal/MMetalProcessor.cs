using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Linq;
using System.Collections.Generic;
using Mentula.Content.MM;

namespace Mentula.Content
{
    [ContentProcessor(DisplayName = "Mentula Metals Processor")]
    internal class MMetalProcessor : ContentProcessor<MMSource, Metal[]>
    {
        public override Metal[] Process(MMSource input, ContentProcessorContext context)
        {
            Utils.CheckProcessorType("Metals", input.Container.Values["DEFAULT"]);

            Metal[] result = new Metal[input.Container.Childs.Length];

            for (int i = 0; i < result.Length; i++)
            {
                Container curr = input.Container.Childs[i];
                Manifest mani = new Manifest();
                string rawValue = "";

                if (curr.TryGetValue("Id", out rawValue)) mani.Id = int.Parse(rawValue);
                if (curr.TryGetValue("Name", out rawValue)) mani.Name = rawValue;

                if (curr.TryGetValue("UTS", out rawValue)) mani.Values.X = float.Parse(rawValue);
                if (curr.TryGetValue("TSAY", out rawValue)) mani.Values.Y = float.Parse(rawValue);
                if (curr.TryGetValue("Density", out rawValue)) mani.Values.Z = float.Parse(rawValue);

                if (curr.TryGetValue("MeltingPoint", out rawValue)) mani.States.X = float.Parse(rawValue);
                if (curr.TryGetValue("VaporizationPoint", out rawValue)) mani.States.Y = float.Parse(rawValue);
                if (curr.TryGetValue("IonizationPoint", out rawValue)) mani.States.Z = float.Parse(rawValue);

                result[i] = new Metal(new StateOfMatter(mani.States), mani.Id, mani.Name, mani.Values);
            }

            return result;
        }

        internal struct Manifest
        {
            public int Id;
            public string Name;
            public Vector3 Values;
            public Vector3 States;

            public Manifest(int id, string name)
            {
                Id = id;
                Name = name;
                Values = default(Vector3);
                States = default(Vector3);
            }
        }
    }
}
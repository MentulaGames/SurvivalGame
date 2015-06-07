using Mentula.Content.MM;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace Mentula.Content.MM
{
    internal class MCreatureReader : ContentTypeReader<Creature[]>
    {
        protected override Creature[] Read(ContentReader input, Creature[] existingInstance)
        {
            int length = input.ReadInt32();
            Creature[] result = new Creature[length];

            for (int i = 0; i < length; i++)
            {
                MCreatureProcessor.Manifest mani = new MCreatureProcessor.Manifest();
                mani.BodyParts = new List<BodyParts>();

                mani.Id = input.ReadInt32();
                mani.Name = input.ReadCString();
                mani.TextureId = input.ReadInt32();
                mani.Color = input.ReadColor();
                mani.Stats = input.ReadStats();

                int partsLength = input.ReadInt32();

                for (int j = 0; j < partsLength; j++)
                {
                    BodyParts cur = new BodyParts(input.ReadCString());

                    int tissueLength = input.ReadInt32();
                    cur.Layers = new TissueLayer[tissueLength];

                    for (int k = 0; k < tissueLength; k++)
                    {
                        TissueManifest matMani = new TissueManifest();

                        matMani.Ess = input.ReadBoolean();
                        matMani.Inf = input.ReadBoolean();
                        matMani.Thick = input.ReadSingle();
                        matMani.Max = input.ReadSingle();

                        matMani.Id = input.ReadInt32();
                        matMani.Name = input.ReadCString();
                        matMani.stats.X = input.ReadSingle();
                        matMani.stats.Y = input.ReadSingle();
                        matMani.stats.Z = input.ReadSingle();

                        cur.Layers[k] = new TissueLayer(new MaterialLayer(matMani.Id, matMani.Name, matMani.stats, matMani.Thick, matMani.Max, true), matMani.Ess, matMani.Inf);
                    }

                    mani.BodyParts.Add(cur);
                }

                result[i] = new Creature(mani.Id, mani.Name, mani.Stats, mani.BodyParts.ToArray(), mani.Color, mani.TextureId);
            }

            return result;
        }

        private struct TissueManifest
        {
            public int Id;
            public string Name;
            public Vector3 stats;

            public bool Ess;
            public bool Inf;
            public float Thick;
            public float Max;
        }
    }
}
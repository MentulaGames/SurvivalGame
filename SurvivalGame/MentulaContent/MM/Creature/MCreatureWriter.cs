using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Mentula.Content.MM
{
    [ContentTypeWriter]
    internal class MCreatureWriter : ContentTypeWriter<Creature[]>
    {
        protected override void Write(ContentWriter output, Creature[] value)
        {
            output.Write(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                Creature cur = value[i];

                output.Write(cur.Id);
                output.WriteString(cur.Name);
                output.Write(cur.Texture);
                output.Write(cur.SkinColor);
                output.Write(cur.Stats);

                output.Write(cur.Parts.Length);

                for (int j = 0; j < cur.Parts.Length; j++)
                {
                    BodyParts curr = cur.Parts[j];

                    output.WriteString(curr.Name);

                    output.Write(curr.Layers.Length);

                    for (int k = 0; k < curr.Layers.Length; k++)
                    {
                        TissueLayer current = curr.Layers[k];

                        output.Write(current.essential);
                        output.Write(current.influencesEffectiveness);
                        output.Write(current.Thickness);
                        output.Write(current.MaxArea);

                        output.Write(current.Id);
                        output.WriteString(current.Name);
                        output.Write(current.Ultimate_Tensile_Strength);
                        output.Write(current.Tensile_Strain_At_Yield);
                        output.Write(current.Density);
                    }
                }
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Creature[]).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Mentula.Content.MM.MCreatureReader, MentulaContent, Version=1.0.0.0, Culture=neutral";
        }
    }
}

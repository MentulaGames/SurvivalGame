using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Mentula.Content.MM
{
    [ContentTypeWriter]
    internal class MBioWriter : ContentTypeWriter<Biomass[]>
    {
        protected override void Write(ContentWriter output, Biomass[] value)
        {
            output.Write(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                Biomass cur = value[i];
                output.Write(cur.Id);
                output.WriteString(cur.Name);
                output.Write(cur.Ultimate_Tensile_Strength);
                output.Write(cur.Tensile_Strain_At_Yield);
                output.Write(cur.Density);
                output.Write(cur.BurnTemperature);
                output.Write(cur.NutritiousValue);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Biomass[]).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Mentula.Content.MM.MBioReader, MentulaContent, Version=1.0.0.0, Culture=neutral";
        }
    }
}
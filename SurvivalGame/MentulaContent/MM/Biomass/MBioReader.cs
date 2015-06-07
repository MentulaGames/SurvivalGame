using Microsoft.Xna.Framework.Content;

namespace Mentula.Content.MM
{
    internal class MBioReader : ContentTypeReader<Biomass[]>
    {
        protected override Biomass[] Read(ContentReader input, Biomass[] existingInstance)
        {
            int length = input.ReadInt32();
            Biomass[] result = new Biomass[length];

            for (int i = 0; i < length; i++)
            {
                MBioProcessor.Manifest mani = new MBioProcessor.Manifest();

                mani.Id = input.ReadInt32();
                mani.Name = input.ReadCString();
                mani.Values.X = input.ReadSingle();
                mani.Values.Y = input.ReadSingle();
                mani.Values.Z = input.ReadSingle();
                mani.Burn = input.ReadSingle();
                mani.Nutr = input.ReadSingle();

                result[i] = new Biomass(mani.Burn, mani.Nutr, mani.Id, mani.Name, mani.Values);
            }

            return result;
        }
    }
}
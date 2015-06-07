using Microsoft.Xna.Framework.Content;

namespace Mentula.Content.MM
{
    internal class MMetalReader : ContentTypeReader<Metal[]>
    {
        protected override Metal[] Read(ContentReader input, Metal[] existingInstance)
        {
            int length = input.ReadInt32();
            Metal[] result = new Metal[length];

            for (int i = 0; i < length; i++)
            {
                MMetalProcessor.Manifest mani = new MMetalProcessor.Manifest();

                mani.Id = input.ReadInt32();
                mani.Name = input.ReadCString();
                mani.Values.X = input.ReadSingle();
                mani.Values.Y = input.ReadSingle();
                mani.Values.Z = input.ReadSingle();
                mani.States = input.ReadVector3();

                result[i] = new Metal(new StateOfMatter(mani.States), mani.Id, mani.Name, mani.Values);
            }

            return result;
        }
    }
}
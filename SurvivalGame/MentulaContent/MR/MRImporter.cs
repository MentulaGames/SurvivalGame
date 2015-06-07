using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;

namespace Mentula.Content.MR
{
    [ContentImporter(".mr", DefaultProcessor = "MRProcessor", DisplayName = "Mentula R Importer")]
    internal class MRImporter : ContentImporter<MRSource>
    {
        public override MRSource Import(string filename, ContentImporterContext context)
        {
            string source = File.ReadAllText(filename);
            return new MRSource(source);
        }
    }
}
using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;

namespace Mentula.Content
{
    [ContentImporter(".mm", DefaultProcessor = "MMProcessor", DisplayName = "Mentula Material Importer")]
    internal class MMImporter : ContentImporter<MMSource>
    {
        public override MMSource Import(string filename, ContentImporterContext context)
        {
            string source = File.ReadAllText(filename);
            return new MMSource(source);
        }
    }
}
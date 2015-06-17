using Microsoft.Xna.Framework.Content.Pipeline;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.Content.MM
{
    [ContentProcessor(DisplayName = "Mentula R Processor")]
    internal class MRProcessor : ContentProcessor<MMSource, R>
    {
        public override R Process(MMSource input, ContentProcessorContext context)
        {
            Utils.CheckProcessorType("R", input.Container.Values["DEFAULT"]);

            int length = input.Container.Childs.Length;
            KeyValuePair<string, KeyValuePair<int, string>[]>[] result = new KeyValuePair<string, KeyValuePair<int, string>[]>[length];

            for (int i = 0; i < length; i++)
            {
                Container curr = input.Container.Childs[i];
                Manifest mani = new Manifest();

                string rawValue = "";

                if (curr.TryGetValue("Default", out rawValue)) mani.Name = rawValue;
                else throw new ParameterNullException("Name");

                mani.Values = new KeyValuePair<int, string>[(curr.Values.Count - 1) >> 1];

                int index = 0;
                for (int j = 1; j < curr.Values.Count; j += 2)
                {
                    int key = 0;

                    string rawKey = curr.Values.ElementAt(j).Value;
                    if (j + 1 < curr.Values.Count) rawValue = curr.Values.ElementAt(j + 1).Value;
                    else throw new ParameterNullException("Value at key: " + rawKey);

                    if (!int.TryParse(rawKey, out key)) throw new ParameterException("Id", rawKey, typeof(int));

                    mani.Values[index] = new KeyValuePair<int, string>(key, rawValue);
                    index++;
                }

                result[i] = new KeyValuePair<string, KeyValuePair<int, string>[]>(mani.Name, mani.Values);
            }

            return new R(result);
        }

        private struct Manifest
        {
            public string Name;
            public KeyValuePair<int, string>[] Values;
        }
    }
}
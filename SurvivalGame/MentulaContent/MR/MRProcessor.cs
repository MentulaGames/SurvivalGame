using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.Content.MR
{
    [ContentProcessor(DisplayName = "Mentula R Processor")]
    internal class MRProcessor : ContentProcessor<MRSource, R>
    {
        public override R Process(MRSource input, ContentProcessorContext context)
        {
            Dictionary<string, KeyValuePair<int, string>[]> result = new Dictionary<string, KeyValuePair<int, string>[]>();

            for (int i = 0; i < input.Raw.Count; i++)
            {
                KeyValuePair<string, string[]> line = input.Raw.ElementAt(i);

                if (line.Key.Contains("Container"))
                {
                    string[] keySplit = line.Key.Split('=', ' ', ',', ']');
                    int size = 0;
                    string dir = "";

                    try { size = int.Parse(keySplit[Array.IndexOf<string>(keySplit, "size") + 1]); }
                    catch (InvalidCastException) { throw new ArgumentException(string.Format("{0} is not a valid size.", keySplit[Array.IndexOf<string>(keySplit, "size") + 1])); }
                    catch (Exception) { throw new ArgumentException(string.Format("The required attribute \"size\" could not be found")); }

                    try { dir = keySplit[Array.IndexOf<string>(keySplit, "dir") + 1]; }
                    catch (Exception) { throw new ArgumentException(string.Format("The required attribute \"dir\" could not be found")); }

                    string[] values = new string[size];
                    KeyValuePair<int, string>[] items = new KeyValuePair<int, string>[size];

                    for (int j = 0; j < size; j++)
                    {
                        string[] valSplit = line.Value[j].Split('=', ' ', ',', ']');
                        int id = 0;
                        string name = "";

                        try { id = int.Parse(valSplit[Array.IndexOf<string>(valSplit, "id") + 1]); }
                        catch (InvalidCastException) { throw new ArgumentException(string.Format("{0} is not a valid id.", valSplit[Array.IndexOf<string>(valSplit, "id") + 1])); }
                        catch (Exception) { throw new ArgumentException(string.Format("The required attribute \"id\" could not be found")); }

                        try { name = valSplit[Array.IndexOf(valSplit, "name") + 1]; }
                        catch (Exception) { throw new ArgumentException(string.Format("The required attribute \"name\" could not be found")); }

                        items[j] = new KeyValuePair<int, string>(id, name);
                    }

                    try { result.Add(dir, items); }
                    catch (Exception) { throw new ArgumentException(string.Format("The directory: {0} is already used. place your items there.", dir)); }
                }
                else throw new ArgumentException(string.Format("The container: \"{0}\" is not suported.", line));
            }

            return new R(result);
        }
    }
}

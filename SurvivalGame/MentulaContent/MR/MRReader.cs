using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mentula.Content.MR
{
    internal class MRReader : ContentTypeReader<R>
    {
        protected override R Read(ContentReader input, R existingInstance)
        {
            Dictionary<string, KeyValuePair<int, string>[]> raw = new Dictionary<string, KeyValuePair<int, string>[]>();

            try
            {
                int diff = input.ReadInt32();

                for (int i = 0; i < diff; i++)
                {
                    int length = input.ReadInt32();
                    string dir = input.ReadCString();

                    KeyValuePair<int, string>[] items = new KeyValuePair<int, string>[length];

                    for (int j = 0; j < length; j++)
                    {
                        int id = input.ReadInt32();
                        string name = input.ReadCString();
                        items[j] = new KeyValuePair<int, string>(id, name);
                    }

                    raw.Add(dir, items);
                }

                return new R(raw);
            }
            catch (Exception)
            {
                throw new FileLoadException("The file could not be loaded.", input.AssetName);
            }
        }
    }
}
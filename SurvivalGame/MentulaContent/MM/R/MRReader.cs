using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;

namespace Mentula.Content.MM
{
    internal class MRReader : ContentTypeReader<R>
    {
        protected override R Read(ContentReader input, R existingInstance)
        {
            try
            {
                int diff = input.ReadInt32();
                KeyValuePair<string, KeyValuePair<int, string>[]>[] raw = new KeyValuePair<string, KeyValuePair<int, string>[]>[diff];

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

                    raw[i] = new KeyValuePair<string, KeyValuePair<int, string>[]>(dir, items);
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.Content.MM
{
    public class R
    {
        public Dictionary<int, string> Values { get; private set; }

        internal R(KeyValuePair<string, KeyValuePair<int, string>[]>[] raw)
        {
            Values = new Dictionary<int, string>();

            for (int i = 0; i < raw.Length; i++)
            {
                KeyValuePair<string, KeyValuePair<int, string>[]> iA = raw.ElementAt(i);

                for (int j = 0; j < iA.Value.Length; j++)
                {
                    KeyValuePair<int, string> item = iA.Value[j];
                    Values.Add(item.Key, iA.Key + "/" + item.Value);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.Content
{
    public class R
    {
        public Dictionary<int, string> Values { get { return values; } }
        private Dictionary<int, string> values;

        internal R(Dictionary<string, KeyValuePair<int, string>[]> raw)
        {
            if (raw == null) throw new ArgumentNullException("raw", "Could not recieve a initialized value from thr processor.");

            values = new Dictionary<int, string>();

            for (int i = 0; i < raw.Count; i++)
            {
                KeyValuePair<string, KeyValuePair<int, string>[]> iA = raw.ElementAt(i);

                for (int j = 0; j < iA.Value.Length; j++)
                {
                    KeyValuePair<int, string> item = iA.Value[j];
                    values.Add(item.Key, iA.Key + "/" + item.Value);
                }
            }
        }
    }
}
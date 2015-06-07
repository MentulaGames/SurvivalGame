using System;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.Content
{
    internal class MRSource
    {
        public string Source { get { return source; } }
        public Dictionary<string, string[]> Raw { get { return raw; } }

        private string source;
        private Dictionary<string, string[]> raw;

        public MRSource(string source)
        {
            this.source = source;
            raw = new Dictionary<string, string[]>();

            string[] newLineSplit = source.Replace("\t", "").Replace("\r", "").Replace("\"", "").Split('\n');

            string key = "";
            List<string> value = new List<string>();

            for (int i = 0; i < newLineSplit.Length; i++)
            {
                string line = newLineSplit[i];

                if (line.Contains('{'))
                {
                    value.Clear();

                    try { key = newLineSplit[i - 1]; }
                    catch (IndexOutOfRangeException) { throw new ArgumentException("A type container is missing."); }
                }
                else if (line.Contains('}'))
                {
                    if (value.Count <= 0) throw new ArgumentNullException(string.Format("The container: {0} is useless (No children).", key));
                    raw.Add(key, value.ToArray());
                    value.Clear();
                }
                else value.Add(line);
            }
        }
    }
}
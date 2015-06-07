using Mentula.Content.MM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mentula.Content.MM
{
    internal class MMSource
    {
        public string Source;
        public Container Container;

        public MMSource()
        {
            Container = new Container();
        }

        public MMSource(string source)
        {
            Source = source;
            Container = new Container();

            string[] newLineSplit = source.Replace("\t", "").Replace("\r", "").Split('\n').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

            Dictionary<string, string> valueLines = new Dictionary<string, string>();
            Stack<string> containers = new Stack<string>();

            for (int i = 0; i < newLineSplit.Length; i++)
            {
                string line = newLineSplit[i];

                try
                {
                    if (line.Contains('{'))
                    {
                        string containerLine = RemoveSpecials(newLineSplit[i - 1]);
                        string name;

                        string[] values = SplitContainer(containerLine, out name);

                        int prvCCount = GetChild(ref containers).Childs.Count(c => c.Name.Contains(name));
                        if (prvCCount > 0) name += prvCCount;
                        containers.Push(name);

                        valueLines.Remove(valueLines.ElementAt(valueLines.Count - 1).Key);

                        if (Container.IsUseless)
                        {
                            Container = new Container(name) { Values = GetValuesFromSplit(values) };
                        }
                        else
                        {
                            Container curr = ConstructChild(ref containers);
                            curr.Values = GetValuesFromSplit(values);
                        }
                    }
                    else if (line.Contains('}'))
                    {
                        Container end = GetChild(ref containers);
                        end.Values = end.Values.Union(GetValuesFromSplit(valueLines.Where(v => v.Value == containers.Peek()).Select(s => s.Key).ToArray())).ToDictionary(d => d.Key, v => v.Value);

                        string[] keys = valueLines.Where(v => v.Value == containers.Peek()).Select(s => s.Key).ToArray();
                        for (int j = 0; j < keys.Length; j++)
                        {
                            valueLines.Remove(keys[j]);
                        }
                        containers.Pop();
                    }
                    else valueLines.Add(line, containers.Count > 0 ? containers.Peek() : "NULL");
                }
                catch (Exception e)
                {
                    throw new ArgumentException("An error occured while trying to read line: '" + line + "'", e);
                }
            }
        }

        private Container GetChild(ref Stack<string> names)
        {
            try
            {
                string[] containers = names.ToArray();
                Array.Reverse(containers);

                Container curr = Container;
                for (int i = 1; i < containers.Length; i++)
                {
                    string name = containers[i];

                    curr.TryGetChild(name, out curr);
                }

                return curr;
            }
            catch (Exception e)
            {
                throw new ArgumentException("An error occured while trying to get a child at: '" + string.Join("/", names) + "'", e);
            }
        }

        private Container ConstructChild(ref Stack<string> names)
        {
            try
            {
                string[] containers = names.ToArray();
                Array.Reverse(containers);

                Container curr = Container;
                for (int i = 1; i < containers.Length; i++)
                {
                    string name = containers[i];

                    if (i == containers.Length - 1)
                    {
                        int length = curr.Childs.Length;
                        Array.Resize(ref curr.Childs, length + 1);
                        curr.Childs[length] = new Container(name);
                    }

                    curr.TryGetChild(name, out curr);
                }

                return curr;
            }
            catch (Exception e)
            {
                throw new ArgumentException("An error occured while trying to construct a child at: '" + string.Join("/", names) + "'", e);
            }
        }

        private Dictionary<string, string> GetValuesFromSplit(string[] values)
        {
            try
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                for (int i = 0; i < values.Length; i++)
                {
                    string line = RemoveSpecials(values[i]);
                    string[] split = line.Split('=', ':', ',');

                    string baseName = split.Length % 2 == 0 ? "" : split[0];
                    for (int j = baseName == "" ? 0 : 1; j < split.Length; j += 2)
                    {
                        if (baseName != "") result.Add(baseName + "." + split[j], split[j + 1]);
                        else result.Add(split[j], split[j + 1]);
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                throw new ArgumentException("An error occured while trying to get values from split: '" + string.Join("/", values) + "'", e);
            }
        }

        private string[] SplitContainer(string line, out string name)
        {
            try
            {
                string[] split = line.Split(':', ',');
                name = split[0];

                if (split.Length == 2) return new string[1] { "DEFAULT=" + split[1] };

                string[] result = new string[split.Length - 1];
                for (int i = 1; i < split.Length; i++)
                {
                    result[i - 1] = split[i];
                }

                return result;
            }
            catch (Exception e)
            {
                throw new ArgumentException("An error occured while trying to split the container: '" + line + "'", e);
            }
        }

        private string RemoveSpecials(string line)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char curr = line[i];

                if (curr != ' ' & curr != '"' & curr != '\'' & curr != '{' & curr != '}' & curr != '[' & curr != ']') sb.Append(curr);
            }

            return sb.ToString();
        }
    }
}

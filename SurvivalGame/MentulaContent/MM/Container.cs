using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mentula.Content.MM
{
    [DebuggerDisplay("{ToString()}")]
    internal class Container
    {
        public string Name;
        public Dictionary<string, string> Values;
        public Container[] Childs;

        public bool IsUseless { get { return string.IsNullOrWhiteSpace(Name) | (Values.Count == 0 & Childs.Length == 0) ? true : false; } }

        public Container()
        {
            Name = "";
            Values = new Dictionary<string, string>();
            Childs = new Container[0];
        }

        public Container(string name)
        {
            Name = name;
            Values = new Dictionary<string, string>();
            Childs = new Container[0];
        }

        public bool TryGetValue(string name, out string value)
        {
            value = Values.FirstOrDefault(v => v.Key.ToUpper() == name.ToUpper()).Value;
            return !string.IsNullOrWhiteSpace(value) ? true : false;
        }

        public bool TryGetChild(string name, out Container value)
        {
            value = Childs.FirstOrDefault(c => c.Name.ToUpper() == name.ToUpper());
            return value != null ? true : false;
        }

        public override string ToString()
        {
            return "Name=" + Name + ", NumValues=" + Values.Count + ", NumChilds=" + Childs.Length;
        }
    }
}
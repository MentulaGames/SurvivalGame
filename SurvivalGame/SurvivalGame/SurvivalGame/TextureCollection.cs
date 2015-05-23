using Mentula.Content;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Mentula
{
    public class TextureCollection : Dictionary<int, Texture2D>
    {
        private ContentManager m_Content;

        public TextureCollection(ContentManager content)
            : base()
        {
            m_Content = content;
        }

        public TextureCollection(ContentManager content, int size)
            : base(size)
        {
            m_Content = content;
        }

        public void LoadFromConfig(string name)
        {
            R config = m_Content.Load<R>(name);

            for (int i = 0; i < config.Values.Count; i++)
            {
                KeyValuePair<int, string> cur = config.Values.ElementAt(i);
                Add(cur.Key, cur.Value);
            }
        }

        public void Add(int id, string name)
        {
            Add(id, m_Content.Load<Texture2D>(name));
        }
    }
}
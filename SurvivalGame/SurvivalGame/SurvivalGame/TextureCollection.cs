using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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

        public void Add(int id, string name)
        {
            Add(id, m_Content.Load<Texture2D>(name));
        }
    }
}

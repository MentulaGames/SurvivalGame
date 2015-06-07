using Mentula.Content;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.SurvivalGame
{
    public class TextureCollection : Dictionary<int, Texture2D>
    {
        private ContentManager content;

        public TextureCollection(ContentManager content)
            : base()
        {
            this.content = content;
        }

        public TextureCollection(ContentManager content, int size)
            : base(size)
        {
            this.content = content;
        }

        public void LoadFromConfig(string name)
        {
            R config = content.Load<R>(name);

            for (int i = 0; i < config.Values.Count; i++)
            {
                KeyValuePair<int, string> cur = config.Values.ElementAt(i);
                Add(cur.Key, cur.Value);
            }
        }

        public void Add(int id, string name)
        {
            Add(id, content.Load<Texture2D>(name));
        }
    }
}
using Mentula.Content;
using Microsoft.Xna.Framework.Content;

namespace Mentula.SurvivalGameServer
{
    public class Content
    {
        public readonly Metal[] Metals;

        public Content(ref ContentManager content, string metals)
        {
            Metals = content.Load<Metal[]>(metals);
        }
    }
}
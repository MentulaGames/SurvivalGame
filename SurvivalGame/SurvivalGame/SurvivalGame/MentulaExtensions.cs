using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mentula.SurvivalGame
{
    public static class MentulaExtensions
    {
        public static void Draw(this SpriteBatch batch, Texture2D texture, Vector2 position, Color color, byte layer)
        {
            batch.Draw(texture, position, null, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f / (layer + 1));
        }
    }
}
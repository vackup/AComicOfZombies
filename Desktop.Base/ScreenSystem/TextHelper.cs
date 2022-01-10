using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktop.Base.ScreenSystem
{
    public static class TextHelper
    {
        /// <summary>
        /// Dibuja textos sombreados en pantalla
        /// </summary>
        /// <param name="spriteBatch"> </param>
        /// <param name="font"></param>
        /// <param name="value"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        public static void DrawShadowedString(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color)
        {
            //spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            //spriteBatch.DrawString(font, value, position, color);
            DrawShadowedString(spriteBatch, font, value, position, color, 0.0f, 1.0f);
        }

        public static void DrawShadowedString(SpriteBatch spriteBatch, SpriteFont font, string value, Vector2 position, Color color, float textRotation, float scaleText)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black, textRotation, Vector2.Zero, scaleText, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, value, position, color, textRotation, Vector2.Zero, scaleText, SpriteEffects.None, 0);
        }
    }
}

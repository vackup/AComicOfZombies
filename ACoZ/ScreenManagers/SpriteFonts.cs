using ACoZ.Helpers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ACoZ.ScreenManagers
{
    /// <summary>
    /// Font helper class.
    /// </summary>
    public class SpriteFonts
    {
        public SpriteFonts(ContentManager contentManager)
        {
            this.TextFont = contentManager.Load<SpriteFont>(GlobalParameters.TEXT_FONT);
            this.TextFontSmall = contentManager.Load<SpriteFont>(GlobalParameters.TEXT_FONT_SMALL);
            this.TittleFont = contentManager.Load<SpriteFont>(GlobalParameters.TITTLE_FONT);
            this.TittleFontMedium = contentManager.Load<SpriteFont>(GlobalParameters.TITTLE_FONT_MEDIUM);
            this.TittleFontSmall = contentManager.Load<SpriteFont>(GlobalParameters.TITTLE_FONT_SMALL);
        }

        public SpriteFont TittleFont { get; private set; }
        public SpriteFont TittleFontMedium { get; private set; }
        public SpriteFont TittleFontSmall { get; private set; }

        public SpriteFont TextFont { get; private set; }
        public SpriteFont TextFontSmall { get; private set; }
    }
}

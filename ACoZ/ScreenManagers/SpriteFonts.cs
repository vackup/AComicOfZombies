using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Platformer.Helpers;

namespace Platformer.ScreenManagers
{
    /// <summary>
    /// Font helper class.
    /// </summary>
    public class SpriteFonts
    {
        public SpriteFonts(ContentManager contentManager)
        {
            TextFont = contentManager.Load<SpriteFont>(GlobalParameters.TEXT_FONT);
            TextFontSmall = contentManager.Load<SpriteFont>(GlobalParameters.TEXT_FONT_SMALL);
            TittleFont = contentManager.Load<SpriteFont>(GlobalParameters.TITTLE_FONT);
            TittleFontMedium = contentManager.Load<SpriteFont>(GlobalParameters.TITTLE_FONT_MEDIUM);
            TittleFontSmall = contentManager.Load<SpriteFont>(GlobalParameters.TITTLE_FONT_SMALL);
        }

        public SpriteFont TittleFont { get; private set; }
        public SpriteFont TittleFontMedium { get; private set; }
        public SpriteFont TittleFontSmall { get; private set; }

        public SpriteFont TextFont { get; private set; }
        public SpriteFont TextFontSmall { get; private set; }
    }
}

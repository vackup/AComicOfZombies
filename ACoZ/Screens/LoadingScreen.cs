#region Using Statements

using System;
using ACoZ.ScreenManagers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace ACoZ.Screens
{
    /// <summary>
    /// The loading screen coordinates transitions between the menu system and the
    /// game itself. Normally one screen will transition off at the same time as
    /// the next screen is transitioning on, but for larger transitions that can
    /// take a longer time to load their data, we want the menu system to be entirely
    /// gone before we start loading the game. This is done as follows:
    /// 
    /// - Tell all the existing screens to transition off.
    /// - Activate a loading screen, which will transition on at the same time.
    /// - The loading screen watches the state of the previous screens.
    /// - When it sees they have finished transitioning off, it activates the real
    ///   next screen, which may take a long time to load its data. The loading
    ///   screen will be the only thing displayed while this load is taking place.
    /// </summary>
    class LoadingScreen : GameScreen
    {
        #region Fields

        private readonly bool _loadingIsSlow;
        private bool _otherScreensAreGone;
        private readonly GameScreen[] _screensToLoad;
        private SpriteBatch _spriteBatch;
        private Vector2 _textPosition;
        private SpriteFont _font;
        private const string MESSAGE = "Loading...";
        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        private LoadingScreen(bool loadingIsSlow,
                              GameScreen[] screensToLoad)
        {
            this._loadingIsSlow = loadingIsSlow;
            this._screensToLoad = screensToLoad;

            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Activates the loading screen.
        /// </summary>
        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
                                PlayerIndex? controllingPlayer,
                                params GameScreen[] screensToLoad)
        {
            // Tell all the current screens to transition off.
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            // Create and activate the loading screen.
            var loadingScreen = new LoadingScreen(loadingIsSlow, screensToLoad);

            screenManager.AddScreen(loadingScreen, controllingPlayer);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (this._otherScreensAreGone)
            {
                this.ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in this._screensToLoad)
                {
                    if (screen != null)
                    {
                        this.ScreenManager.AddScreen(screen, this.ControllingPlayer);
                    }
                }

                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
#if WINDOWS_PHONE || WINDOWS || XBOX
				ScreenManager.Game.ResetElapsedTime();
#endif
                // TODO: hz - IMPLENTAR ScreenManager.Game.ResetElapsedTime();
            }
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            if ((this.ScreenState == ScreenState.Active) &&
                (this.ScreenManager.GetScreens().Length == 1))
            {
                this._otherScreensAreGone = true;
            }

            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
            if (this._loadingIsSlow)
            {
                //var spriteBatch = ScreenManager.SpriteBatch;
                //var font = ScreenManager.SpriteFonts.TextFont;

                //const string message = "Loading...";

                // Center the text in the viewport.
                //var viewport = ScreenManager.GraphicsDevice.Viewport;
                //var viewportSize = new Vector2(_viewport.Width, _viewport.Height);
                //var textSize = font.MeasureString(message);
                //var textPosition = (_viewportSize - textSize) / 2;

                var color = Color.White * this.TransitionAlpha;

                // Draw the text.
                this._spriteBatch.Begin();
                this._spriteBatch.DrawString(this._font, MESSAGE, this._textPosition, color);
                this._spriteBatch.End();
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();

            this._spriteBatch = this.ScreenManager.SpriteBatch;
            this._font = this.ScreenManager.SpriteFonts.TittleFont;
            

            var viewport = this.ScreenManager.GraphicsDevice.Viewport;
            var viewportSize = new Vector2(viewport.Width, viewport.Height);

            this._textPosition = (viewportSize - this._font.MeasureString(MESSAGE)) / 2;
        }

        public override void UnloadContent()
        {
            // No hay nada que limpiar
        }
        #endregion
    }
}

#region Using Statements

using System;
using ACoZ.ScreenManagers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#if WINDOWS_PHONE || IPHONE
using Mobile.Base.ScreenSystem;
#elif SILVERLIGHT
using Web.Base.ScreenSystem;
#elif WINDOWS
using Desktop.Base.ScreenSystem;
#endif

#endregion

namespace ACoZ.Screens
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    class MessageBoxScreen : GameScreen
    {
        #region Fields

        private readonly string _message;
        private Texture2D _gradientTexture;
        #endregion

        #region Events

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message)
            : this(message, true)
        { }


        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message, bool includeUsageText)
        {
            const string usageText = "\nA button, Space, Enter = ok" +
                                     "\nB button, Esc = cancel"; 
            
            if (includeUsageText)
                this._message = message + usageText;
            else
                this._message = message;

            this.IsPopup = true;

            this.TransitionOnTime = TimeSpan.FromSeconds(0.2);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }


        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            this._gradientTexture = this.ScreenManager.Game.Content.Load<Texture2D>("Menu/gradient");
        }

        public override void UnloadContent()
        {
            //_content.Unload();
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputHelper input)
        {
            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (input.IsMenuSelect(this.ControllingPlayer, out playerIndex))
            {
                // Raise the accepted event, then exit the message box.
                if (this.Accepted != null)
                    this.Accepted(this, new PlayerIndexEventArgs(playerIndex));

                this.ExitScreen();
            }
            else if (input.IsMenuCancel(this.ControllingPlayer, out playerIndex))
            {
                // Raise the cancelled event, then exit the message box.
                if (this.Cancelled != null)
                    this.Cancelled(this, new PlayerIndexEventArgs(playerIndex));

                this.ExitScreen();
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = this.ScreenManager.SpriteBatch;
            SpriteFont font = this.ScreenManager.SpriteFonts.TextFont;

            // Darken down any other screens that were drawn beneath the popup.
            this.ScreenManager.FadeBackBufferToBlack(this.TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = this.ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(this._message);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 16;

            Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                          (int)textPosition.Y - vPad,
                                                          (int)textSize.X + hPad * 2,
                                                          (int)textSize.Y + vPad * 2);

            // Fade the popup alpha during transitions.
            Color color = Color.White * this.TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(this._gradientTexture, backgroundRectangle, color);

            // Draw the message box text.
            spriteBatch.DrawString(font, this._message, textPosition, color);

            spriteBatch.End();
        }


        #endregion
    }
}

#region Using Statements

using System;
using ACoZ.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if WINDOWS_PHONE || IPHONE
using Mobile.Base.Controls;
#elif SILVERLIGHT
using Web.Base.Controls;
#elif WINDOWS
using Desktop.Base.Controls;
#endif

#endregion

namespace ACoZ.Screens
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages on a touch device
    /// </summary>
    public class TouchMessageBoxScreen : ButtonMenuScreen
    {
        private readonly string _message;
        private readonly Button.ClickHandler _accepted;

        #region Fields
        private Texture2D _backgroundTexture;
        private int _viewPortHalfWith;
        //private int _messageLines;
        //private readonly ContentManager _content;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public TouchMessageBoxScreen(string message, Button.ClickHandler accepted)
            : base(true)
        {
            this._message = message;
            
            //if (_message.Length > 20)
            //{
            //    var halfLength = _message.Length/2;

            //    while (_message.Substring(halfLength-1, 1) != " ")
            //    {
            //        halfLength++;
            //    }

            //    _message = _message.Substring(0, halfLength) + Environment.NewLine +
            //               _message.Substring(halfLength, _message.Length - halfLength);

            //    _messageLines = 2;
            //}
            //else
            //{
            //    _messageLines = 1;
            //}
            
            this._accepted = accepted;
            //// Flag that there is no need for the game to transition
            //// off when the pause menu is on top of it.
            //IsPopup = true;

            //// Create our menu entries.
            //var confirmMenuEntry = new MenuEntry("Confirm");
            //var cancelMenuEntry = new MenuEntry("Cancel");

            //// Hook up menu event handlers.
            ////confirmMenuEntry.Selected += ConfirmQuitMessageBoxAccepted;
            //confirmMenuEntry.Selected += Accepted;
            //cancelMenuEntry.Selected += OnCancel;

            //// Add entries to the menu.
            //MenuEntries.Add(confirmMenuEntry);
            //MenuEntries.Add(cancelMenuEntry);

            //if (_content == null)
            //{
            //    _content = ScreenManager.Game.Content;
            //}
        }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            base.LoadContent();

            this._backgroundTexture = this.ScreenManager.Game.Content.Load<Texture2D>(GlobalParameters.WEAPON_BUTTON_NORMAL);

            //Creates Menu
            this.CreateMenu();
        }

        private void CreateMenu()
        {
            //float screenWith = Viewport.Width;

            this._viewPortHalfWith = this.Viewport.Width / 2;

            //Add MenuItems
            //1. Play Game
            var confirmButton = new Button
            {
                //DisplayText = "Confirm",
                //TextVisible = true,
                //Font = ScreenManager.SpriteFonts.TittleFont,
                //TextSize = Button.FontSize.Big,
                //TextAlignment = Button.TextAlign.Centre                 
                NormalButtonTexture = this.ScreenManager.ScreenAssetsTexture,
                TextureRectangle = GlobalParameters.ScreenAssetsData["PANTALLA_QUIT0001_psd-confirm"],
            };

            //confirmButton.Width = confirmButton.TextWidth;
            //confirmButton.Height = confirmButton.TextHeight;
            confirmButton.Width = confirmButton.TextureRectangle.Width;
            confirmButton.Height = confirmButton.TextureRectangle.Height;
            confirmButton.Position = new Vector2(this._viewPortHalfWith - confirmButton.Width / 2, GlobalParameters.TOUCH_MESSAGE_BOX_SCREENFIRST_BUTTON_INITIAL_POSITION);
            confirmButton.OnClicked += this._accepted;

            this.PanelMenu.AddChild(confirmButton);

            //2. Options
            var cancelButton = new Button
            {
                //DisplayText = "Cancel",
                //TextVisible = true,
                //Font = ScreenManager.SpriteFonts.TittleFont,
                //TextSize = Button.FontSize.Big,
                //TextAlignment = Button.TextAlign.Centre
                NormalButtonTexture = this.ScreenManager.ScreenAssetsTexture,
                TextureRectangle = GlobalParameters.ScreenAssetsData["PANTALLA_QUIT0001_psd-cancel"],
            };

            //cancelButton.Width = cancelButton.TextWidth;
            //cancelButton.Height = cancelButton.TextHeight;
            cancelButton.Width = cancelButton.TextureRectangle.Width;
            cancelButton.Height = cancelButton.TextureRectangle.Height;
            cancelButton.Position = new Vector2(this._viewPortHalfWith - cancelButton.Width / 2,
                                                 confirmButton.Position.Y + confirmButton.Height +
                                                 GlobalParameters.TOUCH_MESSAGE_BOX_SCREENMARGIN_BETWEEN_BUTTONS);
            cancelButton.OnClicked += OnCancel;

            this.PanelMenu.AddChild(cancelButton);
        }

        public override void UnloadContent()
        {
            //_content.Unload();
        }

        #endregion

        //#region Handle Input
        ///// <summary>
        ///// Event handler for when the user selects ok on the "are you sure
        ///// you want to quit" message box. This uses the loading screen to
        ///// transition from the game back to the main menu screen.
        ///// </summary>
        //void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        //{
        //    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
        //                                                   new MainMenuScreen());
        //}


        //#endregion

        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Darken down any other screens that were drawn beneath the popup.
            this.ScreenManager.FadeBackBufferToBlack(this.TransitionAlpha * 2 / 3);

            // make sure our entries are in the right place before we draw them
            //UpdateMenuEntryLocations();

            //var graphics = ScreenManager.GraphicsDevice;
            var spriteBatch = this.ScreenManager.SpriteBatch;
            var font = this.ScreenManager.SpriteFonts.TextFont;

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            var transitionOffset = (float)Math.Pow(this.TransitionPosition, 2);

            // Draw the menu title centered on the screen
            var titlePosition = new Vector2(this._viewPortHalfWith, GlobalParameters.TOUCH_MESSAGE_BOX_SCREEN_BOX_POSITION_Y);
            var titleOrigin = font.MeasureString(this._message) / 2;
            var titleColor = new Color(192, 192, 192) * this.TransitionAlpha;
            const float titleScale = 1.15f;

            titlePosition.Y -= transitionOffset * 100;

            // Center the message text in the viewport.
            var viewport = this.ScreenManager.GraphicsDevice.Viewport;
            var viewportSize = new Vector2(viewport.Width, viewport.Height);
            var textSize = font.MeasureString(this._message) * titleScale;
            var textPosition = (viewportSize - textSize) / (2 * titleScale);

            // The background includes a border somewhat larger than the text itself.
            const int hPad = 32;
            const int vPad = 50;

            //var backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
            //                                        (int)titlePosition.Y - vPad,
            //                                        (int)textSize.X + hPad * 2,
            //                                        (int)textSize.Y * (3) + vPad * 2);
                                                    //(int)textSize.Y * (menuEntries.Count + 1) + vPad * 2);

            var backgroundRectangle = new Rectangle(0,
                                                    (int)titlePosition.Y - vPad,
                                                    GlobalParameters.SCREEN_WIDTH,
                                                    (int)textSize.Y * (6) + vPad * 2);

            // NOTE: con esto podemos eliminar: viewport, viewportSize y textPosition pero no me queda bien centrado, revisar
            //var backgroundRectangle = new Rectangle((int)((titlePosition.X - titleOrigin.X) / titleScale) - hPad,
            //                                        (int)titlePosition.Y - vPad,
            //                                        (int)textSize.X + hPad * 2,
            //                                        (int)textSize.Y * (menuEntries.Count + 1) + vPad * 2);

            // Fade the popup alpha during transitions.
            var color = Color.White * this.TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(this._backgroundTexture, backgroundRectangle, color);

            // Draw each menu entry in turn.
            //for (int i = 0; i < menuEntries.Count; i++)
            //{
            //    MenuEntry menuEntry = menuEntries[i];

            //    bool isSelected = IsActive && (i == selectedEntry);

            //    menuEntry.Draw(this, isSelected, gameTime);

            //    //position.Y += menuEntry.GetHeight(this);
            //}

            // Draw the menu title centered on the screen
            spriteBatch.DrawString(font, this._message, titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);

            //// Draw the message box text.
            //spriteBatch.DrawString(font, _message, textPosition, titleColor);

            spriteBatch.End();

            base.Draw(gameTime);
        }


        #endregion
    }
}

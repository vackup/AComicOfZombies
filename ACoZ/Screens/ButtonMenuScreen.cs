using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer.ScreenManagers;

#if WINDOWS_PHONE || IPHONE
using Mobile.Base.Controls;
using Mobile.Base.ScreenSystem;
using Microsoft.Xna.Framework.Input.Touch;
#elif SILVERLIGHT
using Web.Base.Controls;
using Web.Base.ScreenSystem;
#elif WINDOWS
using Desktop.Base.Controls;
using Desktop.Base.ScreenSystem;
#endif

namespace Platformer.Screens
{
    public abstract class ButtonMenuScreen : GameScreen
    {
        protected Rectangle Viewport;
        protected PanelControl PanelMenu;
        //private Vector2 _position;
        //private bool _down;

        #region Initialization
        public ButtonMenuScreen(bool isPopUp)
        {
            IsPopup = isPopUp;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

//#if WINDOWS_PHONE || IPHONE
//            EnabledGestures = GestureType.Tap;
//#endif

            // Default position
            PanelMenu = new PanelControl
                          {
                              Position = new Vector2(0, 0)
                          };
        }

        public override void LoadContent()
        {
            UpdateScreen();

            base.LoadContent();
        }

        private void UpdateScreen()
        {
            var viewport = ScreenManager.GraphicsDevice.Viewport;
            Viewport = new Rectangle(0, 0, viewport.Width, viewport.Height);
        }
        #endregion

        #region Input Handling

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public override void HandleInput(InputHelper input)
        {
            PanelMenu.HandleInput(input);
            base.HandleInput(input);
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            if (!IsActive) return;

            // TODO: agregar efectos deslizantes como hacia MenuScreen (no es obligatorio)

            Control.BatchDraw(PanelMenu, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
        }


        #endregion

        #region Game Methods - Update

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            // Allows popup to be closed by back button
            if (IsActive && GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                ExitScreen();
            }

            //Update Menu states
            PanelMenu.Update(gameTime);

            //// TODO: remover esto que lo puse para testear los clicks
            //// INICIO TEST
            //var state = Mouse.GetState();
            //_position = new Vector2(state.X, state.Y);
            //_down = state.LeftButton == ButtonState.Pressed;
            //// FIN TEST

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        #endregion

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected virtual void OnCancel(Button sender)
        {
            ExitScreen();
        }
    }
}

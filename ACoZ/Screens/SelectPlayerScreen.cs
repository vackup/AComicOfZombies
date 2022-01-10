#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer.Helpers;
using Platformer.Players;
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

#endregion

namespace Platformer.Screens
{
    /// <summary>
    /// Buy Weapons Screen
    /// </summary>
    class SelectPlayerScreen : GameScreen
    {
        #region Properties & Variables
        private Rectangle _viewport;
        private PanelControl _pnlMenu;
        private int _currentGoodGuy;
        private ContentManager _content;

        #endregion

        #region Initialization and Load

        public SelectPlayerScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            if (_content == null)
            {
                _content = new ContentManager(ScreenManager.Game.Services, GlobalParameters.CONTENT_FOLDER);
            }

            foreach (var playerInfo in GlobalParameters.PlayersInfoList)
            {
                playerInfo.Init(_content);
            }

            UpdateScreen();

            CreateMenu();

            _currentGoodGuy = PlayerInfoManager.CurrentPlayerIndex;
            PlayerInfoManager.CurrentPlayerInfo = PlayerInfoManager.GetGoodGuy(_currentGoodGuy);

            base.LoadContent();
        }

        private void UpdateScreen()
        {
            var viewport = ScreenManager.GraphicsDevice.Viewport;
            _viewport = new Rectangle(0, 0, viewport.Width, viewport.Height);
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
            // TODO: codigo para hacer que cuando se presiona el boton BACK del telefono, vuelva a otra pantalla. No funciona pero por ahi debe estar. Ver GamePlayScreen cuando se pausa.
            //PlayerIndex playerIndex;

            //if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            //{
            //    // Raise the cancelled event, then exit the message box.
            //    //if (Cancelled != null)
            //    //    Cancelled(this, new PlayerIndexEventArgs(playerIndex));

            //    ExitScreen();
            //}

            _pnlMenu.HandleInput(input);
            base.HandleInput(input);
        }

        // TODO: para analizar en versiones para otras plataformas
        //public override void HandleGamePadInput(InputHelper input)
        //{
        //    base.HandleGamePadInput(input);
        //}
        //public override void HandleKeyboardInput(InputHelper input)
        //{
        //    base.HandleKeyboardInput(input);
        //}

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
            _pnlMenu.Update(gameTime);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        #endregion

        #region Game Methods - Draw
        public override void Draw(GameTime gameTime)
        {
            if (!IsActive) return;

            ScreenManager.SpriteBatch.Begin();

            //Draw Background
            //DrawBackground(ScreenManager.SpriteBatch);

            // Draw current player info
            DrawCurrentPlayerInfo(ScreenManager.SpriteBatch);

            ScreenManager.SpriteBatch.End();

            // Draw Menu
            DrawMenu(gameTime);
        }

        private void DrawCurrentPlayerInfo(SpriteBatch spriteBatch)
        {
            var playerInfo = PlayerInfoManager.GetGoodGuy(_currentGoodGuy);
            playerInfo.Draw(spriteBatch, new Vector2(_viewport.Width / 2 - playerInfo.Texture.Width / 2, 5));
        }

        private void DrawMenu(GameTime gameTime)
        {
            //Draw Weapon Selector list
            Control.BatchDraw(_pnlMenu, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
        }

        #endregion

        #region Menu
        private void CreateMenu()
        {
            //Initialize MenuPanel
            _pnlMenu = new PanelControl();

            var startButton = new Button
                                  {
                                      DisplayText = "start",
                                      TextVisible = true,
                                      Font = ScreenManager.SpriteFonts.TittleFont,
                                      TextSize = Button.FontSize.Big,
                                      TextAlignment = Button.TextAlign.Centre,
                                  };
            

            startButton.Width = startButton.TextWidth;
            startButton.Height = startButton.TextHeight;
            startButton.Position = new Vector2(_viewport.Width - startButton.Width, _viewport.Height - startButton.Height);
            startButton.OnClicked += sender => StartButtonClick();

            var previousPlayerButton = new Button
                                           {
                                               DisplayText = "prev",
                                               TextVisible = true,
                                               Font = ScreenManager.SpriteFonts.TittleFont,
                                               TextSize = Button.FontSize.Big,
                                               TextAlignment = Button.TextAlign.Centre,
                                           };
            previousPlayerButton.Width = previousPlayerButton.TextWidth;
            previousPlayerButton.Height = previousPlayerButton.TextHeight;
            previousPlayerButton.Position = new Vector2(0, 0);
            previousPlayerButton.OnClicked += sender => PreviousButtonClick();

            var nextPlayerButton = new Button
                                       {
                                           DisplayText = "next",
                                           TextVisible = true,
                                           Font = ScreenManager.SpriteFonts.TittleFont,
                                           TextSize = Button.FontSize.Big,
                                           TextAlignment = Button.TextAlign.Centre,
                                       };
            
            nextPlayerButton.Width = nextPlayerButton.TextWidth;
            nextPlayerButton.Height = nextPlayerButton.TextHeight;
            nextPlayerButton.Position = new Vector2(_viewport.Width - nextPlayerButton.Width, 0);
            nextPlayerButton.OnClicked += sender => NextButtonClick();
            
            _pnlMenu.AddChild(startButton);
            _pnlMenu.AddChild(previousPlayerButton);
            _pnlMenu.AddChild(nextPlayerButton);

        }

        private void StartButtonClick()
        {
            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new GameplayScreen(_currentGoodGuy));
        }

        private void PreviousButtonClick()
        {
            _currentGoodGuy--;
            if (_currentGoodGuy < 0)
                _currentGoodGuy = PlayerInfoManager.Count - 1;

            PlayerInfoManager.CurrentPlayerIndex = _currentGoodGuy;
            PlayerInfoManager.CurrentPlayerInfo = PlayerInfoManager.GetGoodGuy(_currentGoodGuy);
        }

        private void NextButtonClick()
        {
            _currentGoodGuy++;
            if (_currentGoodGuy >= PlayerInfoManager.Count)
                _currentGoodGuy = 0;

            PlayerInfoManager.CurrentPlayerIndex = _currentGoodGuy;
            PlayerInfoManager.CurrentPlayerInfo = PlayerInfoManager.GetGoodGuy(_currentGoodGuy);
        }
        #endregion

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            _content.Unload();
        }
    }
}

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Platformer.Helpers;
using Platformer.Levels;
#if WINDOWS_PHONE || IPHONE
using Mobile.Base.Controls;
#elif SILVERLIGHT
using Web.Base.Controls;
#elif WINDOWS
using Desktop.Base.Controls;
#endif

#endregion

namespace Platformer.Screens
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : ButtonMenuScreen
    {
        private readonly Level _level;
        private ContentManager _content;

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="level"></param>
        public PauseMenuScreen(Level level) : base(true)
        {
            _level = level;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            if (_content == null)
            {
                //_content = new ContentManager(ScreenManager.Game.Services, GlobalParameters.CONTENT_FOLDER);
                _content = ScreenManager.Game.Content;
            }

            //Creates Menu
            CreateMenu();
        }
        #endregion

        #region Menu
        private void CreateMenu()
        {
            var backgroundTexture = _content.Load<Texture2D>(GlobalParameters.WEAPON_BUTTON_NORMAL);
            var backgroungImage = new ImageControl(backgroundTexture, new Rectangle(0, 0, GlobalParameters.SCREEN_WIDTH / 2, GlobalParameters.SCREEN_HEIGHT), Vector2.Zero);
            var backgroundHalfWith = backgroungImage.TextureRectangle.Value.Width / 2;

            //Set PanelMenu Position
            PanelMenu.Position = new Vector2(GlobalParameters.SCREEN_WIDTH / 2 - backgroundHalfWith, 0);
            PanelMenu.Size = backgroungImage.Size;

            var pausedTextureRectangle = GlobalParameters.ScreenAssetsData["PANTALLA_PAUSA0001_psd-PAUSED"];
            var pausedTitlePosition = new Vector2(backgroundHalfWith - pausedTextureRectangle.Width / 2, GlobalParameters.FIRST_BUTTON_INITIAL_POSITION);
            var pausedTitle = new ImageControl(ScreenManager.ScreenAssetsTexture, pausedTextureRectangle, pausedTitlePosition);

            //Add MenuItems
            //1. Resume Game
            var resumeGameButton = new Button
                                       {
                                           NormalButtonTexture = ScreenManager.ScreenAssetsTexture,
                                           TextureRectangle = GlobalParameters.ScreenAssetsData["PANTALLA_PAUSA0001_psd-RESUME_GAME"],
                                       };
            resumeGameButton.Width = resumeGameButton.TextureRectangle.Width;
            resumeGameButton.Height = resumeGameButton.TextureRectangle.Height;
            resumeGameButton.Position = new Vector2(backgroundHalfWith - resumeGameButton.Width / 2, pausedTitle.Position.Y + pausedTextureRectangle.Height + GlobalParameters.MARGIN_BETWEEN_BUTTONS);
            resumeGameButton.OnClicked += OnCancel;

            //2. Save Game
            //var saveGameButtonTexture = _content.Load<Texture2D>(GlobalParameters.SAVE_GAME_BUTTON_NORMAL);
#if !IPHONE
            var saveGameButton = new Button
                                     {
                                         NormalButtonTexture = ScreenManager.ScreenAssetsTexture,
                                         TextureRectangle = GlobalParameters.ScreenAssetsData["PANTALLA_PAUSA0001_psd-SAVE_GAME"],
                                     };
            saveGameButton.Width = saveGameButton.TextureRectangle.Width;
            saveGameButton.Height = saveGameButton.TextureRectangle.Height;
            saveGameButton.Position = new Vector2(backgroundHalfWith - saveGameButton.Width/2,
                                                  resumeGameButton.Position.Y + resumeGameButton.Height +
                                                  GlobalParameters.MARGIN_BETWEEN_BUTTONS);
            saveGameButton.OnClicked += SaveGameMenuEntrySelected;
#endif

            //3. Quit Game
            //var quitGameButtonTexture = _content.Load<Texture2D>(GlobalParameters.QUIT_GAME_BUTTON_NORMAL);

            var quitGameButton = new Button
                                     {
                                         NormalButtonTexture = ScreenManager.ScreenAssetsTexture,
                                         TextureRectangle = GlobalParameters.ScreenAssetsData["PANTALLA_PAUSA0001_psd-QUIT_GAME"],
                                     };
            quitGameButton.Width = quitGameButton.TextureRectangle.Width;
            quitGameButton.Height = quitGameButton.TextureRectangle.Height;
#if !IPHONE
            quitGameButton.Position = new Vector2(backgroundHalfWith - quitGameButton.Width/2,
                                                  saveGameButton.Position.Y + saveGameButton.Height +
                                                  GlobalParameters.MARGIN_BETWEEN_BUTTONS);
#else
            quitGameButton.Position = new Vector2(backgroundHalfWith - quitGameButton.Width / 2,
                                                  resumeGameButton.Position.Y + resumeGameButton.Height +
                                                  GlobalParameters.MARGIN_BETWEEN_BUTTONS);
#endif
            quitGameButton.OnClicked += QuitGameMenuEntrySelected;

            //Add MenuItems to Menupanel
            PanelMenu.AddChild(backgroungImage);
            PanelMenu.AddChild(pausedTitle);
            PanelMenu.AddChild(resumeGameButton);
#if !IPHONE
            PanelMenu.AddChild(saveGameButton);
#endif
            PanelMenu.AddChild(quitGameButton);
        }

        #endregion

        #region Click Events
#if !IPHONE
        /// <summary>
        /// Event handler for when the Save Game menu entry is selected.
        /// </summary>
        void SaveGameMenuEntrySelected(Button sender)
        {
            // Hacer un MessageBoxScreen que no haya que confirmar, solo informativo que el juego se grabo
            //_level.SaveLevelData();
        }
#endif

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(Button sender)
        {
            ScreenManager.AddScreen(new TouchMessageBoxScreen(GlobalParameters.QUIT_GAME_MESSAGE, ConfirmQuitMessageBoxAccepted), ControllingPlayer);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        protected virtual void ConfirmQuitMessageBoxAccepted(Button sender)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }
        #endregion

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            //_content.Unload();
        }
    }
}

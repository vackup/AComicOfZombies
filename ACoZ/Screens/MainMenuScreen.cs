#region Using Statements

using System;
using System.Collections.Generic;
using ACoZ.Helpers;
using ACoZ.Players;
using ACoZ.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#if WINDOWS_PHONE || IPHONE
using Mobile.Base.Controls;
#elif SILVERLIGHT
using Web.Base.Controls;
#elif WINDOWS
using Desktop.Base.Controls;
#endif
#if IPHONE
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
#endif
#endregion
namespace ACoZ.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : ButtonMenuScreen
    {
        //private ContentManager _content;

        #region Initialization
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen() : base(false)
        {
        }

        public override void LoadContent()
        {
            //if (_content == null)
            //{
            //    //_content = new ContentManager(ScreenManager.Game.Services, GlobalParameters.CONTENT_FOLDER);

            //    // TODO: probando de usar el ContentManager compartido
            //    _content = ScreenManager.Game.Content;
            //}

            base.LoadContent();

            //Creates Menu
            this.CreateMenu();
        }

        #endregion

        #region Menu
        private void CreateMenu()
        {
            const float firstButtonInitialPosition = 70f;
            const float marginBetweenButtons = 25f;

//#if IPHONE
//    // Al ser el primer screen que carga, todavia no reconocio el giro de pantalla.
//            if (Viewport.Width < Viewport.Height)
//                screenWith = Viewport.Height;

//#endif

            int viewPortHalfWith = GlobalParameters.SCREEN_WIDTH / 2;

            //Add MenuItems
            //1. Play Game
            var playGameButton = new Button
                                     {
                                         NormalButtonTexture = this.ScreenManager.ScreenAssetsTexture,
                                         TextureRectangle = GlobalParameters.ScreenAssetsData["PANTALLA_INICIO0001_psd-Play_Game"],
                                         //DisplayText = "Play Game",
                                         //TextVisible = true,
                                         //Font = ScreenManager.SpriteFonts.TittleFont,
                                         //TextSize = Button.FontSize.Big,
                                         //TextAlignment = Button.TextAlign.Centre
                                     };

            playGameButton.Width = playGameButton.TextureRectangle.Width;
            playGameButton.Height = playGameButton.TextureRectangle.Height;
            playGameButton.Position = new Vector2(viewPortHalfWith - playGameButton.Width/2, firstButtonInitialPosition);
            playGameButton.OnClicked += SelectPlayerEntrySelected;

            this.PanelMenu.AddChild(playGameButton);

            //2. Options
            var optionsButton = new Button
                                    {
                                        NormalButtonTexture = this.ScreenManager.ScreenAssetsTexture,
                                        TextureRectangle = GlobalParameters.ScreenAssetsData["PANTALLA_INICIO0001_psd-Options"],
                                        //DisplayText = "Options",
                                        //TextVisible = true,
                                        //Font = ScreenManager.SpriteFonts.TittleFont,
                                        //TextSize = Button.FontSize.Big,
                                        //TextAlignment = Button.TextAlign.Centre
                                    };

            optionsButton.Width = optionsButton.TextureRectangle.Width;
            optionsButton.Height = optionsButton.TextureRectangle.Height;
            optionsButton.Position = new Vector2(viewPortHalfWith - optionsButton.Width/2,
                                                 playGameButton.Position.Y + playGameButton.Height +
                                                 marginBetweenButtons);
            optionsButton.OnClicked += OptionsMenuEntrySelected;

            this.PanelMenu.AddChild(optionsButton);
            
#if !SILVERLIGHT && !IPHONE
            //3. Exit Game
            var exitGameButton = new Button
                                     {
                                         NormalButtonTexture = this.ScreenManager.ScreenAssetsTexture,
                                         TextureRectangle = GlobalParameters.ScreenAssetsData["PANTALLA_INICIO0001_psd-Exit"],
                                         //DisplayText = "Exit Game",
                                         //TextVisible = true,
                                         //Font = ScreenManager.SpriteFonts.TittleFont,
                                         //TextSize = Button.FontSize.Big,
                                         //TextAlignment = Button.TextAlign.Centre
                                     };

            exitGameButton.Width = exitGameButton.TextureRectangle.Width;
            exitGameButton.Height = exitGameButton.TextureRectangle.Height;
            exitGameButton.Position = new Vector2(viewPortHalfWith - exitGameButton.Width/2,
                                                  optionsButton.Position.Y + optionsButton.Height + marginBetweenButtons);
            exitGameButton.OnClicked += OnCancel;

            this.PanelMenu.AddChild(exitGameButton);
#endif

            // Facebook button
            var facebookButton = new Button
            {
                NormalButtonTexture = this.ScreenManager.ScreenAssetsTexture,
                TextureRectangle = GlobalParameters.ScreenAssetsData["facebook"],
                //TextVisible = true,
                //TextSize = Button.FontSize.Big,
                //TextAlignment = Button.TextAlign.Centre
            };

            facebookButton.Width = facebookButton.TextureRectangle.Width;
            facebookButton.Height = facebookButton.TextureRectangle.Height;
            facebookButton.Position = new Vector2(GlobalParameters.SCREEN_WIDTH - facebookButton.Width - GlobalParameters.RIGHT_MARGIN,
                                                  GlobalParameters.SCREEN_HEIGHT - facebookButton.Height - GlobalParameters.BOTTOM_MARGIN);
            facebookButton.OnClicked += GotoFacebook;

            this.PanelMenu.AddChild(facebookButton);

#if WINDOWS || SILVERLIGHT
            var appStoreTexture = ScreenManager.Game.Content.Load<Texture2D>("AppStore");

            var appStoreButton = new Button
                                     {
                                         NormalButtonTexture = appStoreTexture,
                                         TextureRectangle =
                                             new Rectangle(0, 0, appStoreTexture.Width, appStoreTexture.Height),
                                         Width = appStoreTexture.Width,
                                         Height = appStoreTexture.Height,
                                         //TextVisible = true,
                                         //TextSize = Button.FontSize.Big,
                                         //TextAlignment = Button.TextAlign.Centre
                                     };

            appStoreButton.Position = new Vector2(GlobalParameters.LEFT_MARGIN, GlobalParameters.SCREEN_HEIGHT - appStoreButton.Height - GlobalParameters.BOTTOM_MARGIN);
            appStoreButton.OnClicked += GotoAppStore;

            PanelMenu.AddChild(appStoreButton);
#endif

//#if DEBUG
//            // For testing
//            var testScrollButton = new Button
//            {
//                DisplayText = "Test scroll screen",
//                TextVisible = true,
//                Font = ScreenManager.SpriteFonts.TittleFont,
//                TextSize = Button.FontSize.Big,
//                TextAlignment = Button.TextAlign.Centre
//            };
//
//            testScrollButton.Foreground = Color.Red;
//            testScrollButton.Position = Vector2.Zero;
//            testScrollButton.Width = testScrollButton.TextWidth;
//            testScrollButton.Height = testScrollButton.TextHeight;
//
//            testScrollButton.OnClicked += TestScrollScreenMenuEntrySelected;
//
//            PanelMenu.AddChild(testScrollButton);
//#endif


            //#if WINDOWS_PHONE || WINDOWS || XBOX
            //            if (Global.SaveDevice.FileExists(Global.ContainerName, Global.FileNameSaveGame))
            //            {
            //                //4. Select Player
            //                var loadGameButton = new Button(ScreenManager.Game)
            //                {
            //                    DisplayText = "Load Game",
            //                    TextVisible = true,
            //                    Font = ScreenManager.SpriteFonts.TittleFont,
            //                    TextSize = Button.FontSize.Big,
            //                    TextAlignment = Button.TextAlign.Centre
            //                };

            //                loadGameButton.Width = loadGameButton.TextureRectangle.Width;
            //                loadGameButton.Height = loadGameButton.TextureRectangle.Height;
            //                loadGameButton.Position = new Vector2(viewPortHalfWith - exitGameButton.Width / 2, exitGameButton.Position.Y + exitGameButton.Height + marginBetweenButtons);

            //                loadGameButton.OnClicked += LoadGameMenuEntrySelected;

            //                PanelMenu.AddChild(loadGameButton);
            //            }
            //#endif
        }

        private void GotoFacebook(Button sender)
        {
            const string facebookUrl = "https://www.facebook.com/acomicofzombies";

            Util.GoToUrl(facebookUrl);
        }

#if WINDOWS || SILVERLIGHT
        private void GotoAppStore(Button sender)
        {
            throw new Exception("url no definida");

            const string url = "https://www.facebook.com/acomicofzombies";

            Util.GoToUrl(url);
        }
#endif

#if DEBUG
        // For testing
        private void TestScrollScreenMenuEntrySelected(Button sender)
        {
            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new BuyWeaponsScreen(SetInitialGameData(1)));
        }

        /// <summary>
        /// Crea un ResumeState con datos de testing
        /// </summary>
        /// <param name="currentGoodGuy"></param>
        /// <returns></returns>
        private ResumeState SetInitialGameData(int currentGoodGuy)
        {
            // TODO: se podria cargar desde un archivo de configuracion
            var defaultPrimaryWeapon = new Weapon(GlobalParameters.PRIMARY_WEAPON_TYPE) { State = WeaponState.Equipped };
            var defaultSecondaryWeapon = new Weapon(GlobalParameters.SECONDARY_WEAPON_TYPE) { State = WeaponState.Equipped };

            return new ResumeState
            {
                NextLife = GlobalParameters.INITIAL_NEXT_LIFE,
                CurrentLevel = GlobalParameters.INITIAL_LEVEL,
                PlayerLives = GlobalParameters.INITIAL_LIVES,
                Score = 1000,
                PrimaryWeapon = defaultPrimaryWeapon,
                SecondaryWeapon = defaultSecondaryWeapon,
                AmmoInventory = new Dictionary<int, int>(GlobalParameters.MAX_WEAPONS_AVAILABLE)
                                               {
                                                   {(int)GlobalParameters.PRIMARY_WEAPON_TYPE, GlobalParameters.PRIMARY_WEAPON_AMMO_COUNT},
                                                   {(int)GlobalParameters.SECONDARY_WEAPON_TYPE, GlobalParameters.SECONDARY_WEAPON_AMMO_COUNT}
                                               },
                WeaponInventory = new List<Weapon>(GlobalParameters.MAX_WEAPONS_AVAILABLE)
                                                 {
                                                     defaultPrimaryWeapon,
                                                     defaultSecondaryWeapon
                                                 },
                SelectedPlayerInfo = PlayerInfoManager.GetGoodGuy(currentGoodGuy)
            };
        }
#endif
        #endregion

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            //_content.Unload();
        }

        #region Click Events
        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        private void SelectPlayerEntrySelected(Button sender)
        {
            //ScreenManager.AddScreen(new SelectPlayerScreen(), PlayerIndex.One);
            LoadingScreen.Load(this.ScreenManager, true, PlayerIndex.One, new GameplayScreen((int)PlayerType.Obama));
        }

#if WINDOWS_PHONE || WINDOWS || XBOX
        /// <summary>
        /// Event handler for when the Continue previus Game menu entry is selected.
        /// </summary>
        private void LoadGameMenuEntrySelected(Button sender)
        {
            throw new NotImplementedException();
            //LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new GameplayScreen(true));
        }
#endif


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        private void OptionsMenuEntrySelected(Button sender)
        {
            this.ScreenManager.AddScreen(new OptionsMenuScreen(), PlayerIndex.One);
        }

#if !SILVERLIGHT
        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(Button sender)
        {
            this.ScreenManager.AddScreen(new TouchMessageBoxScreen(GlobalParameters.QUIT_GAME_MESSAGE, ConfirmExitMessageBoxAccepted), PlayerIndex.One);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        protected virtual void ConfirmExitMessageBoxAccepted(Button sender)
        {
            this.ScreenManager.Game.Exit();
        }
#endif

        #endregion
    }
}

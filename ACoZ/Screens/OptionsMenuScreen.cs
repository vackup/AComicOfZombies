using ACoZ.Helpers;
using Microsoft.Xna.Framework;

#if WINDOWS_PHONE || IPHONE
using Mobile.Base.Controls;
#elif SILVERLIGHT
using Web.Base.Controls;
#elif WINDOWS
using Desktop.Base.Controls;
#endif

namespace ACoZ.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    public class OptionsMenuScreen : ButtonMenuScreen// MenuScreen
    {
        #region Fields

        //readonly MenuEntry _musicMenuEntry;
        //MenuEntry ungulateMenuEntry;
        //MenuEntry languageMenuEntry;
        //MenuEntry frobnicateMenuEntry;
        //MenuEntry elfMenuEntry;

        //enum Ungulate
        //{
        //    BactrianCamel,
        //    Dromedary,
        //    Llama,
        //}

        //static Ungulate currentUngulate = Ungulate.Dromedary;

        //static string[] languages = { "C#", "French", "Deoxyribonucleic acid" };
        //static int currentLanguage = 0;

        //static bool frobnicate = true;
        private bool _playMusic = true;
        private Button _musicButton;

        //static int elf = 23;

        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen() : base(false)
        {
        }

        public override void LoadContent()
        {
            //if (_content == null)
            //    _content = new ContentManager(ScreenManager.Game.Services, GlobalParameters.CONTENT_FOLDER);

            base.LoadContent();

            this._playMusic = this.ScreenManager.IsMusicPlaying;

            //Creates Menu
            this.CreateMenu();
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            this._musicButton.DisplayText = "Music: " + (this._playMusic ? "on" : "off");
        }
        #endregion

        #region Menu
        private void CreateMenu()
        {
            const float firstButtonInitialPosition = 70f;
            const float marginBetweenButtons = 25f;

            float screenWith = this.Viewport.Width;

            var viewPortHalfWith = screenWith / 2;

            //Add MenuItems
            //1. Option
            this._musicButton = new Button
            {
                TextVisible = true,
                Font = this.ScreenManager.SpriteFonts.TittleFont,
                TextSize = Button.FontSize.Big,
                TextAlignment = Button.TextAlign.Centre,
                Foreground = GlobalParameters.HudColor
            };

            // Seteamos el texto del boton xq sino _musicButton.TextWidth y _musicButton.TextHeight es 0
            this.SetMenuEntryText();

            this._musicButton.Width = this._musicButton.TextWidth;
            this._musicButton.Height = this._musicButton.TextHeight;
            this._musicButton.Position = new Vector2(viewPortHalfWith - this._musicButton.Width / 2, firstButtonInitialPosition);
            this._musicButton.OnClicked += MusicMenuEntrySelected;

            this.PanelMenu.AddChild(this._musicButton);

            //2. Back
            var backButton = new Button
            {
                DisplayText = "Back",
                TextVisible = true,
                Font = this.ScreenManager.SpriteFonts.TittleFont,
                TextSize = Button.FontSize.Big,
                TextAlignment = Button.TextAlign.Centre,
                Foreground = GlobalParameters.HudColor
            };

            backButton.Width = backButton.TextWidth;
            backButton.Height = backButton.TextHeight;
            backButton.Position = new Vector2(viewPortHalfWith - backButton.Width / 2, this._musicButton.Position.Y + this._musicButton.Height + marginBetweenButtons);
            backButton.OnClicked += OnCancel;

            this.PanelMenu.AddChild(backButton);


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

            //                loadGameButton.Width = loadGameButton.TextWidth;
            //                loadGameButton.Height = loadGameButton.TextHeight;
            //                loadGameButton.Position = new Vector2(viewPortHalfWith - exitGameButton.Width / 2, exitGameButton.Position.Y + exitGameButton.Height + marginBetweenButtons);

            //                loadGameButton.OnClicked += LoadGameMenuEntrySelected;

            //                PanelMenu.AddChild(loadGameButton);
            //            }
            //#endif
        }
        #endregion

        #region Handle Input

        protected override void OnCancel(Button sender)
        {
            if (this.ScreenManager.IsMusicPlaying && !this._playMusic)
            {
                this.ScreenManager.StopMusic();
            }
            else if (!this.ScreenManager.IsMusicPlaying && this._playMusic)
            {
                this.ScreenManager.PlayMusic();
            }

            base.OnCancel(sender);
        }
        
        /// <summary>
        /// Event handler for when the Music menu entry is selected.
        /// </summary>
        void MusicMenuEntrySelected(Button sender)
        {
            this._playMusic = !this._playMusic;

            this.SetMenuEntryText();
        }
        
//#if WINDOWS_PHONE || WINDOWS ||XBOX
//        protected override void OnCancel(PlayerIndex playerIndex)
//        {

//            // make sure the device is ready
//            if (Global.SaveDevice.IsReady)
//            {
//                // save a file asynchronously. this will trigger IsBusy to return true
//                // for the duration of the save process.
//                Global.SaveDevice.SaveAsync(
//                    Global.ContainerName,
//                    Global.FileNameOptions,
//                    stream =>
//                    {
//                        using (var writer = new StreamWriter(stream))
//                        {
//                            //writer.WriteLine(currentLanguage);
//                            //writer.WriteLine(frobnicate);
//                            //writer.WriteLine(elf);
//                            writer.WriteLine(_playMusic);
//                        }
//                    });
//            }

//            base.OnCancel(playerIndex);
//        }
//#endif
        #endregion

        public override void UnloadContent()
        {
            // Nada que unload
            //throw new NotImplementedException();
        }
    }
}

#region Usings
//#if WINDOWS_PHONE || WINDOWS ||XBOX
//using EasyStorage;
//#endif

using Microsoft.Xna.Framework;
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
    class PressStartScreen : ButtonMenuScreen
    {
        //private ContentManager _content;
//#if WINDOWS_PHONE || WINDOWS ||XBOX
//        IAsyncSaveDevice _saveDevice;
//#endif

        #region Initialization
        public PressStartScreen() : base(false)
        {

        }

        public override void LoadContent()
        {
            //if (_content == null)
            //    _content = new ContentManager(ScreenManager.Game.Services, GlobalParameters.CONTENT_FOLDER);

            base.LoadContent();

            //Creates Menu
            this.CreateMenu();
        }
        #endregion

        #region Menu
        private void CreateMenu()
        {

#if WINDOWS_PHONE || IPHONE
            const string startMenuEntry = "Tap HERE to start";
#else
            var startMenuEntry = "Press A to start";
#endif

            //Add MenuItems
            //1. Press Start
            var pressStartButton = new Button
                                       {
                                           DisplayText = startMenuEntry,
                                           TextVisible = true,
                                           Font = this.ScreenManager.SpriteFonts.TittleFont,
                                           TextSize = Button.FontSize.Big,
                                           TextAlignment = Button.TextAlign.Centre
                                       };

            pressStartButton.Width = pressStartButton.TextWidth;
            pressStartButton.Height = pressStartButton.TextHeight;

            float screenWith = this.Viewport.Width;
			float screenHeight = this.Viewport.Height;
#if IPHONE
            // Al ser el primer screen que carga, todavia no reconocio el giro de pantalla.
			if (ScreenManager.Game.Window.CurrentOrientation == DisplayOrientation.Default)
			{
				screenWith = Viewport.Height;
				screenHeight = Viewport.Width;
			}
#endif
			
			pressStartButton.Position = new Vector2(screenWith/2 - pressStartButton.Width/2,
                                                    screenHeight/2 - pressStartButton.Height/2);

            //Event Handler
            pressStartButton.OnClicked += StartMenuEntrySelected;

            //Add MenuItems to Menupanel
            this.PanelMenu.AddChild(pressStartButton);
        }
        #endregion

        #region Click Events
        private void StartMenuEntrySelected(Button sender)
        {
            this.PromptMe(PlayerIndex.One);
        }

        private void PromptMe(PlayerIndex playerIndex)
        {
            // we can set our supported languages explicitly or we can allow the
            // game to support all the languages. the first language given will
            // be the default if the current language is not one of the supported
            // languages. this only affects the text found in message boxes shown
            // by EasyStorage and does not have any affect on the rest of the game.
//#if WINDOWS_PHONE || WINDOWS ||XBOX
//            EasyStorageSettings.SetSupportedLanguages(Language.French, Language.Spanish);
//#endif

            // on Windows Phone we use a save device that uses IsolatedStorage
            // on Windows and Xbox 360, we use a save device that gets a 
            //shared StorageDevice to handle our file IO.
 // OnlyWP
//#if WINDOWS_PHONE
//            _saveDevice = new IsolatedStorageSaveDevice();
//            Global.SaveDevice = _saveDevice;

//            // we use the tap gesture for input on the phone
//            //EnabledGestures = GestureType.Tap;

//            //Once they select a storage device, we can load the main menu.
//            //You'll notice I hard coded PlayerIndex.One here. You'll need to 
//            //change that if you plan on releasing your game. I linked to an
//            //example on how to do that but here's the link if you need it.
//            //http://blog.nickgravelyn.com/2009/03/basic-handling-of-multiple-controllers/
//            ScreenManager.AddScreen(new MainMenuScreen(), playerIndex);
//#elif WINDOWS || XBOX
//            // create and add our SaveDevice
//            SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
//            ScreenManager.Game.Components.Add(sharedSaveDevice);

//            // make sure we hold on to the device
//            _saveDevice = sharedSaveDevice;

//            // hook two event handlers to force the user to choose a new device if they cancel the
//            // device selector or if they disconnect the storage device after selecting it
//            sharedSaveDevice.DeviceSelectorCanceled +=
//                (s, e) => e.Response = SaveDeviceEventResponse.Force;
//            sharedSaveDevice.DeviceDisconnected +=
//                (s, e) => e.Response = SaveDeviceEventResponse.Force;

//            // prompt for a device on the first Update we can
//            sharedSaveDevice.PromptForDevice();

//            sharedSaveDevice.DeviceSelected += (s, e) =>
//            {
//                //Save our save device to the global counterpart, so we can access it
//                //anywhere we want to save/load
//                Global.SaveDevice = (SaveDevice)s;

//                //Once they select a storage device, we can load the main menu.
//                //You'll notice I hard coded PlayerIndex.One here. You'll need to 
//                //change that if you plan on releasing your game. I linked to an
//                //example on how to do that but here's the link if you need it.
//                //http://blog.nickgravelyn.com/2009/03/basic-handling-of-multiple-controllers/
//                ScreenManager.AddScreen(new MainMenuScreen(), playerIndex);
//            };
//#else
            this.ScreenManager.RemoveScreen(this);
            this.ScreenManager.AddScreen(new MainMenuScreen(), PlayerIndex.One);
            //LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new MainMenuScreen());
//#endif

#if XBOX
            // add the GamerServicesComponent
            ScreenManager.Game.Components.Add(
                new Microsoft.Xna.Framework.GamerServices.GamerServicesComponent(ScreenManager.Game));
#endif
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            //_content.Unload();
        }
        #endregion
    }
}

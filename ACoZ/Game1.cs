using System;
using Microsoft.Xna.Framework;
using Platformer.Helpers;
using Platformer.Screens;
using Platformer.ScreenManagers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Desktop.Base.Components;

namespace ACoZ
{
    public class Game1 : Game
    {
#if WINDOWS || XBOX
        private const int TARGET_FRAME_RATE = 60;
#endif       
        private GraphicsDeviceManager _graphics;
        private ScreenManager _screenManager;	

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            Content.RootDirectory = GlobalParameters.CONTENT_FOLDER;
#if !IPHONE
            _graphics.PreferredBackBufferWidth = GlobalParameters.SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = GlobalParameters.SCREEN_HEIGHT;
#endif
            // Framerate differs between platforms.
#if WINDOWS || XBOX
            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TARGET_FRAME_RATE); 
#elif IPHONE && !FREE
			_graphics.IsFullScreen = true;
			_graphics.SupportedOrientations =  DisplayOrientation.LandscapeRight | DisplayOrientation.LandscapeLeft;
#elif IPHONE && FREE
			_graphics.IsFullScreen = true;
			_graphics.SupportedOrientations =  DisplayOrientation.LandscapeRight;
#endif

            // Create the screen manager component.
            _screenManager = new ScreenManager(this);

            Components.Add(_screenManager);


                // Activate the first screens.
                _screenManager.AddScreen(new BackgroundScreen(), null);                

            // TODO: hacer que se pueda grabar
//#if IPHONE || SILVERLIGHT
                _screenManager.AddScreen(new MainMenuScreen(), null);
//#else    
//            _screenManager.AddScreen(new PressStartScreen(), null); // Cambio para seleccionar el storage para grabar las partidas
//#endif



#if WINDOWS
            this.IsMouseVisible = true;
#endif

            // FPS: Frames per second
            //var fpsComponent = new FramePerSeconds(this) {ShowFps = true};
            //Components.Add(fpsComponent);            
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }


        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}

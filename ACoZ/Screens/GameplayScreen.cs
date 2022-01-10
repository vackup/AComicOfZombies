#region Usings
using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer.Helpers;
using Platformer.Levels;
using Platformer.Players;
using Platformer.ScreenManagers;
using Platformer.Weapons;
#if WINDOWS_PHONE || IPHONE
using Mobile.Base.ScreenSystem;
using Mobile.Base.VirtualInput;
using Microsoft.Xna.Framework.Input.Touch;
#elif SILVERLIGHT
using Web.Base.ScreenSystem;
using Web.Base.VirtualInput;
#elif WINDOWS
using Desktop.Base.ScreenSystem;
using Desktop.Base.VirtualInput;
#endif
using Buttons = Microsoft.Xna.Framework.Input.Buttons;
using GamePad = Microsoft.Xna.Framework.Input.GamePad;
using GamePadState = Microsoft.Xna.Framework.Input.GamePadState;
#endregion

namespace Platformer.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        private readonly int _currentGoodGuy;
        private ResumeState _resumeState;

        #region Fields
        private ContentManager _content;

        // Resources for drawing.
        //private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Level _level;
        private bool _wasContinuePressed;

        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);
        private Rectangle _titleSafeArea;
        private Vector2 _timeLocation;
        private Vector2 _scoreLocation;
        private Vector2 _center;
        
#if !WINDOWS_PHONE && !IPHONE
        private const Buttons CONTINUE_BUTTON = Buttons.A;
#endif

        // We store our input states so that we only poll once per frame, 
        // then we use the same input state wherever needed
        private GamePadState _gamePadState;
        private KeyboardState _keyboardState;
#if WINDOWS_PHONE || IPHONE
        private TouchCollection _touchState;
#endif
        private MouseState _mouseState;
        
        //private readonly bool _continuePreviusGame;
        //private List<GestureSample> _gestureSamples = new List<GestureSample>();
        private VirtualGamePadState _virtualGamePadState;
        private VirtualGamePadState _previusVirtualGamePadState;

#if WINDOWS_PHONE || IPHONE || WINDOWS
        //private Texture2D ScreenManager.ScreenAssetsTexture;
        private VirtualGamePad _virtualGamePad;
        private Vector2 _scoreSize;
        //private PanelControl _hudPanel;
        private Vector2 _remainigBulletsIconPosition;
        //private float _bulletTextWidth;
        private decimal _previusAviableBullets;
        private decimal _previusCurrentAmmo;
        private string _currentAmmmoText;
        private string _aviableBulletText;
        private Rectangle _remainigBulletsIconTextureRectangule;
        private decimal _previusScore;
        private string _scoreText;
        private bool _isPauseGame = false;
        private Rectangle _levelClearTextureRectangle;
        private Texture2D _backgroundTexture;
        private Rectangle _backgroundRectangle;
        private string _loseOverlayText;
        private Vector2 _loseOverlayPosition;
        private string _runOutTimeOverlayText;
        private Vector2 _runOutTimeOverlayPosition;
        private string _tapToContinueText;
        private Keys CONTINUE_KEY = Keys.Space;
        private Vector2 _tapToContinueTextSize;
        private Vector2 _loseOverlayTextSize;
        private Vector2 _runOutTimeOverlayTextSize;
        private Vector2 _levelClearPosition;
        private const string SCORE_TEXT = "COINS";
#endif

        // The number of levels in the Levels directory of our content. We assume that
        // levels in our content are 0-based and that all numbers under this constant
        // have a level file present. This allows us to not need to check for the file
        // or handle exceptions, both of which can add unnecessary time to level loading.
        //private const int NUMBER_OF_LEVELS = 6;

        
        #endregion

        #region Constructors
        public GameplayScreen(int currentGoodGuy)
        {
            _currentGoodGuy = currentGoodGuy;
            Init();
        }

        public GameplayScreen(ResumeState resumeState)
        {
            _resumeState = resumeState;
            Init();
        }

        //public GameplayScreen(bool continuePreviusGame)
        //{
        //    Init();

        //    _continuePreviusGame = continuePreviusGame;
        //}

        private void Init()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Carga los parametros iniciales para el player para iniciar el juego (nivel inicial, vidas, armamento, municiones, etc)
        /// </summary>
        /// <returns></returns>
        private ResumeState SetInitialGameData(int currentGoodGuy)
        {
            var defaultPrimaryWeapon = new Weapon(GlobalParameters.PRIMARY_WEAPON_TYPE) { State = WeaponState.Equipped };
#if DEBUG
            var defaultSecondaryWeapon = new Weapon(GlobalParameters.SECONDARY_WEAPON_TYPE) { State = WeaponState.Equipped };
#endif

            return new ResumeState
            {
                NextLife = GlobalParameters.INITIAL_NEXT_LIFE,
                CurrentLevel = GlobalParameters.INITIAL_LEVEL,
                PlayerLives = GlobalParameters.INITIAL_LIVES,
                Score = GlobalParameters.INITIAL_SCORE,
                PrimaryWeapon = defaultPrimaryWeapon,
#if DEBUG
                SecondaryWeapon = defaultSecondaryWeapon,
#endif
                AmmoInventory = new Dictionary<int, int>(GlobalParameters.MAX_WEAPONS_AVAILABLE)
                                               {
                                                   {(int)GlobalParameters.PRIMARY_WEAPON_TYPE, GlobalParameters.PRIMARY_WEAPON_AMMO_COUNT},
#if DEBUG
                                                   {(int)GlobalParameters.SECONDARY_WEAPON_TYPE, GlobalParameters.SECONDARY_WEAPON_AMMO_COUNT}
#endif
                                               },
                WeaponInventory = new List<Weapon>(GlobalParameters.MAX_WEAPONS_AVAILABLE)
                                                 {
                                                     defaultPrimaryWeapon,
#if DEBUG
                                                     defaultSecondaryWeapon
#endif
                                                 },
                SelectedPlayerInfo = PlayerInfoManager.GetGoodGuy(currentGoodGuy)
            };
        }
        #endregion

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public override void LoadContent()
        {
            if (_content == null)
            {
                // Probando de usar el ContentManager compartido
                //_content = new ContentManager(ScreenManager.Game.Services, GlobalParameters.CONTENT_FOLDER);
                _content = ScreenManager.Game.Content;
            }

            _titleSafeArea = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea;

            _spriteBatch = ScreenManager.SpriteBatch;

            
            _scoreSize = ScreenManager.SpriteFonts.TittleFontSmall.MeasureString(SCORE_TEXT);
            
            var timeSizeWidth = ScreenManager.SpriteFonts.TittleFontSmall.MeasureString("00:00").X;

#if WINDOWS_PHONE || IPHONE
            _scoreLocation = new Vector2(GlobalParameters.LEFT_MARGIN + GlobalParameters.ScreenAssetsData["PANTALLA_CONTROLES0001_psd-Pause"].Width + GlobalParameters.LEFT_MARGIN, GlobalParameters.TOP_MARGIN + GlobalParameters.BANNER_MARGIN);
#else
            _scoreLocation = new Vector2(GlobalParameters.LEFT_MARGIN, GlobalParameters.TOP_MARGIN);
#endif
            _timeLocation = new Vector2(GlobalParameters.SCREEN_WIDTH - GlobalParameters.RIGHT_MARGIN - timeSizeWidth, GlobalParameters.TOP_MARGIN + GlobalParameters.BANNER_MARGIN);
            
#if WINDOWS_PHONE || IPHONE //|| WINDOWS
            CreateVirtualGamepad();
#endif

            _center = new Vector2(_titleSafeArea.X + _titleSafeArea.Width / 2.0f,
                                         _titleSafeArea.Y + _titleSafeArea.Height / 2.0f);

            //if (_continuePreviusGame)
            //{
            //    LoadSavedLevelData();
            //}
            //else
            //{
                if (_resumeState == null)
                {
                    _resumeState = SetInitialGameData(_currentGoodGuy);
                    //throw new Exception("_resumeState viene sin valores");
                }

                _resumeState.LoadContent(_content);

                //_levelIndex = _resumeState.CurrentLevel;
                LoadNextLevel(_resumeState.CurrentLevel, _resumeState.PlayerLives, _resumeState.Score, _resumeState.NextLife, _resumeState.WeaponInventory, _resumeState.PrimaryWeapon, _resumeState.SecondaryWeapon, _resumeState.SelectedPlayerInfo, _resumeState.AmmoInventory);
            //}

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            //ScreenManager.Game.ResetElapsedTime();

            // Seteamos los valores del HUD (optimizacion para generar menos garbage)
            _previusCurrentAmmo = _level.Player.CurrentWeapon.CurrentAmmo;
            _currentAmmmoText = _previusCurrentAmmo.ToString(CultureInfo.InvariantCulture);

            _previusAviableBullets = _level.Player.GetCurrentWeaponAvailableAmmo();
            _aviableBulletText = _previusAviableBullets.ToString(CultureInfo.InvariantCulture);

            _previusScore = _level.Score;
            _scoreText = _previusScore.ToString(CultureInfo.InvariantCulture);
            // Fin: Seteamos los valores del HUD (optimizacion para generar menos garbage)

            _levelClearTextureRectangle =
                        GlobalParameters.ScreenAssetsData["PANTALLA_LEVEL0001_psd-LEVEL_CLEAR"];

            _levelClearPosition = _center - new Vector2(_levelClearTextureRectangle.Width, _levelClearTextureRectangle.Height) / 2;

            _remainigBulletsIconTextureRectangule = GlobalParameters.ScreenAssetsData["PANTALLA_CONTROLES0001_psd-RESTO_BALAS"];
            _remainigBulletsIconPosition =
                new Vector2(
                    (ScreenManager.GraphicsDevice.Viewport.Width / 2) - (_remainigBulletsIconTextureRectangule.Width/2),
                    GlobalParameters.TOP_MARGIN/2 + GlobalParameters.BANNER_MARGIN);

            _backgroundTexture = ScreenManager.Game.Content.Load<Texture2D>(GlobalParameters.WEAPON_BUTTON_NORMAL);

            var backGroundHeight = _levelClearTextureRectangle.Height + GlobalParameters.TOP_MARGIN*2 + GlobalParameters.BANNER_MARGIN;
            var backGroundYPosition = _center.Y - (backGroundHeight / 2);

            _backgroundRectangle = new Rectangle(0, (int)backGroundYPosition, GlobalParameters.SCREEN_WIDTH, backGroundHeight);

            _loseOverlayText = "YOU DIE";
            _loseOverlayTextSize = ScreenManager.SpriteFonts.TittleFont.MeasureString(_loseOverlayText);
            _loseOverlayPosition = _center - _loseOverlayTextSize / 2;

            _runOutTimeOverlayText = "RUN OUT OF TIME";
            _runOutTimeOverlayTextSize = ScreenManager.SpriteFonts.TittleFont.MeasureString(_runOutTimeOverlayText);
            _runOutTimeOverlayPosition = _center - _runOutTimeOverlayTextSize / 2;

#if WINDOWS || SILVERLIGHT
            _tapToContinueText = "Press " + CONTINUE_KEY.ToString() + " to continue";
#else
            _tapToContinueText = "Tap to continue";
#endif

            _tapToContinueTextSize = ScreenManager.SpriteFonts.TextFont.MeasureString(_tapToContinueText);

            // Hacemos una recoleccion de basura luego de cargar todos los elementos
            GC.Collect();
        }

        //private void CreateHud()
        //{
        //    _hudPanel = new PanelControl
        //                    {
        //                        Size =
        //                            new Vector2(ScreenManager.GraphicsDevice.Viewport.Width,
        //                                        GlobalParameters.GAME_PLAY_SCREEN_HUD_Y)
        //                        // Ancho = Ancho de pantalla. Alto = position Y del menu de armas);
        //                    };


        //    _remainigBulletsIconTextureRectangule = GlobalParameters.ScreenAssetsData["PANTALLA_CONTROLES0001_psd-RESTO_BALAS"];

        //    _remainigBulletsIconPosition = new Vector2(
        //        (_hudPanel.Size.X/2) - (_remainigBulletsIconTextureRectangule.Width/2),
        //        (_hudPanel.Size.Y/2) - (_remainigBulletsIconTextureRectangule.Height/2));

        //    var remainigBulletsIcon = new ImageControl(ScreenManager.ScreenAssetsTexture,
        //                                               _remainigBulletsIconTextureRectangule,
        //                                               _remainigBulletsIconPosition);

        //    _hudPanel.AddChild(remainigBulletsIcon);
        //}

#if WINDOWS_PHONE || IPHONE //|| WINDOWS
        private void CreateVirtualGamepad()
        {
            

            _virtualGamePad = new VirtualGamePad();
            
            // Set the virtual GamePad
            var pauseGameVirtualButton = new VirtualButtonDefinition
                                             {
                                                 Texture = ScreenManager.ScreenAssetsTexture,
                                                 TextureRect =
                                                     GlobalParameters.ScreenAssetsData["PANTALLA_CONTROLES0001_psd-Pause"],
                                                 Type = VirtualButtons.Start,
                                                 Position = new Vector2(GlobalParameters.LEFT_MARGIN, GlobalParameters.TOP_MARGIN + GlobalParameters.BANNER_MARGIN),
                                             };
            

            var reloadGunVirtualButton = new VirtualButtonDefinition
                                             {
                                                 Texture = ScreenManager.ScreenAssetsTexture,
                                                 TextureRect = GlobalParameters.ScreenAssetsData["PANTALLA_CONTROLES0001_psd-BALAS"],
                                                 Type = VirtualButtons.LeftShoulder,
                                             };
            reloadGunVirtualButton.Position =
                new Vector2(_titleSafeArea.Width - reloadGunVirtualButton.TextureRect.Width - GlobalParameters.RIGHT_MARGIN,
                            _titleSafeArea.Height - reloadGunVirtualButton.TextureRect.Height - GlobalParameters.BOTTOM_MARGIN);

            var changeGunVirtualButton = new VirtualButtonDefinition
            {
                Texture = ScreenManager.ScreenAssetsTexture,
                TextureRect = GlobalParameters.ScreenAssetsData["PANTALLA_CONTROLES0001_psd-INTER_ARMAS"],
                Type = VirtualButtons.RightShoulder,
            };
            //changeGunVirtualButton.Position =
            //    new Vector2(_titleSafeArea.Width - changeGunVirtualButton.TextureRect.Width - GlobalParameters.RIGHT_MARGIN,
            //                GlobalParameters.TOP_MARGIN);
            changeGunVirtualButton.Position =
                new Vector2(_titleSafeArea.Width - changeGunVirtualButton.TextureRect.Width - GlobalParameters.RIGHT_MARGIN,
                            _titleSafeArea.Height - GlobalParameters.BOTTOM_MARGIN - reloadGunVirtualButton.TextureRect.Height - GlobalParameters.BOTTOM_MARGIN - changeGunVirtualButton.TextureRect.Height);
            

            var shootVirtualButton = new VirtualButtonDefinition
                                         {
                                             Texture = ScreenManager.ScreenAssetsTexture,
                                             TextureRect = GlobalParameters.ScreenAssetsData["PANTALLA_CONTROLES0001_psd-REVOLVER"],
                                             Type = VirtualButtons.B,
                                         };
            shootVirtualButton.Position =
                new Vector2(reloadGunVirtualButton.Position.X - shootVirtualButton.TextureRect.Width - GlobalParameters.RIGHT_MARGIN,
                            _titleSafeArea.Height - shootVirtualButton.TextureRect.Height - GlobalParameters.BOTTOM_MARGIN);

            var meleeVirtualButton = new VirtualButtonDefinition
                                         {
                                             Texture = ScreenManager.ScreenAssetsTexture,
                                             TextureRect = GlobalParameters.ScreenAssetsData["PANTALLA_CONTROLES0001_psd-CUCHILLO"],
                                             Type = VirtualButtons.A,
                                         };
            meleeVirtualButton.Position =
                new Vector2(shootVirtualButton.Position.X - meleeVirtualButton.TextureRect.Width - GlobalParameters.RIGHT_MARGIN,
                            _titleSafeArea.Height - meleeVirtualButton.TextureRect.Height - GlobalParameters.BOTTOM_MARGIN);

            var leftVirtualButton = new VirtualButtonDefinition
                                        {
                                            Texture = ScreenManager.ScreenAssetsTexture,
                                            TextureRect = GlobalParameters.ScreenAssetsData["PANTALLA_CONTROLES0001_psd-IZQ"],
                                            Type = VirtualButtons.DPadLeft,
                                        };
            leftVirtualButton.Position = new Vector2(GlobalParameters.LEFT_MARGIN,
                                                     _titleSafeArea.Height - leftVirtualButton.TextureRect.Height -
                                                     GlobalParameters.BOTTOM_MARGIN);
            
            var rightVirtualButton = new VirtualButtonDefinition
                                         {
                                             Texture = ScreenManager.ScreenAssetsTexture,
                                             TextureRect = GlobalParameters.ScreenAssetsData["PANTALLA_CONTROLES0001_psd-DER"],
                                             Type = VirtualButtons.DPadRight,
                                         };
            rightVirtualButton.Position =
                new Vector2(leftVirtualButton.Position.X + leftVirtualButton.TextureRect.Width + GlobalParameters.LEFT_MARGIN,
                            _titleSafeArea.Height - rightVirtualButton.TextureRect.Height - GlobalParameters.BOTTOM_MARGIN);

            _virtualGamePad.ButtonsDefinitions.Add(pauseGameVirtualButton);
            _virtualGamePad.ButtonsDefinitions.Add(changeGunVirtualButton);
            _virtualGamePad.ButtonsDefinitions.Add(reloadGunVirtualButton);
            _virtualGamePad.ButtonsDefinitions.Add(shootVirtualButton);
            _virtualGamePad.ButtonsDefinitions.Add(meleeVirtualButton);
            _virtualGamePad.ButtonsDefinitions.Add(rightVirtualButton);
            _virtualGamePad.ButtonsDefinitions.Add(leftVirtualButton);
        }
#endif
        

        private void LoadSavedLevelData()
        {
            throw new NotImplementedException("Este metodo esta comentado apra testing");
            //// Unloads the content for the current level before loading the next one.
            //if (_level != null)
            //    _level.Dispose();

            //// Load the level.
            //_level = new Level(ScreenManager);

            //_levelIndex = _level.CurrentLevel;
        }

        private void LoadNextLevel(int levelIndex, int currentLives, int totalScore, int nextLife, List<Weapon> weaponInventory, Weapon primaryWeapon, Weapon secondaryWeapon, PlayerInfo playerInfo, Dictionary<int, int> ammoInventory)
        {
            ++levelIndex;

            // Unloads the content for the current level before loading the next one.
            //if (_level != null)
            //    _level.Dispose();

            // Load the level.
            _level = new Level(_content, levelIndex, totalScore, currentLives, nextLife, weaponInventory, primaryWeapon, secondaryWeapon, playerInfo, ammoInventory);
        }

        //private void LoadNextLevel(int currentLives, int totalScore, int nextLife)
        //{
        //    LoadNextLevel(currentLives, totalScore, nextLife,
        //                  new List<Weapon> {GlobalParameters.DefaultPrimaryWeapon, GlobalParameters.DefaultSecondaryWeapon},
        //                  GlobalParameters.DefaultPrimaryWeapon, GlobalParameters.DefaultSecondaryWeapon, GlobalParameters.DefaultPlayerInfo, GlobalParameters.DefaultAmmoInventory);
        //}

        //private void ReloadCurrentLevel(int currentLives)
        //{
        //    --_levelIndex;
        //    --currentLives;
        //    LoadNextLevel(currentLives, _level.Score, _level.NextLife);
        //}

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            //_content.Unload();
        }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (IsActive)
            {
                if (_level.LevelLoaded)
                {
                    // update our level, passing down the GameTime along with all of our input states
                    _level.Update(gameTime, _keyboardState, _gamePadState, _mouseState, _virtualGamePadState);
                }
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputHelper input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            //_pnlMenu.HandleInput(input);
            //rightCommandButton.HandleInput(input);

            // Look up inputs for the active player profile.
            //int playerIndex = (int)ControllingPlayer.Value;

            //_keyboardState = input.CurrentKeyboardStates[playerIndex];
            //_gamePadState = input.CurrentGamePadStates[playerIndex];

            // get all of our input states
#if WINDOWS_PHONE || IPHONE
            _virtualGamePadState = _virtualGamePad.GetState(PlayerIndex.One);
            _touchState = input.TouchState;

            _isPauseGame = (_virtualGamePadState.Buttons.Start == VirtualButtonState.Pressed &&
                            _previusVirtualGamePadState.Buttons.Start == VirtualButtonState.Released);
#else
            _keyboardState = Keyboard.GetState();
            _gamePadState = GamePad.GetState(PlayerIndex.One);
			_mouseState = Mouse.GetState();
#endif

            //_accelerometerState = Accelerometer.GetState();

            
            //_gestureSamples = input.Gestures;

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            //bool gamePadDisconnected = !_gamePadState.IsConnected &&
            //                           input.GamePadWasConnected[playerIndex];

            // Exit the game when back is pressed.
            //if (_gamePadState.Buttons.Back == ButtonState.Pressed)
            //    Exit();



            if (input.IsPauseGame(ControllingPlayer)  || _isPauseGame)// || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(_level), ControllingPlayer);
            }
            else
            {
#if WINDOWS_PHONE || IPHONE
                var continuePressed = _touchState.AnyTouch() || _virtualGamePadState.Buttons.A == VirtualButtonState.Pressed;
#else
                var continuePressed =
                    _keyboardState.IsKeyDown(CONTINUE_KEY) ||
                    _gamePadState.IsButtonDown(CONTINUE_BUTTON) ||
                    _virtualGamePadState.Buttons.A == VirtualButtonState.Pressed;
#endif

                // Perform the appropriate action to advance the game and
                // to get the player back to playing.
                if (!_wasContinuePressed && continuePressed)
                {
                    if (!_level.Player.IsAlive)
                    {
                        GameOver();
                    }
                    else if (_level.TimeRemaining == TimeSpan.Zero)
                    {
                        if (_level.ReachedExit)
                        {
                            var resumeState = new ResumeState
                                                  {
                                                      NextLife = _level.NextLife,
                                                      CurrentLevel = _level.CurrentLevel,
                                                      Score = _level.Score,
                                                      PlayerLives = _level.Player.CurrentHealth,
                                                      WeaponInventory = _level.Player.WeaponInventory,
                                                      SecondaryWeapon = _level.Player.SecondaryWeapon,
                                                      PrimaryWeapon = _level.Player.PrimaryWeapon,
                                                      SelectedPlayerInfo = _level.Player.Type,
                                                      AmmoInventory = _level.Player.AmmoInventory
                                                  };

                            LoadingScreen.Load(ScreenManager, false, null, new BuyWeaponsScreen(resumeState));
                        }
                        else
                        {
                            GameOver();
                        }
                    }
                }

                _wasContinuePressed = continuePressed;
            }

#if WINDOWS_PHONE || IPHONE
            _previusVirtualGamePadState = _virtualGamePadState;
#endif
        }

        /// <summary>
        /// Game Over! Put finishing screen stuff here
        /// </summary>
        private void GameOver()
        {
            // Game Over! Put finishing screen stuff here
            /////////////////////GO TO MAIN MENU///////////////////
            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new BackgroundScreen(), new MainMenuScreen());
        }

        /// <summary>
        /// Draws the game from background to foreground.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Background color
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            if (_level.LevelLoaded)
            {
                // Dibujamos el nivel
                _level.Draw(gameTime, _spriteBatch);

                _spriteBatch.Begin();
                // Dibujamos el HUD
                DrawHud(_spriteBatch);

#if WINDOWS_PHONE || IPHONE //|| WINDOWS
                // Dibujamos el VirtualGamePad
                _virtualGamePad.Draw(gameTime, _spriteBatch);
#endif

                _spriteBatch.End();
            }

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }

        /// <summary>
        /// Dibuja el HUD: Head-Up Display - "http://es.wikipedia.org/wiki/HUD_(inform%C3%A1tica)"
        /// </summary>
        private void DrawHud(SpriteBatch spriteBatch)
        {
            if (!_level.LevelLoaded) return;

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            // Draw time
            TextHelper.DrawShadowedString(spriteBatch, ScreenManager.SpriteFonts.TittleFontSmall,
                               string.Format("{0}:{1}", _level.TimeRemaining.Minutes.ToString("00"), _level.TimeRemaining.Seconds.ToString("00")), _timeLocation,
                               (_level.TimeRemaining > WarningTime || _level.ReachedExit || (int)_level.TimeRemaining.TotalSeconds % 2 == 0)
                                   ? GlobalParameters.HudColor
                                   : Color.Yellow);

            //// Draw Player's current Weapon
            //DrawShadowedString(ScreenManager.SpriteFonts.TextFont,
            //                   _level.Player.CurrentAction == Actions.Gun
            //                       ? string.Format("WEAPON: {0} {1}/{2}", _level.Player.CurrentWeapon.Name, _level.Player.CurrentWeapon.CurrentAmmo, _level.Player.CurrentWeapon.MaxAmmo)
            //                       : string.Format("WEAPON: {0}", _level.Player.CurrentAction),
            //                   _currentGunLocation, Color.Yellow);

            //DrawShadowedString(ScreenManager.SpriteFonts.TextFont,
            //                   string.Format("Ammo {0}", _level.Player.GetCurrentWeaponAvailableAmmo()),
            //                   _hudLocation + new Vector2(0.0f, _timeHeight * 2.4f), Color.Yellow);


            //DrawShadowedString(ScreenManager.SpriteFonts.TextFont, string.Format("Memory used: {0}", GC.GetTotalMemory(false)/1024),
            //                   _hudLocation + new Vector2(0.0f, _timeHeight * 3.6f), Color.Yellow);

#if DEBUG
            spriteBatch.DrawInt32(ScreenManager.SpriteFonts.TextFont, (int) (GC.GetTotalMemory(false) / 1024), Vector2.Zero, Color.Yellow);
#endif

            if (_previusScore != _level.Score)
            {
                _previusScore = _level.Score;
                _scoreText = _level.Score.ToString(CultureInfo.InvariantCulture);
            }

            TextHelper.DrawShadowedString(spriteBatch, ScreenManager.SpriteFonts.TittleFontSmall, string.Format("{0} {1}", SCORE_TEXT, _scoreText), _scoreLocation, GlobalParameters.HudColor);

            //_spriteBatch.DrawInt32(ScreenManager.SpriteFonts.TittleFontSmall, _level.Score,
            //                       _scoreLocation + new Vector2(_scoreSize.X + GlobalParameters.RIGHT_MARGIN, 0.0f), GlobalParameters.HudColor);

            //Control.BatchDraw(_hudPanel, ScreenManager.GraphicsDevice, spriteBatch, Vector2.Zero, gameTime, true);

            spriteBatch.Draw(ScreenManager.ScreenAssetsTexture, _remainigBulletsIconPosition, _remainigBulletsIconTextureRectangule, Color.White);

            // Dibujamos cuantas balas le quedan al player en el arma
            //_spriteBatch.DrawInt32(ScreenManager.SpriteFonts.TittleFontSmall, _level.Player.CurrentWeapon.CurrentAmmo,
            //                       _remainigBulletsIconPosition +
            //                       new Vector2(GlobalParameters.ScreenAssetsData["PANTALLA_CONTROLES0001_psd-RESTO_BALAS"].Width,
            //                                   0.0f), GlobalParameters.HudColor);

            //// Dibujamos cuantas balas le quedan al player para cargar en el arma
            //_spriteBatch.DrawInt32(ScreenManager.SpriteFonts.TittleFontSmall,
            //                       _level.Player.GetCurrentWeaponAvailableAmmo(),
            //                       _remainigBulletsIconPosition +
            //                       new Vector2(GlobalParameters.ScreenAssetsData["PANTALLA_CONTROLES0001_psd-RESTO_BALAS"].Width + _bulletTextWidth,
            //                                   0.0f), GlobalParameters.HudColor);

            if (_previusCurrentAmmo != _level.Player.CurrentWeapon.CurrentAmmo)
            {
                _previusCurrentAmmo = _level.Player.CurrentWeapon.CurrentAmmo;
                _currentAmmmoText = _level.Player.CurrentWeapon.CurrentAmmo.ToString(CultureInfo.InvariantCulture);
            }
            
            if (_previusAviableBullets != _level.Player.GetCurrentWeaponAvailableAmmo())
            {
                _previusAviableBullets = _level.Player.GetCurrentWeaponAvailableAmmo();
                _aviableBulletText = _level.Player.GetCurrentWeaponAvailableAmmo().ToString(CultureInfo.InvariantCulture);
            }

            // Dibujamos cuantas balas le quedan al player en el arma / cuantas balas le quedan al player para cargar en el arma
            TextHelper.DrawShadowedString(spriteBatch, ScreenManager.SpriteFonts.TittleFontSmall,
                               string.Format("X.{0}/{1}", _currentAmmmoText, _aviableBulletText),
                               new Vector2(_remainigBulletsIconPosition.X + _remainigBulletsIconTextureRectangule.Width,
                                   _remainigBulletsIconPosition.Y + (_remainigBulletsIconTextureRectangule.Height / 2) - (_scoreSize.Y / 2)),
                               GlobalParameters.HudColor);

            // Determine the status overlay message to show.
            if (_level.TimeRemaining == TimeSpan.Zero)
            {
                // Draw the background rectangle.
                spriteBatch.Draw(_backgroundTexture, _backgroundRectangle, Color.White);

                if (_level.ReachedExit)
                {
                    spriteBatch.Draw(ScreenManager.ScreenAssetsTexture, _levelClearPosition, _levelClearTextureRectangle, Color.White);

                    TextHelper.DrawShadowedString(spriteBatch, ScreenManager.SpriteFonts.TextFont, _tapToContinueText,
                                                  new Vector2(_center.X - _tapToContinueTextSize.X/2,
                                                              _levelClearPosition.Y +
                                                              _levelClearTextureRectangle.Height), Color.White);
                }
                else
                {
                    //spriteBatch.Draw(_loseOverlay, _center - new Vector2(_loseOverlay.Width, _loseOverlay.Height) / 2, Color.White);
                    TextHelper.DrawShadowedString(spriteBatch, ScreenManager.SpriteFonts.TittleFont, _runOutTimeOverlayText, _runOutTimeOverlayPosition, GlobalParameters.HudColor);

                    TextHelper.DrawShadowedString(spriteBatch, ScreenManager.SpriteFonts.TextFont, _tapToContinueText,
                                                  new Vector2(_center.X - _tapToContinueTextSize.X/2,
                                                              _runOutTimeOverlayPosition.Y +
                                                              _runOutTimeOverlayTextSize.Y), Color.White);
                }
            }
            else if (!_level.Player.IsAlive)
            {
                //var status = _level.Player.CurrentHealth > 0 ? _diedOverlay : _loseOverlay;
                //spriteBatch.Draw(status, _center - new Vector2(status.Width, status.Height) / 2, Color.White);

                // Draw the background rectangle.
                spriteBatch.Draw(_backgroundTexture, _backgroundRectangle, Color.White);

                TextHelper.DrawShadowedString(spriteBatch, ScreenManager.SpriteFonts.TittleFont, _loseOverlayText, _loseOverlayPosition, GlobalParameters.HudColor);

                TextHelper.DrawShadowedString(spriteBatch, ScreenManager.SpriteFonts.TextFont, _tapToContinueText,
                                                  new Vector2(_center.X - _tapToContinueTextSize.X / 2,
                                                              _loseOverlayPosition.Y +
                                                              _loseOverlayTextSize.Y), Color.White);
            }
        }
    }
}

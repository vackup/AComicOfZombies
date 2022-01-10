#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer.Helpers;
using Platformer.ScreenManagers;
// TODO: si hay que optimizar, revisar el uso de este LINQ
using System.Linq;
using Platformer.Weapons;
#if WINDOWS_PHONE || IPHONE
using Mobile.Base.Controls;
using Mobile.Base.ScreenSystem;
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
    public class BuyWeaponsScreen : GameScreen
    {
        #region Properties & Variables
        private Texture2D _weaponButtonNormalTexture;
        private PanelControl _panelWeaponMenuBoarder;
        private ScrollingPanelControl _scrollingWeaponsMenu;
        private readonly ResumeState _resumeStateData;
        private TextControl _primaryWeaponTextControl;
        private TextControl _secondaryWeaponTextControl;
        private PanelControl _panelMenuHeader;
        private Viewport _menuViewport;
        private TextControl _headerTitleTextControl;
        private ContentManager _content;
        private PanelControl _panelMenuFooter;
        private PanelControl _panelDescriptionMenu;
        private Button _weaponActionButton;
        private TextControl _weaponDescriptionTextControl;
        private ImageControl _weaponImageControl;
        private TextControl _scoreTextControl;
        #endregion

        #region Initialization and Load

        public BuyWeaponsScreen(ResumeState resumeState)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _resumeStateData = resumeState;

            // Recargamos las armas que el player estaba usando
            ReloadWeapons();
        }

        public override void LoadContent()
        {
            if (_content == null)
            {
                // Probando de usar el ContentManager compartido
                //_content = new ContentManager(ScreenManager.Game.Services, GlobalParameters.CONTENT_FOLDER);
                _content = ScreenManager.Game.Content;
            }

            _menuViewport = new Viewport
            {
#if WINDOWS_PHONE || WINDOWS ||XBOX
                MinDepth = 0,
                MaxDepth = 1,
#endif
                X = GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_MENU_X,
                Y = GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_MENU_Y,
                Width = GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_MENU_WIDTH,
                Height = GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_MENU_HEIGHT,
            };

            _weaponButtonNormalTexture = _content.Load<Texture2D>(GlobalParameters.WEAPON_BUTTON_NORMAL);

            //UpdateScreen();

            CreateMenuHeader();

            var viewRect = new Rectangle(_menuViewport.X,
                                         _menuViewport.Y,
                                         _menuViewport.Width,
                                         _menuViewport.Height);

            CreateWeaponsListMenu(viewRect);

            CreateWeaponsDescriptionMenu();

            CreateMenuFooter();

            base.LoadContent();
        }

        //private void UpdateScreen()
        //{
        //    var viewport = ScreenManager.GraphicsDevice.Viewport;
        //    _viewport = new Rectangle(0, 0, viewport.Width, viewport.Height);
        //}

        #endregion

        #region Input Handling

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public override void HandleInput(InputHelper input)
        {
            _panelMenuHeader.HandleInput(input);
            _panelWeaponMenuBoarder.HandleInput(input);
            _scrollingWeaponsMenu.HandleInput(input);
            _panelDescriptionMenu.HandleInput(input);
            _panelMenuFooter.HandleInput(input);
            base.HandleInput(input);
        }
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
            _panelMenuHeader.Update(gameTime);
            _panelWeaponMenuBoarder.Update(gameTime);
            _scrollingWeaponsMenu.Update(gameTime);
            _panelDescriptionMenu.Update(gameTime);
            _panelMenuFooter.Update(gameTime);
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        #endregion

        #region Game Methods - Draw
        public override void Draw(GameTime gameTime)
        {
            if (!IsActive) return;

            //Draw Background
            DrawBackground();

            // Draw Menu Header
            DrawMenuHeader(gameTime);
            
            // Draw Menu
            DrawWeaponsMenu(gameTime);

            DrawMenuFooter(gameTime);
        }

        private void DrawMenuHeader(GameTime gameTime)
        {
            Control.BatchDraw(_panelMenuHeader, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
        }

        private void DrawMenuFooter(GameTime gameTime)
        {
            Control.BatchDraw(_panelMenuFooter, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
        }

        private void DrawBackground()
        {
            ScreenManager.GraphicsDevice.Clear(GlobalParameters.AlternativeColor);
            //ScreenManager.SpriteBatch.Begin();
            //ScreenManager.SpriteBatch.Draw(_background, _viewport, Color.White);
            //ScreenManager.SpriteBatch.End();
        }

        private void DrawWeaponsMenu(GameTime gameTime)
        {
            // TODO: como silverlight no soporta varios viewports, hay que rediseñar esta pantalla. Se dejo solo para que compile.

            // Set our viewport. We store the old viewport so we can restore it when we're done in case
            // we want to render to the full viewport at some point.
#if !SILVERLIGHT
            var oldViewport = ScreenManager.GraphicsDevice.Viewport;
            ScreenManager.GraphicsDevice.Viewport = _menuViewport;
#endif
            //Draw Weapon Selector list
            Control.BatchDraw(_scrollingWeaponsMenu, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch,  Vector2.Zero, gameTime);

            // Now that we're done, set our old viewport back on the device
#if !SILVERLIGHT
            ScreenManager.GraphicsDevice.Viewport = oldViewport;
#endif

            Control.BatchDraw(_panelWeaponMenuBoarder, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch, Vector2.Zero, gameTime);

            Control.BatchDraw(_panelDescriptionMenu, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
        }

        #endregion

        #region Menu
        private void CreateMenuHeader()
        {
#if FREE
			var headerFont =  ScreenManager.SpriteFonts.TittleFontSmall;
#else
			var headerFont =  ScreenManager.SpriteFonts.TittleFont;
#endif

            //Initialize Text MenuPanel
            _panelMenuHeader = new PanelControl
                                   {
										Size = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, 
				                   							GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_MENU_Y - GlobalParameters.BANNER_MARGIN), // Ancho = Ancho de pantalla. Alto = position Y del menu de armas
										Position = new Vector2(0, GlobalParameters.BANNER_MARGIN)
                                   };
			       
            var halfHeightMenuHeader = _panelMenuHeader.Size.Y / 2;
            
            // Creamos los textos que iran en el panel
            _headerTitleTextControl = new TextControl(GetWeaponTextControlHeaderText(), headerFont, Color.White);
            _headerTitleTextControl.Position = new Vector2(GlobalParameters.RIGHT_MARGIN, halfHeightMenuHeader - (_headerTitleTextControl.Size.Y / 2));

            _scoreTextControl = new TextControl(GetScoreText(_resumeStateData.Score), headerFont, Color.White);
            _scoreTextControl.Position = new Vector2(_panelMenuHeader.Size.X - _scoreTextControl.Size.X - GlobalParameters.RIGHT_MARGIN, halfHeightMenuHeader - (_scoreTextControl.Size.Y / 2));

            // Agregamos los textos al panel
            _panelMenuHeader.AddChild(_headerTitleTextControl);
            _panelMenuHeader.AddChild(_scoreTextControl);
        }

        private void CreateWeaponsListMenu(Rectangle viewRect)
        {
#if IPHONE
            const int borde = 1;
#else
            const int borde = 2;
#endif
            var lateralIzquierdoRectangle = GlobalParameters.ScreenAssetsData["CONTENEDOR_Lateral_Izquierdo"];
            var arribaLargoRectangle = GlobalParameters.ScreenAssetsData["CONTENEDOR_Arriba_Largo"];
            var abajoLargoRectangle = GlobalParameters.ScreenAssetsData["CONTENEDOR_Abajo_Largo"];
            var lateralDerechoRectangle = GlobalParameters.ScreenAssetsData["CONTENEDOR_Lateral_Derecho"];

            var contenedorArmasBordeIzquierdo = new ImageControl(ScreenManager.ScreenAssetsTexture, lateralIzquierdoRectangle, Vector2.Zero);

            var contenedorArmasBordeSuperior = new ImageControl(ScreenManager.ScreenAssetsTexture, arribaLargoRectangle,
                                                                new Vector2(lateralIzquierdoRectangle.Width - borde, 0));

            var contenedorArmasBordeDerecho = new ImageControl(ScreenManager.ScreenAssetsTexture,
                                                               lateralDerechoRectangle,
                                                               new Vector2(arribaLargoRectangle.Width - borde +
                                                                   lateralIzquierdoRectangle.Width - borde, 0));

            var contenedorArmasBordeInferior = new ImageControl(ScreenManager.ScreenAssetsTexture, abajoLargoRectangle,
                                                                         new Vector2(lateralIzquierdoRectangle.Width - borde,
                                                                             lateralIzquierdoRectangle.Height -
                                                                             abajoLargoRectangle.Height));

            _panelWeaponMenuBoarder = new PanelControl
            {
                Position =
                    new Vector2(
                    viewRect.X - GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_HORIZONTAL_MARGIN,
                    viewRect.Y - GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_VERTICAL_MARGIN)
            };

            _panelWeaponMenuBoarder.AddChild(contenedorArmasBordeDerecho);
            _panelWeaponMenuBoarder.AddChild(contenedorArmasBordeInferior);
            _panelWeaponMenuBoarder.AddChild(contenedorArmasBordeIzquierdo);
            _panelWeaponMenuBoarder.AddChild(contenedorArmasBordeSuperior);

            //Initialize MenuPanel
            _scrollingWeaponsMenu = new ScrollingPanelControl(viewRect);

            var buttonCounter = 0;

            //Add MenuItems
            var weaponsList = GlobalParameters.GetWeaponList();

            #region Calculamos algunas medidas axuliares para no recalcularlas a cada rato dentro del foreach
            var lateralIzquieroTextureWidth = lateralIzquierdoRectangle.Width;
            
            int halfWithNormalButton = GlobalParameters.WEAPON_BUTTON_NORMAL_WIDTH / 2;
            int halfHeightNormalButton = GlobalParameters.WEAPON_BUTTON_NORMAL_HEIGHT / 2;

            int thirdWithNormalButton = GlobalParameters.WEAPON_BUTTON_NORMAL_WIDTH / 3;
            int firstThirdHalfWithNormalButton = thirdWithNormalButton / 2;
            int lastThirdHalfWithNormalButton = firstThirdHalfWithNormalButton + (thirdWithNormalButton * 2);
            #endregion

            foreach (var weapon in weaponsList)
            {
                var weaponItem = weapon;

                var textureRectangle = GlobalParameters.ScreenAssetsData[weaponItem.Type.ToString()];

                var controlContainer = new Control
                                           {
                                               Position =
                                                   new Vector2(lateralIzquieroTextureWidth,
                                                               buttonCounter* 
                                                               (GlobalParameters.WEAPON_BUTTON_NORMAL_HEIGHT +
                                                                GlobalParameters.WEAPON_BUTTON_NORMAL_MARGIN_BETWEEN_BUTTONS))
                                           };
                
                // Creamos el contenedor principal al que se le agregaran los demas elementos
                var weaponButtonBackgroung = new ImageControl(_weaponButtonNormalTexture,
                                                              new Rectangle(0, 0, GlobalParameters.WEAPON_BUTTON_NORMAL_WIDTH,
                                                                            GlobalParameters.WEAPON_BUTTON_NORMAL_HEIGHT),
                                                              Vector2.Zero);
                
                // Creamos los demas elementos que se agregaran al contenedor principal
                var weaponButtonWeaponIcon = new ImageControl(ScreenManager.ScreenAssetsTexture,
                                                              textureRectangle,
                                                              new Vector2((halfWithNormalButton - (textureRectangle.Width/2)),
                                                                  (halfHeightNormalButton - (textureRectangle.Height/2))));

                var weaponButtonWeaponText = new TextControl(weaponItem.Name,
                                                             ScreenManager.SpriteFonts.TextFont, Color.White,
                                                             Vector2.Zero);

                weaponButtonWeaponText.Position = new Vector2(
                    firstThirdHalfWithNormalButton - (weaponButtonWeaponText.Size.X / 2),
                    halfHeightNormalButton - (weaponButtonWeaponText.Size.Y/2));

                var showWeaponOnDescriptionMenuButton = new Button
                                       {
                                           NormalButtonTexture = _weaponButtonNormalTexture,
                                           TextureRectangle = new Rectangle(0, 0, GlobalParameters.WEAPON_BUTTON_NORMAL_WIDTH, GlobalParameters.WEAPON_BUTTON_NORMAL_HEIGHT),
                                           Width = thirdWithNormalButton,
                                           Height = GlobalParameters.WEAPON_BUTTON_NORMAL_HEIGHT / 2,
                                           //ClickedButtonTexture = _btnClicked,
                                           TextVisible = true,
                                           Font = ScreenManager.SpriteFonts.TextFont,
                                           Foreground = Color.White,
                                           ClickedForeground = Color.Red,
                                       };

                showWeaponOnDescriptionMenuButton.Position = new Vector2(lastThirdHalfWithNormalButton - (showWeaponOnDescriptionMenuButton.Width/2),
                                                          halfHeightNormalButton - (showWeaponOnDescriptionMenuButton.Height/2));

                SetListButtonAndWeaponState(showWeaponOnDescriptionMenuButton, weaponItem);

                showWeaponOnDescriptionMenuButton.OnClicked += ShowOnDescriptionMenu;
                showWeaponOnDescriptionMenuButton.Tag = weaponItem;

                // Agregamos los elementos al contenedor principal (controlContainer)
                controlContainer.AddChild(weaponButtonBackgroung);
                controlContainer.AddChild(weaponButtonWeaponIcon);
                controlContainer.AddChild(weaponButtonWeaponText);
                controlContainer.AddChild(showWeaponOnDescriptionMenuButton);

                // Agregamos el contenedor principal (controlContainer) con todos sus elementos al scrollMenu (_scrollingWeaponsMenu)
                _scrollingWeaponsMenu.AddChild(controlContainer);
                buttonCounter++;
            }
        }

        private void CreateWeaponsDescriptionMenu()
        {
            #region Calculamos algunas medidas axuliares para no recalcularlas a cada rato dentro del foreach

            var lateralIzquierdoRectangle = GlobalParameters.ScreenAssetsData["CONTENEDOR_Lateral_Izquierdo"];
            var lateralIzquierdoTextureWidth = lateralIzquierdoRectangle.Width;
            var lateralIzquieroTextureHeight = lateralIzquierdoRectangle.Height;
            
            var abajoCortoHeight = GlobalParameters.ScreenAssetsData["CONTENEDOR_Abajo_Corto"].Height;

            var arribaCortoRectangle = GlobalParameters.ScreenAssetsData["CONTENEDOR_Arriba_Corto"];
            var arribaCortoWidth = arribaCortoRectangle.Width;
            var arribaCortoHeight = arribaCortoRectangle.Height;
            
            int thirdHeightNormalButton = GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_HEIGHT / 3;
            
            var widthPanel = lateralIzquierdoTextureWidth + GlobalParameters.ScreenAssetsData["CONTENEDOR_Lateral_Derecho"].Width + arribaCortoWidth;
            var heightPanel = lateralIzquieroTextureHeight;
            var halfWidthPanel = widthPanel/2;
            var thirdHeightPanel = heightPanel / 3;
            var firstThirdHalfHeightPanel = thirdHeightPanel / 2;
            var lastThirdHalfHeightPanel = firstThirdHalfHeightPanel + (thirdHeightPanel * 2);
            #endregion

            // Creamos el panel para alojar los elementos
            _panelDescriptionMenu = new PanelControl
                                        {
                                            Position =
                                                new Vector2(GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_DESCRIPTION_MENU_X,
                                                            GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_MENU_Y -
                                                            GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_VERTICAL_MARGIN),
                                            Size = new Vector2(widthPanel, heightPanel)
                                        };


            // Creamos los bordes del panel
            var contenedorArmasBordeIzquierdo = new ImageControl(ScreenManager.ScreenAssetsTexture,
                                                                          GlobalParameters.ScreenAssetsData["CONTENEDOR_Lateral_Izquierdo"],
                                                                          new Vector2(0, 0));

            var contenedorArmasBordeSuperior = new ImageControl(ScreenManager.ScreenAssetsTexture,
                                                                         GlobalParameters.ScreenAssetsData["CONTENEDOR_Arriba_Corto"],
                                                                         new Vector2(lateralIzquierdoTextureWidth - 2, 0));

            var contenedorArmasBordeDerecho = new ImageControl(ScreenManager.ScreenAssetsTexture,
                                                                        GlobalParameters.ScreenAssetsData["CONTENEDOR_Lateral_Derecho"],
                                                                        new Vector2(arribaCortoWidth - 2 + lateralIzquierdoTextureWidth - 2, 0));

            var contenedorArmasBordeInferior = new ImageControl(ScreenManager.ScreenAssetsTexture,
                                                                         GlobalParameters.ScreenAssetsData["CONTENEDOR_Abajo_Corto"],
                                                                         new Vector2(lateralIzquierdoTextureWidth - 2,
                                                                             lateralIzquieroTextureHeight - abajoCortoHeight));


            // Agregamos los bordes al panel
            _panelDescriptionMenu.AddChild(contenedorArmasBordeDerecho);
            _panelDescriptionMenu.AddChild(contenedorArmasBordeInferior);
            _panelDescriptionMenu.AddChild(contenedorArmasBordeIzquierdo);
            _panelDescriptionMenu.AddChild(contenedorArmasBordeSuperior);

            // Setamos un arma x default para mostrar la primera vez en la pantalla
            var defaultSelectedWeapon = new Weapon(GlobalParameters.PRIMARY_WEAPON_TYPE);

            // Creamos el contenedor principal al que se le agregaran los demas elementos
            var controlContainer = new Control {Position = new Vector2(lateralIzquierdoTextureWidth, arribaCortoHeight)};

            var weaponButtonBackgroung = new ImageControl(_weaponButtonNormalTexture,
                                                          new Rectangle(0, 0, GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_WIDTH,
                                                                        GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_HEIGHT),
                                                          Vector2.Zero);


            // Creamos los demas elementos que se agregaran al contenedor principal
            // Image Control del arma seleccionada
            var weaponTextureRectangle = GlobalParameters.ScreenAssetsData[defaultSelectedWeapon.Type.ToString()];
            _weaponImageControl = new ImageControl(ScreenManager.ScreenAssetsTexture,
                                                   weaponTextureRectangle,
                                                   new Vector2(
                                                       ((GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_WIDTH/2) - (weaponTextureRectangle.Width/2)),
                                                       (((GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_HEIGHT/3)/2) - (weaponTextureRectangle.Height/2))));

            // Text Control con la descripcion del arma seleccionada
            _weaponDescriptionTextControl = new TextControl(GetWeaponDescription(defaultSelectedWeapon),
                                                            ScreenManager.SpriteFonts.TextFontSmall, Color.White);

            _weaponDescriptionTextControl.Position = new Vector2(
                (GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_WIDTH / 2 - (_weaponDescriptionTextControl.Size.X / 2)),
                thirdHeightNormalButton);
            
            // Boton para las acciones del arma seleccionada
            _weaponActionButton = new Button
            {
                NormalButtonTexture = _weaponButtonNormalTexture,
                TextureRectangle = new Rectangle(0, 0, GlobalParameters.WEAPON_BUTTON_BUY_WIDTH, GlobalParameters.WEAPON_BUTTON_BUY_HEIGHT),
                Width = GlobalParameters.WEAPON_BUTTON_BUY_WIDTH,
                Height = GlobalParameters.WEAPON_BUTTON_BUY_HEIGHT,
                TextVisible = true,
                Font = ScreenManager.SpriteFonts.TextFont,
                Foreground = Color.White,
                ClickedForeground = Color.Red,
            };

            _weaponActionButton.Position = new Vector2(halfWidthPanel - (_weaponActionButton.Width / 2),
                                                      lastThirdHalfHeightPanel - (_weaponActionButton.Height/2));

            SetDescriptionButtonAndWeaponState(_weaponActionButton, defaultSelectedWeapon);
            //_weaponActionButton.OnClicked += sender => EquipWeapon(defaultSelectedWeapon);
            _weaponActionButton.OnClicked += EquipWeapon;
            _weaponActionButton.Tag = defaultSelectedWeapon;


            // Agregamos los elementos al contenedor principal (controlContainer)
            controlContainer.AddChild(weaponButtonBackgroung);
            controlContainer.AddChild(_weaponImageControl);
            controlContainer.AddChild(_weaponDescriptionTextControl);


            // Agregamos el contenedor principal (controlContainer) con todos sus elementos al panelMenu (_panelDescriptionMenu)
            _panelDescriptionMenu.AddChild(controlContainer);
            _panelDescriptionMenu.AddChild(_weaponActionButton);
             
        }

        private void SetDescriptionButtonAndWeaponState(Button button, Weapon weapon)
        {
            // Si el arma es un arma primara o secundaria del Player
            if (_resumeStateData.PrimaryWeapon.Type == weapon.Type
                || (_resumeStateData.SecondaryWeapon != null && _resumeStateData.SecondaryWeapon.Type == weapon.Type))
            {
                if (_resumeStateData.Score < weapon.AmmoScoreValue)
                {
                    button.DisplayText = GlobalParameters.WEAPON_EQUIPPED_TEXT;
                    weapon.State = WeaponState.Equipped;
                }
                else
                {
                    button.DisplayText = string.Format("{0} for {1}", GlobalParameters.WEAPON_BUY_AMMO_TEXT, weapon.AmmoScoreValue);
                    weapon.State = WeaponState.Equipped; // EquippedAvailableToAmmo
                }
            }
            // Si el arma, no es un arma primara o secundaria del Player pero la tiene en el inventario,
            // entonces que pueda equiparsela (EquipWeapon).
            else if (_resumeStateData.WeaponInventory.Any(item => item.Type == weapon.Type))
            {
                button.DisplayText = GlobalParameters.WEAPON_EQUIP_IT_TEXT;
                weapon.State = WeaponState.InInventory;
            }
            // Si el arma no esta en el inventario del Player, entonces que puede comprarla (BuyWeapon)
            // y luego equiparla (si es que posee ptos suficientes).
            else if (_resumeStateData.Score >= weapon.ScoreValue)
            {
                button.DisplayText = GlobalParameters.WEAPON_BUY_IT_TEXT;
                weapon.State = WeaponState.AvailableToBuy;
            }
            else
            {
                button.DisplayText = GlobalParameters.WEAPON_NOT_ENOUGHT_TO_BUY_TEXT;
                weapon.State = WeaponState.NotEnoughtToBuy;
            }

            button.Tag = weapon;
        }

        private void CreateMenuFooter()
        {
            //Initialize Text MenuPanel
            _panelMenuFooter = new PanelControl
            {
                Size = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_FOOTER_HEIGHT),
                Position = new Vector2(0, ScreenManager.GraphicsDevice.Viewport.Height - GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_FOOTER_HEIGHT)
            };

            var quarterWidthMenuFooter = _panelMenuFooter.Size.X / 4;
            var halfHeightMenuFooter = _panelMenuFooter.Size.Y / 2;

            _primaryWeaponTextControl =
                new TextControl(
                    GetWeaponTextControlText(_resumeStateData.PrimaryWeapon),
                    ScreenManager.SpriteFonts.TextFont, Color.Black);

            _primaryWeaponTextControl.Position = new Vector2(GlobalParameters.LEFT_MARGIN, halfHeightMenuFooter - _primaryWeaponTextControl.Size.Y);

            _secondaryWeaponTextControl =
                new TextControl(
                    GetWeaponTextControlText(_resumeStateData.SecondaryWeapon),
                    ScreenManager.SpriteFonts.TextFont, Color.Black) { Position = new Vector2(GlobalParameters.LEFT_MARGIN, halfHeightMenuFooter) };

            var playButton = new Button
            {
                //ClickedButtonTexture = _btnClicked,
                TextVisible = true,
                Font = ScreenManager.SpriteFonts.TittleFontMedium,
                DisplayText = GlobalParameters.BUY_WEAPON_SCREEN_PLAY_BUTTON_TEXT,
                Foreground = Color.White,
                ClickedForeground = Color.Black,
            };

            playButton.Width = playButton.TextWidth;
            playButton.Height = playButton.TextHeight;
            playButton.Position =
                new Vector2((quarterWidthMenuFooter * 3) + ((quarterWidthMenuFooter / 2) - (playButton.Width / 2)),
                            halfHeightMenuFooter - (playButton.Height / 2));

            playButton.OnClicked += sender => LoadNextLevel();

            _panelMenuFooter.AddChild(_primaryWeaponTextControl);
            _panelMenuFooter.AddChild(_secondaryWeaponTextControl);
            _panelMenuFooter.AddChild(playButton);
        }

        /// <summary>
        /// Arma la descripcion del arma que se la pasa como parametro
        /// </summary>
        /// <param name="weaponItem"></param>
        /// <returns></returns>
        private string GetWeaponDescription(Weapon weaponItem)
        {
            var sb = new StringBuilder();

            sb.Append(string.Format("{0} ({1})", weaponItem.Name, weaponItem.Position));
            sb.Append(Environment.NewLine);
            sb.Append(string.Format("Power {0}/{1}{2}",
                                    Math.Round(
                                        (weaponItem.Power/GlobalParameters.WeaponMaxPower)*
                                        GlobalParameters.WEAPON_CHARACTERISTIC_TOTAL_UNITY, 0),
                                    GlobalParameters.WEAPON_CHARACTERISTIC_TOTAL_UNITY,
                                    weaponItem.IsAutomatic ? " (automatic)" : string.Empty));
            sb.Append(Environment.NewLine);
            sb.Append(string.Format("Bullet Velocity {0}/{1}",
                                    Math.Round(
                                        (weaponItem.BulletVelocity/GlobalParameters.WeaponMaxBulletVelocity)*
                                        GlobalParameters.WEAPON_CHARACTERISTIC_TOTAL_UNITY, 0),
                                    GlobalParameters.WEAPON_CHARACTERISTIC_TOTAL_UNITY));
            sb.Append(Environment.NewLine);

            //var maxAmmoScale = weaponItem.MaxAmmo / GlobalParameters.WeaponMaxMaxAmmo;
            //maxAmmoScale = maxAmmoScale > 1 ? 1 - maxAmmoScale * GlobalParameters.WEAPON_CHARACTERISTIC_TOTAL_UNITY : 1f;
            var maxAmmoScale = (weaponItem.MaxAmmo / GlobalParameters.WeaponMaxMaxAmmo) * GlobalParameters.WEAPON_CHARACTERISTIC_TOTAL_UNITY;

            if (maxAmmoScale < 1.0f)
                maxAmmoScale = 1.0f;
            
            sb.Append(string.Format("Max. Ammo {0}/{1}",
                                    Math.Round(maxAmmoScale, 0),
                                    GlobalParameters.WEAPON_CHARACTERISTIC_TOTAL_UNITY));
            sb.Append(Environment.NewLine);

            var reloadTimeScale = weaponItem.ReloadTime/GlobalParameters.WeaponMaxReloadTime;
            reloadTimeScale = reloadTimeScale < 1 ? (1 - reloadTimeScale)*GlobalParameters.WEAPON_CHARACTERISTIC_TOTAL_UNITY : 1f;

            sb.Append(string.Format("Reload Time {0}/{1}",
                                    Math.Round(reloadTimeScale, 0),
                                    GlobalParameters.WEAPON_CHARACTERISTIC_TOTAL_UNITY));
            sb.Append(Environment.NewLine);

            var fireRateScale = weaponItem.FireRate/GlobalParameters.WeaponMaxFireRate;
            fireRateScale = fireRateScale < 1 ? (1 - fireRateScale)*GlobalParameters.WEAPON_CHARACTERISTIC_TOTAL_UNITY : 1f;

            sb.Append(string.Format("Fire Rate {0}/{1}",
                                    Math.Round(fireRateScale, 0),
                                    GlobalParameters.WEAPON_CHARACTERISTIC_TOTAL_UNITY));
            return sb.ToString();
        }

        private string GetWeaponTextControlHeaderText()
        {
            //return string.Format("{0} - $ {1}", GlobalParameters.CHANGE_WEAPON_TEXT, score);
            return GlobalParameters.CHANGE_WEAPON_TEXT;
        }

        private string GetScoreText(int score)
        {
            return string.Format("Coins {0}", score);
        }

        private string GetWeaponTextControlText(Weapon weapon)
        {
            //return string.Format("{0} ({1}): {2}", textToDisplay,
            //                     weapon == null ? string.Empty : weapon.Position.ToString(),
            //                     weapon == null ? string.Empty : weapon.Name);
            if (weapon == null)
            {
                return string.Empty;
            }
            
            return string.Format("{0} weapon equipped {1} ({2} / {3})",
                                 weapon.Position.ToString(),
                                 weapon.Name,
                                 weapon.CurrentAmmo,
                                 _resumeStateData.AmmoInventory == null || !_resumeStateData.AmmoInventory.ContainsKey((int)weapon.Type) ? 0 : _resumeStateData.AmmoInventory[(int)weapon.Type]);
        }

        private string GetButtonDisplayTextToBuy(Weapon weapon)
        {
            //return string.Format("{0}  ({1}) - $ {2}", weapon.Name, weapon.Position, weapon.ScoreValue);
            return string.Format("$ {0}", weapon.ScoreValue);
        }

        //private string GetButtonDisplayText(string textToDisplay, Weapon weapon)
        //{
        //    //return string.Format("{0} ({1}) - {2}", weapon.Name, weapon.Position, textToDisplay);
        //    return string.Format("{0}", textToDisplay);
        //}

        /// <summary>
        /// Muestra el arma seleccionada en el menu de descripcion de armas (setea los valores en los controles correspondientes al menu)
        /// </summary>
        /// <param name="sender">Boton o control que dispara el evento. Asegurarse que posea un campo Tag de tipo Weapon </param>
        private void ShowOnDescriptionMenu(Button sender)
        {
            Weapon weapon;

            if (sender.Tag is Weapon)
            {
                weapon = (Weapon)sender.Tag;
            }
            else
            {
                throw new Exception("El sender no posee un campo Tag de tipo Weapon");
            }

            // Setamos las cosas a mostrar en el menu de descripcion segun el arma que seleccionamos
            // Seteamos la imagen del arma que seleccionamos
            var weaponTextureRectangle = GlobalParameters.ScreenAssetsData[weapon.Type.ToString()];
            _weaponImageControl.Texture = ScreenManager.ScreenAssetsTexture;
            _weaponImageControl.TextureRectangle = weaponTextureRectangle;
            _weaponImageControl.Position =
                new Vector2(((GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_WIDTH/2) - (weaponTextureRectangle.Width/2)),
                            (((GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_HEIGHT/3)/2) - (weaponTextureRectangle.Height/2)));

            // Seteamos el texto con la descripcion del arma que seleccionamos
            _weaponDescriptionTextControl.Text = GetWeaponDescription(weapon);

            // Seteamos el estado del boton, su evento y el estado del arma que seleccionamos
            SetDescriptionButtonAndWeaponState(_weaponActionButton, weapon);
            _weaponActionButton.Tag = weapon;
        }

        /// <summary>
        /// Establece el estado del boton y del arma que se esta poniendo en el boton en funcion 
        ///  de las armas que el player tiene equipadas o en su inventario
        /// </summary>
        /// <param name="button"></param>
        /// <param name="weapon"></param>
        private void SetListButtonAndWeaponState(Button button, Weapon weapon)
        {
            // Si el arma es un arma primara o secundaria del Player
            if (_resumeStateData.PrimaryWeapon.Type == weapon.Type
                || (_resumeStateData.SecondaryWeapon != null && _resumeStateData.SecondaryWeapon.Type == weapon.Type))
            {
                //button.DisplayText = GetButtonDisplayText(GlobalParameters.WEAPON_EQUIPPED_TEXT, weapon);
                button.DisplayText = GlobalParameters.WEAPON_EQUIPPED_TEXT;
                weapon.State = WeaponState.Equipped;
            }
            // Si el arma, no es un arma primara o secundaria del Player pero la tiene en el inventario,
            // entonces que pueda equiparsela (EquipWeapon).
            else if (_resumeStateData.WeaponInventory.Any(item => item.Type == weapon.Type))
            {
                //button.DisplayText = GetButtonDisplayText(GlobalParameters.WEAPON_IN_INVENTORY_TEXT, weapon);
                button.DisplayText = GlobalParameters.WEAPON_IN_INVENTORY_TEXT;
                weapon.State = WeaponState.InInventory;
            }
            // Si el arma no esta en el inventario del Player, entonces que puede comprarla (BuyWeapon)
            // y luego equiparla (si es que posee ptos suficientes).
            else
            {
                button.DisplayText = GetButtonDisplayTextToBuy(weapon);
                weapon.State = _resumeStateData.Score >= weapon.ScoreValue
                                   ? WeaponState.AvailableToBuy
                                   : WeaponState.NotEnoughtToBuy;
            }

            button.Tag = weapon;
        }

        /// <summary>
        /// Equipa el arma seleccionada comprandola de ser necesario siempre y cuando nuestro presupuesto nos alcance.
        /// Actualiza el menu del listado de armas para reflejar el cambio de 
        /// </summary>
        /// <param name="sender">Boton o control que dispara el evento. Asegurarse que posea un campo Tag de tipo Weapon </param>
        private void EquipWeapon(Button sender)
        {
            // TODO: podria mostrar un MessageBox de confirmacion o para avisar que no dispone de la suficiente plata

            Weapon weapon;

            if (sender.Tag is Weapon)
            {
                weapon = (Weapon)sender.Tag;
            }
            else
            {
                throw new Exception("El sender no posee un campo Tag de tipo Weapon");
            }

            // Si el arma posee un estado "no alcanza para comprarla",
            // entonces que retorne sin hacer nada.
            if (weapon.State == WeaponState.NotEnoughtToBuy) 
                return;

            // Si el arma posee un estadio "equipada" y los puntos que se poseen son menores al costo de las municiones,
            // entonces que retorne sin hacer nada.
            if (weapon.State == WeaponState.Equipped && _resumeStateData.Score < weapon.AmmoScoreValue)
                return;
            
            // Si el arma posee un estadio "equipada" y los puntos que se poseen son mayores al costo de las municiones,
            // entonces comprar las municiones.
            if (weapon.State == WeaponState.Equipped && _resumeStateData.Score >= weapon.AmmoScoreValue)
            {
                BuyAmmo(weapon);

                // Refrescamos los menues de la pantalla
                UpdateScreenControls();
            }
            else
            {
                // Si el arma posee un estado "disponible para comprarla", entonces comprarla
                if (weapon.State == WeaponState.AvailableToBuy)
                {
                    BuyWeapon(weapon);
                }

                // Si el arma posee un estado "en inventario"
                if (weapon.State == WeaponState.InInventory)
                {
                    // Cambiamos el arma
                    SetCurrentWeapon(weapon);

                    // Refrescamos los menues de la pantalla
                    UpdateScreenControls();
                }
            }
        }

        /// <summary>
        /// Compra el arma seleccionada. Disminuye nuestros presupuesto y 
        /// la agrega a nuestro inventario para una proxima utilizacion
        /// </summary>
        /// <param name="weapon"></param>
        private void BuyWeapon(Weapon weapon)
        {
            // Si el score es < al costo del arma, retornamos
            if (_resumeStateData.Score < weapon.ScoreValue) return;

            // Disminuimos el score segun el costo del arma (ScoreValue)
            _resumeStateData.Score -= weapon.ScoreValue;

            // Lo agregamos al inventario
            _resumeStateData.WeaponInventory.Add(weapon);

            GetAmmo(weapon);

            weapon.State = WeaponState.InInventory;
        }

        private void BuyAmmo(Weapon weapon)
        {
            // Si el score es < al costo de las municiones, retornamos
            if (_resumeStateData.Score < weapon.AmmoScoreValue) return;

            // Disminuimos el score segun el costo de las municiones (ScoreValue)
            _resumeStateData.Score -= weapon.AmmoScoreValue;

            GetAmmo(weapon);
        }

        private void GetAmmo(Weapon weapon)
        {
            var weaponKey = (int) weapon.Type;

            if (_resumeStateData.AmmoInventory.ContainsKey(weaponKey))
            {
                _resumeStateData.AmmoInventory[weaponKey] += weapon.AmmoPack;
            }
            else
            {
                _resumeStateData.AmmoInventory.Add(weaponKey, weapon.AmmoPack);
            }
        }

        /// <summary>
        /// Establece el arma como arma actual dependiendo si es primaria / secundaria
        /// </summary>
        /// <param name="weapon"></param>
        private void SetCurrentWeapon(Weapon weapon)
        {
            //Weapon previousWeapon;
            
            // Dependiendo de la posicion del arma, la ponemos como primaria o secundaria
            switch (weapon.Position)
            {
                case WeaponPosition.Primary:
                    _resumeStateData.PrimaryWeapon = weapon;
                    break;
                case WeaponPosition.Secondary:
                    _resumeStateData.SecondaryWeapon = weapon;
                    break;
                default:
                    throw new Exception("Position del arma inexistente");
            }

            // Actualizamos los estados de las armas
            weapon.State = WeaponState.Equipped;
            //previousWeapon.State = WeaponState.InInventory;

            //return previousWeapon;
        }

        /// <summary>
        /// Actualiza el estado de los controles de la pantalla:
        /// - los textControls con las armas que le player tiene equipadas 
        /// - los botones del menu de listado de armas
        /// - los controles del menu de descripcion del arma seleccionada
        /// </summary>
        private void UpdateScreenControls()
        {
            //_headerTitleTextControl.Text = GetWeaponTextControlHeaderText();
            _scoreTextControl.Text = GetScoreText(_resumeStateData.Score);
            _primaryWeaponTextControl.Text = GetWeaponTextControlText(_resumeStateData.PrimaryWeapon);
            _secondaryWeaponTextControl.Text = GetWeaponTextControlText(_resumeStateData.SecondaryWeapon);

            //currentButton.DisplayText = GetButtonDisplayText(GlobalParameters.WEAPON_EQUIPPED_TEXT, newWeapon);
            //currentButton.DisplayText = GlobalParameters.WEAPON_EQUIPPED_TEXT;

            // Iteramos todos los controles del menu en busqueda del boton del arma anterior para cambiarle el texto
            for (var i = 0; i < _scrollingWeaponsMenu.ChildCount; i++)
            {
                var insideControl = _scrollingWeaponsMenu[i];

                for (var x = 0; x < insideControl.ChildCount; x++)
                {
                    // Si el control no es un boton, entonces continuamos con la proxima iteracion
                    if (!(insideControl[x] is Button)) continue;

                    var button = (Button)insideControl[x];

                    // Si el tag del boton no es un arma, entonces continuamos con la proxima iteracion
                    if (!(button.Tag is Weapon)) continue;

                    var buttonWeapon = (Weapon)button.Tag;

                    SetListButtonAndWeaponState(button, buttonWeapon);
                }
            }

            SetDescriptionButtonAndWeaponState(_weaponActionButton, (Weapon)_weaponActionButton.Tag);
        }

        private void LoadNextLevel()
        {
            // Volvemos a regarlas las armas (por si no tenia balas cuando llego a esta pantalla y compro)
            ReloadWeapons();

            LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new GameplayScreen(_resumeStateData));
        }

        /// <summary>
        /// Recarga las armas asi el player las tiene listas para el proximo nivel
        /// </summary>
        private void ReloadWeapons()
        {
            if (_resumeStateData.PrimaryWeapon != null) ReloadWeapons(_resumeStateData.PrimaryWeapon);
            if (_resumeStateData.SecondaryWeapon != null) ReloadWeapons(_resumeStateData.SecondaryWeapon);
        }

        private void ReloadWeapons(Weapon weapon)
        {
            if (weapon.CurrentAmmo >= weapon.MaxAmmo) return;

            var availableAmmo = GetWeaponAvailableAmmo(weapon);

            // Si no tenemos municiones disponibles, que retorne sin hacer nada
            if (availableAmmo <= 0) return;

            // Cargamos el arma con las que tenemos disponibles
            SetWeaponAvailableAmmo(weapon, weapon.FastReload(availableAmmo));
        }

        /// <summary>
        /// Setea las municiones para el arma correspondiente
        /// </summary>
        /// <param name="weapon"> </param>
        /// <param name="availableAmmo"></param>
        private void SetWeaponAvailableAmmo(Weapon weapon, int availableAmmo)
        {
            var weaponIndex = (int)weapon.Type;

            if (_resumeStateData.AmmoInventory == null)
                _resumeStateData.AmmoInventory = new Dictionary<int, int>();

            if (_resumeStateData.AmmoInventory.ContainsKey(weaponIndex))
            {
                _resumeStateData.AmmoInventory[weaponIndex] = availableAmmo;
            }
            else
            {
                _resumeStateData.AmmoInventory.Add(weaponIndex, availableAmmo);
            }
        }

        /// <summary>
        /// Obtiene las municiones del inventario del arma en cuestion
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        public int GetWeaponAvailableAmmo(Weapon weapon)
        {
            return _resumeStateData.AmmoInventory == null || !_resumeStateData.AmmoInventory.ContainsKey((int)weapon.Type) ? 0 : _resumeStateData.AmmoInventory[(int)weapon.Type];
        }
        #endregion

        #region Unload
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

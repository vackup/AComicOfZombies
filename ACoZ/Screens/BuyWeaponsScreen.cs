#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ACoZ.Helpers;
using ACoZ.ScreenManagers;
using ACoZ.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
// TODO: si hay que optimizar, revisar el uso de este LINQ

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

namespace ACoZ.Screens
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
            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this._resumeStateData = resumeState;

            // Recargamos las armas que el player estaba usando
            this.ReloadWeapons();
        }

        public override void LoadContent()
        {
            if (this._content == null)
            {
                // Probando de usar el ContentManager compartido
                //_content = new ContentManager(ScreenManager.Game.Services, GlobalParameters.CONTENT_FOLDER);
                this._content = this.ScreenManager.Game.Content;
            }

            this._menuViewport = new Viewport
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

            this._weaponButtonNormalTexture = this._content.Load<Texture2D>(GlobalParameters.WEAPON_BUTTON_NORMAL);

            //UpdateScreen();

            this.CreateMenuHeader();

            var viewRect = new Rectangle(this._menuViewport.X,
                                         this._menuViewport.Y,
                                         this._menuViewport.Width,
                                         this._menuViewport.Height);

            this.CreateWeaponsListMenu(viewRect);

            this.CreateWeaponsDescriptionMenu();

            this.CreateMenuFooter();

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
            this._panelMenuHeader.HandleInput(input);
            this._panelWeaponMenuBoarder.HandleInput(input);
            this._scrollingWeaponsMenu.HandleInput(input);
            this._panelDescriptionMenu.HandleInput(input);
            this._panelMenuFooter.HandleInput(input);
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
            if (this.IsActive && GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.ExitScreen();
            }
            
            //Update Menu states
            this._panelMenuHeader.Update(gameTime);
            this._panelWeaponMenuBoarder.Update(gameTime);
            this._scrollingWeaponsMenu.Update(gameTime);
            this._panelDescriptionMenu.Update(gameTime);
            this._panelMenuFooter.Update(gameTime);
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
        #endregion

        #region Game Methods - Draw
        public override void Draw(GameTime gameTime)
        {
            if (!this.IsActive) return;

            //Draw Background
            this.DrawBackground();

            // Draw Menu Header
            this.DrawMenuHeader(gameTime);
            
            // Draw Menu
            this.DrawWeaponsMenu(gameTime);

            this.DrawMenuFooter(gameTime);
        }

        private void DrawMenuHeader(GameTime gameTime)
        {
            Control.BatchDraw(this._panelMenuHeader, this.ScreenManager.GraphicsDevice, this.ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
        }

        private void DrawMenuFooter(GameTime gameTime)
        {
            Control.BatchDraw(this._panelMenuFooter, this.ScreenManager.GraphicsDevice, this.ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
        }

        private void DrawBackground()
        {
            this.ScreenManager.GraphicsDevice.Clear(GlobalParameters.AlternativeColor);
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
            var oldViewport = this.ScreenManager.GraphicsDevice.Viewport;
            this.ScreenManager.GraphicsDevice.Viewport = this._menuViewport;
#endif
            //Draw Weapon Selector list
            Control.BatchDraw(this._scrollingWeaponsMenu, this.ScreenManager.GraphicsDevice, this.ScreenManager.SpriteBatch,  Vector2.Zero, gameTime);

            // Now that we're done, set our old viewport back on the device
#if !SILVERLIGHT
            this.ScreenManager.GraphicsDevice.Viewport = oldViewport;
#endif

            Control.BatchDraw(this._panelWeaponMenuBoarder, this.ScreenManager.GraphicsDevice, this.ScreenManager.SpriteBatch, Vector2.Zero, gameTime);

            Control.BatchDraw(this._panelDescriptionMenu, this.ScreenManager.GraphicsDevice, this.ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
        }

        #endregion

        #region Menu
        private void CreateMenuHeader()
        {
#if FREE
			var headerFont =  ScreenManager.SpriteFonts.TittleFontSmall;
#else
			var headerFont =  this.ScreenManager.SpriteFonts.TittleFont;
#endif

            //Initialize Text MenuPanel
            this._panelMenuHeader = new PanelControl
                                   {
										Size = new Vector2(this.ScreenManager.GraphicsDevice.Viewport.Width, 
				                   							GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_MENU_Y - GlobalParameters.BANNER_MARGIN), // Ancho = Ancho de pantalla. Alto = position Y del menu de armas
										Position = new Vector2(0, GlobalParameters.BANNER_MARGIN)
                                   };
			       
            var halfHeightMenuHeader = this._panelMenuHeader.Size.Y / 2;
            
            // Creamos los textos que iran en el panel
            this._headerTitleTextControl = new TextControl(this.GetWeaponTextControlHeaderText(), headerFont, Color.White);
            this._headerTitleTextControl.Position = new Vector2(GlobalParameters.RIGHT_MARGIN, halfHeightMenuHeader - (this._headerTitleTextControl.Size.Y / 2));

            this._scoreTextControl = new TextControl(this.GetScoreText(this._resumeStateData.Score), headerFont, Color.White);
            this._scoreTextControl.Position = new Vector2(this._panelMenuHeader.Size.X - this._scoreTextControl.Size.X - GlobalParameters.RIGHT_MARGIN, halfHeightMenuHeader - (this._scoreTextControl.Size.Y / 2));

            // Agregamos los textos al panel
            this._panelMenuHeader.AddChild(this._headerTitleTextControl);
            this._panelMenuHeader.AddChild(this._scoreTextControl);
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

            var contenedorArmasBordeIzquierdo = new ImageControl(this.ScreenManager.ScreenAssetsTexture, lateralIzquierdoRectangle, Vector2.Zero);

            var contenedorArmasBordeSuperior = new ImageControl(this.ScreenManager.ScreenAssetsTexture, arribaLargoRectangle,
                                                                new Vector2(lateralIzquierdoRectangle.Width - borde, 0));

            var contenedorArmasBordeDerecho = new ImageControl(this.ScreenManager.ScreenAssetsTexture,
                                                               lateralDerechoRectangle,
                                                               new Vector2(arribaLargoRectangle.Width - borde +
                                                                   lateralIzquierdoRectangle.Width - borde, 0));

            var contenedorArmasBordeInferior = new ImageControl(this.ScreenManager.ScreenAssetsTexture, abajoLargoRectangle,
                                                                         new Vector2(lateralIzquierdoRectangle.Width - borde,
                                                                             lateralIzquierdoRectangle.Height -
                                                                             abajoLargoRectangle.Height));

            this._panelWeaponMenuBoarder = new PanelControl
            {
                Position =
                    new Vector2(
                    viewRect.X - GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_HORIZONTAL_MARGIN,
                    viewRect.Y - GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_VERTICAL_MARGIN)
            };

            this._panelWeaponMenuBoarder.AddChild(contenedorArmasBordeDerecho);
            this._panelWeaponMenuBoarder.AddChild(contenedorArmasBordeInferior);
            this._panelWeaponMenuBoarder.AddChild(contenedorArmasBordeIzquierdo);
            this._panelWeaponMenuBoarder.AddChild(contenedorArmasBordeSuperior);

            //Initialize MenuPanel
            this._scrollingWeaponsMenu = new ScrollingPanelControl(viewRect);

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
                var weaponButtonBackgroung = new ImageControl(this._weaponButtonNormalTexture,
                                                              new Rectangle(0, 0, GlobalParameters.WEAPON_BUTTON_NORMAL_WIDTH,
                                                                            GlobalParameters.WEAPON_BUTTON_NORMAL_HEIGHT),
                                                              Vector2.Zero);
                
                // Creamos los demas elementos que se agregaran al contenedor principal
                var weaponButtonWeaponIcon = new ImageControl(this.ScreenManager.ScreenAssetsTexture,
                                                              textureRectangle,
                                                              new Vector2((halfWithNormalButton - (textureRectangle.Width/2)),
                                                                  (halfHeightNormalButton - (textureRectangle.Height/2))));

                var weaponButtonWeaponText = new TextControl(weaponItem.Name,
                                                             this.ScreenManager.SpriteFonts.TextFont, Color.White,
                                                             Vector2.Zero);

                weaponButtonWeaponText.Position = new Vector2(
                    firstThirdHalfWithNormalButton - (weaponButtonWeaponText.Size.X / 2),
                    halfHeightNormalButton - (weaponButtonWeaponText.Size.Y/2));

                var showWeaponOnDescriptionMenuButton = new Button
                                       {
                                           NormalButtonTexture = this._weaponButtonNormalTexture,
                                           TextureRectangle = new Rectangle(0, 0, GlobalParameters.WEAPON_BUTTON_NORMAL_WIDTH, GlobalParameters.WEAPON_BUTTON_NORMAL_HEIGHT),
                                           Width = thirdWithNormalButton,
                                           Height = GlobalParameters.WEAPON_BUTTON_NORMAL_HEIGHT / 2,
                                           //ClickedButtonTexture = _btnClicked,
                                           TextVisible = true,
                                           Font = this.ScreenManager.SpriteFonts.TextFont,
                                           Foreground = Color.White,
                                           ClickedForeground = Color.Red,
                                       };

                showWeaponOnDescriptionMenuButton.Position = new Vector2(lastThirdHalfWithNormalButton - (showWeaponOnDescriptionMenuButton.Width/2),
                                                          halfHeightNormalButton - (showWeaponOnDescriptionMenuButton.Height/2));

                this.SetListButtonAndWeaponState(showWeaponOnDescriptionMenuButton, weaponItem);

                showWeaponOnDescriptionMenuButton.OnClicked += ShowOnDescriptionMenu;
                showWeaponOnDescriptionMenuButton.Tag = weaponItem;

                // Agregamos los elementos al contenedor principal (controlContainer)
                controlContainer.AddChild(weaponButtonBackgroung);
                controlContainer.AddChild(weaponButtonWeaponIcon);
                controlContainer.AddChild(weaponButtonWeaponText);
                controlContainer.AddChild(showWeaponOnDescriptionMenuButton);

                // Agregamos el contenedor principal (controlContainer) con todos sus elementos al scrollMenu (_scrollingWeaponsMenu)
                this._scrollingWeaponsMenu.AddChild(controlContainer);
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
            this._panelDescriptionMenu = new PanelControl
                                        {
                                            Position =
                                                new Vector2(GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_DESCRIPTION_MENU_X,
                                                            GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_MENU_Y -
                                                            GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_VERTICAL_MARGIN),
                                            Size = new Vector2(widthPanel, heightPanel)
                                        };


            // Creamos los bordes del panel
            var contenedorArmasBordeIzquierdo = new ImageControl(this.ScreenManager.ScreenAssetsTexture,
                                                                          GlobalParameters.ScreenAssetsData["CONTENEDOR_Lateral_Izquierdo"],
                                                                          new Vector2(0, 0));

            var contenedorArmasBordeSuperior = new ImageControl(this.ScreenManager.ScreenAssetsTexture,
                                                                         GlobalParameters.ScreenAssetsData["CONTENEDOR_Arriba_Corto"],
                                                                         new Vector2(lateralIzquierdoTextureWidth - 2, 0));

            var contenedorArmasBordeDerecho = new ImageControl(this.ScreenManager.ScreenAssetsTexture,
                                                                        GlobalParameters.ScreenAssetsData["CONTENEDOR_Lateral_Derecho"],
                                                                        new Vector2(arribaCortoWidth - 2 + lateralIzquierdoTextureWidth - 2, 0));

            var contenedorArmasBordeInferior = new ImageControl(this.ScreenManager.ScreenAssetsTexture,
                                                                         GlobalParameters.ScreenAssetsData["CONTENEDOR_Abajo_Corto"],
                                                                         new Vector2(lateralIzquierdoTextureWidth - 2,
                                                                             lateralIzquieroTextureHeight - abajoCortoHeight));


            // Agregamos los bordes al panel
            this._panelDescriptionMenu.AddChild(contenedorArmasBordeDerecho);
            this._panelDescriptionMenu.AddChild(contenedorArmasBordeInferior);
            this._panelDescriptionMenu.AddChild(contenedorArmasBordeIzquierdo);
            this._panelDescriptionMenu.AddChild(contenedorArmasBordeSuperior);

            // Setamos un arma x default para mostrar la primera vez en la pantalla
            var defaultSelectedWeapon = new Weapon(GlobalParameters.PRIMARY_WEAPON_TYPE);

            // Creamos el contenedor principal al que se le agregaran los demas elementos
            var controlContainer = new Control {Position = new Vector2(lateralIzquierdoTextureWidth, arribaCortoHeight)};

            var weaponButtonBackgroung = new ImageControl(this._weaponButtonNormalTexture,
                                                          new Rectangle(0, 0, GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_WIDTH,
                                                                        GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_HEIGHT),
                                                          Vector2.Zero);


            // Creamos los demas elementos que se agregaran al contenedor principal
            // Image Control del arma seleccionada
            var weaponTextureRectangle = GlobalParameters.ScreenAssetsData[defaultSelectedWeapon.Type.ToString()];
            this._weaponImageControl = new ImageControl(this.ScreenManager.ScreenAssetsTexture,
                                                   weaponTextureRectangle,
                                                   new Vector2(
                                                       ((GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_WIDTH/2) - (weaponTextureRectangle.Width/2)),
                                                       (((GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_HEIGHT/3)/2) - (weaponTextureRectangle.Height/2))));

            // Text Control con la descripcion del arma seleccionada
            this._weaponDescriptionTextControl = new TextControl(this.GetWeaponDescription(defaultSelectedWeapon),
                                                            this.ScreenManager.SpriteFonts.TextFontSmall, Color.White);

            this._weaponDescriptionTextControl.Position = new Vector2(
                (GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_WIDTH / 2 - (this._weaponDescriptionTextControl.Size.X / 2)),
                thirdHeightNormalButton);
            
            // Boton para las acciones del arma seleccionada
            this._weaponActionButton = new Button
            {
                NormalButtonTexture = this._weaponButtonNormalTexture,
                TextureRectangle = new Rectangle(0, 0, GlobalParameters.WEAPON_BUTTON_BUY_WIDTH, GlobalParameters.WEAPON_BUTTON_BUY_HEIGHT),
                Width = GlobalParameters.WEAPON_BUTTON_BUY_WIDTH,
                Height = GlobalParameters.WEAPON_BUTTON_BUY_HEIGHT,
                TextVisible = true,
                Font = this.ScreenManager.SpriteFonts.TextFont,
                Foreground = Color.White,
                ClickedForeground = Color.Red,
            };

            this._weaponActionButton.Position = new Vector2(halfWidthPanel - (this._weaponActionButton.Width / 2),
                                                      lastThirdHalfHeightPanel - (this._weaponActionButton.Height/2));

            this.SetDescriptionButtonAndWeaponState(this._weaponActionButton, defaultSelectedWeapon);
            //_weaponActionButton.OnClicked += sender => EquipWeapon(defaultSelectedWeapon);
            this._weaponActionButton.OnClicked += EquipWeapon;
            this._weaponActionButton.Tag = defaultSelectedWeapon;


            // Agregamos los elementos al contenedor principal (controlContainer)
            controlContainer.AddChild(weaponButtonBackgroung);
            controlContainer.AddChild(this._weaponImageControl);
            controlContainer.AddChild(this._weaponDescriptionTextControl);


            // Agregamos el contenedor principal (controlContainer) con todos sus elementos al panelMenu (_panelDescriptionMenu)
            this._panelDescriptionMenu.AddChild(controlContainer);
            this._panelDescriptionMenu.AddChild(this._weaponActionButton);
             
        }

        private void SetDescriptionButtonAndWeaponState(Button button, Weapon weapon)
        {
            // Si el arma es un arma primara o secundaria del Player
            if (this._resumeStateData.PrimaryWeapon.Type == weapon.Type
                || (this._resumeStateData.SecondaryWeapon != null && this._resumeStateData.SecondaryWeapon.Type == weapon.Type))
            {
                if (this._resumeStateData.Score < weapon.AmmoScoreValue)
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
            else if (this._resumeStateData.WeaponInventory.Any(item => item.Type == weapon.Type))
            {
                button.DisplayText = GlobalParameters.WEAPON_EQUIP_IT_TEXT;
                weapon.State = WeaponState.InInventory;
            }
            // Si el arma no esta en el inventario del Player, entonces que puede comprarla (BuyWeapon)
            // y luego equiparla (si es que posee ptos suficientes).
            else if (this._resumeStateData.Score >= weapon.ScoreValue)
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
            this._panelMenuFooter = new PanelControl
            {
                Size = new Vector2(this.ScreenManager.GraphicsDevice.Viewport.Width, GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_FOOTER_HEIGHT),
                Position = new Vector2(0, this.ScreenManager.GraphicsDevice.Viewport.Height - GlobalParameters.BUY_WEAPON_SCREEN_WEAPONS_FOOTER_HEIGHT)
            };

            var quarterWidthMenuFooter = this._panelMenuFooter.Size.X / 4;
            var halfHeightMenuFooter = this._panelMenuFooter.Size.Y / 2;

            this._primaryWeaponTextControl =
                new TextControl(
                    this.GetWeaponTextControlText(this._resumeStateData.PrimaryWeapon),
                    this.ScreenManager.SpriteFonts.TextFont, Color.Black);

            this._primaryWeaponTextControl.Position = new Vector2(GlobalParameters.LEFT_MARGIN, halfHeightMenuFooter - this._primaryWeaponTextControl.Size.Y);

            this._secondaryWeaponTextControl =
                new TextControl(
                    this.GetWeaponTextControlText(this._resumeStateData.SecondaryWeapon),
                    this.ScreenManager.SpriteFonts.TextFont, Color.Black) { Position = new Vector2(GlobalParameters.LEFT_MARGIN, halfHeightMenuFooter) };

            var playButton = new Button
            {
                //ClickedButtonTexture = _btnClicked,
                TextVisible = true,
                Font = this.ScreenManager.SpriteFonts.TittleFontMedium,
                DisplayText = GlobalParameters.BUY_WEAPON_SCREEN_PLAY_BUTTON_TEXT,
                Foreground = Color.White,
                ClickedForeground = Color.Black,
            };

            playButton.Width = playButton.TextWidth;
            playButton.Height = playButton.TextHeight;
            playButton.Position =
                new Vector2((quarterWidthMenuFooter * 3) + ((quarterWidthMenuFooter / 2) - (playButton.Width / 2)),
                            halfHeightMenuFooter - (playButton.Height / 2));

            playButton.OnClicked += sender => this.LoadNextLevel();

            this._panelMenuFooter.AddChild(this._primaryWeaponTextControl);
            this._panelMenuFooter.AddChild(this._secondaryWeaponTextControl);
            this._panelMenuFooter.AddChild(playButton);
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
                                 this._resumeStateData.AmmoInventory == null || !this._resumeStateData.AmmoInventory.ContainsKey((int)weapon.Type) ? 0 : this._resumeStateData.AmmoInventory[(int)weapon.Type]);
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
            this._weaponImageControl.Texture = this.ScreenManager.ScreenAssetsTexture;
            this._weaponImageControl.TextureRectangle = weaponTextureRectangle;
            this._weaponImageControl.Position =
                new Vector2(((GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_WIDTH/2) - (weaponTextureRectangle.Width/2)),
                            (((GlobalParameters.WEAPON_BUTTON_NORMAL_SMALL_HEIGHT/3)/2) - (weaponTextureRectangle.Height/2)));

            // Seteamos el texto con la descripcion del arma que seleccionamos
            this._weaponDescriptionTextControl.Text = this.GetWeaponDescription(weapon);

            // Seteamos el estado del boton, su evento y el estado del arma que seleccionamos
            this.SetDescriptionButtonAndWeaponState(this._weaponActionButton, weapon);
            this._weaponActionButton.Tag = weapon;
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
            if (this._resumeStateData.PrimaryWeapon.Type == weapon.Type
                || (this._resumeStateData.SecondaryWeapon != null && this._resumeStateData.SecondaryWeapon.Type == weapon.Type))
            {
                //button.DisplayText = GetButtonDisplayText(GlobalParameters.WEAPON_EQUIPPED_TEXT, weapon);
                button.DisplayText = GlobalParameters.WEAPON_EQUIPPED_TEXT;
                weapon.State = WeaponState.Equipped;
            }
            // Si el arma, no es un arma primara o secundaria del Player pero la tiene en el inventario,
            // entonces que pueda equiparsela (EquipWeapon).
            else if (this._resumeStateData.WeaponInventory.Any(item => item.Type == weapon.Type))
            {
                //button.DisplayText = GetButtonDisplayText(GlobalParameters.WEAPON_IN_INVENTORY_TEXT, weapon);
                button.DisplayText = GlobalParameters.WEAPON_IN_INVENTORY_TEXT;
                weapon.State = WeaponState.InInventory;
            }
            // Si el arma no esta en el inventario del Player, entonces que puede comprarla (BuyWeapon)
            // y luego equiparla (si es que posee ptos suficientes).
            else
            {
                button.DisplayText = this.GetButtonDisplayTextToBuy(weapon);
                weapon.State = this._resumeStateData.Score >= weapon.ScoreValue
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
            if (weapon.State == WeaponState.Equipped && this._resumeStateData.Score < weapon.AmmoScoreValue)
                return;
            
            // Si el arma posee un estadio "equipada" y los puntos que se poseen son mayores al costo de las municiones,
            // entonces comprar las municiones.
            if (weapon.State == WeaponState.Equipped && this._resumeStateData.Score >= weapon.AmmoScoreValue)
            {
                this.BuyAmmo(weapon);

                // Refrescamos los menues de la pantalla
                this.UpdateScreenControls();
            }
            else
            {
                // Si el arma posee un estado "disponible para comprarla", entonces comprarla
                if (weapon.State == WeaponState.AvailableToBuy)
                {
                    this.BuyWeapon(weapon);
                }

                // Si el arma posee un estado "en inventario"
                if (weapon.State == WeaponState.InInventory)
                {
                    // Cambiamos el arma
                    this.SetCurrentWeapon(weapon);

                    // Refrescamos los menues de la pantalla
                    this.UpdateScreenControls();
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
            if (this._resumeStateData.Score < weapon.ScoreValue) return;

            // Disminuimos el score segun el costo del arma (ScoreValue)
            this._resumeStateData.Score -= weapon.ScoreValue;

            // Lo agregamos al inventario
            this._resumeStateData.WeaponInventory.Add(weapon);

            this.GetAmmo(weapon);

            weapon.State = WeaponState.InInventory;
        }

        private void BuyAmmo(Weapon weapon)
        {
            // Si el score es < al costo de las municiones, retornamos
            if (this._resumeStateData.Score < weapon.AmmoScoreValue) return;

            // Disminuimos el score segun el costo de las municiones (ScoreValue)
            this._resumeStateData.Score -= weapon.AmmoScoreValue;

            this.GetAmmo(weapon);
        }

        private void GetAmmo(Weapon weapon)
        {
            var weaponKey = (int) weapon.Type;

            if (this._resumeStateData.AmmoInventory.ContainsKey(weaponKey))
            {
                this._resumeStateData.AmmoInventory[weaponKey] += weapon.AmmoPack;
            }
            else
            {
                this._resumeStateData.AmmoInventory.Add(weaponKey, weapon.AmmoPack);
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
                    this._resumeStateData.PrimaryWeapon = weapon;
                    break;
                case WeaponPosition.Secondary:
                    this._resumeStateData.SecondaryWeapon = weapon;
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
            this._scoreTextControl.Text = this.GetScoreText(this._resumeStateData.Score);
            this._primaryWeaponTextControl.Text = this.GetWeaponTextControlText(this._resumeStateData.PrimaryWeapon);
            this._secondaryWeaponTextControl.Text = this.GetWeaponTextControlText(this._resumeStateData.SecondaryWeapon);

            //currentButton.DisplayText = GetButtonDisplayText(GlobalParameters.WEAPON_EQUIPPED_TEXT, newWeapon);
            //currentButton.DisplayText = GlobalParameters.WEAPON_EQUIPPED_TEXT;

            // Iteramos todos los controles del menu en busqueda del boton del arma anterior para cambiarle el texto
            for (var i = 0; i < this._scrollingWeaponsMenu.ChildCount; i++)
            {
                var insideControl = this._scrollingWeaponsMenu[i];

                for (var x = 0; x < insideControl.ChildCount; x++)
                {
                    // Si el control no es un boton, entonces continuamos con la proxima iteracion
                    if (!(insideControl[x] is Button)) continue;

                    var button = (Button)insideControl[x];

                    // Si el tag del boton no es un arma, entonces continuamos con la proxima iteracion
                    if (!(button.Tag is Weapon)) continue;

                    var buttonWeapon = (Weapon)button.Tag;

                    this.SetListButtonAndWeaponState(button, buttonWeapon);
                }
            }

            this.SetDescriptionButtonAndWeaponState(this._weaponActionButton, (Weapon)this._weaponActionButton.Tag);
        }

        private void LoadNextLevel()
        {
            // Volvemos a regarlas las armas (por si no tenia balas cuando llego a esta pantalla y compro)
            this.ReloadWeapons();

            LoadingScreen.Load(this.ScreenManager, true, PlayerIndex.One, new GameplayScreen(this._resumeStateData));
        }

        /// <summary>
        /// Recarga las armas asi el player las tiene listas para el proximo nivel
        /// </summary>
        private void ReloadWeapons()
        {
            if (this._resumeStateData.PrimaryWeapon != null) this.ReloadWeapons(this._resumeStateData.PrimaryWeapon);
            if (this._resumeStateData.SecondaryWeapon != null) this.ReloadWeapons(this._resumeStateData.SecondaryWeapon);
        }

        private void ReloadWeapons(Weapon weapon)
        {
            if (weapon.CurrentAmmo >= weapon.MaxAmmo) return;

            var availableAmmo = this.GetWeaponAvailableAmmo(weapon);

            // Si no tenemos municiones disponibles, que retorne sin hacer nada
            if (availableAmmo <= 0) return;

            // Cargamos el arma con las que tenemos disponibles
            this.SetWeaponAvailableAmmo(weapon, weapon.FastReload(availableAmmo));
        }

        /// <summary>
        /// Setea las municiones para el arma correspondiente
        /// </summary>
        /// <param name="weapon"> </param>
        /// <param name="availableAmmo"></param>
        private void SetWeaponAvailableAmmo(Weapon weapon, int availableAmmo)
        {
            var weaponIndex = (int)weapon.Type;

            if (this._resumeStateData.AmmoInventory == null)
                this._resumeStateData.AmmoInventory = new Dictionary<int, int>();

            if (this._resumeStateData.AmmoInventory.ContainsKey(weaponIndex))
            {
                this._resumeStateData.AmmoInventory[weaponIndex] = availableAmmo;
            }
            else
            {
                this._resumeStateData.AmmoInventory.Add(weaponIndex, availableAmmo);
            }
        }

        /// <summary>
        /// Obtiene las municiones del inventario del arma en cuestion
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        public int GetWeaponAvailableAmmo(Weapon weapon)
        {
            return this._resumeStateData.AmmoInventory == null || !this._resumeStateData.AmmoInventory.ContainsKey((int)weapon.Type) ? 0 : this._resumeStateData.AmmoInventory[(int)weapon.Type];
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

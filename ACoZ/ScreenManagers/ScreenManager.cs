#region File Description
//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
//using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework.Media;
using Platformer.Animations;
using Platformer.Helpers;
using Platformer.Weapons;

#if WINDOWS_PHONE || IPHONE
using Mobile.Base.ScreenSystem;
#elif SILVERLIGHT
using Web.Base.ScreenSystem;
#elif WINDOWS
using Desktop.Base.ScreenSystem;
#endif

#endregion

namespace Platformer.ScreenManagers
{
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields
        List<GameScreen> screens = new List<GameScreen>(GlobalParameters.MAX_GAMESCREENS);
        List<GameScreen> screensToUpdate = new List<GameScreen>(GlobalParameters.MAX_GAMESCREENS);

        InputHelper input = new InputHelper();

        private Texture2D _blankTexture;
        private bool _isInitialized;

        #endregion

        #region Properties

        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch { get; private set; }


        /// <summary>
        /// Contains all the fonts avaliable for use.
        /// </summary>
        public SpriteFonts SpriteFonts;


        /// <summary>
        /// If true, the manager prints out a list of all the screens
        /// each time it is updated. This can be useful for making sure
        /// everything is being added and removed at the right times.
        /// </summary>
        public bool TraceEnabled { get; set; }

        //public ContentManager GraphicContent { get; set; }
        //public Texture2D[] BlockATexture { get; set; }
        //public Texture2D[] BlockBTexture { get; set; }
        //public Texture2D Checkpoint1Texture { get; set; }
        //public Texture2D Checkpoint2Texture { get; set; }

        

        //public Texture2D ExitTexture { get; set; }
        //public Texture2D GemTexture { get; set; }
        //public Texture2D LadderTexture { get; set; }
        //public Texture2D LifeTexture { get; set; }
        //public Texture2D PlatformTexture { get; set; }
        
        //public SoundEffect PlayerPowerUpSound { get; set; }
        //public SoundEffect GemCollectedSound { get; set; }

        /// <summary>
        /// Especifica si la musica esta siendo tocada
        /// </summary>
        public bool IsMusicPlaying { get; private set; }

        private Song _musicTheme;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        {
            // we must set EnabledGestures before we can query for them, but
            // we don't assume the game wants to read them.
#if WINDOWS_PHONE
            TouchPanel.EnabledGestures = GestureType.None;
#endif
        }


        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            _isInitialized = true;
        }


        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            var contentManager = Game.Content;

            if (contentManager == null)
			{
				throw new Exception("ContentManager not initialize");
			}

#if IPHONE
			GlobalParameters.SCREEN_WIDTH = GraphicsDevice.Viewport.Width;
			GlobalParameters.SCREEN_HEIGHT = GraphicsDevice.Viewport.Height;
			GlobalParameters.ENEMY_LIMIT_DISTANCE = GlobalParameters.SCREEN_WIDTH + GlobalParameters.SCREEN_WIDTH / 4;
#endif

			// Load content belonging to the screen manager.
            _blankTexture = contentManager.Load<Texture2D>("Menu/blank");

            SpriteFonts = new SpriteFonts(contentManager);

            SpriteBatch = new SpriteBatch(GraphicsDevice);            

            LoadAnimationData();

            SetWeaponMaxData();

            LoadScreenAssetsData(contentManager);

            //LoadGameContent(ContentManager);

            // Tell each of the screens to load their content.
            foreach (var screen in screens)
            {
                screen.LoadContent();
            }

#if WINDOWS && OPENGL
            _musicTheme = null;
#else
           _musicTheme = contentManager.Load<Song>(GlobalParameters.MUSIC_THEME);
#endif

#if !DEBUG
			PlayMusic();
#endif
        }

        /// <summary>
        /// Ejecuta la musica en funcion del parametro de configuracion
        /// </summary>
        public void PlayMusic()
        {
            //Known issue that you get exceptions if you use Media PLayer while connected to your PC
            //See http://social.msdn.microsoft.com/Forums/en/windowsphone7series/thread/c8a243d2-d360-46b1-96bd-62b1ef268c66
            //Which means its impossible to test this from VS.
            //So we have to catch the exception and throw it away
            try
            {

                if (_musicTheme == null)
                    return;

                // TODO: Reemplazar MediaPlayer con XACT. Leer para mas info http://stackoverflow.com/questions/3732070/xna-mediaplayer-volume-setter-being-extremely-slow
                // The only sure-fire way to make the problems go away is to not use MediaPlayer at all. You can put your music into XACT and compress it (I think you can use SoundEffect as well).
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(_musicTheme);
                MediaPlayer.Volume = 1.0f;

                IsMusicPlaying = true;
            }
            catch
            {
            }
        }

        /// <summary>
        /// Para la ejecucion de la musica
        /// </summary>
        public void StopMusic()
        {
            try
            {
                MediaPlayer.Stop();

                IsMusicPlaying = false;
            }
            catch
            {
            }
        }

        private void SetWeaponMaxData()
        {
            var weaponsList = GlobalParameters.GetWeaponList();

            foreach (var weapon in weaponsList)
            {
                if (weapon.Power > GlobalParameters.WeaponMaxPower)
                    GlobalParameters.WeaponMaxPower = weapon.Power;

                if (weapon.BulletVelocity > GlobalParameters.WeaponMaxBulletVelocity)
                    GlobalParameters.WeaponMaxBulletVelocity = weapon.BulletVelocity;

                if (weapon.MaxAmmo > GlobalParameters.WeaponMaxMaxAmmo)
                    GlobalParameters.WeaponMaxMaxAmmo = weapon.MaxAmmo;

                if (weapon.ReloadTime > GlobalParameters.WeaponMaxReloadTime)
                    GlobalParameters.WeaponMaxReloadTime = weapon.ReloadTime;

                if (weapon.FireRate > GlobalParameters.WeaponMaxFireRate)
                    GlobalParameters.WeaponMaxFireRate = weapon.FireRate;
            }
        }

        private void LoadScreenAssetsData(ContentManager contentManager)
        {
            ScreenAssetsTexture = contentManager.Load<Texture2D>(GlobalParameters.SCREEN_ASSETS_TEXTURE);
			GlobalParameters.ScreenAssetsData = Loader.LoadData(GlobalParameters.SCREEN_ASSETS_DATA, GlobalParameters.MAX_SCREEN_ASSETS_AVAILABLE);
        }

        public Texture2D ScreenAssetsTexture { get; private set; }

        ///// <summary>
        ///// Carga los datos de los graficos de las armas
        ///// </summary>
        //private void LoadWeaponGraphicData()
        //{
        //    GlobalParameters.WeaponsData = Loader.LoadData<WeaponType>(GlobalParameters.WEAPONS_ASSETS_DATA, GlobalParameters.MAX_WEAPONS_AVAILABLE);
        //}

        /// <summary>
        /// Carga los datos de las animaciones asi no tiene que cargarlas a cada rato
        /// La idea de cargar los datos aca es xq la lectura de un archivo de texto es lenta
        /// </summary>
        private void LoadAnimationData()
        {
            // TODO: revisar si no tarda mucho xq iphone mata al proceso si demora mas de X segs. Si anda lento se podria abrir el archivo 1 sola vez y reutilizarlo
            #region Load fwm
            GlobalParameters.FwmAnimationRectangulesIdle = Loader.LoadData(GlobalParameters.FAST_WEAK_MONSTER_IDLE_ANIMATION, GlobalParameters.FAST_WEAK_MONSTER_ANIMATION_DATA);
            GlobalParameters.FwmAnimationRectangulesRun = Loader.LoadData(GlobalParameters.FAST_WEAK_MONSTER_RUN_ANIMATION, GlobalParameters.FAST_WEAK_MONSTER_ANIMATION_DATA);
            GlobalParameters.FwmAnimationRectangulesDie = Loader.LoadData(GlobalParameters.FAST_WEAK_MONSTER_DIE_ANIMATION, GlobalParameters.FAST_WEAK_MONSTER_ANIMATION_DATA);
            GlobalParameters.FwmAnimationRectangulesHit = Loader.LoadData(GlobalParameters.FAST_WEAK_MONSTER_HIT_ANIMATION, GlobalParameters.FAST_WEAK_MONSTER_ANIMATION_DATA);
            GlobalParameters.FwmAnimationRectangulesAttack = Loader.LoadData(GlobalParameters.FAST_WEAK_MONSTER_ATTACK_ANIMATION, GlobalParameters.FAST_WEAK_MONSTER_ANIMATION_DATA);
            #endregion

            #region ssm
            GlobalParameters.SsmAnimationRectangulesIdle = Loader.LoadData(GlobalParameters.SLOW_STRONG_MONSTER_IDLE_ANIMATION, GlobalParameters.SLOW_STRONG_MONSTER_ANIMATION_DATA);
            GlobalParameters.SsmAnimationRectangulesRun = Loader.LoadData(GlobalParameters.SLOW_STRONG_MONSTER_RUN_ANIMATION, GlobalParameters.SLOW_STRONG_MONSTER_ANIMATION_DATA);
            GlobalParameters.SsmAnimationRectangulesDie = Loader.LoadData(GlobalParameters.SLOW_STRONG_MONSTER_DIE_ANIMATION, GlobalParameters.SLOW_STRONG_MONSTER_ANIMATION_DATA);
            GlobalParameters.SsmAnimationRectangulesHit = Loader.LoadData(GlobalParameters.SLOW_STRONG_MONSTER_HIT_ANIMATION, GlobalParameters.SLOW_STRONG_MONSTER_ANIMATION_DATA);
            GlobalParameters.SsmAnimationRectangulesAttack = Loader.LoadData(GlobalParameters.SLOW_STRONG_MONSTER_ATTACK_ANIMATION, GlobalParameters.SLOW_STRONG_MONSTER_ANIMATION_DATA);
            #endregion

            #region mn
            GlobalParameters.NmAnimationRectangulesIdle = Loader.LoadData(GlobalParameters.NORMAL_MONSTER_IDLE_ANIMATION, GlobalParameters.NORMAL_MONSTER_ANIMATION_DATA);
            GlobalParameters.NmAnimationRectangulesRun = Loader.LoadData(GlobalParameters.NORMAL_MONSTER_RUN_ANIMATION, GlobalParameters.NORMAL_MONSTER_ANIMATION_DATA);
            GlobalParameters.NmAnimationRectangulesDie = Loader.LoadData(GlobalParameters.NORMAL_MONSTER_DIE_ANIMATION, GlobalParameters.NORMAL_MONSTER_ANIMATION_DATA);
            GlobalParameters.NmAnimationRectangulesHit = Loader.LoadData(GlobalParameters.NORMAL_MONSTER_HIT_ANIMATION, GlobalParameters.NORMAL_MONSTER_ANIMATION_DATA);
            GlobalParameters.NmAnimationRectangulesAttack = Loader.LoadData(GlobalParameters.NORMAL_MONSTER_ATTACK_ANIMATION, GlobalParameters.NORMAL_MONSTER_ANIMATION_DATA);
            #endregion

            #region Obama
            GlobalParameters.ObamaAnimationRectangulesIdle = Loader.LoadData(GlobalParameters.OBAMA_IDLE_ANIMATION, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesIdle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.OBAMA_IDLE_ANIMATION));

            GlobalParameters.ObamaAnimationRectangulesRun = Loader.LoadData(GlobalParameters.OBAMA_RUN_ANIMATION, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesRun.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.OBAMA_RUN_ANIMATION));

            GlobalParameters.ObamaAnimationRectangulesDie = Loader.LoadData(GlobalParameters.OBAMA_DIE_ANIMATION, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesDie.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.OBAMA_DIE_ANIMATION));

            GlobalParameters.ObamaAnimationRectangulesBeAttacked = Loader.LoadData(GlobalParameters.OBAMA_BE_ATTACKED_ANIMATION, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesBeAttacked.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.OBAMA_BE_ATTACKED_ANIMATION));

            GlobalParameters.ObamaAnimationRectangulesAttack = Loader.LoadData(GlobalParameters.OBAMA_ATTACK_ANIMATION, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesAttack.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.OBAMA_ATTACK_ANIMATION));

            GlobalParameters.ObamaAnimationRectangulesCelebrate = Loader.LoadData(GlobalParameters.OBAMA_IDLE_ANIMATION, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesCelebrate.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.OBAMA_IDLE_ANIMATION));

            GlobalParameters.ObamaAnimationRectangulesPistolaBerettaM9Idle = Loader.LoadData(GlobalParameters.PistolaBerettaM9IdleAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesPistolaBerettaM9Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.PistolaBerettaM9IdleAnimation));

            GlobalParameters.ObamaAnimationRectangulesPistolaBerettaM9Shoot = Loader.LoadData(GlobalParameters.PistolaBerettaM9ShotAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesPistolaBerettaM9Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.PistolaBerettaM9ShotAnimation));

            GlobalParameters.ObamaAnimationRectangulesPistolaColtM1911Idle = Loader.LoadData(GlobalParameters.PistolaColtM1911IdleAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesPistolaColtM1911Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.PistolaColtM1911IdleAnimation));
            
            GlobalParameters.ObamaAnimationRectangulesPistolaColtM1911Shoot = Loader.LoadData(GlobalParameters.PistolaColtM1911ShotAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesPistolaColtM1911Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.PistolaColtM1911ShotAnimation));

            GlobalParameters.ObamaAnimationRectangulesPistolaColtPythonIdle = Loader.LoadData(GlobalParameters.PistolaColtPythonIdleAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesPistolaColtPythonIdle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.PistolaColtPythonIdleAnimation));

            GlobalParameters.ObamaAnimationRectangulesPistolaColtPythonShoot = Loader.LoadData(GlobalParameters.PistolaColtPythonShotAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesPistolaColtPythonShoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.PistolaColtPythonShotAnimation));

            GlobalParameters.ObamaAnimationRectangulesEscopetaBeretta682Idle = Loader.LoadData(GlobalParameters.EscopetaBeretta682IdleAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesEscopetaBeretta682Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.EscopetaBeretta682IdleAnimation));

            GlobalParameters.ObamaAnimationRectangulesEscopetaBeretta682Shoot = Loader.LoadData(GlobalParameters.EscopetaBeretta682ShotAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesEscopetaBeretta682Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.EscopetaBeretta682ShotAnimation));

            GlobalParameters.ObamaAnimationRectangulesEscopetaIthaca37Idle = Loader.LoadData(GlobalParameters.EscopetaIthaca37IdleAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesEscopetaIthaca37Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.EscopetaIthaca37IdleAnimation));

            GlobalParameters.ObamaAnimationRectangulesEscopetaIthaca37Shoot = Loader.LoadData(GlobalParameters.EscopetaIthaca37ShotAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesEscopetaIthaca37Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.EscopetaIthaca37ShotAnimation));

            GlobalParameters.ObamaAnimationRectangulesEscopetaSpas12Idle = Loader.LoadData(GlobalParameters.EscopetaSpas12IdleAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesEscopetaSpas12Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.EscopetaSpas12IdleAnimation));

            GlobalParameters.ObamaAnimationRectangulesEscopetaSpas12Shoot = Loader.LoadData(GlobalParameters.EscopetaSpas12ShotAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesEscopetaSpas12Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.EscopetaSpas12ShotAnimation));

            GlobalParameters.ObamaAnimationRectangulesSubFusilMp5KIdle = Loader.LoadData(GlobalParameters.SubFusilMp5KIdleAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesSubFusilMp5KIdle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.SubFusilMp5KIdleAnimation));

            GlobalParameters.ObamaAnimationRectangulesSubFusilMp5KShoot = Loader.LoadData(GlobalParameters.SubFusilMp5KShotAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesSubFusilMp5KShoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.SubFusilMp5KShotAnimation));

            GlobalParameters.ObamaAnimationRectangulesSubFusilMp40Idle = Loader.LoadData(GlobalParameters.SubFusilMp40IdleAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesSubFusilMp40Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.SubFusilMp40IdleAnimation));

            GlobalParameters.ObamaAnimationRectangulesSubFusilMp40Shoot = Loader.LoadData(GlobalParameters.SubFusilMp40ShotAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesSubFusilMp40Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.SubFusilMp40ShotAnimation));

            GlobalParameters.ObamaAnimationRectangulesSubFusilUziIdle = Loader.LoadData(GlobalParameters.SubFusilUziIdleAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesSubFusilUziIdle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.SubFusilUziIdleAnimation));

            GlobalParameters.ObamaAnimationRectangulesSubFusilUziShoot = Loader.LoadData(GlobalParameters.SubFusilUziShotAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesSubFusilUziShoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.SubFusilUziShotAnimation));

            GlobalParameters.ObamaAnimationRectangulesFusilAk47Idle = Loader.LoadData(GlobalParameters.FusilAk47IdleAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesFusilAk47Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.FusilAk47IdleAnimation));

            GlobalParameters.ObamaAnimationRectangulesFusilAk47Shoot = Loader.LoadData(GlobalParameters.FusilAk47ShotAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesFusilAk47Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.FusilAk47ShotAnimation));

            GlobalParameters.ObamaAnimationRectangulesFusilM4A1Idle = Loader.LoadData(GlobalParameters.FusilM4A1IdleAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesFusilM4A1Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.FusilM4A1IdleAnimation));

            GlobalParameters.ObamaAnimationRectangulesFusilM4A1Shoot = Loader.LoadData(GlobalParameters.FusilM4A1ShotAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesFusilM4A1Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.FusilM4A1ShotAnimation));

            GlobalParameters.ObamaAnimationRectangulesFusilXm8Idle = Loader.LoadData(GlobalParameters.FusilXm8IdleAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesFusilXm8Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.FusilXm8IdleAnimation));

            GlobalParameters.ObamaAnimationRectangulesFusilXm8Shoot = Loader.LoadData(GlobalParameters.FusilXm8ShotAnimation, GlobalParameters.OBAMA_ANIMATION_DATA);
            if (GlobalParameters.ObamaAnimationRectangulesFusilXm8Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Obama {0} ", GlobalParameters.FusilXm8ShotAnimation));

            GlobalParameters.ObamaWeaponAnimationPosition = LoadObamaWeaponAnimationPosition();
            #endregion

            #region Gordo Mercenario
            GlobalParameters.GordoAnimationRectangulesIdle = Loader.LoadData(GlobalParameters.GORDO_MERCENARIO_IDLE_ANIMATION, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            if (GlobalParameters.GordoAnimationRectangulesIdle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.GORDO_MERCENARIO_IDLE_ANIMATION));

            GlobalParameters.GordoAnimationRectangulesRun = Loader.LoadData(GlobalParameters.GORDO_MERCENARIO_RUN_ANIMATION, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            if (GlobalParameters.GordoAnimationRectangulesRun.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.GORDO_MERCENARIO_RUN_ANIMATION));
            
            GlobalParameters.GordoAnimationRectangulesDie = Loader.LoadData(GlobalParameters.GORDO_MERCENARIO_DIE_ANIMATION, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            if (GlobalParameters.GordoAnimationRectangulesDie.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.GORDO_MERCENARIO_DIE_ANIMATION));

            GlobalParameters.GordoAnimationRectangulesBeAttacked = Loader.LoadData(GlobalParameters.GORDO_MERCENARIO_BE_ATTACKED_ANIMATION, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            if (GlobalParameters.GordoAnimationRectangulesBeAttacked.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.GORDO_MERCENARIO_BE_ATTACKED_ANIMATION));

            GlobalParameters.GordoAnimationRectangulesAttack = Loader.LoadData(GlobalParameters.GORDO_MERCENARIO_ATTACK_ANIMATION, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            if (GlobalParameters.GordoAnimationRectangulesAttack.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.GORDO_MERCENARIO_ATTACK_ANIMATION));
            
            GlobalParameters.GordoAnimationRectangulesCelebrate = Loader.LoadData(GlobalParameters.GORDO_MERCENARIO_IDLE_ANIMATION, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            if (GlobalParameters.GordoAnimationRectangulesCelebrate.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.GORDO_MERCENARIO_IDLE_ANIMATION));

            //GlobalParameters.GordoAnimationRectangulesPistolaBerettaM9Idle = Loader.LoadData(GlobalParameters.PistolaBerettaM9IdleAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesPistolaBerettaM9Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.PistolaBerettaM9IdleAnimation));

            //GlobalParameters.GordoAnimationRectangulesPistolaBerettaM9Shoot = Loader.LoadData(GlobalParameters.PistolaBerettaM9ShotAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesPistolaBerettaM9Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.PistolaBerettaM9ShotAnimation));

            //GlobalParameters.GordoAnimationRectangulesPistolaColtM1911Idle = Loader.LoadData(GlobalParameters.PistolaColtM1911IdleAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesPistolaColtM1911Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.PistolaColtM1911IdleAnimation));

            //GlobalParameters.GordoAnimationRectangulesPistolaColtM1911Shoot = Loader.LoadData(GlobalParameters.PistolaColtM1911ShotAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesPistolaColtM1911Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.PistolaColtM1911ShotAnimation));

            //GlobalParameters.GordoAnimationRectangulesPistolaColtPythonIdle = Loader.LoadData(GlobalParameters.PistolaColtPythonIdleAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesPistolaColtPythonIdle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.PistolaColtPythonIdleAnimation));

            //GlobalParameters.GordoAnimationRectangulesPistolaColtPythonShoot = Loader.LoadData(GlobalParameters.PistolaColtPythonShotAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesPistolaColtPythonShoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.PistolaColtPythonShotAnimation));

            //GlobalParameters.GordoAnimationRectangulesEscopetaBeretta682Idle = Loader.LoadData(GlobalParameters.EscopetaBeretta682IdleAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesEscopetaBeretta682Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.EscopetaBeretta682IdleAnimation));

            //GlobalParameters.GordoAnimationRectangulesEscopetaBeretta682Shoot = Loader.LoadData(GlobalParameters.EscopetaBeretta682ShotAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesEscopetaBeretta682Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.EscopetaBeretta682ShotAnimation));

            //GlobalParameters.GordoAnimationRectangulesEscopetaIthaca37Idle = Loader.LoadData(GlobalParameters.EscopetaIthaca37IdleAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesEscopetaIthaca37Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.EscopetaIthaca37IdleAnimation));

            //GlobalParameters.GordoAnimationRectangulesEscopetaIthaca37Shoot = Loader.LoadData(GlobalParameters.EscopetaIthaca37ShotAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesEscopetaIthaca37Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.EscopetaIthaca37ShotAnimation));

            //GlobalParameters.GordoAnimationRectangulesEscopetaSpas12Idle = Loader.LoadData(GlobalParameters.EscopetaSpas12IdleAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesEscopetaSpas12Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.EscopetaSpas12IdleAnimation));

            //GlobalParameters.GordoAnimationRectangulesEscopetaSpas12Shoot = Loader.LoadData(GlobalParameters.EscopetaSpas12ShotAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesEscopetaSpas12Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.EscopetaSpas12ShotAnimation));

            //GlobalParameters.GordoAnimationRectangulesSubFusilMp5KIdle = Loader.LoadData(GlobalParameters.SubFusilMp5KIdleAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesSubFusilMp5KIdle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.SubFusilMp5KIdleAnimation));

            //GlobalParameters.GordoAnimationRectangulesSubFusilMp5KShoot = Loader.LoadData(GlobalParameters.SubFusilMp5KShotAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesSubFusilMp5KShoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.SubFusilMp5KShotAnimation));

            //GlobalParameters.GordoAnimationRectangulesSubFusilMp40Idle = Loader.LoadData(GlobalParameters.SubFusilMp40IdleAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesSubFusilMp40Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.SubFusilMp40IdleAnimation));

            //GlobalParameters.GordoAnimationRectangulesSubFusilMp40Shoot = Loader.LoadData(GlobalParameters.SubFusilMp40ShotAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesSubFusilMp40Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.SubFusilMp40ShotAnimation));

            //GlobalParameters.GordoAnimationRectangulesSubFusilUziIdle = Loader.LoadData(GlobalParameters.SubFusilUziIdleAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesSubFusilUziIdle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.SubFusilUziIdleAnimation));

            //GlobalParameters.GordoAnimationRectangulesSubFusilUziShoot = Loader.LoadData(GlobalParameters.SubFusilUziShotAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesSubFusilUziShoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.SubFusilUziShotAnimation));

            //GlobalParameters.GordoAnimationRectangulesFusilAk47Idle = Loader.LoadData(GlobalParameters.FusilAk47IdleAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesFusilAk47Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.FusilAk47IdleAnimation));

            //GlobalParameters.GordoAnimationRectangulesFusilAk47Shoot = Loader.LoadData(GlobalParameters.FusilAk47ShotAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesFusilAk47Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.FusilAk47ShotAnimation));

            //GlobalParameters.GordoAnimationRectangulesFusilM4A1Idle = Loader.LoadData(GlobalParameters.FusilM4A1IdleAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesFusilM4A1Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.FusilM4A1IdleAnimation));

            //GlobalParameters.GordoAnimationRectangulesFusilM4A1Shoot = Loader.LoadData(GlobalParameters.FusilM4A1ShotAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesFusilM4A1Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.FusilM4A1ShotAnimation));

            //GlobalParameters.GordoAnimationRectangulesFusilXm8Idle = Loader.LoadData(GlobalParameters.FusilXm8IdleAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesFusilXm8Idle.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.FusilXm8IdleAnimation));

            //GlobalParameters.GordoAnimationRectangulesFusilXm8Shoot = Loader.LoadData(GlobalParameters.FusilXm8ShotAnimation, GlobalParameters.GORDO_MERCENARIO_ANIMATION_DATA);
            //if (GlobalParameters.GordoAnimationRectangulesFusilXm8Shoot.Length == 0) throw new Exception(string.Format("No se encuentran datos para Gordo {0} ", GlobalParameters.FusilXm8ShotAnimation));
            #endregion
        }

        private Dictionary<string, Vector2> LoadObamaWeaponAnimationPosition()
        {
            // GlobalParameters.MAX_WEAPONS_AVAILABLE * 2 xq carga la position en Idle y en Shoot
            var animationPosition = Loader.LoadDataToVector(GlobalParameters.OBAMA_WEAPOND_ANIMATION_DATA,
                                                            GlobalParameters.MAX_WEAPONS_AVAILABLE*4);

            //// GlobalParameters.MAX_WEAPONS_AVAILABLE * 4 xq carga la position en Idle, en Shoot para IdleAnimation y para RunAnimation
            //var animationPosition = new Dictionary<string, Vector2>(GlobalParameters.MAX_WEAPONS_AVAILABLE * 4);

//#if IPHONE
//            throw new NotImplementedException();
//#else
//            // PistolaBerettaM9 OK
//            animationPosition.Add(GlobalParameters.PistolaBerettaM9IdleAnimation, new Vector2(22, 94));
//            animationPosition.Add(GlobalParameters.PistolaBerettaM9ShotAnimation, new Vector2(40, 108));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.PistolaBerettaM9IdleAnimation, "Run"), new Vector2(30, 96));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.PistolaBerettaM9ShotAnimation, "Run"), new Vector2(48, 110));

//            // PistolaColtM1911 OK
//            animationPosition.Add(GlobalParameters.PistolaColtM1911IdleAnimation, new Vector2(22, 90));
//            animationPosition.Add(GlobalParameters.PistolaColtM1911ShotAnimation, new Vector2(22, 108));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.PistolaColtM1911IdleAnimation, "Run"), new Vector2(30, 94));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.PistolaColtM1911ShotAnimation, "Run"), new Vector2(30, 112));
            
//            // PistolaColtPython OK
//            animationPosition.Add(GlobalParameters.PistolaColtPythonIdleAnimation, new Vector2(8, 66));
//            animationPosition.Add(GlobalParameters.PistolaColtPythonShotAnimation, new Vector2(64, 134));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.PistolaColtPythonIdleAnimation, "Run"), new Vector2(16, 70));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.PistolaColtPythonShotAnimation, "Run"), new Vector2(72, 138));

//            // EscopetaBeretta682 OK
//            animationPosition.Add(GlobalParameters.EscopetaBeretta682IdleAnimation, new Vector2(-8, 140));
//            animationPosition.Add(GlobalParameters.EscopetaBeretta682ShotAnimation, new Vector2(40, 114));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.EscopetaBeretta682IdleAnimation, "Run"), new Vector2(0, 144));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.EscopetaBeretta682ShotAnimation, "Run"), new Vector2(48, 118));

//            // EscopetaIthaca37 OK
//            animationPosition.Add(GlobalParameters.EscopetaIthaca37IdleAnimation, new Vector2(2, 136));
//            animationPosition.Add(GlobalParameters.EscopetaIthaca37ShotAnimation, new Vector2(48, 98));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.EscopetaIthaca37IdleAnimation, "Run"), new Vector2(10, 140)); // +8 +4
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.EscopetaIthaca37ShotAnimation, "Run"), new Vector2(56, 102)); // +8 +4

//            // EscopetaSpas12 OK
//            animationPosition.Add(GlobalParameters.EscopetaSpas12IdleAnimation, new Vector2(-14, 130));
//            animationPosition.Add(GlobalParameters.EscopetaSpas12ShotAnimation, new Vector2(32, 94));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.EscopetaSpas12IdleAnimation, "Run"), new Vector2(-6, 132));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.EscopetaSpas12ShotAnimation, "Run"), new Vector2(40, 98));

//            // SubFusilMp5K OK
//            animationPosition.Add(GlobalParameters.SubFusilMp5KIdleAnimation, new Vector2(32, 76));
//            animationPosition.Add(GlobalParameters.SubFusilMp5KShotAnimation, new Vector2(24, 88));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.SubFusilMp5KIdleAnimation, "Run"), new Vector2(40, 80));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.SubFusilMp5KShotAnimation, "Run"), new Vector2(32, 92));

//            //SubFusilMp40 OK
//            animationPosition.Add(GlobalParameters.SubFusilMp40IdleAnimation, new Vector2(30, 60));
//            animationPosition.Add(GlobalParameters.SubFusilMp40ShotAnimation, new Vector2(32, 82));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.SubFusilMp40IdleAnimation, "Run"), new Vector2(38, 62));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.SubFusilMp40ShotAnimation, "Run"), new Vector2(40, 86));

//            // SubFusilUzi OK
//            animationPosition.Add(GlobalParameters.SubFusilUziIdleAnimation, new Vector2(-8, 36));
//            animationPosition.Add(GlobalParameters.SubFusilUziShotAnimation, new Vector2(44, 92));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.SubFusilUziIdleAnimation, "Run"), new Vector2(0, 40));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.SubFusilUziShotAnimation, "Run"), new Vector2(52, 96));

//            // FusilAk47 OK
//            animationPosition.Add(GlobalParameters.FusilAk47IdleAnimation, new Vector2(20, 40));
//            animationPosition.Add(GlobalParameters.FusilAk47ShotAnimation, new Vector2(38, 90));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.FusilAk47IdleAnimation, "Run"), new Vector2(28, 44));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.FusilAk47ShotAnimation, "Run"), new Vector2(44, 94));

//            // FusilM4A1 OK
//            animationPosition.Add(GlobalParameters.FusilM4A1IdleAnimation, new Vector2(22, 62));
//            animationPosition.Add(GlobalParameters.FusilM4A1ShotAnimation, new Vector2(24, 96));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.FusilM4A1IdleAnimation, "Run"), new Vector2(30, 66));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.FusilM4A1ShotAnimation, "Run"), new Vector2(32, 100));

//            // FusilXm8 OK
//            animationPosition.Add(GlobalParameters.FusilXm8IdleAnimation, new Vector2(10, 38));
//            animationPosition.Add(GlobalParameters.FusilXm8ShotAnimation, new Vector2(58, 112));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.FusilXm8IdleAnimation, "Run"), new Vector2(18, 42));
//            animationPosition.Add(string.Format("{0}_{1}", GlobalParameters.FusilXm8ShotAnimation, "Run"), new Vector2(66, 116));
//#endif

            return animationPosition;
        }

        //public ContentManager ContentManager { get; private set; }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in screens)
            {
                screen.UnloadContent();
            }
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            input.Update();

            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            screensToUpdate.Clear();

            foreach (var screen in screens)
                screensToUpdate.Add(screen);

            var otherScreenHasFocus = !Game.IsActive;
            var coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                var screen = screensToUpdate[screensToUpdate.Count - 1];

                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }

            // Print debug trace?
            if (TraceEnabled)
                TraceScreens();
        }


        /// <summary>
        /// Prints a list of all the screens, for debugging.
        /// </summary>
        void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (GameScreen screen in screens)
                screenNames.Add(screen.GetType().Name);

            Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
        }


        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }


        #endregion

        #region Public Methods


        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.ScreenManager = this;
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (_isInitialized)
            {
                screen.LoadContent();
            }

            screens.Add(screen);

            // update the TouchPanel to respond to gestures this screen is interested in
//#if WINDOWS_PHONE || IPHONE
//            TouchPanel.EnabledGestures = screen.EnabledGestures;
//#endif
        }


        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (_isInitialized)
            {
                screen.UnloadContent();
            }

            screens.Remove(screen);
            screensToUpdate.Remove(screen);

//#if WINDOWS_PHONE || IPHONE
//            // if there is a screen still in the manager, update TouchPanel
//            // to respond to gestures that screen is interested in.
//            if (screens.Count > 0)
//            {
//                TouchPanel.EnabledGestures = screens[screens.Count - 1].EnabledGestures;
//            }
//#endif
        }


        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }


        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            SpriteBatch.Begin();

            SpriteBatch.Draw(_blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black * alpha);

            SpriteBatch.End();
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Informs the screen manager to serialize its state to disk.
        /// </summary>
        public void SerializeState()
        {
            // open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // if our screen manager directory already exists, delete the contents
                if (storage.DirectoryExists("ScreenManager"))
                {
                    DeleteState(storage);
                }

                // otherwise just create the directory
                else
                {
                    storage.CreateDirectory("ScreenManager");
                }

                // create a file we'll use to store the list of screens in the stack
                using (IsolatedStorageFileStream stream = storage.CreateFile("ScreenManager\\ScreenList.dat"))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        // write out the full name of all the types in our stack so we can
                        // recreate them if needed.
                        foreach (GameScreen screen in screens)
                        {
                            if (screen.IsSerializable)
                            {
                                writer.Write(screen.GetType().AssemblyQualifiedName);
                            }
                        }
                    }
                }

                // now we create a new file stream for each screen so it can save its state
                // if it needs to. we name each file "ScreenX.dat" where X is the index of
                // the screen in the stack, to ensure the files are uniquely named
                int screenIndex = 0;
                foreach (GameScreen screen in screens)
                {
                    if (screen.IsSerializable)
                    {
                        string fileName = string.Format("ScreenManager\\Screen{0}.dat", screenIndex);

                        // open up the stream and let the screen serialize whatever state it wants
                        using (IsolatedStorageFileStream stream = storage.CreateFile(fileName))
                        {
                            screen.Serialize(stream);
                        }

                        screenIndex++;
                    }
                }
            }
        }

        public bool DeserializeState()
        {
            // open up isolated storage
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // see if our saved state directory exists
                if (storage.DirectoryExists("ScreenManager"))
                {
                    try
                    {
                        // see if we have a screen list
                        if (storage.FileExists("ScreenManager\\ScreenList.dat"))
                        {
                            // load the list of screen types
                            using (IsolatedStorageFileStream stream = storage.OpenFile("ScreenManager\\ScreenList.dat", FileMode.Open, FileAccess.Read))
                            {
                                using (BinaryReader reader = new BinaryReader(stream))
                                {
                                    while (reader.BaseStream.Position < reader.BaseStream.Length)
                                    {
                                        // read a line from our file
                                        string line = reader.ReadString();

                                        // if it isn't blank, we can create a screen from it
                                        if (!string.IsNullOrEmpty(line))
                                        {
                                            Type screenType = Type.GetType(line);
                                            GameScreen screen = Activator.CreateInstance(screenType) as GameScreen;
                                            AddScreen(screen, PlayerIndex.One);
                                        }
                                    }
                                }
                            }
                        }

                        // next we give each screen a chance to deserialize from the disk
                        for (int i = 0; i < screens.Count; i++)
                        {
                            string filename = string.Format("ScreenManager\\Screen{0}.dat", i);
                            using (IsolatedStorageFileStream stream = storage.OpenFile(filename, FileMode.Open, FileAccess.Read))
                            {
                                screens[i].Deserialize(stream);
                            }
                        }

                        return true;
                    }
                    catch (Exception)
                    {
                        // if an exception was thrown while reading, odds are we cannot recover
                        // from the saved state, so we will delete it so the game can correctly
                        // launch.
                        DeleteState(storage);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Deletes the saved state files from isolated storage.
        /// </summary>
        private void DeleteState(IsolatedStorageFile storage)
        {
            // get all of the files in the directory and delete them
            string[] files = storage.GetFileNames("ScreenManager\\*");
            foreach (string file in files)
            {
                storage.DeleteFile(Path.Combine("ScreenManager", file));
            }
        }
#endif

        #endregion

        //#region Private Methods
        //private void LoadGameContent(ContentManager contentManager)
        //{
        //    #region Load Sounds
        //    GemCollectedSound = contentManager.Load<SoundEffect>("Sounds/GemCollected");
        //    PlayerPowerUpSound = contentManager.Load<SoundEffect>("Sounds/PowerUp");
        //    #endregion

        //    #region Load Extra Sprites
        //    LifeTexture = contentManager.Load<Texture2D>("Sprites/Life");
        //    GemTexture = contentManager.Load<Texture2D>("Sprites/Gem");
        //    #endregion
        //}
        //#endregion
    }
}

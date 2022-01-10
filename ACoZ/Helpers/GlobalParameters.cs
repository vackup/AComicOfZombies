using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platformer.Players;
using Platformer.Weapons;

namespace Platformer.Helpers
{
    class GlobalParameters
    {
        #region Parametros globales

#if WINDOWS && NETFX_CORE
        public const string CONTENT_FOLDER = "Content_W8";
#else
         public const string CONTENT_FOLDER  = "Content";
#endif

        /// <summary>
        /// Cancion
        /// </summary>
        public const string MUSIC_THEME = "Sounds/Music";

        /// <summary>
        /// Especifica cuando se ganara la primer vida
        /// </summary>
        public const int INITIAL_NEXT_LIFE = 1000;

        /// <summary>
        /// Especifica el numero maximo de pantallas que va a tener el juego ejecutandose en el momento (x ej: la pantalla de juego + pantalla de pausa + pantalla de confirmacion)
        /// </summary>
        public const int MAX_GAMESCREENS = 5;

        /// <summary>
        /// Especifica el numero maximo de enemigos que pueden aparecer en pantalla en el juego
        /// Este numero hay que multiplicarlo por la cantidad de tipos distintos de enemigos 
        /// y dara cuantos enemigos como maximo va a haber en pantalla
        /// </summary>
        public const int MAX_ENEMIES_POOL = 4;


#if IPHONE
        public static int SCREEN_WIDTH;
		public static int SCREEN_HEIGHT;
#else
        /// <summary>
        /// Establece el ancho de la resolucion de la pantalla. Es util definirlo para los distintos dispositivos posibles.
        /// </summary>
        public const int SCREEN_WIDTH = 960;

        /// <summary>
        /// Establece el alto de la resolucion de la pantalla. Es util definirlo para los distintos dispositivos posibles.
        /// </summary>
        public const int SCREEN_HEIGHT = 640;
#endif

        /// <summary>
        /// Establece cuantos caracteres numericos tendran los datos de la info de las animaciones, por ej:
        /// ANIMATION_NUMERICAL_CHARS = 4 establece 4 caracteres numericos:
        /// - Idle0001
        /// - Idle0002
        /// - Idle0003
        /// </summary>
        public const int ANIMATION_NUMERICAL_CHARS = 4;

        /// <summary>
        /// Establece el limite para que los enemigos desaparezcan de la pantalla (diferencia entre el player y el enemigo)
        /// </summary>
#if IPHONE
		public static int ENEMY_LIMIT_DISTANCE;
#else
		public const int ENEMY_LIMIT_DISTANCE = SCREEN_WIDTH + SCREEN_WIDTH / 4;
#endif
        #endregion

        #region Controles
#if IPHONE
		public static int TOP_MARGIN = 15 * IOsUtils.ScaleFactor;
		public static int BOTTOM_MARGIN = 15 * IOsUtils.ScaleFactor;
		public static int RIGHT_MARGIN = 15 * IOsUtils.ScaleFactor;
		public static int LEFT_MARGIN = 15 * IOsUtils.ScaleFactor;
#else
        public const int TOP_MARGIN = 30;
        public const int BOTTOM_MARGIN = 30;
        public const int RIGHT_MARGIN = 30;
        public const int LEFT_MARGIN = 30;
#endif

#if IPHONE && FREE
		public static int BANNER_MARGIN = 24 * IOsUtils.ScaleFactor;
#else
		public const int BANNER_MARGIN = 0;
#endif	
        #endregion

        #region Level
        /// <summary>
        /// Especifica la cantidad de jugadores que hay para elegir en el juego
        /// </summary>
        public const int MAX_SELECT_PLAYERS = 2;

        /// <summary>
        /// Especifica la cantidad de puntos que se suman x segundo remanente de juego
        /// </summary>
        public const int POINTS_PER_SECOND = 2;
        
        // Iphone = 480x320 - Tile = 40x32 - Lineas = 10 (12 columnas x pantalla)
        // Silverlight / WINDOWS = 960x640 - Tile = 80x64 - Lineas = 10 (12 columnas x pantalla)
        // WP = 800x480 - Tile = 40x32 = Lineas = 15 (20 columnas x pantalla)
#if IPHONE
		public static int TILE_WITH = 40 * IOsUtils.ScaleFactor;
		public static int TILE_HEIGHT = 32 * IOsUtils.ScaleFactor;
#else //  WINDOWS
        public const int TILE_WITH = 80;
        public const int TILE_HEIGHT = 64;
#endif
        // Inicialmente el nivel 1 va a tener 3 pantallas
        // y x cada nivel se van a ir agregando 2 pantallas mas
        public const int INITIAL_SCREENS = 3;
        public const int ADD_SCREEN_PER_LEVEL = 2;
        
        /// <summary>
        /// Indica cada cuantos niveles se va a agregar un tipo de enemigo nuevo
        /// </summary>
        public const int ADD_ENEMY_TYPE_EVERY_X_LEVEL = 5;

        /// <summary>
        /// Indica cada cuantos niveles se va a sumar un enemigos simultaneo
        /// </summary>
        public const int ADD_ENEMY_TO_POOL_EVERY_X_LEVEL = 5;

        /// <summary>
        /// Indica la cantidad de enemigos iniciales que habra en el nivel
        /// </summary>
        public const int INITIAL_ENEMY_POOL = 2;

        /// <summary>
        /// Especifica el numero de tipos de enemigos totales que existen en el juego
        /// </summary>
        public const int MAX_ENEMIES_AVAILABLE = 3;

        /// <summary>
        /// Tiempo base desde cuando podran nacer los enemigos
        /// </summary>
        public const double ENEMY_SPAWN_TIME_FROM_BASE = 1.0;

        /// <summary>
        /// Tiempo techo desde cuando podran nacer los enemigos
        /// </summary>
        public const double ENEMY_SPAWN_TIME_FROM_TOP = 2.5;

        /// <summary>
        /// Tiempo base hasta cuando podran nacer los enemigos
        /// </summary>
        public const double ENEMY_SPAWN_TIME_TO_BASE = 2.75;

        /// <summary>
        /// Tiempo techo hasta cuando podran nacer los enemigos
        /// </summary>
        public const double ENEMY_SPAWN_TIME_TO_TOP = 4.5;

	    /// <summary>
	    /// The EXIT TILE at the end of each level.
	    /// </summary>
		public const string EXIT_TILE = "Tiles/Exit";
        #endregion

        #region Fonts and Menu
        public const string TEXT_FONT = "Fonts/TextFont";
        public const string TEXT_FONT_SMALL = "Fonts/TextFontSmall";
        public const string TITTLE_FONT = "Fonts/TitleFont";
        public const string TITTLE_FONT_MEDIUM = "Fonts/TitleFontMedium";
        public const string TITTLE_FONT_SMALL = "Fonts/TitleFontSmall";

#if IPHONE
        public const string MENU_BACKGROUND = "Menu/background";
#else
        public const string MENU_BACKGROUND = "Menu/background@2x";
#endif
		#endregion
		
        #region Weapons
        public const string PISTOLA_FIRE_SOUND = "Sounds/Weapons/m1911";
        public const string PISTOLA_RELOAD_SOUND = "Sounds/Weapons/rm1911";
        
        public const string ESCOPETA_FIRE_SOUND = "Sounds/Weapons/ithaca";
        public const string ESCOPETA_RELOAD_SOUND = "Sounds/Weapons/rg3";

        public const string SUBFUSIL_FIRE_SOUND = "Sounds/Weapons/uzi";
        public const string SUBFUSIL_RELOAD_SOUND = "Sounds/Weapons/ruzi";
        
        public const string FUSIL_FIRE_SOUND = "Sounds/Weapons/xm8";
        public const string FUSIL_RELOAD_SOUND = "Sounds/Weapons/rxm8";

        public const string WEAPON_EMPTY_SOUND = "Sounds/Weapons/NoBullets";

        public static readonly string PistolaBerettaM9IdleAnimation = String.Format("{0}_{1}", WeaponType.PistolaBerettaM9, WeaponAnimationType.Idle);
        public static readonly string PistolaBerettaM9ShotAnimation = String.Format("{0}_{1}", WeaponType.PistolaBerettaM9, WeaponAnimationType.Shoot);

        public static readonly string PistolaColtM1911IdleAnimation = String.Format("{0}_{1}", WeaponType.PistolaColtM1911, WeaponAnimationType.Idle);
        public static readonly string PistolaColtM1911ShotAnimation = String.Format("{0}_{1}", WeaponType.PistolaColtM1911, WeaponAnimationType.Shoot);

        public static readonly string PistolaColtPythonIdleAnimation = String.Format("{0}_{1}", WeaponType.PistolaColtPython, WeaponAnimationType.Idle);
        public static readonly string PistolaColtPythonShotAnimation = String.Format("{0}_{1}", WeaponType.PistolaColtPython, WeaponAnimationType.Shoot);

        public static readonly string EscopetaBeretta682IdleAnimation = String.Format("{0}_{1}", WeaponType.EscopetaBeretta682, WeaponAnimationType.Idle);
        public static readonly string EscopetaBeretta682ShotAnimation = String.Format("{0}_{1}", WeaponType.EscopetaBeretta682, WeaponAnimationType.Shoot);

        public static readonly string EscopetaIthaca37IdleAnimation = String.Format("{0}_{1}", WeaponType.EscopetaIthaca37, WeaponAnimationType.Idle);
        public static readonly string EscopetaIthaca37ShotAnimation = String.Format("{0}_{1}", WeaponType.EscopetaIthaca37, WeaponAnimationType.Shoot);

        public static readonly string EscopetaSpas12IdleAnimation = String.Format("{0}_{1}", WeaponType.EscopetaSpas12, WeaponAnimationType.Idle);
        public static readonly string EscopetaSpas12ShotAnimation = String.Format("{0}_{1}", WeaponType.EscopetaSpas12, WeaponAnimationType.Shoot);

        public static readonly string SubFusilMp5KIdleAnimation = String.Format("{0}_{1}", WeaponType.SubFusilMp5K, WeaponAnimationType.Idle);
        public static readonly string SubFusilMp5KShotAnimation = String.Format("{0}_{1}", WeaponType.SubFusilMp5K, WeaponAnimationType.Shoot);

        public static readonly string SubFusilMp40IdleAnimation = String.Format("{0}_{1}", WeaponType.SubFusilMp40, WeaponAnimationType.Idle);
        public static readonly string SubFusilMp40ShotAnimation = String.Format("{0}_{1}", WeaponType.SubFusilMp40, WeaponAnimationType.Shoot);

        public static readonly string SubFusilUziIdleAnimation = String.Format("{0}_{1}", WeaponType.SubFusilUzi, WeaponAnimationType.Idle);
        public static readonly string SubFusilUziShotAnimation = String.Format("{0}_{1}", WeaponType.SubFusilUzi, WeaponAnimationType.Shoot);

        public static readonly string FusilAk47IdleAnimation = String.Format("{0}_{1}", WeaponType.FusilAk47, WeaponAnimationType.Idle);
        public static readonly string FusilAk47ShotAnimation = String.Format("{0}_{1}", WeaponType.FusilAk47, WeaponAnimationType.Shoot);

        public static readonly string FusilM4A1IdleAnimation = String.Format("{0}_{1}", WeaponType.FusilM4A1, WeaponAnimationType.Idle);
        public static readonly string FusilM4A1ShotAnimation = String.Format("{0}_{1}", WeaponType.FusilM4A1, WeaponAnimationType.Shoot);

        public static readonly string FusilXm8IdleAnimation = String.Format("{0}_{1}", WeaponType.FusilXm8, WeaponAnimationType.Idle);
        public static readonly string FusilXm8ShotAnimation = String.Format("{0}_{1}", WeaponType.FusilXm8, WeaponAnimationType.Shoot);
        #endregion

        #region Obama
        public static Rectangle[] ObamaAnimationRectangulesIdle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesRun { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesDie { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesBeAttacked { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesAttack { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesCelebrate { get; set; }

        public static Rectangle[] ObamaAnimationRectangulesPistolaBerettaM9Idle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesPistolaBerettaM9Shoot { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesPistolaColtM1911Idle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesPistolaColtM1911Shoot { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesPistolaColtPythonIdle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesPistolaColtPythonShoot { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesEscopetaBeretta682Idle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesEscopetaBeretta682Shoot { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesEscopetaIthaca37Idle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesEscopetaIthaca37Shoot { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesEscopetaSpas12Idle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesEscopetaSpas12Shoot { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesSubFusilMp5KIdle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesSubFusilMp5KShoot { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesSubFusilMp40Idle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesSubFusilMp40Shoot { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesSubFusilUziIdle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesSubFusilUziShoot { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesFusilAk47Idle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesFusilAk47Shoot { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesFusilM4A1Idle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesFusilM4A1Shoot { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesFusilXm8Idle { get; set; }
        public static Rectangle[] ObamaAnimationRectangulesFusilXm8Shoot { get; set; }

        public const string OBAMA_INFO_TEXTURE = "Sprites/ObamaSurvivor/ObamaSelection";
        public const string OBAMA_ARM = "Sprites/Player/Arm_Gun";

#if IPHONE
        public static int OBAMA_HEALTH_BAR_MARGIN = 20 * IOsUtils.ScaleFactor;
#else
        public static int OBAMA_HEALTH_BAR_MARGIN = 20;
#endif

        public const string OBAMA_ATTACK_ANIMATION = "Attack";
        public const string OBAMA_BE_ATTACKED_ANIMATION = "Attacked";
        public const string OBAMA_DIE_ANIMATION = "Die";
        public const string OBAMA_IDLE_ANIMATION = "Idle";
        public const string OBAMA_RUN_ANIMATION = "Run";
        public const string OBAMA_CELEBRATE_ANIMATION = "Idle";
        
        public const string OBAMA_KILLED_SOUND = "Sounds/PlayerKilled";

        public static Dictionary<string, Vector2> ObamaWeaponAnimationPosition { get; set; }

#if IPHONE
		public static string OBAMA_ANIMATION_DATA = string.Format("{0}/Sprites/ObamaSurvivor/Data/Obama_iphone{1}.txt", GlobalParameters.CONTENT_FOLDER, IOsUtils.DeviceImageSufix);
		public static string OBAMA_WEAPOND_ANIMATION_DATA = string.Format("{0}/Sprites/ObamaSurvivor/Data/WeaponsPosition_iphone{1}.txt", GlobalParameters.CONTENT_FOLDER, IOsUtils.DeviceImageSufix);
        public const string OBAMA_ANIMATION_TEXTURE = "Sprites/ObamaSurvivor/Obama_iphone";
		public static int OBAMA_ATTACK_ANIMATION_HEIGHT = 155 * IOsUtils.ScaleFactor;

		public static int OBAMA_KNIFE_WIDTH = 70 * IOsUtils.ScaleFactor;
		public static int OBAMA_KNIFE_HEIGHT = 15 * IOsUtils.ScaleFactor;
#else
        public static string OBAMA_ANIMATION_DATA = string.Format("{0}/Sprites/ObamaSurvivor/Data/Obama_WP7.txt", GlobalParameters.CONTENT_FOLDER);
        public static string OBAMA_WEAPOND_ANIMATION_DATA = string.Format("{0}/Sprites/ObamaSurvivor/Data/WeaponsPosition_WP7.txt", GlobalParameters.CONTENT_FOLDER);
        public const string OBAMA_ANIMATION_TEXTURE = "Sprites/ObamaSurvivor/Obama_WP7";
        public const int OBAMA_ATTACK_ANIMATION_HEIGHT = 310;

        public const int OBAMA_KNIFE_WIDTH = 140;
        public const int OBAMA_KNIFE_HEIGHT = 30;
#endif
        #endregion

        #region Gordo Mercenario
        public static Rectangle[] GordoAnimationRectangulesIdle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesRun { get; set; }
        public static Rectangle[] GordoAnimationRectangulesDie { get; set; }
        public static Rectangle[] GordoAnimationRectangulesBeAttacked { get; set; }
        public static Rectangle[] GordoAnimationRectangulesAttack { get; set; }
        public static Rectangle[] GordoAnimationRectangulesCelebrate { get; set; }

        public static Rectangle[] GordoAnimationRectangulesPistolaBerettaM9Idle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesPistolaBerettaM9Shoot { get; set; }
        public static Rectangle[] GordoAnimationRectangulesPistolaColtM1911Idle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesPistolaColtM1911Shoot { get; set; }
        public static Rectangle[] GordoAnimationRectangulesPistolaColtPythonIdle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesPistolaColtPythonShoot { get; set; }
        public static Rectangle[] GordoAnimationRectangulesEscopetaBeretta682Idle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesEscopetaBeretta682Shoot { get; set; }
        public static Rectangle[] GordoAnimationRectangulesEscopetaIthaca37Idle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesEscopetaIthaca37Shoot { get; set; }
        public static Rectangle[] GordoAnimationRectangulesEscopetaSpas12Idle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesEscopetaSpas12Shoot { get; set; }
        public static Rectangle[] GordoAnimationRectangulesSubFusilMp5KIdle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesSubFusilMp5KShoot { get; set; }
        public static Rectangle[] GordoAnimationRectangulesSubFusilMp40Idle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesSubFusilMp40Shoot { get; set; }
        public static Rectangle[] GordoAnimationRectangulesSubFusilUziIdle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesSubFusilUziShoot { get; set; }
        public static Rectangle[] GordoAnimationRectangulesFusilAk47Idle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesFusilAk47Shoot { get; set; }
        public static Rectangle[] GordoAnimationRectangulesFusilM4A1Idle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesFusilM4A1Shoot { get; set; }
        public static Rectangle[] GordoAnimationRectangulesFusilXm8Idle { get; set; }
        public static Rectangle[] GordoAnimationRectangulesFusilXm8Shoot { get; set; }

        public const string GORDO_MERCENARIO_INFO_TEXTURE = "Sprites/Player/GordoSelection";
        public const string GORDO_MERCENARIO_ARM = "Sprites/Player/Arm_Gun";

        public const int GORDO_MERCENARIO_HEALTH_BAR_MARGIN = 20;

        public const string GORDO_MERCENARIO_ATTACK_ANIMATION = "Attack";
        public const string GORDO_MERCENARIO_BE_ATTACKED_ANIMATION = "Attacked";
        public const string GORDO_MERCENARIO_DIE_ANIMATION = "Die";
        public const string GORDO_MERCENARIO_IDLE_ANIMATION = "Idle";
        public const string GORDO_MERCENARIO_RUN_ANIMATION = "Run";
        public const string GORDO_MERCENARIO_CELEBRATE_ANIMATION = "Parado";

        public const string GORDO_MERCENARIO_KILLED_SOUND = "Sounds/PlayerKilled";

#if IPHONE
		public static string GORDO_MERCENARIO_ANIMATION_DATA = string.Format("{0}/Sprites/Player/Data/Gordo_iphone{1}.txt", GlobalParameters.CONTENT_FOLDER, IOsUtils.DeviceImageSufix);
        public const string GORDO_MERCENARIO_ANIMATION_TEXTURE = "Sprites/Player/Gordo_iphone";

        public const int GORDO_MERCENARIO_WEAPON_NO_SHOOT_ANIMATION_IDLE_POSITION_X = 16;
        public const int GORDO_MERCENARIO_WEAPON_NO_SHOOT_ANIMATION_IDLE_POSITION_Y = 41;

        public const int GORDO_MERCENARIO_WEAPON_SHOOT_ANIMATION_IDLE_POSITION_X = 16;
        public const int GORDO_MERCENARIO_WEAPON_SHOOT_ANIMATION_IDLE_POSITION_Y = 41;

        public const int GORDO_MERCENARIO_KNIFE_WIDTH = 40;
        public const int GORDO_MERCENARIO_KNIFE_HEIGHT = 15;
#else
        public static string GORDO_MERCENARIO_ANIMATION_DATA = string.Format("{0}/Sprites/Player/Data/Gordo_WP7.txt", GlobalParameters.CONTENT_FOLDER);
        public const string GORDO_MERCENARIO_ANIMATION_TEXTURE = "Sprites/Player/Gordo_WP7";

        // TODO: buscar las posiciones correctas segun el gordo
        public const int GORDO_MERCENARIO_WEAPON_IDLE_ANIMATION_POSITION_X = 32;
        public const int GORDO_MERCENARIO_WEAPON_IDLE_ANIMATION_POSITION_Y = 82;

        public const int GORDO_MERCENARIO_WEAPON_SHOOT_ANIMATION_POSITION_X = 32;
        public const int GORDO_MERCENARIO_WEAPON_SHOOT_ANIMATION_POSITION_Y = 82;

        public const int GORDO_MERCENARIO_KNIFE_WIDTH = 80;
        public const int GORDO_MERCENARIO_KNIFE_HEIGHT = 30;
#endif
        #endregion

        #region Zombie global

        public const string ZOMBIE_KILLED_SOUND = "Sounds/MonsterKilled";

        public const string ZOMBIE_ATTACK_SOUND = "Sounds/MonsterAttack";

#if IPHONE        
        /// <summary>
        /// Npc vision field width. Longitud del campo de vision de los enemigos.
        /// </summary>
		public static int SPOTLIGHT_WIDTH = 150 * IOsUtils.ScaleFactor;

        /// <summary>
        /// Npc vision field height. Altura del campo de vision de los enemigos.
        /// </summary>
		public static int SPOTLIGHT_HEIGHT = 40 * IOsUtils.ScaleFactor;
#else
        /// <summary>
        /// Npc vision field width. Longitud del campo de vision de los enemigos.
        /// </summary>
        public const int SPOTLIGHT_WIDTH = 300;

        /// <summary>
        /// Npc vision field height. Altura del campo de vision de los enemigos.
        /// </summary>
        public const int SPOTLIGHT_HEIGHT = 80;
#endif
        #endregion

        #region Slow strong zombie
        public static Rectangle[] SsmAnimationRectangulesIdle { get; set; }
        public static Rectangle[] SsmAnimationRectangulesRun { get; set; }
        public static Rectangle[] SsmAnimationRectangulesDie { get; set; }
        public static Rectangle[] SsmAnimationRectangulesHit { get; set; }
        public static Rectangle[] SsmAnimationRectangulesAttack { get; set; }

        public const string SLOW_STRONG_MONSTER_ATTACK_ANIMATION = "Attack";
        public const string SLOW_STRONG_MONSTER_DIE_ANIMATION = "Die2";
        public const string SLOW_STRONG_MONSTER_HIT_ANIMATION = "Run";
        public const string SLOW_STRONG_MONSTER_IDLE_ANIMATION = "Idle";
        public const string SLOW_STRONG_MONSTER_RUN_ANIMATION = "Run";
#if IPHONE
        public static string SLOW_STRONG_MONSTER_ANIMATION_DATA = string.Format("{0}/Sprites/SlowStrongMonster/Data/SSM_iphone{0}.txt", GlobalParameters.CONTENT_FOLDER, IOsUtils.DeviceImageSufix);
        public const string SLOW_STRONG_MONSTER_ANIMATION_TEXTURE = "Sprites/SlowStrongMonster/SSM_iphone";
#else
        public static string SLOW_STRONG_MONSTER_ANIMATION_DATA = string.Format("{0}/Sprites/SlowStrongMonster/Data/SSM_WP7.txt", GlobalParameters.CONTENT_FOLDER);
        public const string SLOW_STRONG_MONSTER_ANIMATION_TEXTURE = "Sprites/SlowStrongMonster/SSM_WP7";
#endif
        #endregion

        #region Fast weak strong zombie
        public static Rectangle[] FwmAnimationRectangulesIdle { get; set; }
        public static Rectangle[] FwmAnimationRectangulesRun { get; set; }
        public static Rectangle[] FwmAnimationRectangulesDie { get; set; }
        public static Rectangle[] FwmAnimationRectangulesHit { get; set; }
        public static Rectangle[] FwmAnimationRectangulesAttack { get; set; }

        public const string FAST_WEAK_MONSTER_ATTACK_ANIMATION = "Attack";
        public const string FAST_WEAK_MONSTER_DIE_ANIMATION = "Die2";
        public const string FAST_WEAK_MONSTER_HIT_ANIMATION = "Run";
        public const string FAST_WEAK_MONSTER_IDLE_ANIMATION = "Idle";
        public const string FAST_WEAK_MONSTER_RUN_ANIMATION = "Run";
#if IPHONE
		public static string FAST_WEAK_MONSTER_ANIMATION_DATA = string.Format("{0}/Sprites/FastWeakMonster/Data/FWM_iphone{0}.txt", GlobalParameters.CONTENT_FOLDER, IOsUtils.DeviceImageSufix);
        public const string FAST_WEAK_MONSTER_ANIMATION_TEXTURE = "Sprites/FastWeakMonster/FWM_iphone";
#else
        public static string FAST_WEAK_MONSTER_ANIMATION_DATA = string.Format("{0}/Sprites/FastWeakMonster/Data/FWM_WP7.txt", GlobalParameters.CONTENT_FOLDER);
        public const string FAST_WEAK_MONSTER_ANIMATION_TEXTURE = "Sprites/FastWeakMonster/FWM_WP7";
#endif
        #endregion

        #region Normal zombie
        public static Rectangle[] NmAnimationRectangulesIdle { get; set; }
        public static Rectangle[] NmAnimationRectangulesRun { get; set; }
        public static Rectangle[] NmAnimationRectangulesDie { get; set; }
        public static Rectangle[] NmAnimationRectangulesHit { get; set; }
        public static Rectangle[] NmAnimationRectangulesAttack { get; set; }

        public const string NORMAL_MONSTER_ATTACK_ANIMATION = "Attack";
        public const string NORMAL_MONSTER_DIE_ANIMATION = "Die2";
        public const string NORMAL_MONSTER_HIT_ANIMATION = "Run";
        public const string NORMAL_MONSTER_IDLE_ANIMATION = "Idle";
        public const string NORMAL_MONSTER_RUN_ANIMATION = "Run";
#if IPHONE
		public static string NORMAL_MONSTER_ANIMATION_DATA = string.Format("{0}/Sprites/NormalMonster/Data/NM_iphone{1}.txt", GlobalParameters.CONTENT_FOLDER, IOsUtils.DeviceImageSufix);
        public const string NORMAL_MONSTER_ANIMATION_TEXTURE = "Sprites/NormalMonster/NM_iphone";
#else
        public static string NORMAL_MONSTER_ANIMATION_DATA = string.Format("{0}/Sprites/NormalMonster/Data/NM_WP7.txt", GlobalParameters.CONTENT_FOLDER);
        public const string NORMAL_MONSTER_ANIMATION_TEXTURE = "Sprites/NormalMonster/NM_WP7";
#endif
        #endregion

        #region GamePlayScreen
#if IPHONE
		public static int GAME_PLAY_SCREEN_HUD_Y = 50 * IOsUtils.ScaleFactor;
#else
        public const int GAME_PLAY_SCREEN_HUD_Y = 100;
#endif
        #endregion

        #region BuyWeaponScreen
#if IPHONE
        public const string WEAPON_BUTTON_NORMAL = "Buttons/WeaponButtonNormalSmall_iphone";

        public static int WEAPON_BUTTON_NORMAL_WIDTH = 286 * IOsUtils.ScaleFactor;
		public static int WEAPON_BUTTON_NORMAL_HEIGHT = 45 * IOsUtils.ScaleFactor;
		public static int WEAPON_BUTTON_NORMAL_MARGIN_BETWEEN_BUTTONS = 5 * IOsUtils.ScaleFactor;

		public static int WEAPON_BUTTON_NORMAL_SMALL_WIDTH = 140 * IOsUtils.ScaleFactor;
		public static int WEAPON_BUTTON_NORMAL_SMALL_HEIGHT = 135 * IOsUtils.ScaleFactor;

		public static int WEAPON_BUTTON_BUY_WIDTH = 140 * IOsUtils.ScaleFactor;
		public static int WEAPON_BUTTON_BUY_HEIGHT = 25 * IOsUtils.ScaleFactor;

		public static int BUY_WEAPON_SCREEN_WEAPONS_FOOTER_HEIGHT = 50 * IOsUtils.ScaleFactor;
		public static int BUY_WEAPON_SCREEN_WEAPONS_MENU_WIDTH = 293 * IOsUtils.ScaleFactor;
		public static int BUY_WEAPON_SCREEN_WEAPONS_MENU_HEIGHT = 192 * IOsUtils.ScaleFactor;
		public static int BUY_WEAPON_SCREEN_WEAPONS_MENU_X = 9 * IOsUtils.ScaleFactor;
		public static int BUY_WEAPON_SCREEN_WEAPONS_MENU_Y = 74 * IOsUtils.ScaleFactor;
		public static int BUY_WEAPON_SCREEN_WEAPONS_DESCRIPTION_MENU_X = BUY_WEAPON_SCREEN_WEAPONS_MENU_WIDTH  + (25 * IOsUtils.ScaleFactor);
		public static int BUY_WEAPON_SCREEN_WEAPONS_HORIZONTAL_MARGIN = 0 * IOsUtils.ScaleFactor;
		public static int BUY_WEAPON_SCREEN_WEAPONS_VERTICAL_MARGIN = 8 * IOsUtils.ScaleFactor;
#else
        public const string WEAPON_BUTTON_NORMAL = "Buttons/WeaponButtonNormalSmall_WP7";

        public const int WEAPON_BUTTON_NORMAL_WIDTH = 572;
        public const int WEAPON_BUTTON_NORMAL_HEIGHT = 90;
        public const int WEAPON_BUTTON_NORMAL_MARGIN_BETWEEN_BUTTONS = 10;
        
        public const int WEAPON_BUTTON_NORMAL_SMALL_WIDTH = 280;
        public const int WEAPON_BUTTON_NORMAL_SMALL_HEIGHT = 270;

        public const int WEAPON_BUTTON_BUY_WIDTH = 280;
        public const int WEAPON_BUTTON_BUY_HEIGHT = 50;

        public const int BUY_WEAPON_SCREEN_WEAPONS_FOOTER_HEIGHT = 100;
        public const int BUY_WEAPON_SCREEN_WEAPONS_MENU_WIDTH = 586;
        public const int BUY_WEAPON_SCREEN_WEAPONS_MENU_HEIGHT = 384;
        public const int BUY_WEAPON_SCREEN_WEAPONS_MENU_X = 18;
        public const int BUY_WEAPON_SCREEN_WEAPONS_MENU_Y = 148;
        public const int BUY_WEAPON_SCREEN_WEAPONS_DESCRIPTION_MENU_X = BUY_WEAPON_SCREEN_WEAPONS_MENU_WIDTH + 50;
        public const int BUY_WEAPON_SCREEN_WEAPONS_HORIZONTAL_MARGIN = 0;
        public const int BUY_WEAPON_SCREEN_WEAPONS_VERTICAL_MARGIN = 16;
#endif
        #endregion

        #region PauseMenuScreen
        public const string QUIT_GAME_MESSAGE = "Are you sure you want to quit this game?";
#if IPHONE
		public static float FIRST_BUTTON_INITIAL_POSITION = 35f * IOsUtils.ScaleFactor;
		public static float MARGIN_BETWEEN_BUTTONS = 13f * IOsUtils.ScaleFactor;
#else
        public const float FIRST_BUTTON_INITIAL_POSITION = 70f;
        public const float MARGIN_BETWEEN_BUTTONS = 26f;
#endif
        #endregion

        #region TouchMessageBoxScreen
#if IPHONE
		public static float TOUCH_MESSAGE_BOX_SCREENFIRST_BUTTON_INITIAL_POSITION = 100f * IOsUtils.ScaleFactor;
		public static float TOUCH_MESSAGE_BOX_SCREENMARGIN_BETWEEN_BUTTONS = 13f * IOsUtils.ScaleFactor;
		public static float TOUCH_MESSAGE_BOX_SCREEN_BOX_POSITION_Y = 80f * IOsUtils.ScaleFactor;
#else
        public const float TOUCH_MESSAGE_BOX_SCREENFIRST_BUTTON_INITIAL_POSITION = 200f;
        public const float TOUCH_MESSAGE_BOX_SCREENMARGIN_BETWEEN_BUTTONS = 26f;
        public const float TOUCH_MESSAGE_BOX_SCREEN_BOX_POSITION_Y = 160f;
#endif
        #endregion

        #region Weapons & default state parameters
        /// <summary>
        /// Especifica el numero de armas totales que existen en el juego
        /// </summary>
#if FREE
		public const int MAX_WEAPONS_AVAILABLE = 4;
#else
		public const int MAX_WEAPONS_AVAILABLE = 12;
#endif

        public static List<Weapon> GetWeaponList()
        {
            if (_weaponList == null)
            {
                _weaponList = new List<Weapon>(MAX_WEAPONS_AVAILABLE);

#if FREE
				_weaponList.Add(new Weapon(WeaponType.PistolaColtPython));
				_weaponList.Add(new Weapon(WeaponType.EscopetaBeretta682));
				_weaponList.Add(new Weapon(WeaponType.SubFusilUzi));
				_weaponList.Add(new Weapon(WeaponType.FusilAk47));
#else
                foreach (var val in Util.GetValues<WeaponType>())
                {
                    _weaponList.Add(new Weapon(val));
                }
#endif
            }

            return _weaponList;

            //return new List<Weapon>(MAX_WEAPONS_AVAILABLE)
            //           {
            //               new Weapon(WeaponType.PistolaBerettaM9),
            //               new Weapon(WeaponType.PistolaColtM1911),
            //               new Weapon(WeaponType.PistolaColtPython),
            //               new Weapon(WeaponType.EscopetaBeretta682),
            //               new Weapon(WeaponType.EscopetaIthaca37),
            //               new Weapon(WeaponType.EscopetaSpas12),
            //               new Weapon(WeaponType.SubFusilMp5K),
            //               new Weapon(WeaponType.SubFusilMp40),
            //               new Weapon(WeaponType.SubFusilUzi),
            //               new Weapon(WeaponType.FusilAk47),
            //               new Weapon(WeaponType.FusilM4A1),
            //               new Weapon(WeaponType.FusilXm8)
            //           };
        }

        public const float WEAPON_CHARACTERISTIC_TOTAL_UNITY = 5.0f;
        public static float WeaponMaxPower = 0.0f;
        public static float WeaponMaxBulletVelocity = 0.0f;
        public static float WeaponMaxMaxAmmo = 0.0f;
        public static float WeaponMaxReloadTime = 0.0f;
        public static float WeaponMaxFireRate = 0.0f;

        public const WeaponType PRIMARY_WEAPON_TYPE = WeaponType.PistolaColtPython;
        public const int PRIMARY_WEAPON_AMMO_COUNT = 50;
#if DEBUG
        public const WeaponType SECONDARY_WEAPON_TYPE = WeaponType.FusilAk47;
        public const int SECONDARY_WEAPON_AMMO_COUNT = 100;
#endif
        public const string WEAPON_EQUIPPED_TEXT = "Equipped";
        public const string WEAPON_IN_INVENTORY_TEXT = "In Inventory";
        public const string CHANGE_WEAPON_TEXT = "Weapons";
        public const string PRIMARY_WEAPON_TEXT = "Primary Weapon";
        public const string SECONDARY_WEAPON_TEXT = "Secondary Weapon";
        public const string WEAPON_BUY_IT_TEXT = "Buy it";
        public const string WEAPON_BUY_AMMO_TEXT = "Buy ammo";
        public const string WEAPON_EQUIP_IT_TEXT = "Equip it";
        public const string WEAPON_NOT_ENOUGHT_TO_BUY_TEXT = "Not enought to buy";
        //public const string AVAILABLE_SCORE_TEXT = "Total money";

        public const int TOTAL_LIVES = 50;
		public const int INITIAL_LIVES = TOTAL_LIVES;
        public const int INITIAL_LEVEL = -1;
        public const int INITIAL_SCORE = 0;
        
        /// <summary>
        /// Minutos iniciales para completar los niveles
        /// </summary>
        public const double INITIAL_MINUTES_TO_COMPLETE_LEVEL = 2.0;

        /// <summary>
        /// Cuantos minutos tengo que adicionar para X niveles
        /// </summary>
        public const double MINUTES_TO_ADD_EVERY_X_LEVEL = 0.5;

        /// <summary>
        /// Cada cuantos niveles hay que sumar minutos
        /// </summary>
        public const int ADD_MINUTES_EVERY_X_LEVEL = 5; 

        private static List<PlayerInfo> _playersInfoList;
        public static readonly Color HudColor = new Color(255, 76, 35);
        public static readonly Color AlternativeColor = new Color(134, 25, 0);
        private static List<Weapon> _weaponList;


        public const string BUY_WEAPON_SCREEN_PLAY_BUTTON_TEXT = "PLAY";

        //public const string BULLET_TEXTURE = "Sprites/Player/Bullet";
#if IPHONE
		public static int BULLET_WIDTH = 12 * IOsUtils.ScaleFactor;
		public static int BULLET_HEIGHT = 12 * IOsUtils.ScaleFactor;
#else
		public const int BULLET_WIDTH = 12;
		public const int BULLET_HEIGHT = 12;
#endif

        /// <summary>
        /// Especifica la maxima cantidad de balas que contendra el repositorio a manejar
        /// </summary>
        public const int MAX_BULLET_REPOSITORY = 12;

        public static List<PlayerInfo> PlayersInfoList
        {
            get
            {
                return _playersInfoList ?? (_playersInfoList = new List<PlayerInfo>(MAX_SELECT_PLAYERS)
                                             {
                                                 new PlayerInfo(PlayerType.GordoMercenario),
                                                 new PlayerInfo(PlayerType.Obama)
                                             });
            }
        }

        //public static Dictionary<int, Rectangle> WeaponsData { get; set; }

        public static Dictionary<string, Rectangle> ScreenAssetsData { get; set; }

#if IPHONE
        public const string SCREEN_ASSETS_TEXTURE = "ScreenAsset_iphone";
		public static string SCREEN_ASSETS_DATA = string.Format("{0}/Data/ScreenAsset_iphone{0}.txt", GlobalParameters.CONTENT_FOLDER, IOsUtils.DeviceImageSufix);
#else
        public const string SCREEN_ASSETS_TEXTURE = "ScreenAsset_WP7";
        public static string SCREEN_ASSETS_DATA = string.Format("{0}/Data/ScreenAsset_WP7.txt", GlobalParameters.CONTENT_FOLDER);
#endif
        public const int MAX_SCREEN_ASSETS_AVAILABLE = 36;
        #endregion
    }
}

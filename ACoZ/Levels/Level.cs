using System;
using System.Collections.Generic;
using ACoZ.Helpers;
using ACoZ.Npc;
using ACoZ.Npc.Enemies;
using ACoZ.Players;
using ACoZ.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using System.Xml.Serialization;
//using System.IO;
#if WINDOWS_PHONE || IPHONE
using Mobile.Base.VirtualInput;
#elif SILVERLIGHT
using Web.Base.VirtualInput;
#elif WINDOWS
using Desktop.Base.VirtualInput;
#endif
//using Platformer.Npc.Survivors;

namespace ACoZ.Levels
{
    /// <summary>
    /// A uniform grid of tiles with collections of gems and enemies.
    /// The level owns the player and controls the game's win and lose
    /// conditions as well as scoring.
    /// </summary>
    public class Level //: IDisposable
    {
        #region Campos

        private SoundEffect _exitReachedSound;

        // Physical structure of the level.
        private Tile[,] _tiles;
        //private List<Layer> _backGroundLayers;
        private Layer[] _backGroundLayers;
//#if !IPHONE
//        private List<Layer> _foreGroundLayers;
//#endif

        // Key locations in the level.        
        //private Vector2 _start;
        private Point _exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);
        // Level game state.
        //private readonly Random _random = new Random(354668); // Arbitrary, but constant seed
        //private bool _customGemsRequired;
        #endregion


        #region Propiedades
        public Pool<FastWeakMonster> FastWeakMonstersPool;
        public Pool<NormalMonster> NormalMonstersPool;
        public Pool<SlowStrongMonster> SlowStrongMonstersPool;
        //public List<Survivor> Survivors = new List<Survivor>();
        //public List<DeathTile> DeathTiles = new List<DeathTile>();
        //public Vector2 Checkpoint;

        //public ScreenManager ScreenManager { get; private set; }
        public ContentManager Content { get; private set; }

        //public List<Item> Items = new List<Item>();
        //private bool _continuePreviusGame;

        // The rate at which the enemies appear
        private TimeSpan _previousSpawnTime;
        private TimeSpan _enemySpawnTimeFrom;
        private TimeSpan _enemySpawnTimeTo;
        private TimeSpan _enemySpawnTime;
        private Dictionary<string, EnemyParameter> _enemiesParameters;
        private readonly List<Weapon> _weaponInventory;
        private readonly Weapon _primaryWeapon;
        private readonly Weapon _secondaryWeapon;
        private readonly PlayerInfo _playerInfo;
        private readonly Dictionary<int, int> _ammoInventory;
        private readonly int _tilesPerScreen;

        //private ZoomMenu _zoomMenu;
        //private Camera2D _cam;
        public int GemsRemaining { get; set; }
        public bool GemsRequired { get; set; }
        public int GemsToCollect { get; set; }
        public int NextLife { get; private set; }
        public Player Player { get; private set; }
        public float CameraPositionXAxis { get; private set; }
        public float CameraPositionYAxis { get; private set; }
        public int Score { get; set; }
        public bool ReachedExit { get; private set; }
        
        // NOTE: por el momento lo desactivo al tiempo. Pensar que hacer
        public TimeSpan TimeRemaining { get; private set; }


        //public ContentManager Content { get; private set; }
        public bool LevelLoaded { get; private set; }
        #endregion

        #region Loading

        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="contentManager">Current Content manager</param>
        /// <param name="levelIndex">Level number to load</param>
        /// <param name="newScore">Score to load on the level</param>
        /// <param name="currentLives">Current lives of the player</param>
        /// <param name="nextLife">Next life necesary points </param>
        /// <param name="weaponInventory">List of weapons that player has</param>
        /// <param name="primaryWeapon">Primary Weapon of player</param>
        /// <param name="secondaryWeapon">Secondary Weapon of player</param>
        /// <param name="playerInfo"></param>
        /// <param name="ammoInventory"></param>
        public Level(ContentManager contentManager, int levelIndex, int newScore, int currentLives, int nextLife, List<Weapon> weaponInventory, Weapon primaryWeapon, Weapon secondaryWeapon, PlayerInfo playerInfo, Dictionary<int, int> ammoInventory)
        {
            this._weaponInventory = weaponInventory;
            this._primaryWeapon = primaryWeapon;
            this._secondaryWeapon = secondaryWeapon;
            this._ammoInventory = ammoInventory;
            this._playerInfo = playerInfo;

            //ScreenManager = screenManager;
            this.Content = contentManager;

            // Create a new content manager to load content used just by this level.
            //Content = new ContentManager(serviceProvider, GlobalParameters.CONTENT_FOLDER);

            this.CurrentLevel = levelIndex;

            this.LoadTiles(levelIndex);

            this.Player.SetLives(currentLives);

            this.NextLife = nextLife;
            this.Score = newScore;

            // Set the time keepers to zero
            this._previousSpawnTime = TimeSpan.Zero;

            // Load background layer textures. For now, all levels must
            // use the same backgrounds and only use the left-most part of them.
            this.LoadBackground();

            // Load sounds.
            this.LoadSounds();

            //LoadZoomMenu();

            //LoadCamera();
            this.LoadEnemiesPool();

            this._tilesPerScreen = (int)Math.Floor(GlobalParameters.SCREEN_WIDTH / (double)Tile.WIDTH);
        }

        private void LoadEnemiesPool()
        {
            var fastWeakMonstersPoolQuantity = this._enemiesParameters[NpcTypes.FastWeakMonster.ToString()].PoolQuantity;
            var normalMonstersPoolQuantity = this._enemiesParameters[NpcTypes.NormalMonster.ToString()].PoolQuantity;
            var slowStrongMonsterPoolQuantity = this._enemiesParameters[NpcTypes.SlowStrongMonster.ToString()].PoolQuantity;

            if (fastWeakMonstersPoolQuantity > 0)
            {
                this.FastWeakMonstersPool = new Pool<FastWeakMonster>(fastWeakMonstersPoolQuantity);

                foreach (var enemy in this.FastWeakMonstersPool.AllNodes)
                {
                    if (!enemy.Item.IsInit)
                    {
                        enemy.Item.Init(true, this);
                    }
                }
            }

            if (normalMonstersPoolQuantity > 0)
            {
                this.NormalMonstersPool = new Pool<NormalMonster>(normalMonstersPoolQuantity);
                
                foreach (var enemy in this.NormalMonstersPool.AllNodes)
                {
                    if (!enemy.Item.IsInit)
                    {
                        enemy.Item.Init(true, this);
                    }
                }
            }

            if (slowStrongMonsterPoolQuantity > 0)
            {
                this.SlowStrongMonstersPool = new Pool<SlowStrongMonster>(slowStrongMonsterPoolQuantity);

                foreach (var enemy in this.SlowStrongMonstersPool.AllNodes)
                {
                    if (!enemy.Item.IsInit)
                    {
                        enemy.Item.Init(true, this);
                    }
                }
            }
        }

        ///// <summary>
        ///// Load level from saved data
        ///// </summary>
        ///// <param name="screenManager"></param>
        //public Level(ScreenManager screenManager)
        //{
        //    ScreenManager = screenManager;

        //    // Create a new content manager to load content used just by this level.
        //    //Content = new ContentManager(serviceProvider, GlobalParameters.CONTENT_FOLDER);

        //    //LoadSavedLevelData();

        //    //LoadZoomMenu();

        //    //LoadCamera();
        //}

//        private void LoadSavedLevelData()
//        {
//#if WINDOWS_PHONE || WINDOWS ||XBOX
//            if (Global.SaveDevice.FileExists(Global.ContainerName, Global.FileNameSaveGame))
//            {
//                _continuePreviusGame = true;

//                var savedGameData = new SaveGameData();

//                var serializer = new XmlSerializer(typeof(SaveGameData));

//                Global.SaveDevice.Load(
//                    Global.ContainerName,
//                    Global.FileNameSaveGame,
//                    stream => { savedGameData = (SaveGameData)serializer.Deserialize(stream); });

//                // Player data
//                LoadStartTile(savedGameData.PlayerPosition, savedGameData.PlayerLives);

//                // Level Data
//                _currentLevel = savedGameData.Level;
//                LoadTiles(CurrentLevel);
//                Score = savedGameData.Score;
//                NextLife = savedGameData.NextLifeScore;
//                TimeRemaining = TimeSpan.FromSeconds(savedGameData.SecondsRemaining);

//                CameraPositionYAxis = savedGameData.CameraPositionYAxis;
//                CameraPositionXAxis = savedGameData.CameraPositionXAxis;

//                // Enemies Data
//                for (int i = 0; i < savedGameData.Enemies.Count; ++i)
//                {
//                    LoadEnemyTile(savedGameData.Enemies[i].Position, (NpcTypes)Enum.Parse(typeof(NpcTypes), savedGameData.Enemies[i].SpriteSet, true), savedGameData.Enemies[i].IsAlive,
//                                  savedGameData.Enemies[i].CurrentHealth);
//                }

//                // Survivor Data
//                for (int i = 0; i < savedGameData.Survivors.Count; ++i)
//                {
//                    LoadSurvivorTile(savedGameData.Survivors[i].Position, (NpcTypes)Enum.Parse(typeof(NpcTypes), savedGameData.Survivors[i].SpriteSet, true), savedGameData.Survivors[i].IsAlive,
//                                  savedGameData.Survivors[i].CurrentHealth);
//                }

//                // Items Data
//                //for (int i = 0; i < savedGameData.Items.Count; ++i)
//                //{
//                //    LoadItemTile(savedGameData.Items[i].Position, (ItemType)savedGameData.Items[i].ItemObject);
//                //}

//                // Load background layer textures. For now, all levels must
//                // use the same backgrounds and only use the left-most part of them.
//                LoadBackground();

//                // Load sounds.
//                LoadSounds();

//                _continuePreviusGame = false;
//            }
//            else
//            {
//                throw new FileNotFoundException("Saved Level data doesnt exist. Please restart the game.");
//            }
//#endif
//        }

        private void LoadSounds()
        {
            this._exitReachedSound = this.Content.Load<SoundEffect>("Sounds/ExitReached");
        }

        private void LoadBackground()
        {

            // TODO: cargar de LevelData para niveles con fondos distintos

            // Platformer assumes that scrolling speed values have a range between 0 and 1.
            // A value of 0 means no scrolling and 1 means scrolling at the same pace as the level tiles.
            //_backGroundLayers = new List<Layer>
                          //{
                          //    new Layer(Content, "Backgrounds/Layer0", 0.2f, 6),
                          //    new Layer(Content, "Backgrounds/Layer1", 0.5f, 6),
                          //    new Layer(Content, "Backgrounds/Layer2", 0.8f, 6)
                          //};

            this._backGroundLayers = new Layer[2];
            this._backGroundLayers[0] = new Layer(this.Content, "Backgrounds/Layer00", 0.2f, 1);
            //_backGroundLayers[1] = new Layer(Content, "Backgrounds/Layer1", 0.5f, 1);
            this._backGroundLayers[1] = new Layer(this.Content, "Backgrounds/Layer2", 0.8f, 3);
            

//#if !IPHONE
//            _foreGroundLayers = new List<Layer>
//                          {
//                              new Layer(Content, "Backgrounds/Layer3", 0.8f, 4)
//                          };
//#endif
        }

        /// <summary>
        /// Iterates over every tile in the structure file and loads its
        /// appearance and behavior. This method also validates that the
        /// file is well-formed with a player start point, exit, etc.
        /// </summary>
        /// <param name="levelNumber">
        /// Level number to load.
        /// </param>
        public void LoadTiles(int levelNumber)
        {
            // Ponemos la bandera en FALSE para que no se hagan otras acciones mientras el nivel no esta cargado
            this.LevelLoaded = false;

            var levelData = new LevelData(levelNumber);

            // Used to determine how fast enemy respawns
            this._enemySpawnTime = levelData.EnemySpawnTimeTo;

            this._enemySpawnTimeFrom = levelData.EnemySpawnTimeFrom;
            this._enemySpawnTimeTo = levelData.EnemySpawnTimeTo;

            this._enemiesParameters = levelData.EnemiesParameters;

            var width = levelData.Lines[0].Length;

            // HZ: Estas variables las seteamos x defecto en false pero para el futuro podrias ser parametro de cada nivel
            this.GemsRequired = false;
            //_customGemsRequired = false;

            // Allocate the tile grid. Arma una matriz del tamaño del nivel
            this._tiles = new Tile[width, levelData.Lines.Count];

            // Loop over every tile position,
            for (var y = 0; y < this.Height; ++y)
            {
                for (var x = 0; x < this.Width; ++x)
                {
                    // to load each tile.
                    var tileType = levelData.Lines[y][x];
                    this._tiles[x, y] = this.LoadTile(tileType, x, y);
                }
            }

            //TimeRemaining = TimeSpan.FromMinutes(2.0);
            this.TimeRemaining = levelData.TimeToCompleteLevel;

            // Verify that the level has a beginning and an end.
            if (this.Player == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (this._exit == InvalidPosition)
                throw new NotSupportedException("A level must have an exit.");

            this.LevelLoaded = true;
        }

        //private List<string> LevelData(int levelNumber)
        //{

        //}
        //private void LoadTiles(int levelNumber)
        //{ 
        //    // Ponemos la bandera en FALSE para que no se hagan otras acciones mientras el nivel no esta cargado
        //    _levelLoaded = false;

        //    // Llamamos al servicio que nos envia los datos de los niveles.
        //    Uri uri = new Uri(Application.Current.Host.Source, "../PlatformService.svc");
        //    PlatformServiceClient oProxyPlatformServiceClient = new PlatformServiceClient("BasicHttpBinding_IPlatformService", uri.AbsoluteUri);
        //    oProxyPlatformServiceClient.LevelDataCompleted += new EventHandler<LevelDataCompletedEventArgs>(OProxyPlatformServiceClientLevelDataCompleted);
        //    oProxyPlatformServiceClient.LevelDataAsync(levelNumber);
        //}

        ///// <summary>
        ///// Evento que se dispara cuando finaliza la ejecucion del servicio que trae los datos de los niveles
        ///// </summary>
        //void OProxyPlatformServiceClientLevelDataCompleted(object sender, LevelDataCompletedEventArgs e)
        //{
        //    List<string> lines = e.Result;

        //    int width = lines[0].Length;

        //    // Allocate the tile grid. Arma una matriz del tamaño del nivel
        //    _tiles = new Tile[width, lines.Count];

        //    // Loop over every tile position,
        //    for (int y = 0; y < Height; ++y)
        //    {
        //        for (int x = 0; x < Width; ++x)
        //        {
        //            // to load each tile.
        //            char tileType = lines[y][x];
        //            _tiles[x, y] = LoadTile(tileType, x, y);
        //        }
        //    }

        //    // Verify that the level has a beginning and an end.
        //    if (Player == null)
        //        throw new NotSupportedException("A level must have a starting point.");
        //    if (_exit == InvalidPosition)
        //        throw new NotSupportedException("A level must have an exit.");

        //    _levelLoaded = true;
        //}

        /// <summary>
        /// Loads an individual tile's appearance and behavior.
        /// </summary>
        /// <param name="tileType">
        /// The character loaded from the structure file which
        /// indicates what should be loaded.
        /// </param>
        /// <param name="x">
        /// The X location of this tile in tile space.
        /// </param>
        /// <param name="y">
        /// The Y location of this tile in tile space.
        /// </param>
        /// <returns>The loaded tile.</returns>
        private Tile LoadTile(char tileType, int x, int y)
        {
            // Podriamos hacer gemas de distinto tipo y poder cambiandoles el color (que se lean del mapa de servicios)
            // Habria que modificar el metodo Level.LoadTile(char tileType, int x, int y), agregar una nueva opcion para la gema
            // y cambiar el metodo LoadGemTile

            switch (tileType)
            {
                // Blank space
                case '.':
                    return this.LoadPassableTile();

                // Impassable block
                case '#':
                    //return LoadTile(GlobalParameters.TRANSPARENT_TILE, TileCollision.Impassable);
                    return this.LoadImpassableTile();

                // Player 1 start point
                case '1':
                    //return _continuePreviusGame ? LoadPassableTile() : LoadStartTile(x, y);
                    return this.LoadStartTile(x, y);

                // Gem
                //case 'G':
                //    return _continuePreviusGame ? LoadPassableTile() : LoadItemTile(x, y, ItemType.Gem);

                // Exit
                case 'X':
                    return this.LoadExitTile(x, y);

                // Death Tile
                //case '*':
                //    return LoadDeathTile("BlockA", x, y);

                // Floating platform
                //case '-':
                //    return LoadTile("Platform", TileCollision.Platform);

                //// Platform block
                //case '~':
                //    return LoadVarietyTile("BlockB", 2, TileCollision.Platform);

                //// Passable block
                //case ':':
                //    return LoadVarietyTile("BlockB", 2, TileCollision.Passable);
                // Various enemies
                //case 'A':
                //return _continuePreviusGame ? LoadPassableTile() : LoadEnemyTile(x, y, "MonsterA");
                //case 'B':
                //    return _continuePreviusGame ? LoadPassableTile() : LoadEnemyTile(x, y, "MonsterB");
                //case 'C':
                //    return _continuePreviusGame ? LoadPassableTile() : LoadEnemyTile(x, y, "MonsterC");
                //case 'D':
                //    return _continuePreviusGame ? LoadPassableTile() : LoadEnemyTile(x, y, "MonsterD");
                //case 'A':
                //case 'B':
                //case 'C':
                //case 'D':
                //    return LoadPassableTile();

                //Ladder
                //case 'H':
                //    return LoadTile("Ladder", TileCollision.Ladder);

                //Checkpoint
                //case 'P':
                //    return _continuePreviusGame ? LoadPassableTile() : LoadItemTile(x, y, ItemType.Checkpoint);

                // Power Gem
                //case 'Q':
                //    return LoadItemTile(x, y, ItemType.Powerup);

                // Extra Life
                //case 'L':
                //    return _continuePreviusGame ? LoadPassableTile() : LoadItemTile(x, y, ItemType.Life);

                // Survivor
                //case 'S':
                //    return _continuePreviusGame ? LoadPassableTile() : LoadSurvivorTile(x, y, NpcTypes.ObamaSurvivor);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        ///// <summary>
        ///// Instantiates a survivor and puts him in the level.
        ///// </summary>
        //private Tile LoadSurvivorTile(int x, int y, NpcTypes survivorType)
        //{
        //    var position = GetBounds(x, y).GetBottomCenter();

        //    switch (survivorType)
        //    {
        //        case NpcTypes.ObamaSurvivor:
        //            Survivors.Add(new Survivor(this, position));
        //            break;
        //        default:
        //            throw new Exception("No se puede crear sobreviviente ya que no existe definicion del mismo");
        //    }


        //    return new Tile(null, TileCollision.Passable);
        //}

        ///// <summary>
        ///// Instantiates an survivor and puts him in the level.
        ///// </summary>
        //private void LoadSurvivorTile(Vector2 position, NpcTypes survivorType, bool isAlive, int currentHealth)
        //{
        //    switch (survivorType)
        //    {
        //        case NpcTypes.ObamaSurvivor:
        //            Survivors.Add(new Survivor(this, position, isAlive, currentHealth));
        //            break;
        //        default:
        //            throw new Exception("No se puede crear sobreviviente ya que no existe definicion del mismo");
        //    }
        //}

        private Tile LoadPassableTile()
        {
            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadImpassableTile()
        {
            return new Tile(null, TileCollision.Impassable);
        }

        /// <summary>
        /// Creates a new tile. The other tile loading methods typically chain to this
        /// method after performing their special logic.
        /// </summary>
        /// <param name="name">
        /// Path to a tile texture relative to the Content/Tiles directory.
        /// </param>
        /// <param name="collision">
        /// The tile collision type for the new tile.
        /// </param>
        /// <returns>The new tile.</returns>
        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(this.Content.Load<Texture2D>(name), collision);
        }


        ///// <summary>
        ///// Loads a tile with a random appearance.
        ///// </summary>
        ///// <param name="baseName">
        ///// The content name prefix for this group of tile variations. Tile groups are
        ///// name LikeThis0.png and LikeThis1.png and LikeThis2.png.
        ///// </param>
        ///// <param name="variationCount">
        ///// The number of variations in this group.
        ///// </param>
        ///// <param name="collision"></param>
        //private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
        //{
        //    var index = RandomUtil.Next(variationCount);

        //    return LoadTile(baseName + index, collision);
        //}


        /// <summary>
        /// Instantiates a player, puts him in the level, and remembers where to put him when he is resurrected.
        /// </summary>
        private Tile LoadStartTile(int x, int y)
        {
            if (this.Player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            var startPosition = this.GetBounds(x, y).GetBottomCenter();

            switch (this._playerInfo.Type)
            {
                case PlayerType.GordoMercenario:
                    this.Player = new GordoMercenario(this, startPosition, this._weaponInventory, this._primaryWeapon, this._secondaryWeapon, this._ammoInventory);
                    break;
                case PlayerType.Obama:
                    this.Player = new Obama(this, startPosition, this._weaponInventory, this._primaryWeapon, this._secondaryWeapon, this._ammoInventory);
                    break;
                default:
                    throw new Exception("Player no contemplado");
            }

            //return new Tile(null, TileCollision.Passable);
            return this.LoadPassableTile();
        }

        ///// <summary>
        ///// Instantiates a player, puts him in the level, and remembers where to put him when he is resurrected.
        ///// </summary>
        //private void LoadStartTile(Vector2 startPosition, int lives)
        //{
        //    if (Player != null)
        //        throw new NotSupportedException("A level may only have one starting point.");

        //    switch (_playerInfo.Type)
        //    {
        //        case PlayerType.GordoMercenario:
        //            Player = new GordoMercenario(this, startPosition, _weaponInventory, _primaryWeapon, _secondaryWeapon, _ammoInventory);
        //            break;
        //        case PlayerType.Obama:
        //            Player = new Obama(this, startPosition, _weaponInventory, _primaryWeapon, _secondaryWeapon, _ammoInventory);
        //            break;
        //        default:
        //            throw new Exception("Player no contemplado");
        //    }

        //    Player.SetLives(lives);
        //}

        /// <summary>
        /// Remembers the location of the level's exit.
        /// </summary>
        private Tile LoadExitTile(int x, int y)
        {
            if (this._exit != InvalidPosition)
                throw new NotSupportedException("A level may only have one exit.");

            this._exit = this.GetBounds(x, y).Center;

            return this.LoadTile(GlobalParameters.EXIT_TILE, TileCollision.Passable);
        }

        ///// <summary>
        ///// Instantiates a death tile and puts it in the level.
        ///// </summary>
        //private Tile LoadDeathTile(string baseName, int x, int y)
        //{
        //    Point position = GetBounds(x, y).Center;
        //    DeathTiles.Add(new DeathTile(ScreenManager, this, new Vector2(position.X, position.Y), baseName));

        //    return new Tile(null, TileCollision.Passable);
        //}

        ///// <summary>
        ///// Instantiates an enemy and puts him in the level.
        ///// </summary>
        //private Tile LoadEnemyTile(int x, int y, NpcTypes enemyType)
        //{
        //    var position = GetBounds(x, y).GetBottomCenter();

        //    switch (enemyType)
        //    {
        //        case NpcTypes.FastWeakMonster:
        //            Enemies.Add(new FastWeakMonster(this, position) { DrawHealthBar = false });
        //            break;
        //        case NpcTypes.NormalMonster:
        //            Enemies.Add(new NormalMonster(this, position) { DrawHealthBar = false });
        //            break;
        //        case NpcTypes.SlowStrongMonster:
        //            Enemies.Add(new SlowStrongMonster(this, position) { DrawHealthBar = false });
        //            break;
        //        default:
        //            throw new Exception("No se puede crear enemigo ya que no existe definicion del mismo");
        //    }

        //    return new Tile(null, TileCollision.Passable);
        //}

        ///// <summary>
        ///// Instantiates an enemy and puts him in the level.
        ///// </summary>
        //private void LoadEnemyTile(Vector2 enemyPosition, NpcTypes enemyType, bool isAlive, int currentHealth)
        //{
        //    switch (enemyType)
        //    {
        //        case NpcTypes.FastWeakMonster:
        //            Enemies.Add(new FastWeakMonster(this, enemyPosition, isAlive, currentHealth) { DrawHealthBar = false });
        //            break;
        //        case NpcTypes.NormalMonster:
        //            Enemies.Add(new NormalMonster(this, enemyPosition, isAlive, currentHealth) { DrawHealthBar = false });
        //            break;
        //        case NpcTypes.SlowStrongMonster:
        //            Enemies.Add(new SlowStrongMonster(this, enemyPosition, isAlive, currentHealth) { DrawHealthBar = false });
        //            break;
        //        default:
        //            throw new Exception("No se puede crear enemigo ya que no existe definicion del mismo");
        //    }
        //}

        /// <summary>
        /// Instantiates a gem and puts it in the level.
        /// </summary>
        //private Tile LoadGemTile(int x, int y)
        //{
        //    return LoadGemTile(x, y, GemType.Normal);
        //}

        //private Tile LoadGemTile(int x, int y, GemType gemType)
        //{
        //    Point position = GetBounds(x, y).Center;
        //    _gems.Add(new Gem(this, new Vector2(position.X, position.Y), gemType));

        //    return new Tile(null, TileCollision.Passable);
        //}

        ///// <summary>
        ///// Instantiates a item and puts it in the level.
        ///// </summary>
        //private Tile LoadItemTile(int x, int y, ItemType itemType)
        //{
        //    Point position = GetBounds(x, y).Center;

        //    Items.Add(new Item(ScreenManager, this, new Vector2(position.X, position.Y), itemType));
        //    if (itemType == ItemType.Gem)
        //    {
        //        if (GemsRequired && !_customGemsRequired)
        //            GemsToCollect += 1;
        //    }
        //    return new Tile(null, TileCollision.Passable);
        //}

        ///// <summary>
        ///// Instantiates a item and puts it in the level.
        ///// </summary>
        //private void LoadItemTile(Vector2 position, ItemType itemType)
        //{
        //    Items.Add(new Item(ScreenManager, this, new Vector2(position.X, position.Y), itemType));
        //    if (itemType == ItemType.Gem)
        //    {
        //        if (GemsRequired && !_customGemsRequired)
        //            GemsToCollect += 1;
        //    }
        //}

        ///// <summary>
        ///// Unloads the level content.
        ///// </summary>
        //public void Dispose()
        //{
        //    Content.Unload();
        //}

        #endregion

        #region Bounds and collision

        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the left or right edges, but allowing things
        /// to jump beyond the top of the level and fall off the bottom.
        /// </summary>
        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= this.Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= this.Height)
                return TileCollision.Passable;

            return this._tiles[x, y].Collision;
        }

        public TileCollision GetTileCollisionBehindPlayer(Vector2 playerPosition)
        {
            int x = (int)playerPosition.X / Tile.WIDTH;
            int y = (int)(playerPosition.Y - 1) / Tile.HEIGHT;

            // Prevent escaping past the level ends.
            if (x == this.Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y == this.Height)
                return TileCollision.Passable;

            return this._tiles[x, y].Collision;
        }

        public TileCollision GetTileCollisionBelowPlayer(Vector2 playerPosition)
        {
            int x = (int)playerPosition.X / Tile.WIDTH;
            int y = (int)playerPosition.Y / Tile.HEIGHT;

            // Prevent escaping past the level ends.
            if (x == this.Width)
                return TileCollision.Impassable;

            // Allow jumping past the level top and falling through the bottom.
            if (y == this.Height)
                return TileCollision.Passable;

            return this._tiles[x, y].Collision;
        }


        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.WIDTH, y * Tile.HEIGHT, Tile.WIDTH, Tile.HEIGHT);
        }

        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return this._tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return this._tiles.GetLength(1); }
        }

        public int CurrentLevel { get; private set; }

        #endregion

        #region Update

        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update(GameTime gameTime, KeyboardState keyboardState, GamePadState gamePadState, MouseState mouseState, VirtualGamePadState virtualGamePadState)
        {
            // Si el nivel no esta cargado, entonces que no ejecute logica
            // Util por si los datos del nivel vienen a traves de un servicio asincronico
            if (!this.LevelLoaded) return;

            // Pause while the player is dead or time is expired.
            if (!this.Player.IsAlive || this.TimeRemaining == TimeSpan.Zero)
            {
                // Still want to perform physics on the player.
                this.Player.ApplyPhysics(gameTime);
            }
            else if (this.ReachedExit)
            {
                // Animate the time being converted into points.
                var seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f);
                seconds = Math.Min(seconds, (int)Math.Ceiling(this.TimeRemaining.TotalSeconds));
                this.TimeRemaining -= TimeSpan.FromSeconds(seconds);
                this.Score += seconds * GlobalParameters.POINTS_PER_SECOND;
            }
            else
            {
                this.TimeRemaining -= gameTime.ElapsedGameTime;
                this.Player.Update(gameTime, keyboardState, gamePadState, mouseState, virtualGamePadState);

                //UpdateItems(gameTime);

                // Falling off the bottom of the level kills the player.
                //if (Player.BoundingRectangle.Top >= Height * Tile.HEIGHT)
                //    OnPlayerHit();

                //UpdateSurvivors(gameTime);

                this.UpdateEnemies(gameTime);

                // The player has reached the exit if they are standing on the ground and
                // his bounding rectangle contains the center of the exit tile. They can only
                // exit when they have collected all of the gems.
                if (this.Player.IsAlive &&
                    this.Player.IsOnGround &&
                    this.Player.BoundingRectangle.Contains(this._exit))
                {
                    this.OnExitReached();
                }

				// TODO: Nextlife - revisar si esto va a ser util
				//                if (Score > NextLife)
//                {
//                    Player.GainLive();
//                    NextLife += NextLife;
//                }
            }

            // Clamp the time remaining at zero.
            if (this.TimeRemaining < TimeSpan.Zero)
                this.TimeRemaining = TimeSpan.Zero;
        }

        /// <summary>
        /// Animates each gem and checks to allows the player to collect them.
        /// </summary>
        //private void UpdateGems(GameTime gameTime)
        //{
        //    for (int i = 0; i < _gems.Count; ++i)
        //    {
        //        Gem gem = _gems[i];

        //        gem.Update(gameTime);

        //        if (gem.BoundingCircle.Intersects(Player.BoundingRectangle))
        //        {
        //            _gems.RemoveAt(i--);
        //            OnGemCollected(gem, Player);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Animates each gem and checks to allows the player to collect them.
        ///// </summary>
        //private void UpdateItems(GameTime gameTime)
        //{
        //    for (int i = 0; i < Items.Count; ++i)
        //    {
        //        Item item = Items[i];

        //        item.Update(gameTime);

        //        if (item.BoundingCircle.Intersects(Player.BoundingRectangle))
        //        {
        //            OnItemCollected(item, Player, i);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Called when a item is collected.
        ///// </summary>
        ///// <param name="item">The item that was collected.</param>
        ///// <param name="collectedBy">The player who collected this item.</param>
        ///// <param name="number">
        ///// Item Number Order in Items Collection
        ///// </param>
        //private void OnItemCollected(Item item, Player collectedBy, int number)
        //{
        //    item.OnCollected(collectedBy, number);
        //}

        ///// <summary>
        ///// Animates each gem and checks to allows the player to collect them.
        ///// </summary>
        //private void UpdateDeathTiles(GameTime gameTime, bool updateEntireObject)
        //{
        //    for (int i = 0; i < DeathTiles.Count; ++i)
        //    {
        //        DeathTile deathTile = DeathTiles[i];
        //        if (updateEntireObject)
        //            deathTile.Update(gameTime);

        //        if (deathTile.BoundingRectangle.Intersects(Player.BoundingRectangle))
        //        {
        //            OnPlayerKill();
        //        }
        //    }
        //}

        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        private void UpdateEnemies(GameTime gameTime)
        {
            // Si el nivel no tiene enemigos, entonces que no haga nada :)
            if (this._enemiesParameters.Count <= 0) return;

            // Spawn a new enemy enemy every X seconds
            if (this._previousSpawnTime > this._enemySpawnTime)
            {
                this._enemySpawnTime =
                    TimeSpan.FromSeconds(RandomUtil.NextDouble(this._enemySpawnTimeFrom.TotalSeconds,
                                                               this._enemySpawnTimeTo.TotalSeconds));

                //_previousSpawnTime = gameTime.TotalGameTime;
                this._previousSpawnTime = TimeSpan.FromSeconds(0);
                // Add an Enemy
                this.AddEnemy();
            }
            else
            {
                this._previousSpawnTime += gameTime.ElapsedGameTime;
            }

            if (this.FastWeakMonstersPool != null)
            {
                foreach (var node in this.FastWeakMonstersPool.ActiveNodes)
                {
                    node.Item.Update(gameTime);
                    
                    var enemyDistanceFromPlayer = (int)Math.Abs(node.Item.Position.X - this.Player.Position.X);

                    // Si la distancia entre el enemigo y player es > a la distancia limite, que desaparezca
                    if (enemyDistanceFromPlayer > GlobalParameters.ENEMY_LIMIT_DISTANCE)
                    {
                        this.FastWeakMonstersPool.Return(node);
                    }
                    else
                    {
                        if (node.Item.IsAlive)
                        {
                            // Verificar si colisiona el enemigo contra el cuchillo del Player
                            if (node.Item.BoundingRectangle.Intersects(this.Player.KnifeBoundingRectangle))
                            {
                                if (this.Player.IsAttacking && !this.Player.HasHitEnemy)
                                {
                                    this.OnEnemyHit(node.Item, this.Player);
                                }
                            }

                            // Verificar si colisiona el enemigo contra el Player, entonces atacarlo
                            if (node.Item.BoundingRectangle.Intersects(this.Player.BoundingRectangle))
                            {
                                node.Item.AttackPlayer(this.Player);
                                this.OnPlayerHit(node.Item);
                            }
                            else
                            {
                                node.Item.NotAttackingPlayer();
                            }
                        }
                        else if (node.Item.DeathTime < 0)
                        {
                            // Si el enemigo murio, entonces lo marcamos como disponible dentro del pool
                            this.FastWeakMonstersPool.Return(node);
                        }
                    }
                }
            }

            if (this.NormalMonstersPool != null)
            {
                foreach (var node in this.NormalMonstersPool.ActiveNodes)
                {
                    node.Item.Update(gameTime);

                    var enemyDistanceFromPlayer = (int)Math.Abs(node.Item.Position.X - this.Player.Position.X);

                    // Si la distancia entre el enemigo y player es > a la distancia limite, que desaparezca
                    if (enemyDistanceFromPlayer > GlobalParameters.ENEMY_LIMIT_DISTANCE)
                    {
                        this.NormalMonstersPool.Return(node);
                    }
                    else
                    {
                        if (node.Item.IsAlive)
                        {
                            // Verificar si colisiona el enemigo contra el cuchillo del Player
                            if (node.Item.BoundingRectangle.Intersects(this.Player.KnifeBoundingRectangle))
                            {
                                if (this.Player.IsAttacking && !this.Player.HasHitEnemy)
                                {
                                    this.OnEnemyHit(node.Item, this.Player);
                                }
                            }

                            // Verificar si colisiona el enemigo contra el Player, entonces atacarlo
                            if (node.Item.BoundingRectangle.Intersects(this.Player.BoundingRectangle))
                            {
                                node.Item.AttackPlayer(this.Player);
                                this.OnPlayerHit(node.Item);
                            }
                            else
                            {
                                node.Item.NotAttackingPlayer();
                            }
                        }
                        else if (node.Item.DeathTime < 0)
                        {
                            // Si el enemigo murio, entonces lo marcamos como disponible dentro del pool
                            this.NormalMonstersPool.Return(node);
                        }
                    }
                }
            }

            if (this.SlowStrongMonstersPool != null)
            {
                foreach (var node in this.SlowStrongMonstersPool.ActiveNodes)
                {
                    node.Item.Update(gameTime);

                    if (node.Item.IsAlive)
                    {
                        var enemyDistanceFromPlayer = (int)Math.Abs(node.Item.Position.X - this.Player.Position.X);

                        // Si la distancia entre el enemigo y player es > a la distancia limite, que desaparezca
                        if (enemyDistanceFromPlayer > GlobalParameters.ENEMY_LIMIT_DISTANCE)
                        {
                            this.SlowStrongMonstersPool.Return(node);
                        }
                        else
                        {
                            // Verificar si colisiona el enemigo contra el cuchillo del Player
                            if (node.Item.BoundingRectangle.Intersects(this.Player.KnifeBoundingRectangle))
                            {
                                if (this.Player.IsAttacking && !this.Player.HasHitEnemy)
                                {
                                    this.OnEnemyHit(node.Item, this.Player);
                                }
                            }

                            // Verificar si colisiona el enemigo contra el Player, entonces atacarlo
                            if (node.Item.BoundingRectangle.Intersects(this.Player.BoundingRectangle))
                            {
                                node.Item.AttackPlayer(this.Player);
                                this.OnPlayerHit(node.Item);
                            }
                            else
                            {
                                node.Item.NotAttackingPlayer();
                            }
                        }
                    }
                    else if (node.Item.DeathTime < 0)
                    {
                        // Si el enemigo murio, entonces lo marcamos como disponible dentro del pool
                        this.SlowStrongMonstersPool.Return(node);
                    }
                }
            }
        }

        /// <summary>
        /// Agrega enemigos del pool al juego
        /// </summary>
        private void AddEnemy()
        {
            // Si el nivel no tiene definido los porcentajes de aparicion de enemigos
            if (this._enemiesParameters.Count <= 0) return; 

            // Si los pooles no poseen ningun item disponible, entonces retornamos
            if ((this.FastWeakMonstersPool == null || this.FastWeakMonstersPool.AvailableCount <= 0)
                && (this.NormalMonstersPool == null || this.NormalMonstersPool.AvailableCount <= 0)
                && (this.SlowStrongMonstersPool == null || this.SlowStrongMonstersPool.AvailableCount <= 0))
                return;

            var enemyType = this.GetRandomEnemyType();
            var faceDirection = this.GetEnemyFaceDirection();
            var position = this.GetEnemyPosition(faceDirection);

            switch (enemyType)
            {
                case NpcTypes.FastWeakMonster:
                    // Verify there are some left in the pool
                    if (this.FastWeakMonstersPool != null && this.FastWeakMonstersPool.AvailableCount > 0)
                    {
                        // Get an enemy from the pool.
                        var fastWeakMonster = this.FastWeakMonstersPool.Get().Item;

                        fastWeakMonster.Start(position);
                        fastWeakMonster.Direction = faceDirection;
                    }
                    break;
                case NpcTypes.NormalMonster:
                    // Verify there are some left in the pool
                    if (this.NormalMonstersPool != null && this.NormalMonstersPool.AvailableCount > 0)
                    {
                        // Get an enemy from the pool.
                        var normalMonster = this.NormalMonstersPool.Get().Item;

                        normalMonster.Start(position);
                        normalMonster.Direction = faceDirection;
                    }
                    break;
                case NpcTypes.SlowStrongMonster:
                    // Verify there are some left in the pool
                    if (this.SlowStrongMonstersPool != null && this.SlowStrongMonstersPool.AvailableCount > 0)
                    {
                        // Get an enemy from the pool.
                        var slowStrongMonster = this.SlowStrongMonstersPool.Get().Item;

                        slowStrongMonster.Start(position);
                        slowStrongMonster.Direction = faceDirection;
                    }
                    break;
                default:
                    throw new Exception("No se puede crear enemigo ya que no existe definicion del mismo");
            }
        }

        private Vector2 GetEnemyPosition(FaceDirection faceDirection)
        {
            var position = Vector2.Transform(
                faceDirection == FaceDirection.Left ? new Vector2(GlobalParameters.SCREEN_WIDTH + 10, this.Player.Position.Y) : new Vector2(-10, this.Player.Position.Y),
                Matrix.Invert(Matrix.CreateTranslation(-this.CameraPositionXAxis,
                                                       -this.CameraPositionYAxis, 0.0f)));

            return position;
        }

        private FaceDirection GetEnemyFaceDirection()
        {
            int enemySide;

            if (this.Player.IsRunning)
            {
                enemySide = this.Player.Direction == SpriteEffects.FlipHorizontally ? 1 : 0;
            }
            else
            {
                enemySide = RandomUtil.Next(0, 2);
            }

            return enemySide > 0 ? FaceDirection.Left : FaceDirection.Right;
        }

        private NpcTypes GetRandomEnemyType()
        {
            var enemyType = NpcTypes.SlowStrongMonster;
            double percent = 0;
            var random = RandomUtil.NextDouble();

            foreach (var enemyParameter in this._enemiesParameters)
            {
                percent += enemyParameter.Value.Percent;
                if (random > percent) continue;
                enemyType = (NpcTypes) Enum.Parse(typeof (NpcTypes), enemyParameter.Key, true);
                break;
            }
            return enemyType;
        }

        ///// <summary>
        ///// Animates each survivor.
        ///// </summary>
        //private void UpdateSurvivors(GameTime gameTime)
        //{
        //    for (int i = 0; i < Survivors.Count; ++i)
        //    {
        //        Survivor survivor = Survivors[i];

        //        if (survivor.IsAlive)
        //        {
        //            survivor.Update(gameTime);

        //            // Catching an survivor
        //            if (Player.IsCatching && survivor.BoundingRectangle.Intersects(Player.BoundingRectangle))
        //            {
        //                OnSurvivorCatched(survivor, Player);
        //            }

        //            // Stopping an survivor
        //            if (Player.IsStoping && survivor.BoundingRectangle.Intersects(Player.BoundingRectangle) && survivor.CurrentSate != CharacterStates.Stopped)
        //            {
        //                OnSurvivorStopped(survivor, Player);
        //            }

        //            // Attack survivors with knife
        //            //if (survivor.BoundingRectangle.Intersects(Player.KnifeBoundingRectangle))
        //            //{
        //            //    if (Player.IsAttacking && !Player.HasHitEnemy)
        //            //    {
        //            //        OnNpCharacterHit(survivor, Player);
        //            //    }

        //            //}
        //        }
        //    }

        //    // Whether successful or not, stop trying to do actions.
        //    Player.IsCatching = false;
        //    Player.IsStoping = false;
        //}

        //private void OnSurvivorStopped(Survivor survivor, Player player)
        //{
        //    // Stop trying to stop
        //    player.IsStoping = false;
        //    //player.CatchedCharacter = survivor;

        //    survivor.OnStopped(player);
        //}

        //private void OnSurvivorCatched(Survivor survivor, Player player)
        //{
        //    // Stop trying to catch
        //    player.IsCatching = false;
        //    player.CatchedCharacter = survivor;

        //    survivor.OnCatched(player);
        //}


        private void OnEnemyHit(Enemy enemyHit, Player playerHitBy)
        {
            enemyHit.OnHit(playerHitBy, null);
            playerHitBy.HasHitEnemy = true;
        }

        //private void OnSurvivorHit(Survivor survivor, Enemy enemy)
        //{
        //    survivor.OnHit(null);
        //    enemy.AttackSurvivor(survivor);
        //}

        //private void OnEnemyKilled(Enemy enemy, Player killedBy)
        //{
        //    // Llamamos la campo static para sumar el puntaje por matar un enemigo.
        //    // Si quisieramos que los enemigos sumanaran distinto puntaje x tipo, deberiamos llamar al
        //    // objeto enermy que le pasamos al metodo OnEnemyKilled
        //    _score += Enemy.POINT_VALUE;
        //    enemy.OnHit(killedBy);
        //}

        ///// <summary>
        ///// Called when a gem is collected.
        ///// </summary>
        ///// <param name="gem">The gem that was collected.</param>
        ///// <param name="collectedBy">The player who collected this gem.</param>
        //private void OnGemCollected(Gem gem, Player collectedBy)
        //{
        //    // Cuando todas las gemeas eran iguales, valia la propiedad estatica, pero ahora que cada una es diferente
        //    // hay que acudir a la propiedad del objeto
        //    //_score += Gem.PointValue;
        //    _score += gem.PointValue;

        //    gem.OnCollected(collectedBy);
        //}

        /// <summary>
        /// Called when the player is killed.
        /// </summary>
        /// <param name="hitBy">
        /// The enemy who killed the player. This is null if the player was not killed by an
        /// enemy, such as when a player falls into a hole.
        /// </param>
        private void OnPlayerHit(Enemy hitBy)
        {
            this.Player.OnHit(hitBy);
        }

        ///// <summary>
        ///// Called when the player is instantly killed.
        ///// </summary>
        //private void OnPlayerKill()
        //{
        //    Player.OnKilled(null);
        //}

        /// <summary>
        /// Called when the player reaches the level's exit.
        /// </summary>
        private void OnExitReached()
        {
            this.Player.OnReachedExit();
            this._exitReachedSound.Play();
            this.ReachedExit = true;
        }

        ///// <summary>
        ///// Restores the player to the starting point to try the level again
        ///// or Game over if has no more lifes.
        ///// </summary>
        //public void StartNewLife()
        //{
        //    if (Player.CurrentHealth < 0)
        //    {
        //        // Game Over! Put finishing screen stuff here
        //        /////////////////////GO TO MAIN MENU///////////////////
        //        LoadingScreen.Load(ScreenManager, true, PlayerIndex.One, new BackgroundScreen(), new MainMenuScreen());
        //    }
        //    else
        //    {
        //        Player.TakeLive();

        //        Player.Reset(Checkpoint != Vector2.Zero ? Checkpoint : _start);
        //    }
        //}

        #endregion

        #region Draw

        /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Si el nivel no esta cargado aun, volvemos. Esto es util cuando estamos esperando la respuesta asincronica de un servicio
            if (!this.LevelLoaded) return;

            // Dibujamos los elementos que van atras (background)
            spriteBatch.Begin();
            this.DrawBackground(spriteBatch);
            spriteBatch.End();
			
			
            // Scrolleamos la camara
            this.ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
            var cameraTransform = Matrix.CreateTranslation(-this.CameraPositionXAxis, -this.CameraPositionYAxis, 0.0f);

            // Dibujamos los elementos que son afectados por la camara
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, cameraTransform);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);

            this.DrawTiles(spriteBatch);

            //DrawItems(spriteBatch, gameTime);

            //DrawDeathTiles(spriteBatch, gameTime);

            this.DrawPlayer(spriteBatch, gameTime);

            //DrawSurvivors(spriteBatch, gameTime);

            this.DrawEnemies(spriteBatch, gameTime);

            spriteBatch.End();
      
//#if !IPHONE
//            // Dibujamos los elementos que van adelante (foreground)
//            spriteBatch.Begin();
//            DrawForeground(spriteBatch);
//            spriteBatch.End();
//#endif
            
        }

//#if !IPHONE
//        private void DrawForeground(SpriteBatch spriteBatch)
//        {
//            foreach (var layer in _foreGroundLayers)
//                layer.Draw(spriteBatch, CameraPositionXAxis);
//        }
//#endif

        private void DrawBackground(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < this._backGroundLayers.Length; i++)
                this._backGroundLayers[i].Draw(spriteBatch, this.CameraPositionXAxis);
        }

        //private void DrawItems(SpriteBatch spriteBatch, GameTime gameTime)
        //{
        //    for (int i = 0; i < Items.Count; i++)
        //    {
        //        Item item = Items[i];
        //        item.Draw(gameTime, spriteBatch);
        //    }
        //}

        /// <summary>
        /// This method calculates how much background is scrolled when the player reaches the screen's edge.
        /// When the begin scrolling is platform-dependent. Because the Zune screen is the narrowest of the three, 
        /// it looks the farthest ahead. The other two don't look ahead as much. This factor is used to calculate
        /// the edges of the screen and how far to scroll when the player reaches that edge.
        /// Scrolling continues until either end of the level is reached. At that point, the camera position is clamped.
        ///  Actualiza _cameraPositionXAxis y _cameraPositionYAxis en funcion de la posicion del PLAYER y de los limites del nivel.
        /// _cameraPositionXAxis y _cameraPositionYAxis luego se usaran para posicionar la camara.
        /// </summary>
        /// <param name="viewport"></param>
        private void ScrollCamera(Viewport viewport)
        {
            // Calculate the scrolling borders for the X-axis. 
            // Estable en que porcentaje de la pantalla debe situarse el player (0.5 es en la mitad de la pantalla)
            const float leftMargin = 0.5f;
            
            // Calculate the edges of the screen.
            var marginWidth = viewport.Width * leftMargin;
            var marginLeft = this.CameraPositionXAxis + marginWidth;
            var marginRight = this.CameraPositionXAxis + viewport.Width - marginWidth;

            // Calculate how far to scroll when the player is near the edges of the screen.
            var cameraMovementX = 0.0f;
            
            if (this.Player.Position.X < marginLeft)
                cameraMovementX = this.Player.Position.X - marginLeft;
            else if (this.Player.Position.X > marginRight)
                cameraMovementX = this.Player.Position.X - marginRight;
            
            // Update the camera position, but prevent scrolling off the ends of the level.
            float maxCameraPosition = Tile.WIDTH * this.Width - viewport.Width;
            this.CameraPositionXAxis = MathHelper.Clamp(this.CameraPositionXAxis + cameraMovementX, 0.0f, maxCameraPosition);


            // Calculate the scrolling borders for the Y-axis. 
            const float topMargin = 0.3f;
            const float bottomMargin = 0.3f;//0.1f;

            float marginTop = this.CameraPositionYAxis + viewport.Height * topMargin;
            float marginBottom = this.CameraPositionYAxis + viewport.Height - viewport.Height * bottomMargin;

            // Calculate how far to vertically scroll when the player is near the top or bottom of the screen. 
            float cameraMovementY = 0.0f;

            if (this.Player.Position.Y < marginTop) //above the top margin 
                cameraMovementY = this.Player.Position.Y - marginTop;
            else if (this.Player.Position.Y > marginBottom) //below the bottom margin
                cameraMovementY = this.Player.Position.Y - marginBottom;

            // Tracks the highest scrolling point for the camera:
            float maxCameraPositionYOffset = Tile.HEIGHT * this.Height - viewport.Height;
            this.CameraPositionYAxis = MathHelper.Clamp(this.CameraPositionYAxis + cameraMovementY, 0.0f, maxCameraPositionYOffset);
        }

        private void DrawEnemies(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Draw FastWeakMonsters
            if (this.FastWeakMonstersPool != null)
            {
                foreach (var enemy in this.FastWeakMonstersPool)
                {
                    if (enemy.IsAlive || enemy.DeathTime > 0)
                        enemy.Draw(gameTime, spriteBatch);
                }
            }

            // Draw NormalMonsters
            if (this.NormalMonstersPool != null)
            {
                foreach (var enemy in this.NormalMonstersPool)
                {
                    if (enemy.IsAlive || enemy.DeathTime > 0)
                        enemy.Draw(gameTime, spriteBatch);
                }
            }

            // Draw SlowStrongMonsters
            if (this.SlowStrongMonstersPool != null)
            {
                foreach (var enemy in this.SlowStrongMonstersPool)
                {
                    if (enemy.IsAlive || enemy.DeathTime > 0)
                        enemy.Draw(gameTime, spriteBatch);
                }
            }
        }

        //private void DrawSurvivors(SpriteBatch spriteBatch, GameTime gameTime)
        //{
        //    for (int i = 0; i < Survivors.Count; i++)
        //    {
        //        Survivor survivor = Survivors[i];
        //        if (survivor.IsAlive || survivor.DeathTime > 0)
        //            survivor.Draw(gameTime, spriteBatch);
        //    }
        //}

        //private void DrawDeathTiles(SpriteBatch spriteBatch, GameTime gameTime)
        //{
        //    for (int i = 0; i < DeathTiles.Count; i++)
        //    {
        //        DeathTile deathTile = DeathTiles[i];
        //        deathTile.Draw(gameTime, spriteBatch, true);
        //    }
        //}

        private void DrawPlayer(SpriteBatch spriteBatch, GameTime gameTime)
        {
            this.Player.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            // Because the scrolling extension draws tiles off-screen, you should be aware that 
            // this could impact the frame rate. To avoid any slowdown you'll need to implement
            // a simple culling feature that limits the amount of tiles drawn to only those on
            // the screen at the time. This reduces the drawing load, speeding up the game.

            // Calculate the visible range of tiles.
            var left = (int)Math.Floor(this.CameraPositionXAxis / Tile.WIDTH);
            //var right = left + (int)Math.Floor(spriteBatch.GraphicsDevice.Viewport.Width / (double)Tile.WIDTH);
            var right = left + this._tilesPerScreen;
            right = Math.Min(right, this.Width - 1);

            // For each tile position
            for (int y = 0; y < this.Height; ++y)
            {
                //for (int x = 0; x < Width; ++x)
                for (int x = left; x <= right; ++x)
                {
                    // If there is a visible tile in that position
                    if (this._tiles[x, y].Texture == null) continue;

                    // Draw it in screen space.
                    var position = new Vector2(x, y) * Tile.Size;
                    spriteBatch.Draw(this._tiles[x, y].Texture, position, Color.White);
                }
            }
        }

        public void SaveLevelData()
        {
            throw new NotImplementedException("Save / Load Game no implementado");
//#if WINDOWS_PHONE || WINDOWS ||XBOX
//            // make sure the device is ready
//            if (Global.SaveDevice.IsReady)
//            {

//                SaveGameData dataToSave = new SaveGameData();

//                // Player data
//                dataToSave.PlayerPosition = Player.Position;
//                dataToSave.PlayerLives = Player.CurrentHealth;

//                // Level Data
//                dataToSave.Level = CurrentLevel;
//                dataToSave.Score = Score;
//                dataToSave.NextLifeScore = NextLife;
//                dataToSave.SecondsRemaining = TimeRemaining.TotalSeconds;

//                dataToSave.CameraPositionXAxis = CameraPositionXAxis;
//                dataToSave.CameraPositionYAxis = CameraPositionYAxis;

//                // Enemies Data
//                dataToSave.Enemies = new List<SaveGameDataEnemy>();

//                for (int i = 0; i < Enemies.Count; ++i)
//                {
//                    SaveGameDataEnemy tmpSaveGameDataEnemy = new SaveGameDataEnemy();

//                    tmpSaveGameDataEnemy.IsAlive = Enemies[i].IsAlive;
//                    tmpSaveGameDataEnemy.Position = Enemies[i].Position;
//                    tmpSaveGameDataEnemy.CurrentHealth = Enemies[i].CurrentHealth;
//                    //tmpSaveGameDataEnemy.SpriteSet = Enemies[i].SpriteSet;

//                    dataToSave.Enemies.Add(tmpSaveGameDataEnemy);
//                }

//                // Survivors Data
//                dataToSave.Survivors = new List<SaveGameDataSurvivor>();

//                for (int i = 0; i < Survivors.Count; ++i)
//                {
//                    SaveGameDataSurvivor tmpSaveGameDataSurvivor = new SaveGameDataSurvivor();

//                    tmpSaveGameDataSurvivor.IsAlive = Survivors[i].IsAlive;
//                    tmpSaveGameDataSurvivor.Position = Survivors[i].Position;
//                    tmpSaveGameDataSurvivor.CurrentHealth = Survivors[i].CurrentHealth;
//                    //tmpSaveGameDataSurvivor.SpriteSet = Survivors[i].SpriteSet;

//                    dataToSave.Survivors.Add(tmpSaveGameDataSurvivor);
//                }

//                // Items Data
//                dataToSave.Items = new List<SaveGameDataItem>();

//                //for (int i = 0; i < Items.Count; ++i)
//                //{
//                //    SaveGameDataItem tmpSaveGameDataItem = new SaveGameDataItem();
//                //    tmpSaveGameDataItem.Position = Items[i].Position;
//                //    tmpSaveGameDataItem.ItemObject = (int)Items[i].ItemType;
//                //    tmpSaveGameDataItem.IsOn = Items[i].IsItemOn;

//                //    dataToSave.Items.Add(tmpSaveGameDataItem);
//                //}

//                // Convert the object to XML data and put it in the stream.
//                XmlSerializer serializer = new XmlSerializer(typeof(SaveGameData));

//                // save a file asynchronously. this will trigger IsBusy to return true
//                // for the duration of the save process.
//                Global.SaveDevice.SaveAsync(
//                    Global.ContainerName,
//                    Global.FileNameSaveGame,
//                    stream => serializer.Serialize(stream, dataToSave));
//            }
//#endif
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Platformer.Animations;
using Platformer.Helpers;
using Platformer.Levels;
using Platformer.Npc.Enemies;
using Platformer.Weapons;
#if WINDOWS_PHONE || IPHONE
using Microsoft.Xna.Framework.Input.Touch;
using Mobile.Base.Components;
using Mobile.Base.VirtualInput;
#elif SILVERLIGHT
using Web.Base.Components;
using Web.Base.VirtualInput;
#elif WINDOWS
using Desktop.Base.Components;
using Desktop.Base.VirtualInput;
#endif

namespace Platformer.Players
{
    /// <summary>
    /// Our fearless adventurer!
    /// </summary>
    public abstract class Player
    {
        /// <summary>
        /// Maximo tiempo q le podran volver a lastimar, es decir, cuanto tiempo tiene que pasar entre mordedura y mordedura
        /// del zombie para que el player le vuelva a bajar la energia. A mayor tiempo, menos dano pueden hacerle.
        /// </summary>
        protected float MaxHitTime;

        public Vector2 Position { get; protected set; }
        
        protected HealthBar HealthBar;
        public bool DrawHealthBar { get; set; }

        public Actions CurrentAction { get; set; }
        //public MouseState MouseStateCurrent { get; private set; }

        //private readonly List<Bullet> _bullets;
        private Pool<Bullet> _bulletsPool;

        //private MouseState _mouseStatePrevious;
        //private GamePadState _previousGamePadState;

        //private const int LADDER_ALIGNMENT = 6;

        private bool _wasClimbing;

        //This used to be private float movement;
        private Vector2 _movement;

        KeyboardState _previousKeyboardState = Keyboard.GetState();

        // Animations
        protected Animation IdleAnimation;
        protected Animation RunAnimation;
        protected Animation DieAnimation;
        protected Animation AttackAnimation;
        protected Animation BeAttackedAnimation;
        protected Animation CelebrateAnimation;

        //protected Animation PrimaryWeaponIdleAnimation;
        //protected Animation SecondaryWeaponIdleAnimation;
        //protected Animation PrimaryWeaponIdleShootingAnimation;
        //protected Animation SecondaryWeaponIdleShootingAnimation;


        /// <summary>
        /// Maximo tiempo q esta paralizado x ser lastimado, es decir, cuanto tiempo se queda quieto luego de una mordedura 
        /// (tiempo que tiene que esperar cuando es mordido para volver a moverse). A mayor tiempo, mas tiempo va a estar parado.
        /// </summary>
        protected float MaxParalyzedTime = 0.5f;
        
        private float _hitTime;
        private float _paralyzedTime;

        public SpriteEffects Direction { get; private set; }

        protected AnimationPlayer Sprite;
        protected AnimationPlayer WeaponSprite;

        // Sounds
        protected SoundEffect KilledSound;

        private float _previousBottom;

        public bool IsClimbing { get; private set; }

        //public GameObject CurrentWeaponAnimation { get; protected set; }

        public Level Level { get; private set; }

        public bool IsAlive { get; private set; }

        private Vector2 _velocity;

        /// <summary>
        /// Constants for controling horizontal movement.
        /// Aceleracion o velocidad con la cual se mueve, se desplaza por el nivel. A mayor aceleracion, mas rapido se puede mover.
        /// </summary>
        protected float MoveAcceleration = 13000.0f;

#if WINDOWS_PHONE || IPHONE
        private const float MAX_MOVE_SPEED = 1250.0f;
#else
        private const float MAX_MOVE_SPEED = 1750.0f;
#endif
        private const float GROUND_DRAG_FACTOR = 0.48f;
        private const float AIR_DRAG_FACTOR = 0.58f;

        // Constants for controlling vertical movement
        //private const float MAX_JUMP_TIME = 0.35f;
        //private const float JUMP_LAUNCH_VELOCITY = -3500.0f;
        private const float GRAVITY_ACCELERATION = 3400.0f;
        private const float MAX_FALL_SPEED = 550.0f;
        //private const float JUMP_CONTROL_POWER = 0.14f; 

        // Input configuration
        private const float MOVE_STICK_SCALE = 1.0f;
        //private const float ACCELEROMETER_SCALE = 1.5f;
        //private const Buttons JUMP_BUTTON = Buttons.A;

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround { get; private set; }

        // Jumping state
        //private bool _isJumping;
        //private bool _wasJumping;
        //private float _jumpTime;

        /// <summary>
        /// Attacking state 
        /// </summary>
        public bool IsAttacking;
        /// <summary>
        /// Maximo Tiempo q esta atacando con el melee. Hasta que no pase este tiempo no puede volver a ejecutar otra accion (moverse, disparar, etc)
        /// A mayor tiempo, mas lento es en atacar. 
        /// </summary>
        protected float MaxAttackTime = 0.33f;
        private float _attackTime;

        protected Rectangle LocalBounds;

        private Rectangle _boundingRectangle;
        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                _boundingRectangle.X = (int) Math.Round(Position.X - LocalBounds.Width/2);
                _boundingRectangle.Y = (int) Math.Round(Position.Y - LocalBounds.Height);
                _boundingRectangle.Width = LocalBounds.Width;
                _boundingRectangle.Height = LocalBounds.Height;

                return _boundingRectangle;
            }
        }

        private Rectangle _knifeBoundingRectangle;
        
        public Rectangle KnifeBoundingRectangle
        {
            get
            {
                // Para que empice del borde posterior
                //_knifeBoundingRectangle.X = Direction == SpriteEffects.FlipHorizontally
                //                                ? BoundingRectangle.X + BoundingRectangle.Width
                //                                : BoundingRectangle.X - KnifeWidth;

                // Para que empiece desde la mitad
                _knifeBoundingRectangle.X = Direction == SpriteEffects.FlipHorizontally
                                                ? BoundingRectangle.X + BoundingRectangle.Width/2
                                                : BoundingRectangle.X - KnifeWidth + BoundingRectangle.Width/2;

                _knifeBoundingRectangle.Y = (BoundingRectangle.Y + BoundingRectangle.Height / 2) - (KnifeHeight / 2);
                _knifeBoundingRectangle.Width = KnifeWidth;
                //_knifeBoundingRectangle.Height = BoundingRectangle.Height;
                _knifeBoundingRectangle.Height = KnifeHeight;

                return _knifeBoundingRectangle;
            }
        }

        protected int KnifeWidth { get; set; }
        protected int KnifeHeight { get; set; }

        public bool HasHitEnemy;
        
        public Weapon CurrentWeapon  { get; private set; }
        public Dictionary<int, int> AmmoInventory { get; private set; }
        private VirtualGamePadState _previusVirtualGamePadState;
        private Rectangle _screenRect;
        private bool _drawWeaponAnimation;

#if DEBUG
        private Texture2D _dummyTexture;
#endif

        public Weapon PrimaryWeapon { get; private set; }
        public Weapon SecondaryWeapon { get; private set; }
        
        private const Keys SWITCH_WEAPON_KEY = Keys.O;
        private const Keys RELOAD_WEAPON_KEY = Keys.L;
        private const Keys SHOOT_KEY = Keys.K;
        private const Keys MELEE_KEY = Keys.J;
        private const Keys LEFT_KEY = Keys.A;
        private const Keys RIGHT_KEY = Keys.D;

        /// <summary>
        /// Constructors a new player.
        /// </summary>
        protected Player(Level level, Vector2 position, List<Weapon> weaponInventory, Weapon primaryWeapon, Weapon secondaryWeapon, Dictionary<int, int> ammoInventory)
        {
			TotalHealth = GlobalParameters.TOTAL_LIVES;

            Direction = SpriteEffects.FlipHorizontally;
            CurrentAction = Actions.Gun;
            
            Level = level;

            WeaponInventory = weaponInventory;

            PrimaryWeapon = primaryWeapon;
            SecondaryWeapon = secondaryWeapon;
            
            CurrentWeapon = PrimaryWeapon;

            AmmoInventory = ammoInventory;

            LoadBullets();

            SetInit(position);

            HealthBar = new HealthBar(Level.Content);
            DrawHealthBar = true;

            //Rectangle the size of the screen so bullets that fly off screen are deleted.
            _screenRect = new Rectangle(0, 0, GlobalParameters.SCREEN_WIDTH, GlobalParameters.SCREEN_HEIGHT);

            var left = (int)Math.Round(Position.X - Sprite.Origin.X) + LocalBounds.X;
            var top = (int)Math.Round(Position.Y - Sprite.Origin.Y) + LocalBounds.Y;

            _boundingRectangle = new Rectangle(left, top, LocalBounds.Width, LocalBounds.Height);

            _knifeBoundingRectangle =
                new Rectangle(left + (int) Math.Round(Position.X - Sprite.Origin.X) + LocalBounds.X,
                              (int) Math.Round(Position.Y - Sprite.Origin.Y) + LocalBounds.Y, LocalBounds.Width,
                              LocalBounds.Height);
        }

        private void LoadBullets()
        {
            _bulletsPool = new Pool<Bullet>(GlobalParameters.MAX_BULLET_REPOSITORY);
            
            //var bulletTexture = Level.Content.Load<Texture2D>(GlobalParameters.BULLET_TEXTURE);

            // If there is any one-time initialization of all resources, it can be done here.
            foreach (var bullet in _bulletsPool.AllNodes)
            {
                //bullet.Item.Init(bulletTexture);
                bullet.Item.Init(GlobalParameters.BULLET_WIDTH, GlobalParameters.BULLET_HEIGHT);
            }
        }

        private void SetInit(Vector2 position)
        {
            Position = position;
            _velocity = Vector2.Zero;
            IsAlive = true;
        }

        public List<Weapon> WeaponInventory { get; private set; }

        public PlayerInfo Type { get; protected set; }

        public int TotalHealth { get; private set; }
        public int CurrentHealth { get; private set; }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position)
        {
            SetInit(position);
            PlayAnimation(IdleAnimation);
        }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        /// <remarks>
        /// We pass in all of the input states so that our game is only polling the hardware
        /// once per frame. We also pass the game's orientation because when using the accelerometer,
        /// we need to reverse our motion when the orientation is in the LandscapeRight orientation.
        /// </remarks>
        public void Update(GameTime gameTime, KeyboardState keyboardState, GamePadState gamePadState, MouseState mouseState, VirtualGamePadState virtualGamePadState)
        {
            var elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (_hitTime > 0.0f)
            {
                _hitTime -= elapsed;
            }

            if (_paralyzedTime > 0.0f)
            {
                _paralyzedTime -= elapsed;
                _velocity = Vector2.Zero;
                //PlayAnimation(BeAttackedAnimation);
            }
            else
            {
                HandleInput(gameTime, keyboardState, gamePadState, mouseState, virtualGamePadState);

                UpdateAttackTime(gameTime);

                //UpdateShootTime(gameTime);

                ApplyPhysics(gameTime);
            }

            if (IsAlive)
                {
                    if (IsAttacking)
                    {
                        PlayAnimation(AttackAnimation);
                        _drawWeaponAnimation = false;
                    }
                    else
                    {
                        // Si la velocidad actual es mayor a 0, implica que se esta moviendo
                        // y que tiene que poner la animacion de correr.
                        // Sino verifica sino lo estan atacando, caso contrario pone Idle
                        if (IsRunning)
                        {
                            PlayAnimation(RunAnimation);
                            _drawWeaponAnimation = true;

                            if (CurrentWeapon.IsShooting)
                            {
                                PlayWeaponAnimation(CurrentWeapon.ShootAnimation);

                                // Centra el brazo (arm) en el personaje. Util para manejar un personaje compuesto x 2 sprites.
                                CurrentWeapon.GameData.Position = Direction == SpriteEffects.FlipHorizontally
                                   ? new Vector2(Position.X + CurrentWeapon.ShootAnimationRunningPosition.X, Position.Y - CurrentWeapon.ShootAnimationRunningPosition.Y)
                                   : new Vector2(Position.X - CurrentWeapon.ShootAnimationRunningPosition.X, Position.Y - CurrentWeapon.ShootAnimationRunningPosition.Y);
                            }
                            else
                            {
                                PlayWeaponAnimation(CurrentWeapon.IdleAnimation);

                                // Centra el brazo (arm) en el personaje. Util para manejar un personaje compuesto x 2 sprites.
                                CurrentWeapon.GameData.Position = Direction == SpriteEffects.FlipHorizontally
                                   ? new Vector2(Position.X + CurrentWeapon.IdleAnimationRunningPosition.X, Position.Y - CurrentWeapon.IdleAnimationRunningPosition.Y)
                                   : new Vector2(Position.X - CurrentWeapon.IdleAnimationRunningPosition.X, Position.Y - CurrentWeapon.IdleAnimationRunningPosition.Y);
                            }
                        }
                        else if (IsBeenHit)
                        {
                            PlayAnimation(BeAttackedAnimation);
                            _drawWeaponAnimation = false;
                        }
                        else
                        {
                            PlayAnimation(IdleAnimation);
                            _drawWeaponAnimation = true;

                            if (CurrentWeapon.IsShooting)
                            {
                                PlayWeaponAnimation(CurrentWeapon.ShootAnimation);

                                // Centra el brazo (arm) en el personaje. Util para manejar un personaje compuesto x 2 sprites.
                                CurrentWeapon.GameData.Position = Direction == SpriteEffects.FlipHorizontally
                                   ? new Vector2(Position.X + CurrentWeapon.ShootAnimationPosition.X, Position.Y - CurrentWeapon.ShootAnimationPosition.Y)
                                   : new Vector2(Position.X - CurrentWeapon.ShootAnimationPosition.X, Position.Y - CurrentWeapon.ShootAnimationPosition.Y);
                            }
                            else
                            {
                                PlayWeaponAnimation(CurrentWeapon.IdleAnimation);

                                // Centra el brazo (arm) en el personaje. Util para manejar un personaje compuesto x 2 sprites.
                                CurrentWeapon.GameData.Position = Direction == SpriteEffects.FlipHorizontally
                                   ? new Vector2(Position.X + CurrentWeapon.IdleAnimationPosition.X, Position.Y - CurrentWeapon.IdleAnimationPosition.Y)
                                   : new Vector2(Position.X - CurrentWeapon.IdleAnimationPosition.X, Position.Y - CurrentWeapon.IdleAnimationPosition.Y);
                            }
                        }
                    }
                }

                //Reset our variables every frame
                _movement = Vector2.Zero;
                _wasClimbing = IsClimbing;
                IsClimbing = false;
            //}

            CurrentWeapon.Update(gameTime);
            UpdateBullets();
        }

        ///// <summary>
        ///// Indica si el player puede disparar, o sea que tiene la intencion (IsShooting) y tiene minuciones (CurrentWeapon.CurrentAmmo > 0)
        ///// </summary>
        //private bool CanShoot
        //{
        //    get { return IsShooting && CurrentWeapon.CurrentAmmo > 0; }
        //}

        public bool IsBeenHit
        {
            get { return _hitTime > 0; }
        }

        public bool IsRunning
        {
            get { return Math.Abs(_velocity.X) - 0.02f > 0; }
        }

        private void UpdateBullets()
        {
            foreach (var bulletNode in _bulletsPool.ActiveNodes)
            {
                var bulletUsed = false;

                //Move our bullet based on it's velocity
                bulletNode.Item.Position += bulletNode.Item.Velocity;

                // Si la bala sale de los limites del nivel, entonces se descarta
                if (!_screenRect.Contains((int)(bulletNode.Item.Position.X - Level.CameraPositionXAxis),
                                          (int)(bulletNode.Item.Position.Y - Level.CameraPositionYAxis)))
                {
                    _bulletsPool.Return(bulletNode);
                    continue;
                }

                // Verificamos cada enemigo con la bala en curso (solo una bala puede lastimar a un enemigo o sea que cuando le da a uno,
                // esa bala ya no puede lastimar a otro enemigo)
                if (Level.FastWeakMonstersPool != null)
                {
                    foreach (var enemyNodes in Level.FastWeakMonstersPool.ActiveNodes)
                    {
                        // si el enemigo no esta vivo, que continue
                        if (!enemyNodes.Item.IsAlive) continue;

                        if (!bulletNode.Item.BoundingRectangle.Intersects(enemyNodes.Item.BoundingRectangle)) continue;

                        OnEnemyHit(enemyNodes.Item, this, bulletNode.Item);
                        _bulletsPool.Return(bulletNode);
                        bulletUsed = true;
                        break; // One bullet can hit one enemy
                    }
                }

                if (bulletUsed) continue;

                // Verificamos cada enemigo con la bala en curso (solo una bala puede lastimar a un enemigo o sea que cuando le da a uno,
                // esa bala ya no puede lastimar a otro enemigo)
                if (Level.NormalMonstersPool != null)
                {
                    foreach (var enemyNodes in Level.NormalMonstersPool.ActiveNodes)
                    {
                        // si el enemigo no esta vivo, que continue
                        if (!enemyNodes.Item.IsAlive) continue;

                        if (!bulletNode.Item.BoundingRectangle.Intersects(enemyNodes.Item.BoundingRectangle)) continue;

                        OnEnemyHit(enemyNodes.Item, this, bulletNode.Item);
                        _bulletsPool.Return(bulletNode);
                        bulletUsed = true;
                        break; // One bullet can hit one enemy
                    }
                }

                if (bulletUsed) continue;

                // Verificamos cada enemigo con la bala en curso (solo una bala puede lastimar a un enemigo o sea que cuando le da a uno,
                // esa bala ya no puede lastimar a otro enemigo)
                if (Level.SlowStrongMonstersPool != null)
                {
                    foreach (var enemyNodes in Level.SlowStrongMonstersPool.ActiveNodes)
                    {
                        // si el enemigo no esta vivo, que continue
                        if (!enemyNodes.Item.IsAlive) continue;

                        if (!bulletNode.Item.BoundingRectangle.Intersects(enemyNodes.Item.BoundingRectangle)) continue;

                        OnEnemyHit(enemyNodes.Item, this, bulletNode.Item);
                        _bulletsPool.Return(bulletNode);
                        //bulletUsed = true;
                        break; // One bullet can hit one enemy
                    }
                }
            }
        }

        /// <summary>
        /// Called when the player has been hit.
        /// </summary>
        /// <param name="attackingEnemy">
        /// The enemy who hit the player. This parameter is null if the player was
        /// not hit by an enemy.
        /// </param>
        public void OnHit(Enemy attackingEnemy)
        {
            // NOTE: estas reglas irian aca o en otro lado? es decir, validar que un enemigo lo puede morder

            // Si el enemigo que esta atacando ya tiene "AttackTime" entonces no lo puede volver a atacar hasta que se termine ese tiempo
            if (attackingEnemy.AttackTime > 0.0f) return;

            CurrentHealth--;

            if (CurrentHealth < 0)
            {
                OnKilled();//attackingEnemy);
            }
            else
            {
                // Le seteamos el AttackTime al enemigo segun el MaxHitTime del Player en cuestion
                //attackingEnemy.SetMaxAttackTime(MaxHitTime);
                attackingEnemy.AttackTime = MaxHitTime;
                _hitTime = MaxHitTime; // seria lo mismo hacer _hitTime = MaxHitTime;

                // Si no esta paralizado todavia, entonces lo paralizamos un toque. Esto sirve para que no pase caminando delante de los enemigos sin ningun efecto
                if (_paralyzedTime <= 0.0f)
                {
                    _paralyzedTime = MaxParalyzedTime;
                }
            }
        }

        private void OnEnemyHit(Enemy enemy, Player hitBy, Bullet bullet)
        {
            enemy.OnHit(hitBy, bullet);
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void HandleInput(GameTime gameTime, KeyboardState currentKeyboardState, GamePadState gamePadState, MouseState mouseState, VirtualGamePadState virtualGamePadState)
        {
            //Arm rotation (Xbox)
            //_arm.rotation = (float)Math.Atan2(gamePadState.ThumbSticks.Right.X, gamePadState.ThumbSticks.Right.Y);
            //float xDistance;
            //float yDistance;

            //MouseStateCurrent = mouseState;

            //Vector2 mouseWorldPosition = Vector2.Transform(new Vector2(mouseState.X, mouseState.Y),
            //                        Matrix.Invert(Matrix.CreateTranslation(-_level.CameraPositionXAxis, -_level.CameraPositionYAxis, 0.0f)));

            //if (_flip == SpriteEffects.FlipHorizontally) //Facing right
            //{
            //    //Calculate the distance from the player to the mouse's X and Y position
            //    xDistance = mouseWorldPosition.X - _arm.Position.X;
            //    yDistance = mouseWorldPosition.Y - _arm.Position.Y;

            //    _arm.Rotation = (float)Math.Atan2(yDistance, xDistance);

            //    //If we try to aim behind our head then flip the
            //    //character around so he doesn't break his arm!
            //    if (_arm.Rotation > MathHelper.PiOver2 || _arm.Rotation < -MathHelper.PiOver2)
            //        _flip = SpriteEffects.None;
            //}
            //else //Facing left
            //{
            //    //Calculate the distance from the player to the mouse's X and Y position
            //    xDistance = _arm.Position.X - mouseWorldPosition.X;
            //    yDistance = _arm.Position.Y - mouseWorldPosition.Y;

            //    _arm.Rotation = (float)Math.Atan2(yDistance, xDistance);

            //    //Once again, if we try to aim behind us then
            //    //flip our character.
            //    if (_arm.Rotation > MathHelper.PiOver2 || _arm.Rotation < -MathHelper.PiOver2)
            //        _flip = SpriteEffects.FlipHorizontally;
            //}

            //Shoot = RightTrigger or SHOOT_KEY
            //if (previousGamePadState.Triggers.Right < 0.5 && gamePadState.Triggers.Right > 0.5)
            //if ((_mouseStatePrevious.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            //    || (_previousKeyboardState.IsKeyUp(SHOOT_KEY) && currentKeyboardState.IsKeyDown(SHOOT_KEY)))
            DoAction(gameTime, currentKeyboardState, virtualGamePadState);

            //_mouseStatePrevious = mouseState;
            
            // Move the player with accelerometer
            //if (Math.Abs(accelState.Acceleration.Y) > 0.10f)
            //{
            //    throw new NotImplementedException("Move the player with accelerometer: comentado x HZ, me parece que esto deberia ser algo asi, al igual que se hace con el gamepad stick");
            //    // set our movement speed
            //    // _movement = MathHelper.Clamp(-accelState.Acceleration.Y * ACCELEROMETER_SCALE, -1f, 1f);
            //    // comentado x HZ, me parece que esto deberia ser algo asi, al igual que se hace con el gamepad stick
            //    // testear con el wp emulator y el acelerometro
            //    // _movement.X = gamePadState.ThumbSticks.Left.X * MOVE_STICK_SCALE;
            //    // _movement.Y = gamePadState.ThumbSticks.Left.Y * MOVE_STICK_SCALE;
            //    // if we're in the LandscapeLeft orientation, we must reverse our movement
            //    if (orientation == DisplayOrientation.LandscapeRight)
            //        _movement = -_movement;
            //}

            //bool isScreenClicked = IsScreenClicked(touchState, mouseState);

            //if (isScreenClicked)
            //{
            //    _clickedRectangule = GetClickedRectangule(GetClickedWorldPosition(touchState, mouseState));
            //}

            // Check if the player wants to move horizantally.
            SetHorizontalMovement(currentKeyboardState, gamePadState, virtualGamePadState);

            // Check if the player wants to move vertically.
            //SetVerticalMovement(currentKeyboardState, gamePadState);

            // Check if the player wants to jump.
            //SetJumpingState(currentKeyboardState, gamePadState, touchState);

            // Set current action
            //SetCurrentAction(currentKeyboardState);

            SetCurrentWeapon(currentKeyboardState, virtualGamePadState);

            ReloadCurrentWeapon(currentKeyboardState, virtualGamePadState);

            _previusVirtualGamePadState = virtualGamePadState;
            _previousKeyboardState = currentKeyboardState;
        }

        private void ReloadCurrentWeapon(KeyboardState currentKeyboardState, VirtualGamePadState virtualGamePadState)
        {
            if ((_previusVirtualGamePadState.Buttons.LeftShoulder == VirtualButtonState.Released && virtualGamePadState.Buttons.LeftShoulder == VirtualButtonState.Pressed)
                || (currentKeyboardState.IsKeyUp(RELOAD_WEAPON_KEY) && _previousKeyboardState.IsKeyDown(RELOAD_WEAPON_KEY)))
            {
                // Si el arma esta siendo cargada, que retorne sin hacer nada
                if (CurrentWeapon.IsReloading) return;

                // Si la cantidad de municiones es ("mayor o" - caso imposible) igual que la cantidad maxima de municiones que carga el arma,
                // que retorne sin hacer nada
                if (CurrentWeapon.CurrentAmmo >= CurrentWeapon.MaxAmmo) return;

                var availableAmmo = GetCurrentWeaponAvailableAmmo();

                // Si no tenemos municiones disponibles, que retorne sin hacer nada
                if (availableAmmo <= 0) return;

                // Cargamos el arma con las que tenemos disponibles
                SetCurrentWeaponAvailableAmmo(CurrentWeapon.Reload(availableAmmo));
            }
        }

        /// <summary>
        /// Setea el arma correspondiente segun la accion elegida
        /// </summary>
        /// <param name="currentKeyboardState"></param>
        /// <param name="virtualGamePadState"></param>
        private void SetCurrentWeapon(KeyboardState currentKeyboardState, VirtualGamePadState virtualGamePadState)
        {
            if ((_previusVirtualGamePadState.Buttons.RightShoulder == VirtualButtonState.Released && virtualGamePadState.Buttons.RightShoulder == VirtualButtonState.Pressed)
                || (currentKeyboardState.IsKeyUp(SWITCH_WEAPON_KEY) && _previousKeyboardState.IsKeyDown(SWITCH_WEAPON_KEY)))
            {
                if (SecondaryWeapon != null)
                {
                    CurrentWeapon = PrimaryWeapon.Type == CurrentWeapon.Type ? SecondaryWeapon : PrimaryWeapon;
                }
            }
        }

        //private void ResetMovement()
        //{
        //    _movePlayerLeft = false;
        //    _movePlayerRight = false;
        //    _movePlayerUp = false;
        //    _movePlayerDown = false;
        //}

        public void DoAction(GameTime gameTime)
        {
            switch (CurrentAction)
            {
                case Actions.UseKnife:
                    UseKnike();
                    break;
                case Actions.Gun:
                    FireWeapon(gameTime);
                    break;
                //case Actions.CatchSurvivor:
                //    if (CatchedCharacter == null)
                //    {
                //        IsCatching = true;
                //    }
                //    break;
                //case Actions.ReleaseSurvivor:
                //    if (CatchedCharacter != null)
                //    {
                //        CatchedCharacter.OnReleased();
                //        CatchedCharacter = null;
                //    }
                //    break;
                //case Actions.ThrowSurvivor:
                //    if (CatchedCharacter != null)
                //    {
                //        CatchedCharacter.CurrentSate = CharacterStates.Thrown;
                //        CatchedCharacter = null;
                //    }
                //    break;
                //case Actions.StopSurvivor:
                //    {
                //        if (CatchedCharacter != null)
                //        {
                //            CatchedCharacter.OnStopped(this);
                //            CatchedCharacter = null;
                //        }
                //        else
                //        {
                //            IsStoping = true;
                //        }
                //    }
                //    break;
            }
        }

        private void UseKnike()
        {
            IsAttacking = true;
            HasHitEnemy = false;
            _attackTime = MaxAttackTime;
        }

        private void FireWeapon(GameTime gameTime)
        {
            // Si el arma esta recargandose, que retorne
            if (CurrentWeapon.IsReloading) return;

            // Si todavia no puede disparar (no se cumplio el FireRate)
            if (!CurrentWeapon.CanShoot) return;
            
            CurrentWeapon.Fire(gameTime, _bulletsPool, Direction);
        }

        /// <summary>
        /// Ejecuta una accion segun el boton o tecla presionada
        /// Execute an accion according button or key pressed
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="currentKeyboardState"></param>
        /// <param name="currentVirtualGamePadState"></param>
        private void DoAction(GameTime gameTime, KeyboardState currentKeyboardState, VirtualGamePadState currentVirtualGamePadState)
        {
            // TODO: agregar el boton del GamePad que se usa para el Melee y el arma

            // Si el player esta atacando con su melee entonces no puede volver a atacar ni disparar (no puede ejecutar otra accion)
            if (_attackTime > 0.0f) return;

            if ((currentVirtualGamePadState.Buttons.A == VirtualButtonState.Pressed && _previusVirtualGamePadState.Buttons.A == VirtualButtonState.Released)
                || (_previousKeyboardState.IsKeyDown(MELEE_KEY) && currentKeyboardState.IsKeyUp(MELEE_KEY)))
            {
                //_nextActionTime = MAX_NEXT_ACTION_TIME;
                CurrentAction = Actions.UseKnife;
                DoAction(gameTime);
                return;
            }

            if ((currentVirtualGamePadState.Buttons.B == VirtualButtonState.Pressed &&
                 (_previusVirtualGamePadState.Buttons.B == VirtualButtonState.Released ||
                  (CurrentWeapon.IsAutomatic && CurrentWeapon.HasAmmo)))
                ||
                (_previousKeyboardState.IsKeyDown(SHOOT_KEY) &&
                 (currentKeyboardState.IsKeyUp(SHOOT_KEY) || (CurrentWeapon.IsAutomatic && CurrentWeapon.HasAmmo))))
            {
                CurrentAction = Actions.Gun;
                DoAction(gameTime);
                return;
            }
        }

        ///// <summary>
        ///// Check if the player wants to move vertically
        ///// </summary>
        ///// <param name="keyboardState"></param>
        ///// <param name="gamePadState"></param>
        //private void SetVerticalMovement(KeyboardState keyboardState, GamePadState gamePadState)
        //{
        //    // Set if player wants to move right or left
        //    //MovePlayerUpOrDown(isScreenClicked);
            
        //    // Get analog horizontal movement.
        //    _movement.Y = gamePadState.ThumbSticks.Left.Y * MOVE_STICK_SCALE;

        //    // Ignore small movements to prevent running in place.
        //    if (Math.Abs(_movement.Y) < 0.5f)
        //        _movement.Y = 0.0f;

        //    //LADDER
        //    if (gamePadState.IsButtonDown(Buttons.DPadUp) ||
        //        keyboardState.IsKeyDown(UP_KEY) ||
        //        _movePlayerUp)
        //    {
        //        IsClimbing = false;

        //        if (IsAlignedToLadder())
        //        {
        //            //We need to check the tile behind the player,
        //            //not what he is standing on
        //            if (Level.GetTileCollisionBehindPlayer(_position) == TileCollision.Ladder)
        //            {
        //                IsClimbing = true;
        //                _isJumping = false;
        //                IsOnGround = false;
        //                _movement.Y = LADDER_UP;
        //            }
        //        }
        //    }
        //    else if (gamePadState.IsButtonDown(Buttons.DPadDown) ||
        //             keyboardState.IsKeyDown(DOWN_KEY) ||
        //            _movePlayerDown)
        //    {
        //        IsClimbing = false;

        //        if (IsAlignedToLadder())
        //        {
        //            // Check the tile the player is standing on
        //            if (Level.GetTileCollisionBelowPlayer(_position) == TileCollision.Ladder)
        //            {
        //                IsClimbing = true;
        //                _isJumping = false;
        //                IsOnGround = false;
        //                _movement.Y = LADDER_DOWN;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Check if the player wants to move horizantally
        /// </summary>
        /// <param name="keyboardState"></param>
        /// <param name="gamePadState"></param>
        /// <param name="virtualGamePadState"></param>
        private void SetHorizontalMovement(KeyboardState keyboardState, GamePadState gamePadState, VirtualGamePadState virtualGamePadState)
        {
            // Si el tiempo para la proxima accion no paso, entonces que no pueda caminar ni hacer nada
            if (_attackTime > 0) return;

            // Get analog horizontal movement.
            _movement.X = gamePadState.ThumbSticks.Left.X * MOVE_STICK_SCALE;

            // Ignore small movements to prevent running in place.
            if (Math.Abs(_movement.X) < 0.5f)
                _movement.X = 0.0f;
            
            // If any digital horizontal movement input is found, override the analog movement.
            if (gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                keyboardState.IsKeyDown(LEFT_KEY) ||
                //_movePlayerLeft ||
                virtualGamePadState.Buttons.DPadLeft == VirtualButtonState.Pressed)
            {
                _movement.X = -1.0f;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                     keyboardState.IsKeyDown(RIGHT_KEY) ||
                   // _movePlayerRight ||
                    virtualGamePadState.Buttons.DPadRight == VirtualButtonState.Pressed)
            {
                _movement.X = 1.0f;
            }
        }

        /// <summary>
        /// Set if player wants to move right or left
        /// </summary>
//        private void MovePlayerLeftOrRight(bool isScreenClicked)
//        {
//            if (isScreenClicked)
//            {
//                // Si el player se encuentra dentro del espacio del click que no se mueva
//                if (!BoundingRectangle.Intersects(_clickedRectangule))
//                {
//                    if (BoundingRectangle.Right < _clickedRectangule.Left)
//                        // Si estamos haciendo click a la derecha del player
//                    {
//                        _movePlayerRight = true;
//                        _movePlayerLeft = false;
//                    }
//                    else if (_clickedRectangule.Right < BoundingRectangle.Left)
//                        // Si estamos haciendo click a la izquierda del player
//                    {
//                        _movePlayerLeft = true;
//                        _movePlayerRight = false;
//                    }
//                }
//                //else
//                //{
//                //    _movePlayerRight = false;
//                //    _movePlayerLeft = false;
//                //}
//            }
//            // Si el player se encuentra dentro del espacio del click
//            //else if (BoundingRectangle.Intersects(_clickedRectangule))
//            //{
//            //    _movePlayerRight = false;
//            //    _movePlayerLeft = false;
//            //}
//            // Si el player se esta moviendo para la derecha y su lateral derecho alcanza o supera al lateral derecho del CLICK
//            // entonces que detenga su moviemiento hacia la derecha
//            else if (_movePlayerRight && BoundingRectangle.Right >= _clickedRectangule.Right)
//            {
//                _movePlayerRight = false;
//            }
//            // Si el player se esta moviendo para la izquierda y su lateral izquierdo alcanza o supera al lateral izquierdo del CLICK
//            // entonces que detenga su moviemiento hacia la izquierda
//            else if (_movePlayerLeft && BoundingRectangle.Left <= _clickedRectangule.Left)
//            {
//                _movePlayerLeft = false;
//            }
//        }

//        private bool IsScreenClicked(TouchCollection touchState, MouseState mouseState)
//        {
//            bool isClicked;

//#if WINDOWS_PHONE            
//            isClicked = touchState.AnyTouch() && touchState[0].State == TouchLocationState.Pressed;
//#else
//            isClicked = mouseState.LeftButton == ButtonState.Pressed;
//#endif
//            return isClicked;
//        }

        //private float GetRightClickedMargin()
        //{
        //    return _clickedPosicionX + CLICK_MOVE_MARGIN;
        //}

        //private float GetLeftClickedMargin()
        //{
        //    return _clickedPosicionX - CLICK_MOVE_MARGIN;
        //}

        //private float GetTopClickedMargin()
        //{
        //    return _clickedPosicionY - CLICK_MOVE_MARGIN;
        //}

        //private float GetBottomClickedMargin()
        //{
        //    return _clickedPosicionY + CLICK_MOVE_MARGIN;
        //}

        ///// <summary>
        ///// Check if the player wants to jump according to input
        ///// </summary>
        ///// <param name="keyboardState"></param>
        ///// <param name="gamePadState"></param>
        ///// <param name="touchState"></param>
        //private void SetJumpingState(KeyboardState keyboardState, GamePadState gamePadState, TouchCollection touchState)
        //{
        //    _isJumping =
        //        gamePadState.IsButtonDown(JUMP_BUTTON) ||
        //        keyboardState.IsKeyDown(UP_KEY);
        //    //|| touchState.AnyTouch();
        //}

        ///// <summary>
        ///// Set player's current action according to input
        ///// </summary>
        ///// <param name="currentKeyboardState">Current Keyboard State</param>
        //private void SetCurrentAction(KeyboardState currentKeyboardState)
        //{
        //    if (_previousKeyboardState.IsKeyUp(Keys.D1) && currentKeyboardState.IsKeyDown(Keys.D1))
        //    {
        //        CurrentAction = Actions.UseKnife;
        //    }
        //    else if (_previousKeyboardState.IsKeyUp(Keys.D2) && currentKeyboardState.IsKeyDown(Keys.D2))
        //    {
        //        CurrentAction = Actions.Gun;
        //    }
        //    else if (_previousKeyboardState.IsKeyUp(Keys.D3) && currentKeyboardState.IsKeyDown(Keys.D3))
        //    {
        //        CurrentAction = Actions.CatchSurvivor;
        //    }
        //    else if (_previousKeyboardState.IsKeyUp(Keys.D4) && currentKeyboardState.IsKeyDown(Keys.D4))
        //    {
        //        CurrentAction = Actions.ReleaseSurvivor;
        //    }
        //    else if (_previousKeyboardState.IsKeyUp(Keys.D5) && currentKeyboardState.IsKeyDown(Keys.D5))
        //    {
        //        CurrentAction = Actions.ThrowSurvivor;
        //    }
        //    else if (_previousKeyboardState.IsKeyUp(Keys.D6) && currentKeyboardState.IsKeyDown(Keys.D6))
        //    {
        //        CurrentAction = Actions.StopSurvivor;
        //    }
        //}

        ////LADDER
        //private bool IsAlignedToLadder()
        //{
        //    var playerOffset = ((int)_position.X % Tile.WIDTH) - Tile.CENTER;

        //    if (Math.Abs(playerOffset) <= LADDER_ALIGNMENT &&
        //        Level.GetTileCollisionBelowPlayer(new Vector2(
        //            Level.Player._position.X,
        //            Level.Player._position.Y + 1)) == TileCollision.Ladder ||
        //        Level.GetTileCollisionBelowPlayer(new Vector2(
        //            Level.Player._position.X,
        //            Level.Player._position.Y - 1)) == TileCollision.Ladder)
        //    {
        //        // Align the player with the middle of the tile
        //        _position.X -= playerOffset;
        //        return true;
        //    }
            
        //    return false;
        //}

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            var elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;
            var previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            //LADDER
            if (!IsClimbing)
            {
                if (_wasClimbing)
                    _velocity.Y = 0;
                else
                    _velocity.Y = MathHelper.Clamp(
                        _velocity.Y + GRAVITY_ACCELERATION*elapsed,
                        -MAX_FALL_SPEED,
                        MAX_FALL_SPEED);
            }
            else
            {
                _velocity.Y = _movement.Y*MoveAcceleration*elapsed;
            }

            _velocity.X += _movement.X*MoveAcceleration*elapsed;

            //_velocity.Y = DoJump(_velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (IsOnGround)
                _velocity.X *= GROUND_DRAG_FACTOR;
            else
                _velocity.X *= AIR_DRAG_FACTOR;


            // Prevent the player from running faster than his top speed.            
            _velocity.X = MathHelper.Clamp(_velocity.X, -MAX_MOVE_SPEED, MAX_MOVE_SPEED);

            // Apply velocity.
            Position += _velocity * elapsed;
            Position = new Vector2((float) Math.Round(Position.X), (float) Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                _velocity.X = 0;

            if (Position.Y == previousPosition.Y)
                _velocity.Y = 0;
        }


        ///// <summary>
        ///// Calculates the Y velocity accounting for jumping and
        ///// animates accordingly.
        ///// </summary>
        ///// <remarks>
        ///// During the accent of a jump, the Y velocity is completely
        ///// overridden by a power curve. During the decent, gravity takes
        ///// over. The jump velocity is controlled by the jumpTime field
        ///// which measures time into the accent of the current jump.
        ///// </remarks>
        ///// <param name="velocityY">
        ///// The player's current velocity along the Y axis.
        ///// </param>
        ///// <param name="gameTime">
        ///// El valor de gameTime del juego en cuestion
        ///// </param>
        ///// <returns>
        ///// A new Y velocity if beginning or continuing a jump.
        ///// Otherwise, the existing Y velocity.
        ///// </returns>
        //private float DoJump(float velocityY, GameTime gameTime)
        //{
        //    // If the player wants to jump
        //    if (_isJumping)
        //    {
        //        // Begin or continue a jump
        //        if ((!_wasJumping && IsOnGround) || _jumpTime > 0.0f)
        //        {
        //            if (_jumpTime == 0.0f)
        //                JumpSound.Play();

        //            _jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        //            PlayAnimation(JumpAnimation);
        //        }

        //        // If we are in the ascent of the jump
        //        if (0.0f < _jumpTime && _jumpTime <= MAX_JUMP_TIME)
        //        {
        //            // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
        //            velocityY = JUMP_LAUNCH_VELOCITY * (1.0f - (float)Math.Pow(_jumpTime / MAX_JUMP_TIME, JUMP_CONTROL_POWER));
        //        }
        //        else
        //        {
        //            // Reached the apex of the jump
        //            _jumpTime = 0.0f;
        //        }
        //    }
        //    else
        //    {
        //        // Continues not jumping or cancels a jump in progress
        //        _jumpTime = 0.0f;
        //    }
        //    _wasJumping = _isJumping;

        //    return velocityY;
        //}

        private void UpdateAttackTime(GameTime gameTime)
        {
            // If the player wants to attack
            if (IsAttacking)
            {
                // Begin or continue an attack
                if (_attackTime > 0.0f)
                {
                    _attackTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    IsAttacking = false;
                    HasHitEnemy = false;
                }
            }
            else
            {
                //Continues not attack or cancels an attack in progress
                _attackTime = 0.0f;
            }
        }

        //private void UpdateShootTime(GameTime gameTime)
        //{
        //    // If the player wants to attack
        //    if (IsShooting)
        //    {
        //        // Begin or continue an attack
        //        if (_shootTime > 0.0f)
        //        {
        //            _shootTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        //        }
        //        else
        //        {
        //            IsShooting = false;
        //        }
        //    }
        //    else
        //    {
        //        //Continues not attack or cancels an attack in progress
        //        _shootTime = 0.0f;
        //    }
        //}

        /// <summary>
        /// Detects and resolves all collisions between the player and his neighboring
        /// tiles. When a collision is detected, the player is pushed away along one
        /// axis to prevent overlapping. There is some special logic for the Y axis to
        /// handle platforms which behave differently depending on direction of movement.
        /// </summary>
        private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            var bounds = BoundingRectangle;
            var leftTile = (int)Math.Floor((float)bounds.Left / Tile.WIDTH);
            var rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.WIDTH)) - 1;
            var topTile = (int)Math.Floor((float)bounds.Top / Tile.HEIGHT);
            var bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.HEIGHT)) - 1;

            // Reset flag to search for ground collision.
            IsOnGround = false;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);

                    //if (collision == TileCollision.Checkpoint)
                    //{
                    //    _level.Checkpoint = (new Vector2(x, y) * Tile.Size) + (Tile.Size / 2);
                    //}
                    //else
                    if (collision == TileCollision.Passable) continue;

                    // Determine collision depth (with direction) and magnitude.
                    var tileBounds = Level.GetBounds(x, y);
                    var depth = bounds.GetIntersectionDepth(tileBounds);
                    if (depth != Vector2.Zero)
                    {
                        float absDepthX = Math.Abs(depth.X);
                        float absDepthY = Math.Abs(depth.Y);

                        // Resolve the collision along the shallow axis.
                        if (absDepthY < absDepthX || collision == TileCollision.Platform)
                        {
                            // If we crossed the top of a tile, we are on the ground.
                            if (_previousBottom <= tileBounds.Top)
                                IsOnGround = true;

                            // Ignore platforms, unless we are on the ground.
                            if (collision == TileCollision.Impassable || IsOnGround)
                            {
                                // Resolve the collision along the Y axis.
                                Position = new Vector2(Position.X, Position.Y + depth.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                        else if (collision == TileCollision.Impassable) // Ignore platforms.
                        {
                            // Resolve the collision along the X axis.
                            Position = new Vector2(Position.X + depth.X, Position.Y);

                            // Perform further collisions with the new bounds.
                            bounds = BoundingRectangle;
                        }

                        // Resolve the collision along the shallow axis.
                        //if (absDepthY < absDepthX || collision == TileCollision.Platform)
                        //{
                        //    // If we crossed the top of a tile, we are on the ground.
                        //    if (_previousBottom <= tileBounds.Top)
                        //    {
                        //        if (collision == TileCollision.Ladder)
                        //        {
                        //            if (!IsClimbing && !_isJumping)
                        //            {
                        //                // When walking over a ladder
                        //                IsOnGround = true;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            IsOnGround = true;
                        //            IsClimbing = false;
                        //            _isJumping = false;
                        //        }
                        //    }

                        //    // Ignore platforms, unless we are on the ground.
                        //    if (collision == TileCollision.Impassable || IsOnGround)
                        //    {
                        //        // Resolve the collision along the Y axis.
                        //        _position = new Vector2(_position.X, _position.Y + depth.Y);

                        //        // Perform further collisions with the new bounds.
                        //        bounds = BoundingRectangle;
                        //    }
                        //}
                        //else if (collision == TileCollision.Impassable) // Ignore platforms.
                        //{
                        //    // Resolve the collision along the X axis.
                        //    _position = new Vector2(_position.X + depth.X, _position.Y);

                        //    // Perform further collisions with the new bounds.
                        //    bounds = BoundingRectangle;
                        //}
                        //LADDER
                        //else if (collision == TileCollision.Ladder && !IsClimbing)
                        //{
                        //    // When walking in front of a ladder, falling off a ladder
                        //    // but not climbing

                        //    // Resolve the collision along the Y axis.
                        //    _position = new Vector2(_position.X, _position.Y);

                        //    // Perform further collisions with the new bounds.
                        //    bounds = BoundingRectangle;
                        //}
                    }
                }
            }

            //if (!IsOnGround)
            //{
            //    for (int i = 0; i < Level.Survivors.Count; ++i)
            //    {
            //        Survivor survivor = Level.Survivors[i];

            //        if (survivor.CurrentSate == CharacterStates.Stopped)
            //        {
            //            Vector2 survivorDepth = RectangleExtensions.GetIntersectionDepth(bounds,
            //                                                                             survivor.BoundingRectangle);

            //            if ((survivorDepth.Y < 0 && survivorDepth.Y >= -10) && survivorDepth != Vector2.Zero)
            //            {
            //                // Resolve the collision along the Y axis.
            //                _position = new Vector2(_position.X, survivor.Position.Y - survivor.BoundingRectangle.Height);

            //                // Perform further collisions with the new bounds.
            //                bounds = BoundingRectangle;

            //                IsOnGround = true;
            //                IsClimbing = false;
            //                _isJumping = false;
            //            }
            //        }
            //    }
            //}

            // Save the new bounds bottom.
            _previousBottom = bounds.Bottom;
        }

        /// <summary>
        /// Called when the player has been killed.
        /// </summary>
        ///// <param name="killedBy">
        ///// The enemy who killed the player. This parameter is null if the player was
        ///// not killed by an enemy (fell into a hole).
        ///// </param>
        public void OnKilled()//Enemy killedBy)
        {
            //ResetMovement();
            
            IsAlive = false;

            //if (killedBy != null)
                KilledSound.Play();
            //else
            //    FallSound.Play();

            PlayAnimation(DieAnimation);
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void OnReachedExit()
        {
            PlayAnimation(CelebrateAnimation);
        }

        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Comentario de HZ: si comento estas lineas, se produce el efecto de Days2Die, 
            // o sea que el personaje mire solo para donde esta la mira, su punto de vision.
            // Si su punto de vision esta atras, entonces camina para atras (su punto de vision siempre esta adelante)

            // Flip the sprite to face the way we are moving.
            if (_velocity.X > 0)
                Direction = SpriteEffects.FlipHorizontally;
            else if (_velocity.X < 0)
                Direction = SpriteEffects.None;

            // Draw that sprite.
            Sprite.Draw(gameTime, spriteBatch, Position, Direction);
            
            // Si no esta atacando que dibuje el arma que corresponde
            //if ((!IsAttacking) && (_hitTime <= 0))
            if (_drawWeaponAnimation)
            {
                WeaponSprite.Draw(gameTime, spriteBatch, CurrentWeapon.GameData.Position, Direction);
            }

            // TODO: Draw the bullets?
            //DrawBullets(spriteBatch);

#if DEBUG && !IPHONE
            // Draw BoundingRectangle (for DEBUG purpouse)
            if (_dummyTexture == null)
            {
                _dummyTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _dummyTexture.SetData(new Color[] { Color.White });
            }

            spriteBatch.Draw(_dummyTexture, BoundingRectangle, Color.Red);
            spriteBatch.Draw(_dummyTexture, KnifeBoundingRectangle, Color.Green);
#endif
            
            if (CurrentHealth >= 0 && DrawHealthBar)
            {
                //_healthBar.Draw(spriteBatch, Position, _sprite.Animation.FrameHeight, CurrentHealth, TotalHealth);
                HealthBar.Draw(spriteBatch, Position, BoundingRectangle.Height + HealthBarMargin, CurrentHealth,
                               TotalHealth);
            }
        }

        protected int HealthBarMargin { get; set; }

        //private void DrawBullets(SpriteBatch spriteBatch)
        //{
        //    // Draw each active missile
        //    foreach (var missile in _bulletsPool)
        //    {
        //        missile.Draw(spriteBatch);
        //    }
        //}

        /// <summary>
        /// Set animation to play and set current localbound based on animation
        /// </summary>
        /// <param name="animation"></param>
        protected void PlayAnimation(Animation animation)
        {
            if (animation == null) throw new Exception("La animacion del player no puede ser null");

            Sprite.PlayAnimation(animation);
            LocalBounds = animation.BoundingRectangle;
        }

        /// <summary>
        /// Set weapon animation to play
        /// </summary>
        /// <param name="animation"></param>
        protected void PlayWeaponAnimation(Animation animation)
        {
            if (animation == null) throw new Exception("La animacion del arma no puede ser null");

            WeaponSprite.PlayAnimation(animation);
        }

        public void GainLive()
        {
            SetLives(TotalHealth);
        }

        public void SetLives(int currentLives)
        {
            CurrentHealth = currentLives;

            if (CurrentHealth > TotalHealth) TotalHealth = CurrentHealth;
        }

        //public void TakeLive()
        //{
        //    CurrentHealth--;
        //}

        public int GetCurrentWeaponAvailableAmmo()
        {
            return AmmoInventory == null || !AmmoInventory.ContainsKey((int)CurrentWeapon.Type) ? 0 : AmmoInventory[(int)CurrentWeapon.Type];
        }

        /// <summary>
        /// Setea las municiones para el arma correspondiente
        /// </summary>
        /// <param name="availableAmmo"></param>
        private void SetCurrentWeaponAvailableAmmo(int availableAmmo)
        {
            var weaponIndex = (int) CurrentWeapon.Type;

            if (AmmoInventory == null)
                AmmoInventory = new Dictionary<int, int>();

            if (AmmoInventory.ContainsKey(weaponIndex))
            {
                AmmoInventory[weaponIndex] = availableAmmo;
            }
            else
            {
                AmmoInventory.Add(weaponIndex, availableAmmo);
            }
        }
    }
}

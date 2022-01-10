using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
//#if WINDOWS_PHONE || IPHONE
//using Mobile.Base.Components;
//#elif SILVERLIGHT
//using Web.Base.Components;
//#elif WINDOWS
//using Desktop.Base.Components;
//#endif
//using Platformer.Helpers;
using Platformer.Animations;
using Platformer.Helpers;
using Platformer.Levels;
using Platformer.Players;
using Platformer.Weapons;

namespace Platformer.Npc
{
    /// <summary>
    /// An evil monster hell-bent on impeding the progress of our fearless adventurer.
    /// </summary>
    public abstract class Npc
    {
        //protected AnimatedSprite AnimationSet;
        //protected Texture2D SpriteTexture;
        //protected AnimatedSpriteInstance SpriteInstance;

        public CharacterStates CurrentSate;

        protected bool FollowPlayer;
        
        protected int TotalHealth;
        protected int CurrentHealth;

        public const int POINT_VALUE = 50;

        // Hitted state
        //public bool WasHitted;
        
        /// <summary>
        /// Máximo tiempo que durará el impacto del player y hará que el enemigo no pueda hacer otra cosa (x ej: atacar)
        /// Medido en segundos.
        /// </summary>
        const float MAX_HITTED_TIME = 0.33f;

        /// <summary>
        /// Tiempo que durará el impacto del player y hará que el enemigo no pueda hacer otra cosa (x ej: atacar)
        /// Medido en segundos.
        /// </summary>
        public float HittedTime;

        protected Level Level { get; private set; }

        /// <summary>
        /// Position in world space of the bottom center of this Npc.
        /// </summary>
        public Vector2 Position { get; protected set; }

        protected Rectangle LocalBounds;

        private Rectangle _boundingRectangle;
        private Rectangle _spotlightRectangle;

        /// <summary>
        /// Ancho del campo de vision del NPC
        /// </summary>
        protected int SpotlightWidth;

        /// <summary>
        /// Alto del campo de vsion del NPC
        /// </summary>
        protected int SpotlightHeight;

        protected Npc()
        {
            SpotlightHeight = GlobalParameters.SPOTLIGHT_HEIGHT;
            SpotlightWidth = GlobalParameters.SPOTLIGHT_WIDTH;
        }

        /// <summary>
        /// Gets a rectangle which bounds this Npc in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                //int left = (int)Math.Round(Position.X - Sprite.Origin.X) + LocalBounds.X;
                //int top = (int)Math.Round(Position.Y - Sprite.Origin.Y) + LocalBounds.Y;

                //return new Rectangle(left, top, LocalBounds.Width, LocalBounds.Height);

                //_boundingRectangle.X = (int) Math.Round(Position.X - Sprite.Origin.X) + LocalBounds.X;
                //_boundingRectangle.Y = (int)Math.Round(Position.Y - Sprite.Origin.Y) + LocalBounds.Y;
                _boundingRectangle.X = (int) Math.Round(Position.X - LocalBounds.Width / 2);
                _boundingRectangle.Y = (int) Math.Round(Position.Y - LocalBounds.Height);
                _boundingRectangle.Width = LocalBounds.Width;
                _boundingRectangle.Height = LocalBounds.Height;

                return _boundingRectangle;
            }
        }

        /// <summary>
        /// Campo de audicion / vision
        /// </summary>
        public Rectangle SpotlightRectangle
        {
            get
            {
                // Situamos el campo de vision en el medio de personaje, seria como un campo de audicion / vision
                //_spotlightRectangle.X = ((int)Math.Round(Position.X - Sprite.Origin.X) + LocalBounds.X) - (SPOTLIGHT_WIDTH / 2);
                
                // Sobre el borde delantero
                //_spotlightRectangle.X = Direction == FaceDirection.Right
                //                                  ? BoundingRectangle.X + BoundingRectangle.Width
                //                                  : BoundingRectangle.X - SpotlightWidth;
                _spotlightRectangle.X = (BoundingRectangle.X + BoundingRectangle.Width / 2) - (SpotlightWidth / 2); // Centrado en X
                
                //_spotlightRectangle.Y = (int)Math.Round(Position.Y - Sprite.Origin.Y) + LocalBounds.Y;
                _spotlightRectangle.Y = (BoundingRectangle.Y + BoundingRectangle.Height / 2) - (SpotlightHeight / 2); // Centrado en Y
                _spotlightRectangle.Width = SpotlightWidth;
                _spotlightRectangle.Height = SpotlightHeight;

                return _spotlightRectangle;
            }
        }

        protected bool SeePlayer;

        // HZ: No usamos la textura ya que no se tiene que ver el campo de vision
        // Si quisieramos debuguear y ver como es el campo de vision, entonces si tendriamos que usar la textura
        //private readonly Texture2D _spotlightTexture;

        // Animations
        protected Animation RunAnimation;
        protected Animation IdleAnimation;
        protected AnimationPlayer Sprite;
        protected Animation DieAnimation;
        protected Animation HitAnimation;

        // Sounds
        private SoundEffect _killedSound;

        public bool IsAlive { get; private set; }

        //public int CurrentHealth
        //{
        //    get { return _currentHealth; }
        //}

        private const float DEATH_TIME_MAX = 1.0f;
        public float DeathTime { get; private set; }

        /// <summary>
        /// The direction this Npc is facing and moving along the X axis.
        /// </summary>
        public FaceDirection Direction = FaceDirection.Left;

        /// <summary>
        /// How long this Npc has been waiting before turning around.
        /// </summary>
        protected float WaitTime;

        /// <summary>
        /// Acceleration of the enemies
        /// </summary>
        protected float Acceleration = ACCELERATION_NORMAL;

        //private Vector2 Sprite.Origin;

        //protected HealthBar HealthBar;

        /// <summary>
        /// How long to wait before turning around.
        /// </summary>
        protected const float MAX_WAIT_TIME = 0.5f;

        /// <summary>
        /// The speed at which this Npc moves along the X axis.
        /// </summary>
        protected const float MOVE_SPEED = 64.0f;

        /// <summary>
        /// The coeficient at which this Npc moves normal along the X axis.
        /// </summary>
        protected const float ACCELERATION_NORMAL = 1.0f;

        /// <summary>
        /// The coeficient at which this Npc moves chase the player along the X axis.
        /// </summary>
        protected const float ACCELERATION_CHASING = 2.0f;

        //private const bool DEBUG_ENEMIES = false;

        public bool IsInit { get; protected set; }

//        /// <summary>
//        /// Constructs a new Npc.
//        /// </summary>
//        public Npc(Level level, Vector2 position, bool followPlayer)
//        {
//            //Init(followPlayer, level, position);

//// Todas cosas usabas para "ver" bounds y elementos que no se ven (como radio de vision)
////#if DEBUG
//            // Load spotlight Texture (campo de vision)
//            // HZ: No usamos la textura ya que no se tiene que ver el campo de vision
//            // Si quisieramos debuguear y ver como es el campo de vision, entonces si tendriamos que usar la textura
//            //_spotlightTexture = Level.Content.Load<Texture2D>("Overlays/spotlight2");
////            _localBoundRectangle = new RectangleOverlay(Color.Blue, Level.ScreenManager.GraphicsDevice);
////#endif
//        }

        public virtual void Init(bool followPlayer, Level level)
        {
            //Reset();

            //DrawHealthBar = true;
            FollowPlayer = followPlayer;
            Level = level;
            
            //Load sounds. 
            _killedSound = Level.Content.Load<SoundEffect>(GlobalParameters.ZOMBIE_KILLED_SOUND);
            
            TotalHealth = 5;
            CurrentHealth = TotalHealth;

            // Trae problemas de performance, aun cuando no se dibuje
            //if (DrawHealthBar)
            //    HealthBar = new HealthBar(Level.Content);

            IsInit = true;

            InitBoundingRectangule();

            InitSpotlightRectangle();
        }

        private void InitSpotlightRectangle()
        {
            var left = (int)Math.Round(Position.X - Sprite.Origin.X) + LocalBounds.X;
            var top = (int)Math.Round(Position.Y - Sprite.Origin.Y) + LocalBounds.Y;

            _spotlightRectangle = new Rectangle(left - (SpotlightWidth/2),
                                                top,
                                                SpotlightWidth,
                                                SpotlightHeight);
        }

        private void InitBoundingRectangule()
        {
            var left = (int)Math.Round(Position.X - Sprite.Origin.X) + LocalBounds.X;
            var top = (int)Math.Round(Position.Y - Sprite.Origin.Y) + LocalBounds.Y;

            _boundingRectangle = new Rectangle(left, top, LocalBounds.Width, LocalBounds.Height);
        }

        /// <summary>
        /// Establece los parametros iniciales x defecto y position inicial
        /// </summary>
        /// <param name="position">Posicion inicial</param>
        public virtual void Start(Vector2 position)
        {
            Position = position;
            IsAlive = true;
            DeathTime = DEATH_TIME_MAX;
            CurrentHealth = TotalHealth;
        }
        
        //public Npc(Level level, Vector2 position, bool isAlive, int currentHealth, bool followPlayer) : this (level, position, followPlayer)
        //{
        //    //SpriteSet = npcType.ToString();
        //    //_followPlayer = followPlayer;
        //    //Level = level;
        //    //Position = position;
        //    //NPCType = npcType;

        //    IsAlive = isAlive;
        //    CurrentHealth = currentHealth;

        //    //LoadContent(SpriteSet);
        //}

        /// <summary>
        /// Loads a particular Npc sprite sheet and sounds.
        /// </summary>
        //public virtual void LoadContent(string spriteSet)
        //{
        //    // Load animations.
        //    spriteSet = "Sprites/" + spriteSet + "/";

        //    _idleAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Idle"), 0.15f, true);
        //    _runAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Run"), 0.1f, true);
        //    _dieAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Die"), 0.07f, false);
        //    _hitAnimation = new Animation(Level.Content.Load<Texture2D>(spriteSet + "Run"), 0.07f, false);
        //    _sprite.PlayAnimation(_idleAnimation);


        //    // Calculate bounds within texture size.
        //    SetLocalBounds();

        //    //Load sounds. 
        //    _killedSound = Level.Content.Load<SoundEffect>("Sounds/MonsterKilled");

        //    // Load spotlight Texture (campo de vision)
        //    // HZ: No usamos la textura ya que no se tiene que ver el campo de vision
        //    // Si quisieramos debuguear y ver como es el campo de vision, entonces si tendriamos que usar la textura
        //    spotlightTexture = Level.Content.Load<Texture2D>("Overlays/spotlight2");

        //    //Load the HealthBar image from the disk into the Texture2D object
        //    _healthBar = Level.Content.Load<Texture2D>("HealthBar");

        //    _totalHealth = 5;
        //    _currentHealth = _totalHealth;
        //}

        //protected virtual void SetLocalBounds()
        //{
        //    int width = (int)(_idleAnimation.FrameWidth * 0.4);
        //    int left = (_idleAnimation.FrameWidth - width) / 2;
        //    int height = (int)(_idleAnimation.FrameWidth * 0.8);
        //    int top = _idleAnimation.FrameHeight - height;
        //    _localBounds = new Rectangle(left, top, width, height);
        //}


        /// <summary>
        /// Paces back and forth along a platform, waiting at either end.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Si el character esta vivo
            if (IsAlive)
            {
                CheckVision();
                DoIA(gameTime, elapsed);
            }
            else
            {
                Die(gameTime);
            }
        }

        protected virtual void CheckVision()
        {
            SeePlayer = FollowPlayer && SpotlightRectangle.Intersects(Level.Player.BoundingRectangle);
        }

        protected virtual void Die(GameTime gameTime)
        {
            DeathTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected virtual void DoIA(GameTime gameTime, float elapsed)
        {
            if (CurrentSate == CharacterStates.Attacked) return;

            // Calculate tile position based on the side we are walking towards.
            var posX = Position.X + LocalBounds.Width / 2 * (int)Direction;
            var tileX = (int)Math.Floor(posX / Tile.WIDTH) - (int)Direction;
            var tileY = (int)Math.Floor(Position.Y / Tile.HEIGHT);

            if (WaitTime > 0)
            {
                // Wait for some amount of time.
                WaitTime = Math.Max(0.0f, WaitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (WaitTime <= 0.0f && !SeePlayer)
                {
                    // Then turn around.
                    Direction = (FaceDirection)(-(int)Direction);
                }
            }
            else
            {
                // If we are about to run into a wall or off a cliff, start waiting.
                if (Level.GetCollision(tileX + (int)Direction, tileY - 1) == TileCollision.Impassable ||
                    (Level.GetCollision(tileX + (int)Direction, tileY) == TileCollision.Passable))
                {
                    WaitTime = MAX_WAIT_TIME;
                }
                else
                {
                    //if (_seePlayer) // Si el enemigo ve al personaje ...
                    //{
                    //    // ...  si esta dado vuelta, se da vuelta para perseguirlo
                    //    if (Level.Player.Position.X < _position.X && _direction == FaceDirection.Right)
                    //    {
                    //        _direction = FaceDirection.Left;
                    //    }
                    //    else if (Level.Player.Position.X > _position.X && _direction == FaceDirection.Left)
                    //    {
                    //        _direction = FaceDirection.Right;
                    //    }

                    //    // ... acelera su paso
                    //    _acceleration = ACCELERATION_CHASING;
                    //}
                    //else
                    //{
                    Acceleration = ACCELERATION_NORMAL;
                    //}

                    // Move in the current direction.
                    var velocity = new Vector2((int)Direction * MOVE_SPEED * Acceleration * elapsed, 0.0f);
                    Position = Position + velocity;
                }
            }
        }

        /// <summary>
        /// Draws the animated Npc.
        /// </summary>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Stop running when the game is paused or before turning around.
            if (((!Level.Player.IsAlive ||
                Level.ReachedExit ||
                Level.TimeRemaining == TimeSpan.Zero) || (WaitTime > 0 && !SeePlayer)) && IsAlive)
            {
                PlayAnimation(IdleAnimation);
            }
            else if (!IsAlive)
            {
                PlayAnimation(DieAnimation);
            }
            else if (CurrentSate == CharacterStates.Attacked)
            {
                PlayAnimation(HitAnimation);
            }
            else
            {
                PlayAnimation(RunAnimation);
            }


            // Draw facing the way the Npc is moving.
            var flip = Direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Sprite.Draw(gameTime, spriteBatch, Position, flip);
        }

        public void OnHit(Player killedBy, Bullet bullet)
        {
            OnHit(bullet);
        }

        internal virtual void OnHit(Bullet bullet)
        {
            if (bullet == null)
            {
                CurrentHealth = CurrentHealth - 1;
                //throw new Exception("El objeto de ataque no puede ser null");
            }
            else
            {
                CurrentHealth = CurrentHealth - bullet.Power;    
            }
            

            //Force the health to remain between 0 and 100
            CurrentHealth = (int)MathHelper.Clamp(CurrentHealth, 0, TotalHealth);

            if (CurrentHealth == 0)
            {
                IsAlive = false;
                _killedSound.Play();
            }
            else
            {
                if (HittedTime != MAX_HITTED_TIME)
                {
                    CurrentSate = CharacterStates.Attacked;
                    HittedTime = MAX_HITTED_TIME;
                }
            }
        }

        /// <summary>
        /// Set animation to play and set current localbound based on animation
        /// </summary>
        /// <param name="animation"></param>
        protected void PlayAnimation(Animation animation)
        {
            Sprite.PlayAnimation(animation);
            LocalBounds = animation.BoundingRectangle;
        }
    }
}

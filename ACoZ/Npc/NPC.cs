using System;
using ACoZ.Animations;
using ACoZ.Helpers;
using ACoZ.Levels;
using ACoZ.Players;
using ACoZ.Weapons;
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

namespace ACoZ.Npc
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
            this.SpotlightHeight = GlobalParameters.SPOTLIGHT_HEIGHT;
            this.SpotlightWidth = GlobalParameters.SPOTLIGHT_WIDTH;
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
                this._boundingRectangle.X = (int) Math.Round(this.Position.X - this.LocalBounds.Width / 2);
                this._boundingRectangle.Y = (int) Math.Round(this.Position.Y - this.LocalBounds.Height);
                this._boundingRectangle.Width = this.LocalBounds.Width;
                this._boundingRectangle.Height = this.LocalBounds.Height;

                return this._boundingRectangle;
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
                this._spotlightRectangle.X = (this.BoundingRectangle.X + this.BoundingRectangle.Width / 2) - (this.SpotlightWidth / 2); // Centrado en X
                
                //_spotlightRectangle.Y = (int)Math.Round(Position.Y - Sprite.Origin.Y) + LocalBounds.Y;
                this._spotlightRectangle.Y = (this.BoundingRectangle.Y + this.BoundingRectangle.Height / 2) - (this.SpotlightHeight / 2); // Centrado en Y
                this._spotlightRectangle.Width = this.SpotlightWidth;
                this._spotlightRectangle.Height = this.SpotlightHeight;

                return this._spotlightRectangle;
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
            this.FollowPlayer = followPlayer;
            this.Level = level;
            
            //Load sounds. 
            this._killedSound = this.Level.Content.Load<SoundEffect>(GlobalParameters.ZOMBIE_KILLED_SOUND);
            
            this.TotalHealth = 5;
            this.CurrentHealth = this.TotalHealth;

            // Trae problemas de performance, aun cuando no se dibuje
            //if (DrawHealthBar)
            //    HealthBar = new HealthBar(Level.Content);

            this.IsInit = true;

            this.InitBoundingRectangule();

            this.InitSpotlightRectangle();
        }

        private void InitSpotlightRectangle()
        {
            var left = (int)Math.Round(this.Position.X - this.Sprite.Origin.X) + this.LocalBounds.X;
            var top = (int)Math.Round(this.Position.Y - this.Sprite.Origin.Y) + this.LocalBounds.Y;

            this._spotlightRectangle = new Rectangle(left - (this.SpotlightWidth/2),
                                                top,
                                                this.SpotlightWidth,
                                                this.SpotlightHeight);
        }

        private void InitBoundingRectangule()
        {
            var left = (int)Math.Round(this.Position.X - this.Sprite.Origin.X) + this.LocalBounds.X;
            var top = (int)Math.Round(this.Position.Y - this.Sprite.Origin.Y) + this.LocalBounds.Y;

            this._boundingRectangle = new Rectangle(left, top, this.LocalBounds.Width, this.LocalBounds.Height);
        }

        /// <summary>
        /// Establece los parametros iniciales x defecto y position inicial
        /// </summary>
        /// <param name="position">Posicion inicial</param>
        public virtual void Start(Vector2 position)
        {
            this.Position = position;
            this.IsAlive = true;
            this.DeathTime = DEATH_TIME_MAX;
            this.CurrentHealth = this.TotalHealth;
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
            if (this.IsAlive)
            {
                this.CheckVision();
                this.DoIA(gameTime, elapsed);
            }
            else
            {
                this.Die(gameTime);
            }
        }

        protected virtual void CheckVision()
        {
            this.SeePlayer = this.FollowPlayer && this.SpotlightRectangle.Intersects(this.Level.Player.BoundingRectangle);
        }

        protected virtual void Die(GameTime gameTime)
        {
            this.DeathTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected virtual void DoIA(GameTime gameTime, float elapsed)
        {
            if (this.CurrentSate == CharacterStates.Attacked) return;

            // Calculate tile position based on the side we are walking towards.
            var posX = this.Position.X + this.LocalBounds.Width / 2 * (int)this.Direction;
            var tileX = (int)Math.Floor(posX / Tile.WIDTH) - (int)this.Direction;
            var tileY = (int)Math.Floor(this.Position.Y / Tile.HEIGHT);

            if (this.WaitTime > 0)
            {
                // Wait for some amount of time.
                this.WaitTime = Math.Max(0.0f, this.WaitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (this.WaitTime <= 0.0f && !this.SeePlayer)
                {
                    // Then turn around.
                    this.Direction = (FaceDirection)(-(int)this.Direction);
                }
            }
            else
            {
                // If we are about to run into a wall or off a cliff, start waiting.
                if (this.Level.GetCollision(tileX + (int)this.Direction, tileY - 1) == TileCollision.Impassable ||
                    (this.Level.GetCollision(tileX + (int)this.Direction, tileY) == TileCollision.Passable))
                {
                    this.WaitTime = MAX_WAIT_TIME;
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
                    this.Acceleration = ACCELERATION_NORMAL;
                    //}

                    // Move in the current direction.
                    var velocity = new Vector2((int)this.Direction * MOVE_SPEED * this.Acceleration * elapsed, 0.0f);
                    this.Position = this.Position + velocity;
                }
            }
        }

        /// <summary>
        /// Draws the animated Npc.
        /// </summary>
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Stop running when the game is paused or before turning around.
            if (((!this.Level.Player.IsAlive ||
                this.Level.ReachedExit ||
                this.Level.TimeRemaining == TimeSpan.Zero) || (this.WaitTime > 0 && !this.SeePlayer)) && this.IsAlive)
            {
                this.PlayAnimation(this.IdleAnimation);
            }
            else if (!this.IsAlive)
            {
                this.PlayAnimation(this.DieAnimation);
            }
            else if (this.CurrentSate == CharacterStates.Attacked)
            {
                this.PlayAnimation(this.HitAnimation);
            }
            else
            {
                this.PlayAnimation(this.RunAnimation);
            }


            // Draw facing the way the Npc is moving.
            var flip = this.Direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            this.Sprite.Draw(gameTime, spriteBatch, this.Position, flip);
        }

        public void OnHit(Player killedBy, Bullet bullet)
        {
            this.OnHit(bullet);
        }

        internal virtual void OnHit(Bullet bullet)
        {
            if (bullet == null)
            {
                this.CurrentHealth = this.CurrentHealth - 1;
                //throw new Exception("El objeto de ataque no puede ser null");
            }
            else
            {
                this.CurrentHealth = this.CurrentHealth - bullet.Power;    
            }
            

            //Force the health to remain between 0 and 100
            this.CurrentHealth = (int)MathHelper.Clamp(this.CurrentHealth, 0, this.TotalHealth);

            if (this.CurrentHealth == 0)
            {
                this.IsAlive = false;
                this._killedSound.Play();
            }
            else
            {
                if (this.HittedTime != MAX_HITTED_TIME)
                {
                    this.CurrentSate = CharacterStates.Attacked;
                    this.HittedTime = MAX_HITTED_TIME;
                }
            }
        }

        /// <summary>
        /// Set animation to play and set current localbound based on animation
        /// </summary>
        /// <param name="animation"></param>
        protected void PlayAnimation(Animation animation)
        {
            this.Sprite.PlayAnimation(animation);
            this.LocalBounds = animation.BoundingRectangle;
        }
    }
}

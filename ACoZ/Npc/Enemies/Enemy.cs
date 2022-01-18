#region File Description
//-----------------------------------------------------------------------------
// Enemy.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using ACoZ.Animations;
using ACoZ.Helpers;
using ACoZ.Levels;
using ACoZ.Players;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ACoZ.Npc.Enemies
{
    public abstract class Enemy : Npc
    {
        public int PointValue { get; protected set; }
        //private Random _random = new Random(); // Arbitrary, but constant seed
        protected Vector2 Velocity;
        protected Animation AttackAnimation;
        //protected string AttackAnimation;

        //public Survivor AttackedSurvivor { get; private set; }
        public Player AttackedPlayer { get; private set; }
        //public Npc SawCharacter;
        //private bool _seeSurvivor;
        private bool _alreadySumPoint;
		private SoundEffect _attackSound;
        private TimeSpan _attackSoundDuration;

#if DEBUG
        private Texture2D _dummyTexture;
#endif

#if IPHONE
        private const double ATTACK_SOUND_DURATION = 1;
#endif

        ///// <summary>
        ///// Constructs a new Enemy.
        ///// </summary>
        //public Enemy(Level level, Vector2 position)
        //    : base(level, position, true)
        //{
        //    // Set initial character velocity movenment
        //    Velocity = Vector2.Zero;
        //}

        public override void Init(bool followPlayer, Level level)
        {
            base.Init(followPlayer, level);

            this._attackSound = this.Level.Content.Load<SoundEffect>(GlobalParameters.ZOMBIE_ATTACK_SOUND);

            this._attackSoundDuration = TimeSpan.FromSeconds(0);

            // Set initial character velocity movenment
            this.Velocity = Vector2.Zero;
        }

        //public Enemy(Level level, Vector2 position, bool isAlive, int currentHealth)
        //    : base(level, position, isAlive, currentHealth, true)
        //{
        //    // Set initial character velocity movenment
        //    Velocity = Vector2.Zero;
        //}

        //public void AttackSurvivor(Survivor survivor)
        //{
        //    AttackedSurvivor = survivor;
        //    CurrentSate = CharacterStates.Attacking;
        //}

        public void AttackPlayer(Player player)
        {
            this.AttackedPlayer = player;
            this.CurrentSate = CharacterStates.Attacking;

            if (this._attackSoundDuration > TimeSpan.FromSeconds(0))
                return;

#if IPHONE
            _attackSoundDuration = TimeSpan.FromSeconds(ATTACK_SOUND_DURATION);
#else
            this._attackSoundDuration = this._attackSound.Duration;
#endif
            this._attackSound.Play();
        }

        public override void Start(Vector2 position)
        {
            this._alreadySumPoint = false;
            base.Start(position);
        }

        protected override void Die(GameTime gameTime)
        {
            base.Die(gameTime);

            // Si ya sumo los ptos por muerte, entonces que retorne
            if (this._alreadySumPoint) return;

            this.Level.Score += this.PointValue;
            this._alreadySumPoint = true;
        }

        protected override void DoIA(GameTime gameTime, float elapsed)
        {
            switch (this.CurrentSate)
            {
                case CharacterStates.Attacking:
                    this.Velocity = Vector2.Zero;

                    this.ApplyVelocity();

                    //if (AttackedSurvivor != null)
                    //{
                    //    if (!AttackedSurvivor.IsAlive)
                    //    {
                    //        StopAttackingSurvivor();
                    //    }
                    //}

                    if (this.AttackedPlayer != null)
                    {
                        if (!this.AttackedPlayer.IsAlive)
                        {
                            this.StopAttackingPlayer();
                        }
                    }
                    break;
                case CharacterStates.Walking:
                    // Calculate tile position based on the side we are walking towards.
                    var posX = this.Position.X + this.LocalBounds.Width/2*(int) this.Direction;
                    var tileX = (int) Math.Floor(posX/Tile.WIDTH) - (int) this.Direction;
                    var tileY = (int) Math.Floor(this.Position.Y/Tile.HEIGHT);

                    if (this.WaitTime > 0)
                    {
                        // Wait for some amount of time.
                        this.WaitTime = Math.Max(0.0f, this.WaitTime - (float) gameTime.ElapsedGameTime.TotalSeconds);
                        if (this.WaitTime <= 0.0f && !this.SeePlayer)
                        {
                            // Then turn around.
                            this.Direction = (FaceDirection) (-(int) this.Direction);
                        }
                    }
                    else
                    {
                        // If we are about to run into a wall or off a cliff, start waiting.
                        if (this.Level.GetCollision(tileX + (int) this.Direction, tileY - 1) == TileCollision.Impassable ||
                            (this.Level.GetCollision(tileX + (int) this.Direction, tileY) == TileCollision.Passable))
                        {
                            this.WaitTime = MAX_WAIT_TIME;
                        }
                        else
                        {
                            //int nextrandom = RandomUtil.Next(200);
                            //Acceleration = ACCELERATION_NORMAL * nextrandom / 100;

                            //Acceleration = ACCELERATION_NORMAL;

                            if (this.SeePlayer) // Si el enemigo ve al personaje pero no a un sobreviviente ...
                            {
                                // ...  si esta dado vuelta, se da vuelta para perseguirlo
                                if (this.Level.Player.Position.X < this.Position.X && this.Direction == FaceDirection.Right)
                                {
                                    this.Direction = FaceDirection.Left;
                                }
                                else if (this.Level.Player.Position.X > this.Position.X && this.Direction == FaceDirection.Left)
                                {
                                    this.Direction = FaceDirection.Right;
                                }

                                // TODO: revisar esto sobre como boludea un zombie
                                // ... acelera su paso
                                //_acceleration = ACCELERATION_CHASING;
                            }
                            //else if (_seeSurvivor) // Si vio un sobreviviente
                            //{
                            //    if (SawCharacter != null)
                            //    {
                            //        // ...  si esta dado vuelta, se da vuelta para perseguirlo
                            //        if (SawCharacter.Position.X < Position.X && Direction == FaceDirection.Right)
                            //        {
                            //            Direction = FaceDirection.Left;
                            //        }
                            //        else if (SawCharacter.Position.X > Position.X && Direction == FaceDirection.Left)
                            //        {
                            //            Direction = FaceDirection.Right;
                            //        }

                            //        // ... acelera su paso
                            //        //_acceleration = ACCELERATION_CHASING;
                            //    }
                            //}
                            
                            // Move in the current direction.
                            //var velocity = new Vector2((int) _direction*MOVE_SPEED*_acceleration*elapsed, 0.0f);
                            //_position = _position + velocity;
                            this.Velocity = new Vector2((int) this.Direction*MOVE_SPEED*this.Acceleration*elapsed, 0.0f);
                            this.ApplyVelocity();
                        }
                    }
                    break;
                case CharacterStates.Attacked:
                    this.BeHitted(gameTime, elapsed);
                    break;
            }
        }

        private void ApplyVelocity()
        {
            this.Position += this.Velocity;
            this.Position = new Vector2((float)Math.Round(this.Position.X), (float)Math.Round(this.Position.Y));
        }

        //private void StopAttackingSurvivor()
        //{
        //    CurrentSate = CharacterStates.Walking;
        //    AttackedSurvivor = null;
        //}

        private void StopAttackingPlayer()
        {
            this.CurrentSate = CharacterStates.Walking;
            this.AttackedPlayer = null;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
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
            else if (this.CurrentSate == CharacterStates.Attacking)
            {
                this.PlayAnimation(this.AttackAnimation);
            }
            else
            {
                this.PlayAnimation(this.RunAnimation);
            }

            // Draw facing the way the Npc is moving.
            var flip = this.Direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            this.Sprite.Draw(gameTime, spriteBatch, this.Position, flip);

#if DEBUG && !IPHONE
            // Draw BoundingRectangle (for DEBUG purpouse)
            if (_dummyTexture == null)
            {
                _dummyTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _dummyTexture.SetData(new Color[] { Color.White });
            }

            spriteBatch.Draw(_dummyTexture, BoundingRectangle, Color.Blue);
            spriteBatch.Draw(_dummyTexture, SpotlightRectangle, Color.Yellow);
#endif
        }

        private void BeHitted(GameTime gameTime, float elapsed)
        {
            // If the player wants to attack
            if (this.CurrentSate == CharacterStates.Attacked)
            {
                // Begin or continue an attack
                if (this.HittedTime > 0.0f)
                {
                    this.HittedTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    int empujado;

                    // ...  si esta dado vuelta, se da vuelta para perseguirlo
                    if (this.Level.Player.Position.X < this.Position.X)
                    {
                        empujado = (int)FaceDirection.Right;
                    }
                    else
                    {
                        empujado = (int)FaceDirection.Left;
                    }

                    // Move in the current direction.
                    var velocity = new Vector2(empujado * MOVE_SPEED * this.Acceleration * elapsed, 0.0f);
                    this.Position = this.Position + velocity;
                }
                else
                {
                    this.CurrentSate = CharacterStates.Walking;
                }
            }
            else
            {
                //Continues not attack or cancels an attack in progress
                this.HittedTime = 0.0f;
            }
        }

        protected override void CheckVision()
        {
            if (this.CurrentSate != CharacterStates.Attacking)
            {
                this.SeePlayer = this.FollowPlayer && this.SpotlightRectangle.Intersects(this.Level.Player.BoundingRectangle);

                //_seeSurvivor = false;

                // Si ve al player, vemos aleatoriamente si quiere tratar de ver a los sobrevivientes
                //if (WantToSeeSurvivors())
                //{
                //    // Attack survivors
                //    for (int x = 0; x < Level.Survivors.Count; ++x)
                //    {
                //        Survivor survivor = Level.Survivors[x];

                //        if (survivor.IsAlive && survivor.BoundingRectangle.Intersects(SpotlightRectangle))// && SawCharacter == null)
                //        {
                //            SeeSurvivor(survivor);
                //        }
                //    }
                //}
            }
            else
            {
                this.SeePlayer = false;
                //_seeSurvivor = false;
            }
        }

        //private bool WantToSeeSurvivors()
        //{
        //    if (_seePlayer)
        //    {
        //        return RandomUtil.Next(100) >= 50;
        //    }
        //    return true;
        //}

        //private void SeeSurvivor(Survivor survivor)
        //{
        //    SawCharacter = survivor;
        //    _seePlayer = false;
        //    _seeSurvivor = true;
        //}

        public void NotAttackingPlayer()
        {
            // Si el estado era de Ataque y estaba atacando a un player
            if (this.CurrentSate == CharacterStates.Attacking && this.AttackedPlayer != null)
            {
                this.StopAttackingPlayer();
            }
        }

        //public void SetMaxAttackTime(float maxHitTime)
        //{
        //    AttackTime = maxHitTime;
        //}

        //public float AttackTime { get; private set; }
        public float AttackTime { get; set; }

        public override void Update(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (this.AttackTime > 0.0f)
            {
                this.AttackTime -= elapsed;
            }

            if (this._attackSoundDuration > TimeSpan.FromSeconds(0))
            {
                this._attackSoundDuration = this._attackSoundDuration - TimeSpan.FromSeconds(elapsed);
            }
            
            base.Update(gameTime);
        }
    }
}

#region File Description
//-----------------------------------------------------------------------------
// Enemy.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Platformer.Animations;
using Platformer.Helpers;
using Platformer.Levels;
using Platformer.Players;

namespace Platformer.Npc.Enemies
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

            _attackSound = Level.Content.Load<SoundEffect>(GlobalParameters.ZOMBIE_ATTACK_SOUND);

            _attackSoundDuration = TimeSpan.FromSeconds(0);

            // Set initial character velocity movenment
            Velocity = Vector2.Zero;
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
            AttackedPlayer = player;
            CurrentSate = CharacterStates.Attacking;

            if (_attackSoundDuration > TimeSpan.FromSeconds(0))
                return;

#if IPHONE
            _attackSoundDuration = TimeSpan.FromSeconds(ATTACK_SOUND_DURATION);
#else
            _attackSoundDuration = _attackSound.Duration;
#endif
            _attackSound.Play();
        }

        public override void Start(Vector2 position)
        {
            _alreadySumPoint = false;
            base.Start(position);
        }

        protected override void Die(GameTime gameTime)
        {
            base.Die(gameTime);

            // Si ya sumo los ptos por muerte, entonces que retorne
            if (_alreadySumPoint) return;

            Level.Score += PointValue;
            _alreadySumPoint = true;
        }

        protected override void DoIA(GameTime gameTime, float elapsed)
        {
            switch (CurrentSate)
            {
                case CharacterStates.Attacking:
                    Velocity = Vector2.Zero;

                    ApplyVelocity();

                    //if (AttackedSurvivor != null)
                    //{
                    //    if (!AttackedSurvivor.IsAlive)
                    //    {
                    //        StopAttackingSurvivor();
                    //    }
                    //}

                    if (AttackedPlayer != null)
                    {
                        if (!AttackedPlayer.IsAlive)
                        {
                            StopAttackingPlayer();
                        }
                    }
                    break;
                case CharacterStates.Walking:
                    // Calculate tile position based on the side we are walking towards.
                    var posX = Position.X + LocalBounds.Width/2*(int) Direction;
                    var tileX = (int) Math.Floor(posX/Tile.WIDTH) - (int) Direction;
                    var tileY = (int) Math.Floor(Position.Y/Tile.HEIGHT);

                    if (WaitTime > 0)
                    {
                        // Wait for some amount of time.
                        WaitTime = Math.Max(0.0f, WaitTime - (float) gameTime.ElapsedGameTime.TotalSeconds);
                        if (WaitTime <= 0.0f && !SeePlayer)
                        {
                            // Then turn around.
                            Direction = (FaceDirection) (-(int) Direction);
                        }
                    }
                    else
                    {
                        // If we are about to run into a wall or off a cliff, start waiting.
                        if (Level.GetCollision(tileX + (int) Direction, tileY - 1) == TileCollision.Impassable ||
                            (Level.GetCollision(tileX + (int) Direction, tileY) == TileCollision.Passable))
                        {
                            WaitTime = MAX_WAIT_TIME;
                        }
                        else
                        {
                            //int nextrandom = RandomUtil.Next(200);
                            //Acceleration = ACCELERATION_NORMAL * nextrandom / 100;

                            //Acceleration = ACCELERATION_NORMAL;

                            if (SeePlayer) // Si el enemigo ve al personaje pero no a un sobreviviente ...
                            {
                                // ...  si esta dado vuelta, se da vuelta para perseguirlo
                                if (Level.Player.Position.X < Position.X && Direction == FaceDirection.Right)
                                {
                                    Direction = FaceDirection.Left;
                                }
                                else if (Level.Player.Position.X > Position.X && Direction == FaceDirection.Left)
                                {
                                    Direction = FaceDirection.Right;
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
                            Velocity = new Vector2((int) Direction*MOVE_SPEED*Acceleration*elapsed, 0.0f);
                            ApplyVelocity();
                        }
                    }
                    break;
                case CharacterStates.Attacked:
                    BeHitted(gameTime, elapsed);
                    break;
            }
        }

        private void ApplyVelocity()
        {
            Position += Velocity;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
        }

        //private void StopAttackingSurvivor()
        //{
        //    CurrentSate = CharacterStates.Walking;
        //    AttackedSurvivor = null;
        //}

        private void StopAttackingPlayer()
        {
            CurrentSate = CharacterStates.Walking;
            AttackedPlayer = null;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
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
            else if (CurrentSate == CharacterStates.Attacking)
            {
                PlayAnimation(AttackAnimation);
            }
            else
            {
                PlayAnimation(RunAnimation);
            }

            // Draw facing the way the Npc is moving.
            var flip = Direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Sprite.Draw(gameTime, spriteBatch, Position, flip);

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
            if (CurrentSate == CharacterStates.Attacked)
            {
                // Begin or continue an attack
                if (HittedTime > 0.0f)
                {
                    HittedTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                    int empujado;

                    // ...  si esta dado vuelta, se da vuelta para perseguirlo
                    if (Level.Player.Position.X < Position.X)
                    {
                        empujado = (int)FaceDirection.Right;
                    }
                    else
                    {
                        empujado = (int)FaceDirection.Left;
                    }

                    // Move in the current direction.
                    var velocity = new Vector2(empujado * MOVE_SPEED * Acceleration * elapsed, 0.0f);
                    Position = Position + velocity;
                }
                else
                {
                    CurrentSate = CharacterStates.Walking;
                }
            }
            else
            {
                //Continues not attack or cancels an attack in progress
                HittedTime = 0.0f;
            }
        }

        protected override void CheckVision()
        {
            if (CurrentSate != CharacterStates.Attacking)
            {
                SeePlayer = FollowPlayer && SpotlightRectangle.Intersects(Level.Player.BoundingRectangle);

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
                SeePlayer = false;
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
            if (CurrentSate == CharacterStates.Attacking && AttackedPlayer != null)
            {
                StopAttackingPlayer();
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

            if (AttackTime > 0.0f)
            {
                AttackTime -= elapsed;
            }

            if (_attackSoundDuration > TimeSpan.FromSeconds(0))
            {
                _attackSoundDuration = _attackSoundDuration - TimeSpan.FromSeconds(elapsed);
            }
            
            base.Update(gameTime);
        }
    }
}

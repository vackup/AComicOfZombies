using Microsoft.Xna.Framework.Graphics;
using Platformer.Animations;
using Platformer.Helpers;
using Platformer.Levels;

namespace Platformer.Npc.Enemies
{
    /// <summary>
    /// Zombie1
    /// </summary>
    public class SlowStrongMonster : Enemy
    {
        private const int TOTAL_HEALTH = 5;

       /// <summary>
        /// Set animations, character bounds and caracteristics
        /// </summary>
        //private void Init(int currentHealth)
        public override void Init(bool followPlayer, Level level)
        {
            base.Init(followPlayer, level);

            var spriteTexture = Level.Content.Load<Texture2D>(GlobalParameters.SLOW_STRONG_MONSTER_ANIMATION_TEXTURE);

            IdleAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.SsmAnimationRectangulesIdle);

            RunAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.SsmAnimationRectangulesRun);
            RunAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            DieAnimation = new Animation(spriteTexture, 0.075f, false, GlobalParameters.SsmAnimationRectangulesDie);
            DieAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            HitAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.SsmAnimationRectangulesHit);
            HitAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            AttackAnimation = new Animation(spriteTexture, 0.0375f, true, GlobalParameters.SsmAnimationRectangulesAttack);
            AttackAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            // Set animation to play
            PlayAnimation(IdleAnimation);

            //Load sounds. 
            //_killedSound = Level.Content.Load<SoundEffect>("Sounds/MonsterKilled");

            // Load spotlight Texture (campo de vision)
            // HZ: No usamos la textura ya que no se tiene que ver el campo de vision
            // Si quisieramos debuguear y ver como es el campo de vision, entonces si tendriamos que usar la textura
            //spotlightTexture = Level.Content.Load<Texture2D>("Overlays/spotlight2");

            // Caracteristicas
            TotalHealth = TOTAL_HEALTH;
            Acceleration = 1.0f;
            PointValue = 30;
            CurrentHealth = TOTAL_HEALTH;
        }
    }
}

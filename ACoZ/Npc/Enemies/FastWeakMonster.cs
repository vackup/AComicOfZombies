using Microsoft.Xna.Framework.Graphics;
using Platformer.Animations;
using Platformer.Helpers;
using Platformer.Levels;

namespace Platformer.Npc.Enemies
{
    public class FastWeakMonster : Enemy
    {
        private const int TOTAL_HEALTH = 1;
        
        /// <summary>
        /// Set animations, character bounds and caracteristics
        /// </summary>
        //private void Init(int currentHealth)
        public override void  Init(bool followPlayer, Level level)
        {
            base.Init(followPlayer, level);

            var spriteTexture = Level.Content.Load<Texture2D>(GlobalParameters.FAST_WEAK_MONSTER_ANIMATION_TEXTURE);

            IdleAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.FwmAnimationRectangulesIdle);

            RunAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.FwmAnimationRectangulesRun);
            RunAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            DieAnimation = new Animation(spriteTexture, 0.075f, false, GlobalParameters.FwmAnimationRectangulesDie);
            DieAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            HitAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.FwmAnimationRectangulesHit);
            HitAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            AttackAnimation = new Animation(spriteTexture, 0.0375f, true, GlobalParameters.FwmAnimationRectangulesAttack);
            AttackAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            //Load sounds. 
            //_killedSound = Level.Content.Load<SoundEffect>("Sounds/MonsterKilled");

            // Load spotlight Texture (campo de vision)
            // HZ: No usamos la textura ya que no se tiene que ver el campo de vision
            // Si quisieramos debuguear y ver como es el campo de vision, entonces si tendriamos que usar la textura
            //spotlightTexture = Level.Content.Load<Texture2D>("Overlays/spotlight2");
            
            TotalHealth = TOTAL_HEALTH;
            Acceleration = 2.0f;
            PointValue = 10;
            CurrentHealth = TOTAL_HEALTH;
        }
    }
}

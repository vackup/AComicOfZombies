using ACoZ.Animations;
using ACoZ.Helpers;
using ACoZ.Levels;
using Microsoft.Xna.Framework.Graphics;

namespace ACoZ.Npc.Enemies
{
    public class NormalMonster : Enemy
    {
        private const int TOTAL_HEALTH = 3;

        /// <summary>
        /// Set animations, character bounds and caracteristics
        /// </summary>
        //private void Init(int currentHealth)
        public override void Init(bool followPlayer, Level level)
        {
            base.Init(followPlayer, level);

            var spriteTexture = this.Level.Content.Load<Texture2D>(GlobalParameters.NORMAL_MONSTER_ANIMATION_TEXTURE);

            this.IdleAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.NmAnimationRectangulesIdle);

            this.RunAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.NmAnimationRectangulesRun);
            this.RunAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            this.DieAnimation = new Animation(spriteTexture, 0.075f, false, GlobalParameters.NmAnimationRectangulesDie);
            this.DieAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            this.HitAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.NmAnimationRectangulesHit);
            this.HitAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            this.AttackAnimation = new Animation(spriteTexture, 0.0375f, true, GlobalParameters.NmAnimationRectangulesAttack);
            this.AttackAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            this.PlayAnimation(this.IdleAnimation);

            //Load sounds. 
            //_killedSound = Level.Content.Load<SoundEffect>("Sounds/MonsterKilled");

            // Load spotlight Texture (campo de vision)
            // HZ: No usamos la textura ya que no se tiene que ver el campo de vision
            // Si quisieramos debuguear y ver como es el campo de vision, entonces si tendriamos que usar la textura
            //spotlightTexture = Level.Content.Load<Texture2D>("Overlays/spotlight2");

            this.TotalHealth = TOTAL_HEALTH;
            this.Acceleration = 1.5f;
            this.PointValue = 20;
            this.CurrentHealth = TOTAL_HEALTH;
        }
    }
}

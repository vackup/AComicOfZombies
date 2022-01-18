using System;
using System.Collections.Generic;
using ACoZ.Animations;
using ACoZ.Helpers;
using ACoZ.Levels;
using ACoZ.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ACoZ.Players
{
    public class GordoMercenario : Player
    {
        public GordoMercenario(Level level, Vector2 position, List<Weapon> weaponInventory, Weapon primaryWeapon, Weapon secondaryWeapon, Dictionary<int, int> ammoInventory) 
            : base(level, position, weaponInventory, primaryWeapon, secondaryWeapon, ammoInventory)
        {
			//CurrentWeaponAnimation = new GameObject(Level.Content.Load<Texture2D>(GlobalParameters.GORDO_MERCENARIO_ARM));

            this.HealthBarMargin = GlobalParameters.GORDO_MERCENARIO_HEALTH_BAR_MARGIN;

            var spriteTexture = this.Level.Content.Load<Texture2D>(GlobalParameters.GORDO_MERCENARIO_ANIMATION_TEXTURE);

            this.IdleAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.GordoAnimationRectangulesIdle);

            this.RunAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.GordoAnimationRectangulesRun);
            this.RunAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            this.DieAnimation = new Animation(spriteTexture, 0.075f, false, GlobalParameters.GordoAnimationRectangulesDie);
            this.DieAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            this.BeAttackedAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.GordoAnimationRectangulesBeAttacked);
            this.BeAttackedAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            this.AttackAnimation = new Animation(spriteTexture, 0.075f, false, GlobalParameters.GordoAnimationRectangulesAttack);
            this.AttackAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            this.CelebrateAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.GordoAnimationRectangulesCelebrate);
            this.CelebrateAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            primaryWeapon.IdleAnimation = this.LoadWeaponAnimation(primaryWeapon, spriteTexture, WeaponAnimationType.Idle);
            primaryWeapon.IdleAnimationPosition = this.GetWeaponAnimationPosition(primaryWeapon, WeaponAnimationType.Idle);

            primaryWeapon.ShootAnimation = this.LoadWeaponAnimation(primaryWeapon, spriteTexture, WeaponAnimationType.Shoot);
            primaryWeapon.ShootAnimationPosition = this.GetWeaponAnimationPosition(primaryWeapon, WeaponAnimationType.Shoot);

            secondaryWeapon.IdleAnimation = this.LoadWeaponAnimation(secondaryWeapon, spriteTexture, WeaponAnimationType.Idle);
            secondaryWeapon.IdleAnimationPosition = this.GetWeaponAnimationPosition(secondaryWeapon, WeaponAnimationType.Idle);

            secondaryWeapon.ShootAnimation = this.LoadWeaponAnimation(secondaryWeapon, spriteTexture, WeaponAnimationType.Shoot);
            secondaryWeapon.ShootAnimationPosition = this.GetWeaponAnimationPosition(secondaryWeapon, WeaponAnimationType.Shoot);

            // Set animation to play
            this.PlayAnimation(this.IdleAnimation);
            this.PlayWeaponAnimation(primaryWeapon.IdleAnimation);

            // Load sounds.            
            this.KilledSound = this.Level.Content.Load<SoundEffect>(GlobalParameters.GORDO_MERCENARIO_KILLED_SOUND);

            this.Type = new PlayerInfo(PlayerType.GordoMercenario);

            this.KnifeWidth = GlobalParameters.GORDO_MERCENARIO_KNIFE_WIDTH;
            this.KnifeHeight = GlobalParameters.GORDO_MERCENARIO_KNIFE_HEIGHT;

            // Personalizacion de caracteristas del player
            this.MaxAttackTime = 0.9f;
            this.MaxHitTime = 1.0f;
            this.MaxParalyzedTime = 0.375f;
            this.MoveAcceleration = 9750.0f;
        }

        private Vector2 GetWeaponAnimationPosition(Weapon weapon, WeaponAnimationType weaponAnimationType)
        {
            throw new NotImplementedException();
        }

        private Animation LoadWeaponAnimation(Weapon weapon, Texture2D spriteTexture, WeaponAnimationType weaponAnimationType)
        {
            throw new NotImplementedException();

            //Rectangle[] animationRectangle;

            //switch (weapon.Type)
            //{
            //    case WeaponType.SubFusilMp40:
            //        if (weaponAnimationType == WeaponAnimationType.Idle)
            //        {
            //            animationRectangle = GlobalParameters.ObamaAnimationRectangulesSubFusilMp40Idle;
            //        }
            //        else if (weaponAnimationType == WeaponAnimationType.Shoot)
            //        {
            //            animationRectangle = GlobalParameters.ObamaAnimationRectangulesSubFusilMp40Idle;
            //        }
            //        else
            //        {
            //            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    default:
            //        throw new Exception(string.Format("Arma {0} no soportada", weapon.Type));
            //}

            //return new Animation(spriteTexture, 0.075f, true, animationRectangle);
        }
    }
}

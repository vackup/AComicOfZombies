using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Platformer.Animations;
using Platformer.Helpers;
using Platformer.Levels;
using Platformer.Weapons;

namespace Platformer.Players
{
    public class GordoMercenario : Player
    {
        public GordoMercenario(Level level, Vector2 position, List<Weapon> weaponInventory, Weapon primaryWeapon, Weapon secondaryWeapon, Dictionary<int, int> ammoInventory) 
            : base(level, position, weaponInventory, primaryWeapon, secondaryWeapon, ammoInventory)
        {
			//CurrentWeaponAnimation = new GameObject(Level.Content.Load<Texture2D>(GlobalParameters.GORDO_MERCENARIO_ARM));

            HealthBarMargin = GlobalParameters.GORDO_MERCENARIO_HEALTH_BAR_MARGIN;

            var spriteTexture = Level.Content.Load<Texture2D>(GlobalParameters.GORDO_MERCENARIO_ANIMATION_TEXTURE);

            IdleAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.GordoAnimationRectangulesIdle);

            RunAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.GordoAnimationRectangulesRun);
            RunAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            DieAnimation = new Animation(spriteTexture, 0.075f, false, GlobalParameters.GordoAnimationRectangulesDie);
            DieAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            BeAttackedAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.GordoAnimationRectangulesBeAttacked);
            BeAttackedAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            AttackAnimation = new Animation(spriteTexture, 0.075f, false, GlobalParameters.GordoAnimationRectangulesAttack);
            AttackAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            CelebrateAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.GordoAnimationRectangulesCelebrate);
            CelebrateAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            primaryWeapon.IdleAnimation = LoadWeaponAnimation(primaryWeapon, spriteTexture, WeaponAnimationType.Idle);
            primaryWeapon.IdleAnimationPosition = GetWeaponAnimationPosition(primaryWeapon, WeaponAnimationType.Idle);

            primaryWeapon.ShootAnimation = LoadWeaponAnimation(primaryWeapon, spriteTexture, WeaponAnimationType.Shoot);
            primaryWeapon.ShootAnimationPosition = GetWeaponAnimationPosition(primaryWeapon, WeaponAnimationType.Shoot);

            secondaryWeapon.IdleAnimation = LoadWeaponAnimation(secondaryWeapon, spriteTexture, WeaponAnimationType.Idle);
            secondaryWeapon.IdleAnimationPosition = GetWeaponAnimationPosition(secondaryWeapon, WeaponAnimationType.Idle);

            secondaryWeapon.ShootAnimation = LoadWeaponAnimation(secondaryWeapon, spriteTexture, WeaponAnimationType.Shoot);
            secondaryWeapon.ShootAnimationPosition = GetWeaponAnimationPosition(secondaryWeapon, WeaponAnimationType.Shoot);

            // Set animation to play
            PlayAnimation(IdleAnimation);
            PlayWeaponAnimation(primaryWeapon.IdleAnimation);

            // Load sounds.            
            KilledSound = Level.Content.Load<SoundEffect>(GlobalParameters.GORDO_MERCENARIO_KILLED_SOUND);

            Type = new PlayerInfo(PlayerType.GordoMercenario);

            KnifeWidth = GlobalParameters.GORDO_MERCENARIO_KNIFE_WIDTH;
            KnifeHeight = GlobalParameters.GORDO_MERCENARIO_KNIFE_HEIGHT;

            // Personalizacion de caracteristas del player
            MaxAttackTime = 0.9f;
            MaxHitTime = 1.0f;
            MaxParalyzedTime = 0.375f;
            MoveAcceleration = 9750.0f;
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

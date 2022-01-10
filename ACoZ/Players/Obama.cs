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
    public class Obama : Player
    {
        public Obama(Level level, Vector2 position, List<Weapon> weaponInventory, Weapon primaryWeapon, Weapon secondaryWeapon, Dictionary<int, int> ammoInventory)
            : base(level, position, weaponInventory, primaryWeapon, secondaryWeapon, ammoInventory)
        {
            HealthBarMargin = GlobalParameters.OBAMA_HEALTH_BAR_MARGIN;

            var spriteTexture = Level.Content.Load<Texture2D>(GlobalParameters.OBAMA_ANIMATION_TEXTURE);

            IdleAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.ObamaAnimationRectangulesIdle);

            RunAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.ObamaAnimationRectangulesRun);
            RunAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            DieAnimation = new Animation(spriteTexture, 0.075f, false, GlobalParameters.ObamaAnimationRectangulesDie);
            DieAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            BeAttackedAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.ObamaAnimationRectangulesBeAttacked);
            BeAttackedAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            // TODO: parametrizar todos los datos en un archivo de configuracion
            AttackAnimation = new Animation(spriteTexture, 0.05f, false, GlobalParameters.ObamaAnimationRectangulesAttack, GlobalParameters.ObamaAnimationRectangulesAttack[0].Width, GlobalParameters.OBAMA_ATTACK_ANIMATION_HEIGHT);
            AttackAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            CelebrateAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.ObamaAnimationRectangulesCelebrate);
            CelebrateAnimation.SetBoundingRectangle(IdleAnimation.BoundingRectangle);

            if (primaryWeapon != null)
            {
                primaryWeapon.IdleAnimation = LoadWeaponAnimation(primaryWeapon, spriteTexture, WeaponAnimationType.Idle);
                primaryWeapon.IdleAnimationPosition = GetWeaponAnimationPosition(primaryWeapon, WeaponAnimationType.Idle);
                primaryWeapon.IdleAnimationRunningPosition = GetWeaponRunningAnimationPosition(primaryWeapon, WeaponAnimationType.Idle);

                primaryWeapon.ShootAnimation = LoadWeaponAnimation(primaryWeapon, spriteTexture, WeaponAnimationType.Shoot);
                primaryWeapon.ShootAnimationPosition = GetWeaponAnimationPosition(primaryWeapon, WeaponAnimationType.Shoot);
                primaryWeapon.ShootAnimationRunningPosition = GetWeaponRunningAnimationPosition(primaryWeapon, WeaponAnimationType.Shoot);

                PlayWeaponAnimation(primaryWeapon.IdleAnimation);
            }

            if (secondaryWeapon != null)
            {
                secondaryWeapon.IdleAnimation = LoadWeaponAnimation(secondaryWeapon, spriteTexture, WeaponAnimationType.Idle);
                secondaryWeapon.IdleAnimationPosition = GetWeaponAnimationPosition(secondaryWeapon, WeaponAnimationType.Idle);
                secondaryWeapon.IdleAnimationRunningPosition = GetWeaponRunningAnimationPosition(secondaryWeapon, WeaponAnimationType.Idle);

                secondaryWeapon.ShootAnimation = LoadWeaponAnimation(secondaryWeapon, spriteTexture, WeaponAnimationType.Shoot);
                secondaryWeapon.ShootAnimationPosition = GetWeaponAnimationPosition(secondaryWeapon, WeaponAnimationType.Shoot);
                secondaryWeapon.ShootAnimationRunningPosition = GetWeaponRunningAnimationPosition(secondaryWeapon, WeaponAnimationType.Shoot);
            }


            // Set animation to play
            PlayAnimation(IdleAnimation);
            
            // Load sounds.            
            KilledSound = Level.Content.Load<SoundEffect>(GlobalParameters.OBAMA_KILLED_SOUND);

            Type = new PlayerInfo(PlayerType.Obama);

            KnifeWidth = GlobalParameters.OBAMA_KNIFE_WIDTH;
            KnifeHeight = GlobalParameters.OBAMA_KNIFE_HEIGHT;

            // Personalizacion de caracteristas del player
            MaxAttackTime = 0.47f;
            MaxHitTime = 0.75f;
            //MaxParalyzedTime = 0.375f;
            MaxParalyzedTime = 0.5f;
            MoveAcceleration = 13000.0f;
        }

        private Vector2 GetWeaponRunningAnimationPosition(Weapon weapon, WeaponAnimationType weaponAnimationType)
        {
            return GlobalParameters.ObamaWeaponAnimationPosition[string.Format("{0}_{1}_{2}", weapon.Type, weaponAnimationType, "Run")];
        }

        private Vector2 GetWeaponAnimationPosition(Weapon weapon, WeaponAnimationType weaponAnimationType)
        {
            var key = string.Format("{0}_{1}", weapon.Type, weaponAnimationType);

            if (!GlobalParameters.ObamaWeaponAnimationPosition.ContainsKey(key))
                throw new Exception(string.Format("No se encuentra la animacion de arma con el key {0}", key));

                return GlobalParameters.ObamaWeaponAnimationPosition[key];

            //switch (weapon.Type)
            //{
            //    case WeaponType.PistolaBerettaM9:
            //        switch (weaponAnimationType)
            //        {
            //            case WeaponAnimationType.Idle:
            //                weaponAnimationPosition = GlobalParameters.ObamaWeaponIdleAnimationPosition;
            //                break;
            //            case WeaponAnimationType.Shoot:
            //                weaponAnimationPosition = new Vector2(GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_X,
            //                                   GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_Y);
            //                break;
            //            default:
            //                throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    case WeaponType.PistolaColtM1911:
            //        switch (weaponAnimationType)
            //        {
            //            case WeaponAnimationType.Idle:
            //                weaponAnimationPosition = GlobalParameters.ObamaWeaponIdleAnimationPosition;
            //                break;
            //            case WeaponAnimationType.Shoot:
            //                weaponAnimationPosition = new Vector2(GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_X,
            //                                   GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_Y);
            //                break;
            //            default:
            //                throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    case WeaponType.PistolaColtPython:
            //        switch (weaponAnimationType)
            //        {
            //            case WeaponAnimationType.Idle:
            //                weaponAnimationPosition = GlobalParameters.ObamaWeaponIdleAnimationPosition;
            //                break;
            //            case WeaponAnimationType.Shoot:
            //                weaponAnimationPosition = new Vector2(GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_X,
            //                                   GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_Y);
            //                break;
            //            default:
            //                throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    case WeaponType.EscopetaBeretta682:
            //        switch (weaponAnimationType)
            //        {
            //            case WeaponAnimationType.Idle:
            //                weaponAnimationPosition = GlobalParameters.ObamaWeaponIdleAnimationPosition;
            //                break;
            //            case WeaponAnimationType.Shoot:
            //                weaponAnimationPosition = new Vector2(GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_X,
            //                                   GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_Y);
            //                break;
            //            default:
            //                throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    case WeaponType.EscopetaIthaca37:
            //        switch (weaponAnimationType)
            //        {
            //            case WeaponAnimationType.Idle:
            //                weaponAnimationPosition = GlobalParameters.ObamaWeaponIdleAnimationPosition;
            //                break;
            //            case WeaponAnimationType.Shoot:
            //                weaponAnimationPosition = new Vector2(GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_X,
            //                                   GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_Y);
            //                break;
            //            default:
            //                throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    case WeaponType.EscopetaSpas12:
            //        switch (weaponAnimationType)
            //        {
            //            case WeaponAnimationType.Idle:
            //                weaponAnimationPosition = GlobalParameters.ObamaWeaponIdleAnimationPosition;
            //                break;
            //            case WeaponAnimationType.Shoot:
            //                weaponAnimationPosition = new Vector2(GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_X,
            //                                   GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_Y);
            //                break;
            //            default:
            //                throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    case WeaponType.SubFusilMp5K:
            //        switch (weaponAnimationType)
            //        {
            //            case WeaponAnimationType.Idle:
            //                weaponAnimationPosition = GlobalParameters.ObamaWeaponIdleAnimationPosition;
            //                break;
            //            case WeaponAnimationType.Shoot:
            //                weaponAnimationPosition = new Vector2(GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_X,
            //                                   GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_Y);
            //                break;
            //            default:
            //                throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    case WeaponType.SubFusilMp40:
            //        switch (weaponAnimationType)
            //        {
            //            case WeaponAnimationType.Idle:
            //                weaponAnimationPosition = GlobalParameters.ObamaWeaponIdleAnimationPosition;
            //                break;
            //            case WeaponAnimationType.Shoot:
            //                weaponAnimationPosition = new Vector2(GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_X,
            //                                   GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_Y);
            //                break;
            //            default:
            //                throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    case WeaponType.SubFusilUzi:
            //        switch (weaponAnimationType)
            //        {
            //            case WeaponAnimationType.Idle:
            //                weaponAnimationPosition = GlobalParameters.ObamaWeaponIdleAnimationPosition;
            //                break;
            //            case WeaponAnimationType.Shoot:
            //                weaponAnimationPosition = new Vector2(GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_X,
            //                                   GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_Y);
            //                break;
            //            default:
            //                throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    case WeaponType.FusilAk47:
            //        switch (weaponAnimationType)
            //        {
            //            case WeaponAnimationType.Idle:
            //                weaponAnimationPosition = GlobalParameters.ObamaWeaponIdleAnimationPosition;
            //                break;
            //            case WeaponAnimationType.Shoot:
            //                weaponAnimationPosition = new Vector2(GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_X,
            //                                   GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_Y);
            //                break;
            //            default:
            //                throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    case WeaponType.FusilM4A1:
            //        switch (weaponAnimationType)
            //        {
            //            case WeaponAnimationType.Idle:
            //                weaponAnimationPosition = GlobalParameters.ObamaWeaponIdleAnimationPosition;
            //                break;
            //            case WeaponAnimationType.Shoot:
            //                weaponAnimationPosition = new Vector2(GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_X,
            //                                   GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_Y);
            //                break;
            //            default:
            //                throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    case WeaponType.FusilXm8:
            //        switch (weaponAnimationType)
            //        {
            //            case WeaponAnimationType.Idle:
            //                weaponAnimationPosition = GlobalParameters.ObamaWeaponIdleAnimationPosition;
            //                break;
            //            case WeaponAnimationType.Shoot:
            //                weaponAnimationPosition = new Vector2(GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_X,
            //                                   GlobalParameters.OBAMA_WEAPON_SHOOT_ANIMATION_POSITION_Y);
            //                break;
            //            default:
            //                throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
            //        }
            //        break;
            //    default:
            //        throw new Exception(string.Format("Arma {0} no soportada", weapon.Type));
            //}

            //return weaponAnimationPosition;
        }

        private Animation LoadWeaponAnimation(Weapon weapon, Texture2D spriteTexture, WeaponAnimationType weaponAnimationType)
        {
            // TODO: pasar a archivo con parametrizacion
            Rectangle[] animationRectangle;

            switch (weapon.Type)
            {
                case WeaponType.PistolaBerettaM9:
                    switch (weaponAnimationType)
                    {
                        case WeaponAnimationType.Idle:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesPistolaBerettaM9Idle;
                            break;
                        case WeaponAnimationType.Shoot:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesPistolaBerettaM9Shoot;
                            break;
                        default:
                            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
                    }
                    break;
                case WeaponType.PistolaColtM1911:
                    switch (weaponAnimationType)
                    {
                        case WeaponAnimationType.Idle:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesPistolaColtM1911Idle;
                            break;
                        case WeaponAnimationType.Shoot:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesPistolaColtM1911Shoot;
                            break;
                        default:
                            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
                    }
                    break;
                case WeaponType.PistolaColtPython:
                    switch (weaponAnimationType)
                    {
                        case WeaponAnimationType.Idle:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesPistolaColtPythonIdle;
                            break;
                        case WeaponAnimationType.Shoot:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesPistolaColtPythonShoot;
                            break;
                        default:
                            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
                    }
                    break;
                case WeaponType.EscopetaBeretta682:
                    switch (weaponAnimationType)
                    {
                        case WeaponAnimationType.Idle:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesEscopetaBeretta682Idle;
                            break;
                        case WeaponAnimationType.Shoot:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesEscopetaBeretta682Shoot;
                            break;
                        default:
                            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
                    }
                    break;
                case WeaponType.EscopetaIthaca37:
                    switch (weaponAnimationType)
                    {
                        case WeaponAnimationType.Idle:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesEscopetaIthaca37Idle;
                            break;
                        case WeaponAnimationType.Shoot:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesEscopetaIthaca37Shoot;
                            break;
                        default:
                            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
                    }
                    break;
                case WeaponType.EscopetaSpas12:
                    switch (weaponAnimationType)
                    {
                        case WeaponAnimationType.Idle:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesEscopetaSpas12Idle;
                            break;
                        case WeaponAnimationType.Shoot:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesEscopetaSpas12Shoot;
                            break;
                        default:
                            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
                    }
                    break;
                case WeaponType.SubFusilMp5K:
                    switch (weaponAnimationType)
                    {
                        case WeaponAnimationType.Idle:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesSubFusilMp5KIdle;
                            break;
                        case WeaponAnimationType.Shoot:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesSubFusilMp5KShoot;
                            break;
                        default:
                            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
                    }
                    break;
                case WeaponType.SubFusilMp40:
                    switch (weaponAnimationType)
                    {
                        case WeaponAnimationType.Idle:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesSubFusilMp40Idle;
                            break;
                        case WeaponAnimationType.Shoot:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesSubFusilMp40Shoot;
                            break;
                        default:
                            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
                    }
                    break;
                case WeaponType.SubFusilUzi:
                    switch (weaponAnimationType)
                    {
                        case WeaponAnimationType.Idle:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesSubFusilUziIdle;
                            break;
                        case WeaponAnimationType.Shoot:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesSubFusilUziShoot;
                            break;
                        default:
                            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
                    }
                    break;
                case WeaponType.FusilAk47:
                    switch (weaponAnimationType)
                    {
                        case WeaponAnimationType.Idle:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesFusilAk47Idle;
                            break;
                        case WeaponAnimationType.Shoot:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesFusilAk47Shoot;
                            break;
                        default:
                            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
                    }
                    break;
                case WeaponType.FusilM4A1:
                    switch (weaponAnimationType)
                    {
                        case WeaponAnimationType.Idle:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesFusilM4A1Idle;
                            break;
                        case WeaponAnimationType.Shoot:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesFusilM4A1Shoot;
                            break;
                        default:
                            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
                    }
                    break;
                case WeaponType.FusilXm8:
                    switch (weaponAnimationType)
                    {
                        case WeaponAnimationType.Idle:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesFusilXm8Idle;
                            break;
                        case WeaponAnimationType.Shoot:
                            animationRectangle = GlobalParameters.ObamaAnimationRectangulesFusilXm8Shoot;
                            break;
                        default:
                            throw new Exception(string.Format("WeaponAnimationType {0} para el arma {1} no soportada", weaponAnimationType, weapon.Type));
                    }
                    break;
                default:
                    throw new Exception(string.Format("Arma {0} no soportada", weapon.Type));
            }
            
            return new Animation(spriteTexture, 0.075f, weapon.IsAutomatic, animationRectangle);
        }
    }
}

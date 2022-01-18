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
    public class Obama : Player
    {
        public Obama(Level level, Vector2 position, List<Weapon> weaponInventory, Weapon primaryWeapon, Weapon secondaryWeapon, Dictionary<int, int> ammoInventory)
            : base(level, position, weaponInventory, primaryWeapon, secondaryWeapon, ammoInventory)
        {
            this.HealthBarMargin = GlobalParameters.OBAMA_HEALTH_BAR_MARGIN;

            var spriteTexture = this.Level.Content.Load<Texture2D>(GlobalParameters.OBAMA_ANIMATION_TEXTURE);

            this.IdleAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.ObamaAnimationRectangulesIdle);

            this.RunAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.ObamaAnimationRectangulesRun);
            this.RunAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            this.DieAnimation = new Animation(spriteTexture, 0.075f, false, GlobalParameters.ObamaAnimationRectangulesDie);
            this.DieAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            this.BeAttackedAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.ObamaAnimationRectangulesBeAttacked);
            this.BeAttackedAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            // TODO: parametrizar todos los datos en un archivo de configuracion
            this.AttackAnimation = new Animation(spriteTexture, 0.05f, false, GlobalParameters.ObamaAnimationRectangulesAttack, GlobalParameters.ObamaAnimationRectangulesAttack[0].Width, GlobalParameters.OBAMA_ATTACK_ANIMATION_HEIGHT);
            this.AttackAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            this.CelebrateAnimation = new Animation(spriteTexture, 0.075f, true, GlobalParameters.ObamaAnimationRectangulesCelebrate);
            this.CelebrateAnimation.SetBoundingRectangle(this.IdleAnimation.BoundingRectangle);

            if (primaryWeapon != null)
            {
                primaryWeapon.IdleAnimation = this.LoadWeaponAnimation(primaryWeapon, spriteTexture, WeaponAnimationType.Idle);
                primaryWeapon.IdleAnimationPosition = this.GetWeaponAnimationPosition(primaryWeapon, WeaponAnimationType.Idle);
                primaryWeapon.IdleAnimationRunningPosition = this.GetWeaponRunningAnimationPosition(primaryWeapon, WeaponAnimationType.Idle);

                primaryWeapon.ShootAnimation = this.LoadWeaponAnimation(primaryWeapon, spriteTexture, WeaponAnimationType.Shoot);
                primaryWeapon.ShootAnimationPosition = this.GetWeaponAnimationPosition(primaryWeapon, WeaponAnimationType.Shoot);
                primaryWeapon.ShootAnimationRunningPosition = this.GetWeaponRunningAnimationPosition(primaryWeapon, WeaponAnimationType.Shoot);

                this.PlayWeaponAnimation(primaryWeapon.IdleAnimation);
            }

            if (secondaryWeapon != null)
            {
                secondaryWeapon.IdleAnimation = this.LoadWeaponAnimation(secondaryWeapon, spriteTexture, WeaponAnimationType.Idle);
                secondaryWeapon.IdleAnimationPosition = this.GetWeaponAnimationPosition(secondaryWeapon, WeaponAnimationType.Idle);
                secondaryWeapon.IdleAnimationRunningPosition = this.GetWeaponRunningAnimationPosition(secondaryWeapon, WeaponAnimationType.Idle);

                secondaryWeapon.ShootAnimation = this.LoadWeaponAnimation(secondaryWeapon, spriteTexture, WeaponAnimationType.Shoot);
                secondaryWeapon.ShootAnimationPosition = this.GetWeaponAnimationPosition(secondaryWeapon, WeaponAnimationType.Shoot);
                secondaryWeapon.ShootAnimationRunningPosition = this.GetWeaponRunningAnimationPosition(secondaryWeapon, WeaponAnimationType.Shoot);
            }


            // Set animation to play
            this.PlayAnimation(this.IdleAnimation);
            
            // Load sounds.            
            this.KilledSound = this.Level.Content.Load<SoundEffect>(GlobalParameters.OBAMA_KILLED_SOUND);

            this.Type = new PlayerInfo(PlayerType.Obama);

            this.KnifeWidth = GlobalParameters.OBAMA_KNIFE_WIDTH;
            this.KnifeHeight = GlobalParameters.OBAMA_KNIFE_HEIGHT;

            // Personalizacion de caracteristas del player
            this.MaxAttackTime = 0.47f;
            this.MaxHitTime = 0.75f;
            //MaxParalyzedTime = 0.375f;
            this.MaxParalyzedTime = 0.5f;
            this.MoveAcceleration = 13000.0f;
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

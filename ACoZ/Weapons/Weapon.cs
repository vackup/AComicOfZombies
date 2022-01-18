using System;
using ACoZ.Animations;
using ACoZ.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ACoZ.Weapons
{
    public class Weapon
    {
        private float _totalSecondsBulletFire;
        //private float _remaingReloadTime;
        private float _remaingReloadTimePerBullet;
        private bool _reloadSoundPlayed;
        private int _ammoToLoad;
        private float _shootingTime;
        private const float MAX_SHOOTING_TIME = 0.15f;

        public bool IsReloading { get; private set; }
        
        /// <summary>
        /// Maxima cantidad de municiones que carga un arma
        /// </summary>
        public int MaxAmmo { get; private set; }

        public int CurrentAmmo { get; private set; }
        public WeaponType Type { get; private set; }
        public WeaponPosition Position { get; private set; }
        
        public Animation IdleAnimation { get; set; }
        public Animation ShootAnimation { get; set; }
        public Vector2 IdleAnimationPosition { get; set; }
        public Vector2 ShootAnimationPosition { get; set; }
        public Vector2 ShootAnimationRunningPosition { get; set; }
        public Vector2 IdleAnimationRunningPosition { get; set; }

        public GameObject GameData { get; private set; }
        
        /// <summary>
        /// Daño que causara en los enemigos
        /// </summary>
        public int Power { get; private set; }

        /// <summary>
        /// Velocidad a la que se mueve la bala en la pantalla
        /// </summary>
        public float BulletVelocity { get; private set; }

        /// <summary>
        /// Fire a new bullet every "FireRate" seconds
        /// </summary>
        public float FireRate { get; private set; }

        /// <summary>
        /// Tiempo que tarda el arma en recargarse
        /// </summary>
        public float ReloadTime { get; private set; }

        /// <summary>
        /// Especifica si el tiempo de carga es x bala o total (util para armas que no tienen cargador)
        /// </summary>
        public bool IsReloadTimePerBullet { get; private set; }

        /// <summary>
        /// Costo del arma para comprarla
        /// </summary>
        public int ScoreValue { get; private set; }

        /// <summary>
        /// Costo de las municiones para comprarla
        /// </summary>
        public int AmmoScoreValue { get; private set; }

        /// <summary>
        /// Especifica si el arma es automatica (dispara con el gatillo presionado) o no (hay que presionar y levantar el gatillo)
        /// </summary>
        public bool IsAutomatic { get; private set; }

        /// <summary>
        /// Especifica cuantas ammmo se compran en el store de armas
        /// </summary>
        public int AmmoPack { get; private set; }

        public WeaponState State { get; set; }

        public SoundEffect FireSoundEffect { get; private set; }
        public SoundEffect ReloadSoundEffect { get; private set; }
        public SoundEffect EmptySoundEffect { get; private set; }

        public string Name { get; private set; }

        public bool HasAmmo { get { return this.CurrentAmmo > 0; } }

        public Weapon(WeaponType weaponType)
        {
            this.CanShoot = true;

            this.Type = weaponType;

            this.GameData = new GameObject();

            // Todas las pistolas tienen el mismo poder, difieren en la cantidad de balas que cargan (MaxAmmo) y en el fire rate (FireRate). Cargan x bala
            // Las escopetas son las que mayor tienen, pero son las lentas (FireRate) y cargan menos balas (MaxAmmo). Cargan x bala
            // Los subfusiles tienen el mismo poder que las pistolas. Son automaticos.
            // Los fusiles tienen el mayor poder que los subfusiles pero menos que las escopetas. Son automaticos.
            switch (this.Type)
            {
                case WeaponType.PistolaColtPython:
                    this.Position = WeaponPosition.Primary;
                    this.Power = 1;
                    this.BulletVelocity = 15.0f;
                    this.FireRate = 0.75f;
                    this.ScoreValue = 100;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_RELOAD_SOUND);
                    this.MaxAmmo = 6;
                    this.CurrentAmmo = this.MaxAmmo;
                    this.Name = "Colt Python";
                    this.IsReloadTimePerBullet = true;
                    this.ReloadTime = 1.00f;
                    this.IsAutomatic = false;
                    break;
                case WeaponType.PistolaColtM1911:
                    this.Position = WeaponPosition.Primary;    
                    this.Power = 1;
                    this.BulletVelocity = 15.0f;
                    this.FireRate = 0.65f;
                    this.ScoreValue = 150;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(Assets.PISTOLA_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_RELOAD_SOUND);
                    this.MaxAmmo = 7;
                    this.CurrentAmmo = this.MaxAmmo;
                    this.Name = "Colt M1911";
                    this.IsReloadTimePerBullet = true;
                    this.ReloadTime = 1.00f;
                    this.IsAutomatic = false;
                    break;
                case WeaponType.PistolaBerettaM9:
                    this.Position = WeaponPosition.Primary;
                    this.Power = 1;
                    this.BulletVelocity = 15.0f;
                    this.FireRate = 0.55f;
                    this.ScoreValue = 200;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_RELOAD_SOUND);
                    this.MaxAmmo = 15;
                    this.CurrentAmmo = this.MaxAmmo;
                    this.Name = "Beretta M9";
                    this.IsReloadTimePerBullet = true;
                    this.ReloadTime = 1.00f;
                    this.IsAutomatic = false;
                    break;   
                case WeaponType.EscopetaBeretta682:
                    this.Position = WeaponPosition.Secondary;
                    this.Power = 3;
                    this.BulletVelocity = 15.0f;
                    this.FireRate = 1.25f;
                    this.ScoreValue = 400;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_RELOAD_SOUND);
                    this.MaxAmmo = 2;
                    this.CurrentAmmo = this.MaxAmmo;
                    this.Name = "Beretta 682";
                    this.IsReloadTimePerBullet = true;
                    this.ReloadTime = 1.00f;
                    this.IsAutomatic = false;
                    break;
                case WeaponType.EscopetaIthaca37:
                    this.Position = WeaponPosition.Secondary;
                    this.Power = 3;
                    this.BulletVelocity = 15.0f;
                    this.FireRate = 1.25f;
                    this.ScoreValue = 600;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_RELOAD_SOUND);
                    this.MaxAmmo = 4;
                    this.CurrentAmmo = this.MaxAmmo;
                    this.Name = "Ithaca 37";
                    this.IsReloadTimePerBullet = true;
                    this.ReloadTime = 1.00f;
                    this.IsAutomatic = false;
                    break;
                case WeaponType.EscopetaSpas12:
                    this.Position = WeaponPosition.Secondary;
                    this.Power = 3;
                    this.BulletVelocity = 15.0f;
                    this.FireRate = 1.25f;
                    this.ScoreValue = 800;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_RELOAD_SOUND);
                    this.MaxAmmo = 8;
                    this.CurrentAmmo = this.MaxAmmo;
                    this.Name = "Spas 12";
                    this.IsReloadTimePerBullet = true;
                    this.ReloadTime = 1.00f;
                    this.IsAutomatic = false;
                    break;
                case WeaponType.SubFusilUzi:
                    this.Position = WeaponPosition.Primary;
                    this.Power = 1;
                    this.BulletVelocity = 15.0f;
                    this.FireRate = 0.1f;
                    this.ScoreValue = 1000;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_RELOAD_SOUND);
                    this.MaxAmmo = 20;
                    this.CurrentAmmo = this.MaxAmmo;
                    this.Name = "Uzi";
                    this.IsReloadTimePerBullet = false;
                    this.ReloadTime = 2.00f;
                    this.IsAutomatic = true;
                    break;
                case WeaponType.SubFusilMp5K:
                    this.Position = WeaponPosition.Primary;
                    this.Power = 1;
                    this.BulletVelocity = 15.0f;
                    this.FireRate = 0.1f;
                    this.ScoreValue = 1250;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_RELOAD_SOUND);
                    this.MaxAmmo = 30;
                    this.CurrentAmmo = this.MaxAmmo;
                    this.Name = "MP 5 K";
                    this.IsReloadTimePerBullet = false;
                    this.ReloadTime = 2.00f;
                    this.IsAutomatic = true;
                    break;
                case WeaponType.SubFusilMp40:
                    this.Position = WeaponPosition.Primary;
                    this.Power = 1;
                    this.BulletVelocity = 15.0f;
                    this.FireRate = 0.1f;
                    this.ScoreValue = 1500;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_RELOAD_SOUND);
                    this.MaxAmmo = 40;
                    this.CurrentAmmo = this.MaxAmmo;
                    this.Name = "MP 40";
                    this.IsReloadTimePerBullet = false;
                    this.ReloadTime = 2.00f;
                    this.IsAutomatic = true;
                    break;
                case WeaponType.FusilAk47:
                    this.Position = WeaponPosition.Secondary;
                    this.Power = 2;
                    this.BulletVelocity = 15.0f;
                    this.FireRate = 0.05f;
                    this.ScoreValue = 2000;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_RELOAD_SOUND);
                    this.MaxAmmo = 30;
                    this.CurrentAmmo = this.MaxAmmo;
                    this.Name = "AK 47";
                    this.IsReloadTimePerBullet = false;
                    this.ReloadTime = 1.00f;
                    this.IsAutomatic = true;
                    break;
                case WeaponType.FusilM4A1:
                    this.Position = WeaponPosition.Secondary;
                    this.Power = 2;
                    this.BulletVelocity = 15.0f;
                    this.FireRate = 0.05f;
                    this.ScoreValue = 2500;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_RELOAD_SOUND);
                    this.MaxAmmo = 40;
                    this.CurrentAmmo = this.MaxAmmo;
                    this.Name = "M4A1";
                    this.IsReloadTimePerBullet = false;
                    this.ReloadTime = 1.00f;
                    this.IsAutomatic = true;
                    break;
                case WeaponType.FusilXm8:
                    this.Position = WeaponPosition.Secondary;
                    this.Power = 5;
                    this.BulletVelocity = 15.0f;
                    this.FireRate = 0.05f;
                    this.ScoreValue = 3000;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_RELOAD_SOUND);
                    this.MaxAmmo = 50;
                    this.CurrentAmmo = this.MaxAmmo;
                    this.Name = "XM 8";
                    this.IsReloadTimePerBullet = false;
                    this.ReloadTime = 1.00f;
                    this.IsAutomatic = true;
                    break;
                default:
                    throw new Exception("Wrong gun");
            }

            //EmptySoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.WEAPON_EMPTY_SOUND);

            if (this.Name == string.Empty)
                throw new Exception(string.Format("Weapon name not set for type {0}", this.Type));

            this.AmmoPack = this.MaxAmmo*3;
            this.AmmoScoreValue = (int)(this.ScoreValue * 0.2);
        }

        //public void Fire(GameTime gameTime, Pool<Bullet> bulletsPool, SpriteEffects flip, GameObject playerArm)
        public void Fire(GameTime gameTime, Pool<Bullet> bulletsPool, SpriteEffects flip)
        {
            if (this.IsReloading)
                throw new Exception("No se puede disparar el arma mientras esta siendo cargada.");

            // Si no quedan mas balas en el arma, entonces que ejecute el sonido de vacio y retorne
            if (this.CurrentAmmo <= 0)
            {
                this.EmptySoundEffect.Play();
                return;
            }

            // Actualiza el tiempo de disparo para la animacion
            this.IsShooting = true;            

            // Fire a new bullet enemy every X seconds
            // Si la cantidad de segundos desde el ultimo disparo (totalSeconds - _previousTotalSecondsBulletFire) es < que el tiempo de disparo (FireRate),
            // entonces que no haga nada.
            //var actualTotalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
            //if (actualTotalSeconds - _previousTotalSecondsBulletFire < FireRate) return;
            //_previousTotalSecondsBulletFire = actualTotalSeconds;
            if (!this.CanShoot) return;

            this.CanShoot = false;
            //_totalSecondsBulletFire = (float)gameTime.TotalGameTime.TotalSeconds;
            this._totalSecondsBulletFire = 0.0f;

            // Si no existen balas disponibles en el Pool de balas, entonces que retorne
            if (bulletsPool.AvailableCount <= 0) return;

            // Get a bullet from the pool.
            var bullet = bulletsPool.Get().Item;

            //And set it to alive.
            //bullet.Alive = true;
            bullet.Power = this.Power;

            var armCos = (float)Math.Cos(this.GameData.Rotation);
            var armSin = (float)Math.Sin(this.GameData.Rotation); 

            if (flip == SpriteEffects.FlipHorizontally) //Facing right
            {
                //var armCos = (float) Math.Cos(playerArm.Rotation); // - MathHelper.PiOver2);
                //var armSin = (float) Math.Sin(playerArm.Rotation); // - MathHelper.PiOver2);

                //Set the initial position of our bullet at the end of our gun arm
                //42 is obtained be taking the width of the Arm_Gun texture / 2
                //and subtracting the width of the Bullet texture / 2. ((96/2)-(12/2))
                //bullet.Position = new Vector2(
                //    playerArm.Position.X + 42*armCos,
                //    playerArm.Position.Y + 42*armSin);

                bullet.Position = new Vector2(
                    this.GameData.Position.X + 42 * armCos,
                    this.GameData.Position.Y + 42 * armSin);

                //And give it a velocity of the direction we're aiming.
                //Increase/decrease speed by changing 15.0f
                //bullet.velocity = new Vector2(
                //    (float)Math.Cos(_arm.rotation - MathHelper.PiOver2),
                //    (float)Math.Sin(_arm.rotation - MathHelper.PiOver2)) * 15.0f;
                bullet.Velocity = new Vector2(
                                      armCos,
                                      armSin)*this.BulletVelocity;
            }
            else //Facing left
            {
                //var armCos = (float) Math.Cos(playerArm.Rotation); // + MathHelper.PiOver2);
                //var armSin = (float) Math.Sin(playerArm.Rotation); // + MathHelper.PiOver2);

                //Set the initial position of our bullet at the end of our gun arm
                //42 is obtained be taking the width of the Arm_Gun texture / 2
                //and subtracting the width of the Bullet texture / 2. ((96/2)-(12/2))
                //bullet.Position = new Vector2(
                //    playerArm.Position.X - 42*armCos,
                //    playerArm.Position.Y - 42*armSin);
                bullet.Position = new Vector2(
                   this.GameData.Position.X - 42 * armCos,
                   this.GameData.Position.Y - 42 * armSin);

                //And give it a velocity of the direction we're aiming.
                //Increase/decrease speed by changing 15.0f
                bullet.Velocity = new Vector2(
                                      -armCos,
                                      -armSin)*this.BulletVelocity;
            }

            this.FireSoundEffect.Play();

            this.CurrentAmmo--;
        }

        /// <summary>
        /// Indica si esta disparando una bala
        /// </summary>
        public bool CanShoot { get; private set; }

        /// <summary>
        /// Cargar el arma
        /// </summary>
        /// <param name="availableAmmo">Municiones que tenemos disponibles para cargar</param>
        /// <returns>Municiones restantes, es decir, la diferencia entre las municiones que teniamos disponibles y las que el arma necesitaba reponer</returns>
        public int Reload(int availableAmmo)
        {
            if (this.IsReloading)
                throw new Exception("No se puede cargar el arma mientras esta siendo cargada.");

            this.IsReloading = true;
            this._reloadSoundPlayed = false;

            // Calculamos las municiones que hay que cargar al arma
            var weaponAmmoSpace = this.MaxAmmo - this.CurrentAmmo;

            int remainingAmmo;

            // Si las municiones disponibles son mayores a las municiones a cargar,
            // entonces cargamos las municiones a cargar y devolvemos las municiones restantes que no se usaron
            // sino solo cargamos las municiones que tenemos disponibles
            if (availableAmmo > weaponAmmoSpace)
            {
                this._ammoToLoad = weaponAmmoSpace;
                remainingAmmo = availableAmmo - weaponAmmoSpace;
            }
            else
            {
                this._ammoToLoad = availableAmmo;
                remainingAmmo = 0;
            }

            // Seteamos el tiempo de carga remanente
            //_remaingReloadTime = IsReloadTimePerBullet ? (ReloadTime * _ammoToLoad) + ReloadTime : ReloadTime;
            this._remaingReloadTimePerBullet = this.IsReloadTimePerBullet ? 0.0f : this.ReloadTime; // para que las armas que cargan x bala, la primer bala la cargue al toque
            //CurrentAmmo += ammoToLoad;

            return remainingAmmo;
        }

        /// <summary>
        /// Actualiza el tiempo que puede volver a disparar segun el FireRate del arma
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateCanShootTime(GameTime gameTime)
        {
            // Si puede disparar, que no calcule ningun tiempo
            //if (CanShoot && Math.Abs(_totalSecondsBulletFire - 0.0f) < 0.001f) return;
            if (this.CanShoot) return;

            //var elapsed = (float)gameTime.TotalGameTime.TotalSeconds;
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            this._totalSecondsBulletFire += elapsed;

            // Si no puede disparar, entonces hacemos el calculo del proximo FireRate
            // Si la cantidad de segundo que paso desde el ultimo disparo (elapsed - _totalSecondsBulletFire) es >= al FireRate
            // entonces puede volver a disparar
            //if (elapsed - _totalSecondsBulletFire < FireRate) return;
            if (this._totalSecondsBulletFire < this.FireRate) return;

            this.CanShoot = true;
            //_totalSecondsBulletFire = 0.0f;
        }

        /// <summary>
        /// Actualiza el tiempo de disparo del arma para dibujar la animacion (tiene que tener cierta relacion con el FireRate o algo asi)
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateShootTime(GameTime gameTime)
        {
            // If the player wants to attack
            if (this.IsShooting)
            {
                // Begin or continue an attack
                if (this._shootingTime > 0.0f)
                {
                    this._shootingTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    this.IsShooting = false;
                }
            }
            else
            {
                //Continues not attack or cancels an attack in progress
                this._shootingTime = 0.0f;
            }
        }

        private bool _isShooting;

        /// <summary>
        /// Indica si el arma esta disparando todavia. Sirve para saber cuando dejar de dibujar la animacion de disparo
        /// </summary>
        public bool IsShooting
        {
            get
            {
                return this._isShooting;
            }
            private set
            {
                this._shootingTime = MAX_SHOOTING_TIME; 
                this._isShooting = value;
            }
        }

        public void Update(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!this.IsReloading)
            {
                this.UpdateCanShootTime(gameTime);
                this.UpdateShootTime(gameTime);
                return;
            }

            if (this.IsReloadTimePerBullet)
            {
                // Si todavia quedan balas x regarcar
                if (this._ammoToLoad > 0)
                {
                    // Si la carga es x bala, entonces calculamos el _remaingReloadTimePerBullet y cuando finaliza,
                    // cargamos una bala sumando a CurrentAmmo y ejecutamos el sonido de recarga 
                    // (cuyo fin deberia coincidir con el proximo _remaingReloadTimePerBullet = 0 asi se vuelve a ejecutar otro sonido)
                    if (this._remaingReloadTimePerBullet > 0)
                    {
                        this._remaingReloadTimePerBullet -= elapsed;
                    }
                    else
                    {
                        this.ReloadSoundEffect.Play();
                        this._remaingReloadTimePerBullet = this.ReloadTime;
                        this._ammoToLoad--;
                        this.CurrentAmmo++;
                    }
                }
                else
                {
                    this._ammoToLoad = 0;
                    this.IsReloading = false;    
                }
            }
            else
            {
                // Si el arma no se carga x bala, ejecutamos el sonido del recarga cuyo finde debera coincidir con _remaingReloadTime = 0;
                // Las balas se suman todas juntas al final de la recarga
                if (!this._reloadSoundPlayed)
                {
                    this.ReloadSoundEffect.Play();
                    this._reloadSoundPlayed = true;
                }

                if (this._remaingReloadTimePerBullet > 0)
                {
                    this._remaingReloadTimePerBullet -= elapsed;
                }
                else
                {
                    // Si el arma no se carga x bala, las balas se suman todas juntas (a diferencia de la carga x bala que se suma en cada ciclo)
                    this.CurrentAmmo += this._ammoToLoad;
                    this._ammoToLoad = 0;
                    this.IsReloading = false;
                }
            }
        }

        public void LoadSoundEffects(ContentManager contentManager)
        {
            switch (this.Type)
            {
                case WeaponType.PistolaColtPython:
                    this.FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_FIRE_SOUND);
                    this.ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_RELOAD_SOUND);
                    break;
                case WeaponType.PistolaColtM1911:
                    this.FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_FIRE_SOUND);
                    this.ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_RELOAD_SOUND);
                    break;
                case WeaponType.PistolaBerettaM9:
                    this.FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_FIRE_SOUND);
                    this.ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_RELOAD_SOUND);
                    break;
                case WeaponType.EscopetaBeretta682:
                    this.FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_FIRE_SOUND);
                    this.ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_RELOAD_SOUND);
                    break;
                case WeaponType.EscopetaIthaca37:
                    this.FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_FIRE_SOUND);
                    this.ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_RELOAD_SOUND);
                    break;
                case WeaponType.EscopetaSpas12:
                    this.FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_FIRE_SOUND);
                    this.ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_RELOAD_SOUND);
                    break;
                case WeaponType.SubFusilUzi:
                    this.FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_FIRE_SOUND);
                    this.ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_RELOAD_SOUND);
                    break;
                case WeaponType.SubFusilMp5K:
                    this.FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_FIRE_SOUND);
                    this.ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_RELOAD_SOUND);
                    break;
                case WeaponType.SubFusilMp40:
                    this.FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_FIRE_SOUND);
                    this.ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_RELOAD_SOUND);
                    break;
                case WeaponType.FusilAk47:
                    this.FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_FIRE_SOUND);
                    this.ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_RELOAD_SOUND);
                    break;
                case WeaponType.FusilM4A1:
                    this.FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_FIRE_SOUND);
                    this.ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_RELOAD_SOUND);
                    break;
                case WeaponType.FusilXm8:
                    this.FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_FIRE_SOUND);
                    this.ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_RELOAD_SOUND);
                    break;
                default:
                    throw new Exception("Wrong gun");
            }

            this.EmptySoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.WEAPON_EMPTY_SOUND);

            if (this.EmptySoundEffect == null || this.FireSoundEffect == null || this.ReloadSoundEffect == null)
                throw new Exception("One or more of weapons sound effect not set");
        }

        public int FastReload(int availableAmmo)
        {
            // Por si justo se ejecuta el otro reload
            this.IsReloading = true;

            int remainingAmmo;

            // Si estaban cargandose balas antes de llegar a esta pantalla
            if (this._ammoToLoad > 0)
            {
                // solo le cargamos las balas que quedaban x cargar
                this.CurrentAmmo += this._ammoToLoad;

                // Las balas remanentes (remainingAmmo) son las disponibles (availableAmmo)
                // xq cuando habia comenzado a cargar anteriormente, ya las habia restado
                remainingAmmo = availableAmmo;
            }
            else
            {
                // Calculamos las municiones que hay que cargar al arma
                var weaponAmmoSpace = this.MaxAmmo - this.CurrentAmmo;

                // Si las municiones disponibles son mayores a las municiones a cargar,
                // entonces cargamos las municiones a cargar y devolvemos las municiones restantes que no se usaron
                // sino solo cargamos las municiones que tenemos disponibles
                if (availableAmmo > weaponAmmoSpace)
                {
                    this.CurrentAmmo += weaponAmmoSpace;
                    remainingAmmo = availableAmmo - weaponAmmoSpace;
                }
                else
                {
                    this.CurrentAmmo += availableAmmo;
                    remainingAmmo = 0;
                }
            }

            this.IsReloading = false;

            return remainingAmmo;
        }
    }
}

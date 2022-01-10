using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Platformer.Animations;
using Platformer.Helpers;

namespace Platformer.Weapons
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

        public bool HasAmmo { get { return CurrentAmmo > 0; } }

        public Weapon(WeaponType weaponType)
        {
            CanShoot = true;

            Type = weaponType;

            GameData = new GameObject();

            // Todas las pistolas tienen el mismo poder, difieren en la cantidad de balas que cargan (MaxAmmo) y en el fire rate (FireRate). Cargan x bala
            // Las escopetas son las que mayor tienen, pero son las lentas (FireRate) y cargan menos balas (MaxAmmo). Cargan x bala
            // Los subfusiles tienen el mismo poder que las pistolas. Son automaticos.
            // Los fusiles tienen el mayor poder que los subfusiles pero menos que las escopetas. Son automaticos.
            switch (Type)
            {
                case WeaponType.PistolaColtPython:
                    Position = WeaponPosition.Primary;
                    Power = 1;
                    BulletVelocity = 15.0f;
                    FireRate = 0.75f;
                    ScoreValue = 100;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_RELOAD_SOUND);
                    MaxAmmo = 6;
                    CurrentAmmo = MaxAmmo;
                    Name = "Colt Python";
                    IsReloadTimePerBullet = true;
                    ReloadTime = 1.00f;
                    IsAutomatic = false;
                    break;
                case WeaponType.PistolaColtM1911:
                    Position = WeaponPosition.Primary;    
                    Power = 1;
                    BulletVelocity = 15.0f;
                    FireRate = 0.65f;
                    ScoreValue = 150;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(Assets.PISTOLA_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_RELOAD_SOUND);
                    MaxAmmo = 7;
                    CurrentAmmo = MaxAmmo;
                    Name = "Colt M1911";
                    IsReloadTimePerBullet = true;
                    ReloadTime = 1.00f;
                    IsAutomatic = false;
                    break;
                case WeaponType.PistolaBerettaM9:
                    Position = WeaponPosition.Primary;
                    Power = 1;
                    BulletVelocity = 15.0f;
                    FireRate = 0.55f;
                    ScoreValue = 200;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_RELOAD_SOUND);
                    MaxAmmo = 15;
                    CurrentAmmo = MaxAmmo;
                    Name = "Beretta M9";
                    IsReloadTimePerBullet = true;
                    ReloadTime = 1.00f;
                    IsAutomatic = false;
                    break;   
                case WeaponType.EscopetaBeretta682:
                    Position = WeaponPosition.Secondary;
                    Power = 3;
                    BulletVelocity = 15.0f;
                    FireRate = 1.25f;
                    ScoreValue = 400;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_RELOAD_SOUND);
                    MaxAmmo = 2;
                    CurrentAmmo = MaxAmmo;
                    Name = "Beretta 682";
                    IsReloadTimePerBullet = true;
                    ReloadTime = 1.00f;
                    IsAutomatic = false;
                    break;
                case WeaponType.EscopetaIthaca37:
                    Position = WeaponPosition.Secondary;
                    Power = 3;
                    BulletVelocity = 15.0f;
                    FireRate = 1.25f;
                    ScoreValue = 600;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_RELOAD_SOUND);
                    MaxAmmo = 4;
                    CurrentAmmo = MaxAmmo;
                    Name = "Ithaca 37";
                    IsReloadTimePerBullet = true;
                    ReloadTime = 1.00f;
                    IsAutomatic = false;
                    break;
                case WeaponType.EscopetaSpas12:
                    Position = WeaponPosition.Secondary;
                    Power = 3;
                    BulletVelocity = 15.0f;
                    FireRate = 1.25f;
                    ScoreValue = 800;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_RELOAD_SOUND);
                    MaxAmmo = 8;
                    CurrentAmmo = MaxAmmo;
                    Name = "Spas 12";
                    IsReloadTimePerBullet = true;
                    ReloadTime = 1.00f;
                    IsAutomatic = false;
                    break;
                case WeaponType.SubFusilUzi:
                    Position = WeaponPosition.Primary;
                    Power = 1;
                    BulletVelocity = 15.0f;
                    FireRate = 0.1f;
                    ScoreValue = 1000;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_RELOAD_SOUND);
                    MaxAmmo = 20;
                    CurrentAmmo = MaxAmmo;
                    Name = "Uzi";
                    IsReloadTimePerBullet = false;
                    ReloadTime = 2.00f;
                    IsAutomatic = true;
                    break;
                case WeaponType.SubFusilMp5K:
                    Position = WeaponPosition.Primary;
                    Power = 1;
                    BulletVelocity = 15.0f;
                    FireRate = 0.1f;
                    ScoreValue = 1250;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_RELOAD_SOUND);
                    MaxAmmo = 30;
                    CurrentAmmo = MaxAmmo;
                    Name = "MP 5 K";
                    IsReloadTimePerBullet = false;
                    ReloadTime = 2.00f;
                    IsAutomatic = true;
                    break;
                case WeaponType.SubFusilMp40:
                    Position = WeaponPosition.Primary;
                    Power = 1;
                    BulletVelocity = 15.0f;
                    FireRate = 0.1f;
                    ScoreValue = 1500;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_RELOAD_SOUND);
                    MaxAmmo = 40;
                    CurrentAmmo = MaxAmmo;
                    Name = "MP 40";
                    IsReloadTimePerBullet = false;
                    ReloadTime = 2.00f;
                    IsAutomatic = true;
                    break;
                case WeaponType.FusilAk47:
                    Position = WeaponPosition.Secondary;
                    Power = 2;
                    BulletVelocity = 15.0f;
                    FireRate = 0.05f;
                    ScoreValue = 2000;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_RELOAD_SOUND);
                    MaxAmmo = 30;
                    CurrentAmmo = MaxAmmo;
                    Name = "AK 47";
                    IsReloadTimePerBullet = false;
                    ReloadTime = 1.00f;
                    IsAutomatic = true;
                    break;
                case WeaponType.FusilM4A1:
                    Position = WeaponPosition.Secondary;
                    Power = 2;
                    BulletVelocity = 15.0f;
                    FireRate = 0.05f;
                    ScoreValue = 2500;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_RELOAD_SOUND);
                    MaxAmmo = 40;
                    CurrentAmmo = MaxAmmo;
                    Name = "M4A1";
                    IsReloadTimePerBullet = false;
                    ReloadTime = 1.00f;
                    IsAutomatic = true;
                    break;
                case WeaponType.FusilXm8:
                    Position = WeaponPosition.Secondary;
                    Power = 5;
                    BulletVelocity = 15.0f;
                    FireRate = 0.05f;
                    ScoreValue = 3000;
                    //FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_FIRE_SOUND);
                    //ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_RELOAD_SOUND);
                    MaxAmmo = 50;
                    CurrentAmmo = MaxAmmo;
                    Name = "XM 8";
                    IsReloadTimePerBullet = false;
                    ReloadTime = 1.00f;
                    IsAutomatic = true;
                    break;
                default:
                    throw new Exception("Wrong gun");
            }

            //EmptySoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.WEAPON_EMPTY_SOUND);

            if (Name == string.Empty)
                throw new Exception(string.Format("Weapon name not set for type {0}", Type));

            AmmoPack = MaxAmmo*3;
            AmmoScoreValue = (int)(ScoreValue * 0.2);
        }

        //public void Fire(GameTime gameTime, Pool<Bullet> bulletsPool, SpriteEffects flip, GameObject playerArm)
        public void Fire(GameTime gameTime, Pool<Bullet> bulletsPool, SpriteEffects flip)
        {
            if (IsReloading)
                throw new Exception("No se puede disparar el arma mientras esta siendo cargada.");

            // Si no quedan mas balas en el arma, entonces que ejecute el sonido de vacio y retorne
            if (CurrentAmmo <= 0)
            {
                EmptySoundEffect.Play();
                return;
            }

            // Actualiza el tiempo de disparo para la animacion
            IsShooting = true;            

            // Fire a new bullet enemy every X seconds
            // Si la cantidad de segundos desde el ultimo disparo (totalSeconds - _previousTotalSecondsBulletFire) es < que el tiempo de disparo (FireRate),
            // entonces que no haga nada.
            //var actualTotalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
            //if (actualTotalSeconds - _previousTotalSecondsBulletFire < FireRate) return;
            //_previousTotalSecondsBulletFire = actualTotalSeconds;
            if (!CanShoot) return;

            CanShoot = false;
            //_totalSecondsBulletFire = (float)gameTime.TotalGameTime.TotalSeconds;
            _totalSecondsBulletFire = 0.0f;

            // Si no existen balas disponibles en el Pool de balas, entonces que retorne
            if (bulletsPool.AvailableCount <= 0) return;

            // Get a bullet from the pool.
            var bullet = bulletsPool.Get().Item;

            //And set it to alive.
            //bullet.Alive = true;
            bullet.Power = Power;

            var armCos = (float)Math.Cos(GameData.Rotation);
            var armSin = (float)Math.Sin(GameData.Rotation); 

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
                    GameData.Position.X + 42 * armCos,
                    GameData.Position.Y + 42 * armSin);

                //And give it a velocity of the direction we're aiming.
                //Increase/decrease speed by changing 15.0f
                //bullet.velocity = new Vector2(
                //    (float)Math.Cos(_arm.rotation - MathHelper.PiOver2),
                //    (float)Math.Sin(_arm.rotation - MathHelper.PiOver2)) * 15.0f;
                bullet.Velocity = new Vector2(
                                      armCos,
                                      armSin)*BulletVelocity;
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
                   GameData.Position.X - 42 * armCos,
                   GameData.Position.Y - 42 * armSin);

                //And give it a velocity of the direction we're aiming.
                //Increase/decrease speed by changing 15.0f
                bullet.Velocity = new Vector2(
                                      -armCos,
                                      -armSin)*BulletVelocity;
            }

            FireSoundEffect.Play();

            CurrentAmmo--;
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
            if (IsReloading)
                throw new Exception("No se puede cargar el arma mientras esta siendo cargada.");

            IsReloading = true;
            _reloadSoundPlayed = false;

            // Calculamos las municiones que hay que cargar al arma
            var weaponAmmoSpace = MaxAmmo - CurrentAmmo;

            int remainingAmmo;

            // Si las municiones disponibles son mayores a las municiones a cargar,
            // entonces cargamos las municiones a cargar y devolvemos las municiones restantes que no se usaron
            // sino solo cargamos las municiones que tenemos disponibles
            if (availableAmmo > weaponAmmoSpace)
            {
                _ammoToLoad = weaponAmmoSpace;
                remainingAmmo = availableAmmo - weaponAmmoSpace;
            }
            else
            {
                _ammoToLoad = availableAmmo;
                remainingAmmo = 0;
            }

            // Seteamos el tiempo de carga remanente
            //_remaingReloadTime = IsReloadTimePerBullet ? (ReloadTime * _ammoToLoad) + ReloadTime : ReloadTime;
            _remaingReloadTimePerBullet = IsReloadTimePerBullet ? 0.0f : ReloadTime; // para que las armas que cargan x bala, la primer bala la cargue al toque
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
            if (CanShoot) return;

            //var elapsed = (float)gameTime.TotalGameTime.TotalSeconds;
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _totalSecondsBulletFire += elapsed;

            // Si no puede disparar, entonces hacemos el calculo del proximo FireRate
            // Si la cantidad de segundo que paso desde el ultimo disparo (elapsed - _totalSecondsBulletFire) es >= al FireRate
            // entonces puede volver a disparar
            //if (elapsed - _totalSecondsBulletFire < FireRate) return;
            if (_totalSecondsBulletFire < FireRate) return;

            CanShoot = true;
            //_totalSecondsBulletFire = 0.0f;
        }

        /// <summary>
        /// Actualiza el tiempo de disparo del arma para dibujar la animacion (tiene que tener cierta relacion con el FireRate o algo asi)
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateShootTime(GameTime gameTime)
        {
            // If the player wants to attack
            if (IsShooting)
            {
                // Begin or continue an attack
                if (_shootingTime > 0.0f)
                {
                    _shootingTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    IsShooting = false;
                }
            }
            else
            {
                //Continues not attack or cancels an attack in progress
                _shootingTime = 0.0f;
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
                return _isShooting;
            }
            private set
            {
                _shootingTime = MAX_SHOOTING_TIME; 
                _isShooting = value;
            }
        }

        public void Update(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!IsReloading)
            {
                UpdateCanShootTime(gameTime);
                UpdateShootTime(gameTime);
                return;
            }

            if (IsReloadTimePerBullet)
            {
                // Si todavia quedan balas x regarcar
                if (_ammoToLoad > 0)
                {
                    // Si la carga es x bala, entonces calculamos el _remaingReloadTimePerBullet y cuando finaliza,
                    // cargamos una bala sumando a CurrentAmmo y ejecutamos el sonido de recarga 
                    // (cuyo fin deberia coincidir con el proximo _remaingReloadTimePerBullet = 0 asi se vuelve a ejecutar otro sonido)
                    if (_remaingReloadTimePerBullet > 0)
                    {
                        _remaingReloadTimePerBullet -= elapsed;
                    }
                    else
                    {
                        ReloadSoundEffect.Play();
                        _remaingReloadTimePerBullet = ReloadTime;
                        _ammoToLoad--;
                        CurrentAmmo++;
                    }
                }
                else
                {
                    _ammoToLoad = 0;
                    IsReloading = false;    
                }
            }
            else
            {
                // Si el arma no se carga x bala, ejecutamos el sonido del recarga cuyo finde debera coincidir con _remaingReloadTime = 0;
                // Las balas se suman todas juntas al final de la recarga
                if (!_reloadSoundPlayed)
                {
                    ReloadSoundEffect.Play();
                    _reloadSoundPlayed = true;
                }

                if (_remaingReloadTimePerBullet > 0)
                {
                    _remaingReloadTimePerBullet -= elapsed;
                }
                else
                {
                    // Si el arma no se carga x bala, las balas se suman todas juntas (a diferencia de la carga x bala que se suma en cada ciclo)
                    CurrentAmmo += _ammoToLoad;
                    _ammoToLoad = 0;
                    IsReloading = false;
                }
            }
        }

        public void LoadSoundEffects(ContentManager contentManager)
        {
            switch (Type)
            {
                case WeaponType.PistolaColtPython:
                    FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_FIRE_SOUND);
                    ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_RELOAD_SOUND);
                    break;
                case WeaponType.PistolaColtM1911:
                    FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_FIRE_SOUND);
                    ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_RELOAD_SOUND);
                    break;
                case WeaponType.PistolaBerettaM9:
                    FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_FIRE_SOUND);
                    ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.PISTOLA_RELOAD_SOUND);
                    break;
                case WeaponType.EscopetaBeretta682:
                    FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_FIRE_SOUND);
                    ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_RELOAD_SOUND);
                    break;
                case WeaponType.EscopetaIthaca37:
                    FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_FIRE_SOUND);
                    ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_RELOAD_SOUND);
                    break;
                case WeaponType.EscopetaSpas12:
                    FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_FIRE_SOUND);
                    ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.ESCOPETA_RELOAD_SOUND);
                    break;
                case WeaponType.SubFusilUzi:
                    FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_FIRE_SOUND);
                    ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_RELOAD_SOUND);
                    break;
                case WeaponType.SubFusilMp5K:
                    FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_FIRE_SOUND);
                    ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_RELOAD_SOUND);
                    break;
                case WeaponType.SubFusilMp40:
                    FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_FIRE_SOUND);
                    ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.SUBFUSIL_RELOAD_SOUND);
                    break;
                case WeaponType.FusilAk47:
                    FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_FIRE_SOUND);
                    ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_RELOAD_SOUND);
                    break;
                case WeaponType.FusilM4A1:
                    FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_FIRE_SOUND);
                    ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_RELOAD_SOUND);
                    break;
                case WeaponType.FusilXm8:
                    FireSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_FIRE_SOUND);
                    ReloadSoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.FUSIL_RELOAD_SOUND);
                    break;
                default:
                    throw new Exception("Wrong gun");
            }

            EmptySoundEffect = contentManager.Load<SoundEffect>(GlobalParameters.WEAPON_EMPTY_SOUND);

            if (EmptySoundEffect == null || FireSoundEffect == null || ReloadSoundEffect == null)
                throw new Exception("One or more of weapons sound effect not set");
        }

        public int FastReload(int availableAmmo)
        {
            // Por si justo se ejecuta el otro reload
            IsReloading = true;

            int remainingAmmo;

            // Si estaban cargandose balas antes de llegar a esta pantalla
            if (_ammoToLoad > 0)
            {
                // solo le cargamos las balas que quedaban x cargar
                CurrentAmmo += _ammoToLoad;

                // Las balas remanentes (remainingAmmo) son las disponibles (availableAmmo)
                // xq cuando habia comenzado a cargar anteriormente, ya las habia restado
                remainingAmmo = availableAmmo;
            }
            else
            {
                // Calculamos las municiones que hay que cargar al arma
                var weaponAmmoSpace = MaxAmmo - CurrentAmmo;

                // Si las municiones disponibles son mayores a las municiones a cargar,
                // entonces cargamos las municiones a cargar y devolvemos las municiones restantes que no se usaron
                // sino solo cargamos las municiones que tenemos disponibles
                if (availableAmmo > weaponAmmoSpace)
                {
                    CurrentAmmo += weaponAmmoSpace;
                    remainingAmmo = availableAmmo - weaponAmmoSpace;
                }
                else
                {
                    CurrentAmmo += availableAmmo;
                    remainingAmmo = 0;
                }
            }

            IsReloading = false;

            return remainingAmmo;
        }
    }
}

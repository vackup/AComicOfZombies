using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Platformer.Levels;
using Platformer.Players;
using Platformer.ScreenManagers;

namespace Platformer
{
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    public class Item
    {
        private ScreenManager _screenManager;

        private Texture2D _texture;
        private Vector2 _origin;
        private SoundEffect _collectedSound;

        /*public const int PointValue = 30;
        public readonly Color Color = Color.Yellow;*/

        public ItemType ItemType {get; set;}

        // The gem is animated from a base position along the Y axis.
        private Vector2 _basePosition;
        private float _bounce;

        public bool IsBouncing;

        public Level Level { get; private set; }

        /// <summary>
        /// Gets the current position of this gem in world space.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return _basePosition + new Vector2(0.0f, _bounce);
            }
        }

        /// <summary>
        /// Gets a circle which bounds this gem in world space.
        /// </summary>
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.WIDTH / 3.0f);
            }
        }

        /// <summary>
        /// Gets a circle which bounds this gem in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X - (_texture.Width / 2), (int)Position.Y - (_texture.Height / 2), _texture.Width, _texture.Height);
            }
        }

        public bool IsItemOn { get; private set; }

        public int PointValue { get; private set; }

        public Color Color { get; private set; }

        public Item(ScreenManager thisScreenManager, Level level, Vector2 position, ItemType itemType, bool isItemOn)
        {
            IsItemOn = isItemOn;
            SetItem(thisScreenManager, level, position, itemType);
        }

        public Item(ScreenManager thisScreenManager, Level level, Vector2 position, ItemType itemType)
        {
            IsItemOn = false;
            SetItem(thisScreenManager, level, position, itemType);
        }

        private void SetItem(ScreenManager thisScreenManager, Level level, Vector2 position, ItemType itemType)
        {
            _screenManager = thisScreenManager;
            Level = level;
            _basePosition = position;
            ItemType = itemType;
            
            switch (ItemType)
            {
                //case ItemType.Checkpoint:
                //    _texture = _screenManager.Checkpoint1Texture;
                //    if (_screenManager.GemCollectedSound != null)
                //        _collectedSound = _screenManager.GemCollectedSound;
                //    else
                //        throw new NullReferenceException("screenManager.GemCollectedSound is null");
                //    Color = Color.White;
                //    IsBouncing = false;
                //    break;

                case ItemType.Gem:
                    throw new Exception("item gem no definido");
                    //PointValue = 30;
                    //if (_screenManager.GemTexture != null)
                    //    _texture = _screenManager.GemTexture;
                    //else
                    //    throw new NullReferenceException("screenManager.GemTexture is null");
                    //_collectedSound = _screenManager.GemCollectedSound;
                    //Color = Color.Yellow;
                    //IsBouncing = true;
                    break;

                    //case ItemType.Powerup:
                    //    PointValue = 100;
                    //    if (screenManager.GemTexture != null)
                    //        texture = screenManager.GemTexture;
                    //    else
                    //        throw new NullReferenceException("screenManager.GemTexture is null");
                    //    collectedSound = screenManager.PlayerPowerUpSound;
                    //    Color = Color.Red;
                    //    isBouncing = true;
                    //    break;

                    //case ItemType.Heart:
                    //    if (screenManager.HeartIconTexture != null)
                    //        texture = screenManager.HeartIconTexture;
                    //    else
                    //        throw new NullReferenceException("screenManager.HeartIconTexture is null");
                    //    collectedSound = screenManager.PlayerPowerUpSound;
                    //    Color = Color.White;
                    //    isBouncing = true;
                    //    break;

                case ItemType.Life:
                    throw new Exception("item life no definido");
                    //_texture = _screenManager.LifeTexture;
                    //if (_screenManager.GemCollectedSound != null) 
                    //    _collectedSound = _screenManager.GemCollectedSound;
                    //else
                    //    throw new NullReferenceException("screenManager.GemCollectedSound is null");
                    //Color = Color.White;
                    //IsBouncing = true;
                    break;

                    //case ItemType.Timer:
                    //    if (screenManager.TimeClockTexture != null)
                    //        texture = screenManager.TimeClockTexture;
                    //    else
                    //        throw new NullReferenceException("screenManager.TimeClockTexture is null");
                    //    collectedSound = screenManager.GemCollectedSound;
                    //    Color = Color.White;
                    //    isBouncing = true;
                    //    break;
                default:
                    throw  new Exception("Item no definido");
            }

            //texture = screenManager.GemTexture;
            _origin = new Vector2(_texture.Width / 2.0f, _texture.Height / 2.0f);
        }


        /// <summary>
        /// Loads the item texture and collected sound.
        /// </summary>
        public void LoadContent()
        {
            if (_texture != null)
            {
                _origin = new Vector2(_texture.Width / 2.0f, _texture.Height / 2.0f);
            }
            else
            {
                throw new Exception("Item no definido. Textura no cargada.");   
            }
            //collectedSound = content.Load<SoundEffect>("Sounds/GemCollected");
        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float bounceHeight = 0.18f;
            const float bounceRate = 3.0f;
            const float bounceSync = -0.75f;

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * bounceRate + Position.X * bounceSync;
            if (IsBouncing)
                _bounce = (float)Math.Sin(t) * bounceHeight * _texture.Height;
            else
                _bounce = 0 - _texture.Height/4;

        }

        /// <summary>
        /// Called when this gem has been collected by a player and removed from the level.
        /// </summary>
        /// <param name="collectedBy">
        /// The player who collected this gem. Although currently not used, this parameter would be
        /// useful for creating special powerup gems. For example, a gem could make the player invincible.
        /// </param>
        /// <param name="itemNumber">
        /// Item Number Order in Items Collection
        /// </param>
        public void OnCollected(Player collectedBy, int itemNumber)
        {
            switch (ItemType)
            {
                //case ItemType.Checkpoint:
                //    int checkpointnumber = 0;
                //    //Set every other checkpoint to false so only one would be selected.
                //    foreach (Item item in Level.Items)
                //    {
                //        //Make sure the currently touched checkpoint doesnt get modified.
                //        if (checkpointnumber != itemNumber)
                //        {
                //            if (item.IsItemOn)
                //            {
                //                item.IsItemOn = false;
                //                item._texture = _screenManager.Checkpoint1Texture;
                //            }
                //        }
                //        checkpointnumber++;
                //    }
                //    if (!IsItemOn)
                //    {
                //        Level.Checkpoint = Position;
                //        IsItemOn = true;
                //        _texture = _screenManager.Checkpoint2Texture;
                //        PlaySound();
                //    }
                //    checkpointnumber = 0;
                //    break;

                case ItemType.Gem:
                   // Level.Items.RemoveAt(itemNumber--);
                    //Level.Score += this.PointValue;
                    //Level.GemsRemaining -= 1;
                    PlaySound();
                    break;

                //case ItemType.Powerup:
                //    _level.items.RemoveAt(itemNumber--);
                //    collectedBy.PowerUp();
                //    PlaySound();
                //    break;

                case ItemType.Life:
                    //Level.Items.RemoveAt(itemNumber--);
                    //collectedBy.GainLive();
                    PlaySound();
                    break;
                //case ItemType.Heart:
                //    _level.items.RemoveAt(itemNumber--);
                //    collectedBy.CurrentHealth += 1;
                //    PlaySound();
                //    break;
                //case ItemType.Timer:
                //    _level.items.RemoveAt(itemNumber--);
                //    _level.TimeRemaining += TimeSpan.FromSeconds(30);
                //    PlaySound();
                //    break;

            }
    

        }

        private void PlaySound()
        {
            try
            {
                //collectedSound.Play(screenManager.Settings.SoundVolumeAmount);
                _collectedSound.Play();
            }
            catch
            {}
        }

        /// <summary>
        /// Draws the item in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Color, 0.0f, _origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}

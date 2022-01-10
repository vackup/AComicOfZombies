using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Platformer
{
    public class GameObject
    {
        public Texture2D Sprite { get; private set; }
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Vector2 Center { get; private set; }
        public Vector2 Velocity { get; set; }
        //public bool Alive;
        public int Power { get; set; }
        protected int Height;
        protected int Width;

        protected Rectangle _boundingRectangle;

        public virtual Rectangle BoundingRectangle
        {
            get
            {
                _boundingRectangle.X = (int)Position.X;
                _boundingRectangle.Y = (int)Position.Y;
                _boundingRectangle.Width = Width;
                _boundingRectangle.Height = Height;

                return _boundingRectangle;
            }
        }

        public GameObject()
        {
            Rotation = 0.0f;
            Position = Vector2.Zero;
        }

        public GameObject(Texture2D loadedTexture) //: this(loadedTexture, 1)
        {
            Init(loadedTexture);
        }

        public void Init(Texture2D loadedTexture)
        {
            Power = 1;
            Rotation = 0.0f;
            Position = Vector2.Zero;
            Sprite = loadedTexture;
            Width = Sprite.Width;
            Height = Sprite.Height;
            Center = new Vector2(Width / 2, Height / 2);
            Velocity = Vector2.Zero;


            _boundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        }

        public void Init(int width, int height)
        {
            Width = width;
            Height = height;
            Power = 1;
            Rotation = 0.0f;
            Position = Vector2.Zero;
            Center = new Vector2(Width / 2, Height / 2);
            Velocity = Vector2.Zero;
            //Alive = false;

            _boundingRectangle = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, Color.White);
        }
    }
}

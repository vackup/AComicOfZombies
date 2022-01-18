using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ACoZ
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
                this._boundingRectangle.X = (int)this.Position.X;
                this._boundingRectangle.Y = (int)this.Position.Y;
                this._boundingRectangle.Width = this.Width;
                this._boundingRectangle.Height = this.Height;

                return this._boundingRectangle;
            }
        }

        public GameObject()
        {
            this.Rotation = 0.0f;
            this.Position = Vector2.Zero;
        }

        public GameObject(Texture2D loadedTexture) //: this(loadedTexture, 1)
        {
            this.Init(loadedTexture);
        }

        public void Init(Texture2D loadedTexture)
        {
            this.Power = 1;
            this.Rotation = 0.0f;
            this.Position = Vector2.Zero;
            this.Sprite = loadedTexture;
            this.Width = this.Sprite.Width;
            this.Height = this.Sprite.Height;
            this.Center = new Vector2(this.Width / 2, this.Height / 2);
            this.Velocity = Vector2.Zero;


            this._boundingRectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);
        }

        public void Init(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Power = 1;
            this.Rotation = 0.0f;
            this.Position = Vector2.Zero;
            this.Center = new Vector2(this.Width / 2, this.Height / 2);
            this.Velocity = Vector2.Zero;
            //Alive = false;

            this._boundingRectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Sprite, this.Position, Color.White);
        }
    }
}

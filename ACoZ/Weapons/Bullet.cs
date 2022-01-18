using Microsoft.Xna.Framework;

namespace ACoZ.Weapons
{
    public class Bullet : GameObject
    {
        public override Rectangle BoundingRectangle
        {
            get
            {
                this._boundingRectangle.X = (int) this.Position.X - this.Width*2;
                this._boundingRectangle.Y = (int) this.Position.Y - this.Height*2;
                this._boundingRectangle.Width = this.Width*4;
                this._boundingRectangle.Height = this.Height*4;

                return this._boundingRectangle;
            }
        }
    }


}
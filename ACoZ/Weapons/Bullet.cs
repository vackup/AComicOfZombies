using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platformer.Helpers;
using Platformer.Levels;

namespace Platformer.Weapons
{
    public class Bullet : GameObject
    {
        public override Rectangle BoundingRectangle
        {
            get
            {
                _boundingRectangle.X = (int) Position.X - Width*2;
                _boundingRectangle.Y = (int) Position.Y - Height*2;
                _boundingRectangle.Width = Width*4;
                _boundingRectangle.Height = Height*4;

                return _boundingRectangle;
            }
        }
    }


}
using System;
using Microsoft.Xna.Framework;

namespace ACoZ
{
    class Projectile
    {
        // Thanks to:
        // http://stackoverflow.com/questions/3273396/animate-sprite-along-a-curve-path-in-xna
        // http://www.youtube.com/watch?v=MSzZd8TLOK4

        private float _initialPositionX;
        private float _initialPositionY;
        private int _initialVelocity;
        private double _initialAngle;
        
        const double ACCELERATION = -9.8;

        double _time;

        private Vector2 _position = Vector2.Zero; // Use this when drawing your sprite

        public Vector2 Position
        {
            get { return this._position; }
        }

        /// <summary>
        /// Projectile constructor
        /// </summary>
        /// <param name="initialPositionX">Initial Position X</param>
        /// <param name="initialPositionY">Initial Position Y</param>
        /// <param name="initialVelocity">Initial Velocity</param>
        /// <param name="initialAngle">Initial Angle</param>
        public Projectile(float initialPositionX, float initialPositionY, int initialVelocity, double initialAngle)
        {
            this._initialPositionX = initialPositionX;
            this._initialPositionY = initialPositionY;

            this._initialVelocity = initialVelocity;

            this._initialAngle = initialAngle;
        }

        public void Update(GameTime gameTime)
        {

            this._time += (float)gameTime.ElapsedGameTime.TotalSeconds*10;

            this._position.X = (float)(this._initialPositionX + this._initialVelocity * Math.Cos(this._initialAngle) * this._time);
            this._position.Y = (float)(this._initialPositionY + this._initialVelocity * Math.Sin(this._initialAngle) * this._time + 0.5f * ACCELERATION * Math.Pow(this._time, 2));

        }
    }
}

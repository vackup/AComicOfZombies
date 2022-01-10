using System;
using Microsoft.Xna.Framework;

namespace Platformer
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
            get { return _position; }
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
            _initialPositionX = initialPositionX;
            _initialPositionY = initialPositionY;

            _initialVelocity = initialVelocity;

            _initialAngle = initialAngle;
        }

        public void Update(GameTime gameTime)
        {

            _time += (float)gameTime.ElapsedGameTime.TotalSeconds*10;

            _position.X = (float)(_initialPositionX + _initialVelocity * Math.Cos(_initialAngle) * _time);
            _position.Y = (float)(_initialPositionY + _initialVelocity * Math.Sin(_initialAngle) * _time + 0.5f * ACCELERATION * Math.Pow(_time, 2));

        }
    }
}

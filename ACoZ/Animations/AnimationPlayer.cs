using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ACoZ.Animations
{
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    public struct AnimationPlayer
    {
        /// <summary>
        /// Gets the animation which is currently playing.
        /// </summary>
        public Animation Animation { get; private set; }

        /// <summary>
        /// Gets the index of the current frame in the animation.
        /// </summary>
        public int FrameIndex { get; private set; }

        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        private float _time;

        /// <summary>
        /// Gets a texture origin at the bottom center of each frame.
        /// </summary>
        public Vector2 Origin { get; private set; }

        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(Animation animation)
        {
            // If this animation is already running, do not restart it.
            if (this.Animation == animation)
                return;

            // Start the new animation.
            this.Animation = animation;
            this.FrameIndex = 0;
            this._time = 0.0f;
            this.Origin = new Vector2(this.Animation.FrameWidth / 2.0f, this.Animation.FrameHeight);
        }

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            this.Draw(gameTime, spriteBatch, position, spriteEffects, Color.White);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects, Color color)
        {
            if (this.Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            // Process passing time.
            this._time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (this._time > this.Animation.FrameTime)
            {
                this._time -= this.Animation.FrameTime;

                // Advance the frame index; looping or clamping as appropriate.
                if (this.Animation.IsLooping)
                {
                    this.FrameIndex = (this.FrameIndex + 1) % this.Animation.FrameCount;
                }
                else
                {
                    this.FrameIndex = Math.Min(this.FrameIndex + 1, this.Animation.FrameCount - 1);
                }
            }

            // Calculate the source rectangle of the current frame.
            // Returns the Modulus (remainder) of currentFrame divided by maxRowFrame
            //Example: currentFrame is 20, maxRowFrame is 15. 20 Mod 15 = 5. So, our X frame is 5.
            //var rectanguleX = (FrameIndex % Animation.MaxRowFrame) * Animation.FrameWidth;

            //// Returns the whole number of currentFrame/maxRowFrame: 20/15 = 1.333, integer of 1.333 is 1
            //var rectanguleY = (int)(FrameIndex / Animation.MaxRowFrame) * Animation.FrameHeight;

            //var source = new Rectangle(rectanguleX, rectanguleY, Animation.FrameWidth, Animation.FrameHeight);
            //Rectangle source = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);

            var animPostion = position;

            // Para manejar animaciones con distinto alto
            if (this.Animation.FrameHeight > this.Animation.RectangleFrames[this.FrameIndex].Height)
            {
                animPostion.Y += (this.Animation.FrameHeight - this.Animation.RectangleFrames[this.FrameIndex].Height); 
            }


            // Draw the current frame.
            spriteBatch.Draw(this.Animation.Texture, animPostion, this.Animation.RectangleFrames[this.FrameIndex], color, 0.0f, this.Origin, 1.0f, spriteEffects, 0.0f);
        }

        
    }
}

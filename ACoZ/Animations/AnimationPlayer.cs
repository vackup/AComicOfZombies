#region File Description
//-----------------------------------------------------------------------------
// AnimationPlayer.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platformer.Animations
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
            if (Animation == animation)
                return;

            // Start the new animation.
            Animation = animation;
            FrameIndex = 0;
            _time = 0.0f;
            Origin = new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight);
        }

        /// <summary>
        /// Advances the time position and draws the current frame of the animation.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            Draw(gameTime, spriteBatch, position, spriteEffects, Color.White);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects, Color color)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            // Process passing time.
            _time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (_time > Animation.FrameTime)
            {
                _time -= Animation.FrameTime;

                // Advance the frame index; looping or clamping as appropriate.
                if (Animation.IsLooping)
                {
                    FrameIndex = (FrameIndex + 1) % Animation.FrameCount;
                }
                else
                {
                    FrameIndex = Math.Min(FrameIndex + 1, Animation.FrameCount - 1);
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
            if (Animation.FrameHeight > Animation.RectangleFrames[FrameIndex].Height)
            {
                animPostion.Y += (Animation.FrameHeight - Animation.RectangleFrames[FrameIndex].Height); 
            }


            // Draw the current frame.
            spriteBatch.Draw(Animation.Texture, animPostion, Animation.RectangleFrames[FrameIndex], color, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
        }

        
    }
}

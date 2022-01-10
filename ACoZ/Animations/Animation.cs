#region File Description
//-----------------------------------------------------------------------------
// Animation.cs
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
    /// Represents an animated texture.
    /// </summary>
    /// <remarks>
    /// Currently, this class assumes that each frame of animation is
    /// as wide as each animation is tall. The number of frames in the
    /// animation are inferred from this.
    /// </remarks>
    public class Animation
    {
        public Rectangle[] RectangleFrames { get; private set; }

        /// <summary>
        /// Bounds to collision detection
        /// </summary>
        public Rectangle BoundingRectangle { get; private set; }

        /// <summary>
        /// All frames in the animation arranged horizontally.
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        public float FrameTime { get; private set; }

        /// <summary>
        /// When the end of the animation is reached, should it
        /// continue playing from the beginning?
        /// </summary>
        public bool IsLooping { get; private set; }

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public int FrameCount { get; private set; }

        /// <summary>
        /// Gets the width of a frame in the animation.
        /// </summary>
        public int FrameWidth { get; private set; }

        /// <summary>
        /// Gets the height of a frame in the animation.
        /// </summary>
        public int FrameHeight { get; private set; }

        public int RowCount { get; private set; }

        public int MaxRowFrame { get; private set; }

        ///// <summary>
        ///// Constructors a new animation. Frames are squeare (same Witdh and Height)
        ///// </summary>        
        //public Animation(Texture2D texture, float frameTime, bool isLooping)
        //{
        //    Texture = texture;
        //    FrameTime = frameTime;
        //    IsLooping = isLooping;
        //    FrameWidth = Texture.Height;
        //    FrameHeight = Texture.Height;
        //    FrameCount = Texture.Width / FrameWidth;
        //    RowCount = 1;
        //    MaxRowFrame = FrameCount;

        //    SetDefaultBoundingRectangle();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture">Textura para dibujar la animaciones</param>
        /// <param name="frameTime">Duracion de tiempo en mostrar cada frame</param>
        /// <param name="isLooping"></param>
        /// <param name="rectangleFrames">Array de rectangulos de frames dentro de la textura</param>
        public Animation(Texture2D texture, float frameTime, bool isLooping, Rectangle[] rectangleFrames)
        {
            RectangleFrames = rectangleFrames;
            Texture = texture;
            FrameTime = frameTime;
            IsLooping = isLooping;
            //FrameWidth = Texture.Width / frameCount;
            //FrameHeight = Texture.Height;
            FrameCount = rectangleFrames.Length;
            RowCount = 1;
            MaxRowFrame = FrameCount;

            FrameWidth = rectangleFrames[0].Width;
            FrameHeight = rectangleFrames[0].Height;

            SetDefaultBoundingRectangle();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="texture">Textura para dibujar la animaciones</param>
        /// <param name="frameTime">Duracion de tiempo en mostrar cada frame</param>
        /// <param name="isLooping"></param>
        /// <param name="rectangleFrames"></param>
        /// <param name="width"></param>
        /// <param name="height">Altura maxima de la animacion. Si la animacion esta compuesto x cuadros de distinto tamaño, hay que poner el del tamaño más alto</param>
        public Animation(Texture2D texture, float frameTime, bool isLooping, Rectangle[] rectangleFrames, int width, int height)
        {
            RectangleFrames = rectangleFrames;
            Texture = texture;
            FrameTime = frameTime;
            IsLooping = isLooping;
            //FrameWidth = Texture.Width / frameCount;
            //FrameHeight = Texture.Height;
            FrameCount = rectangleFrames.Length;
            RowCount = 1;
            MaxRowFrame = FrameCount;

            FrameWidth = width;
            FrameHeight = height;

            SetDefaultBoundingRectangle();
        }

        ///// <summary>
        ///// Constructors a new animation. Height = texture.Height. Witdh != Height. All frames are in row.
        ///// </summary>        
        //public Animation(Texture2D texture, float frameTime, bool isLooping, int frameCount)
        //{
        //    Texture = texture;
        //    FrameTime = frameTime;
        //    IsLooping = isLooping;
        //    FrameWidth = Texture.Width / frameCount;
        //    FrameHeight = Texture.Height;
        //    FrameCount = frameCount;
        //    RowCount = 1;
        //    MaxRowFrame = FrameCount;

        //    SetDefaultBoundingRectangle();
        //}

        ///// <summary>
        ///// Constructors a new animation. Height = texture.Height. Witdh != Height. All frames are in different rows.
        ///// </summary>        
        //public Animation(Texture2D texture, float frameTime, bool isLooping, int frameCount, int frameWidth, int frameHeight)
        //{
        //    Texture = texture;
        //    FrameTime = frameTime;
        //    IsLooping = isLooping;
        //    FrameWidth = frameWidth;
        //    FrameHeight = frameHeight;
        //    FrameCount = frameCount;
			
        //    if (Texture.Height % frameHeight != 0)
        //    {
        //        throw new Exception(string.Format("Malas dimensiones para la textura. Texture.Height ({0}) % frameHeight ({1}) != 0", Texture.Height, frameHeight));
        //    } 

        //    RowCount = Texture.Height / FrameHeight;
        //    MaxRowFrame = Texture.Width/FrameWidth;

        //    SetDefaultBoundingRectangle();
        //}

        public void SetBoundingRectangle(Rectangle animationBounds)
        {
            BoundingRectangle = animationBounds;
        }

        public void SetBoundingRectangle(float widthPercent, float heightPercent)
        {
            // Calculate bounds within texture size.            
            var width = (int)(FrameWidth * widthPercent);
            var left = (FrameWidth - width) / 2;
            var height = (int)(FrameHeight * heightPercent);
            var top = FrameHeight - height;

            BoundingRectangle = new Rectangle(left, top, width, height);
        }

        //public void SetBoundingRectangle(int width, int height)
        //{
        //    // Calculate bounds within texture size.            
        //    var left = (FrameWidth - width) / 2;
        //    var top = FrameHeight - height;

        //    BoundingRectangle = new Rectangle(left, top, width, height);
        //}

        private void SetDefaultBoundingRectangle()
        {
            // Calculate bounds within texture size.            
            var width = (int)(FrameWidth * 0.8);
            var left = (FrameWidth - width) / 2;
            var height = (int)(FrameHeight * 0.9);
            var top = FrameHeight - height;

            BoundingRectangle = new Rectangle(left, top, width, height);
        }
    }
}

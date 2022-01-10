#region File Description
//-----------------------------------------------------------------------------
// Tile.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Platformer.Helpers;

namespace Platformer.Levels
{
    /// <summary>
    /// Stores the appearance and collision behavior of a tile.
    /// </summary>
    struct Tile
    {
        public Texture2D Texture;
        public TileCollision Collision;

        public static int WIDTH = GlobalParameters.TILE_WITH;
        public static int HEIGHT = GlobalParameters.TILE_HEIGHT;
		public static int CENTER = WIDTH / 2;

        public static readonly Vector2 Size = new Vector2(WIDTH, HEIGHT);

        /// <summary>
        /// Constructs a new tile.
        /// </summary>
        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }
    }
}

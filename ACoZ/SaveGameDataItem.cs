using System;
using Microsoft.Xna.Framework;

namespace Platformer
{
#if WINDOWS && !NETFX_CORE
    [Serializable]
#endif
    public struct SaveGameDataItem
    {
        public Vector2 Position;
        public int ItemObject;
        public bool IsOn; // Usefull if it's a checkpoint
    }
}
using System;
using Microsoft.Xna.Framework;
using System.Xml.Serialization;

namespace Platformer
{
#if WINDOWS && !NETFX_CORE
    [Serializable]
#endif
    public struct SaveGameDataSurvivor
    {
        public bool IsAlive;
        public Vector2 Position;
        public int CurrentHealth;
        public string SpriteSet;
    }
}
using System;
using Microsoft.Xna.Framework;

namespace ACoZ
{
#if WINDOWS && !NETFX_CORE
    [Serializable]
#endif
    public struct SaveGameDataEnemy
    {
        public bool IsAlive;
        public Vector2 Position;
        public int CurrentHealth;
        public string SpriteSet;
    }
}
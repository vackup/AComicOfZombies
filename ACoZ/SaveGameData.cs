using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Platformer
{
#if WINDOWS && !NETFX_CORE
    [Serializable]
#endif
    public struct SaveGameData
    {
        public Vector2 PlayerPosition;
        public int PlayerLives;

        public int Level;
        public int Score;
        public int NextLifeScore;
        public double SecondsRemaining;

        public List<SaveGameDataItem> Items;
        public List<SaveGameDataEnemy> Enemies;
        public List<SaveGameDataSurvivor> Survivors;

        public float CameraPositionXAxis;
        public float CameraPositionYAxis;
    }
}

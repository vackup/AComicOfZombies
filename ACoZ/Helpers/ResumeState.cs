using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Platformer.Players;
using Platformer.Weapons;

namespace Platformer.Helpers
{
    public class ResumeState
    {
        public Vector2 CameraPosition { get; set; }
        public Vector2 PlayerPosition { get; set; }
        public Vector2 BackgroundPosition { get; set; }
        public int PlayerLives { get; set; }
        public int CurrentLevel { get; set; }
        public int Score { get; set; }
        public Weapon PrimaryWeapon { get; set; }
        public Weapon SecondaryWeapon { get; set; }
        public List<Weapon> WeaponInventory { get; set; }
        public PlayerInfo SelectedPlayerInfo { get; set; }

        public int NextLife { get; set; }

        public Dictionary<int, int> AmmoInventory;

        public void LoadContent(ContentManager contentManager)
        {
            if (PrimaryWeapon != null) PrimaryWeapon.LoadSoundEffects(contentManager);
            if (SecondaryWeapon != null) SecondaryWeapon.LoadSoundEffects(contentManager);
        }
    }
}

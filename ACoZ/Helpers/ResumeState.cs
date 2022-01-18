using System.Collections.Generic;
using ACoZ.Players;
using ACoZ.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ACoZ.Helpers
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
            if (this.PrimaryWeapon != null) this.PrimaryWeapon.LoadSoundEffects(contentManager);
            if (this.SecondaryWeapon != null) this.SecondaryWeapon.LoadSoundEffects(contentManager);
        }
    }
}

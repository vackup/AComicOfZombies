using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktop.Base.Components
{
    public class HealthBar
    {
        private readonly Texture2D _healthBar;

        public HealthBar(ContentManager contentManager)
        {
            _healthBar = contentManager.Load<Texture2D>("HealthBar");
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 characterPosition, int characterHeight, int currentHealth, int totalHealth)
        {
            //Draw the negative space for the health bar
            spriteBatch.Draw(_healthBar, new Rectangle((int)characterPosition.X - _healthBar.Width / 2, (int)characterPosition.Y - characterHeight - 10, _healthBar.Width, 6), new Rectangle(0, 7, _healthBar.Width, 6), Color.Gray);

            //Draw the current health level based on the current Health
            spriteBatch.Draw(_healthBar, new Rectangle((int)characterPosition.X - _healthBar.Width / 2, (int)characterPosition.Y - characterHeight - 10, (int)(_healthBar.Width * ((double)currentHealth / totalHealth)), 6), new Rectangle(0, 7, _healthBar.Width, 6), Color.Red);

            //Draw the box around the health bar
            spriteBatch.Draw(_healthBar, new Rectangle((int)characterPosition.X - _healthBar.Width / 2, (int)characterPosition.Y - characterHeight - 10, _healthBar.Width, 6), new Rectangle(0, 0, _healthBar.Width, 6), Color.White);
        }
    }
}

#region Using Statements

using System;
using System.Collections.Generic;
using ACoZ.ScreenManagers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#if WINDOWS_PHONE || IPHONE
using Mobile.Base.ScreenSystem;
#elif SILVERLIGHT
using Web.Base.ScreenSystem;
#elif WINDOWS
using Desktop.Base.ScreenSystem;
#endif

#endregion

namespace ACoZ.Screens
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public abstract class MenuScreen : GameScreen
    {
        #region Fields

#if WINDOWS_PHONE || IPHONE
        // the number of pixels to pad above and below menu entries for touch input
        const int MENU_ENTRY_PADDING = 10;
#endif

        protected List<MenuEntry> menuEntries = new List<MenuEntry>();
        protected int selectedEntry = 0;
        protected string menuTitle;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return this.menuEntries; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
//#if WINDOWS_PHONE || IPHONE
//            // menus generally only need Tap for menu selection
//            EnabledGestures = GestureType.Tap;
//#endif

            this.menuTitle = menuTitle;

            this.TransitionOnTime = TimeSpan.FromSeconds(0.5);
            this.TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        #endregion

        #region Handle Input

#if WINDOWS_PHONE || IPHONE
        /// <summary>
        /// Allows the screen to create the hit bounds for a particular menu entry.
        /// </summary>
        protected virtual Rectangle GetMenuEntryHitBounds(MenuEntry entry)
        {
            // the hit bounds are the entire width of the screen, and the height of the entry
            // with some additional padding above and below.
            return new Rectangle(
                0,
                (int)entry.Position.Y - MENU_ENTRY_PADDING,
                ScreenManager.GraphicsDevice.Viewport.Width,
                entry.GetHeight(this) + (MENU_ENTRY_PADDING * 2));
        }
#endif

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputHelper input)
        {
			
#if WINDOWS_PHONE || IPHONE
			// we cancel the current menu screen if the user presses the back button
            PlayerIndex player;
            if (input.IsNewButtonPress(Buttons.Back, ControllingPlayer, out player))
            {
                OnCancel(player);
            }
			
			
            // look for any taps that occurred and select any entries that were tapped
            foreach (GestureSample gesture in input.Gestures)
            {
				if (gesture.GestureType == GestureType.Tap)
				{
                    // convert the position to a Point that we can test against a Rectangle
                    Point tapLocation = new Point((int)gesture.Position.X, (int)gesture.Position.Y);
			//if (input.TouchState.Count > 0)
            //{
                //TouchLocation location = input.TouchState[0];
				//Point tapLocation = new Point((int)location.Position.X, (int)location.Position.Y);

                // iterate the entries to see if any were tapped
                for (int i = 0; i < menuEntries.Count; i++)
                {
                    MenuEntry menuEntry = menuEntries[i];

                    if (GetMenuEntryHitBounds(menuEntry).Contains(tapLocation))
                    {
                        // select the entry. since gestures are only available on Windows Phone,
                        // we can safely pass PlayerIndex.One to all entries since there is only
                        // one player on Windows Phone.
                        OnSelectEntry(i, PlayerIndex.One);
                    }
                }				
			//}
                }
            }
            
#else
            // Move to the previous menu entry?
            if (input.IsMenuUp(this.ControllingPlayer))
            {
                this.selectedEntry--;

                if (this.selectedEntry < 0)
                    this.selectedEntry = this.menuEntries.Count - 1;
            }

            // Move to the next menu entry?
            if (input.IsMenuDown(this.ControllingPlayer))
            {
                this.selectedEntry++;

                if (this.selectedEntry >= this.menuEntries.Count)
                    this.selectedEntry = 0;
            }

            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            if (input.IsMenuSelect(this.ControllingPlayer, out playerIndex))
            {
                this.OnSelectEntry(this.selectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel(this.ControllingPlayer, out playerIndex))
            {
                this.OnCancel(playerIndex);
            }
#endif
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            this.menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            this.ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            this.OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            var transitionOffset = (float)Math.Pow(this.TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
#if !IPHONE
			var position = new Vector2(0f, 175f);
#else
			var position = new Vector2(0f, 85f);
#endif

            // update each menu entry's location in turn
            foreach (var menuEntry in this.menuEntries)
            {
                // each entry is to be centered horizontally
                position.X = this.ScreenManager.GraphicsDevice.Viewport.Width / 2 - menuEntry.GetWidth(this) / 2;

                if (this.ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 512;

                // set the entry's position
                menuEntry.Position = position;

#if WINDOWS_PHONE || IPHONE
                // move down for the next entry the size of this entry plus our padding
                position.Y += menuEntry.GetHeight(this) + (MENU_ENTRY_PADDING * 2);
#else
                // move down for the next entry the size of this entry
                position.Y += menuEntry.GetHeight(this);
#endif
            }
        }
		
		bool down;
		Vector2 position;

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < this.menuEntries.Count; i++)
            {
                bool isSelected = this.IsActive && (i == this.selectedEntry);

                this.menuEntries[i].Update(this, isSelected, gameTime);
            }
			
			MouseState state = Mouse.GetState();
			this.position = new Vector2(state.X, state.Y);
			this.down = state.LeftButton == ButtonState.Pressed;
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {			
            // make sure our entries are in the right place before we draw them
            this.UpdateMenuEntryLocations();

            GraphicsDevice graphics = this.ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = this.ScreenManager.SpriteBatch;
            SpriteFont font = this.ScreenManager.SpriteFonts.TextFont;

            //Vector2 position = new Vector2(100, 150);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            //float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            //if (ScreenState == ScreenState.TransitionOn)
            //    position.X -= transitionOffset * 256;
            //else
            //    position.X += transitionOffset * 512;

            spriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < this.menuEntries.Count; i++)
            {
                MenuEntry menuEntry = this.menuEntries[i];

                bool isSelected = this.IsActive && (i == this.selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);

                //position.Y += menuEntry.GetHeight(this);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            var transitionOffset = (float)Math.Pow(this.TransitionPosition, 2);

            // Draw the menu title centered on the screen
            var titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            var titleOrigin = font.MeasureString(this.menuTitle) / 2;
            var titleColor = new Color(192, 192, 192) * this.TransitionAlpha;
            const float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, this.menuTitle, titlePosition, titleColor, 0, titleOrigin, titleScale, SpriteEffects.None, 0);
			
			// TODO: remover esto que lo puse para testear los clicks
			spriteBatch.DrawString(font, this.position.ToString(), this.position + new Vector2(5), this.down ? Color.Red : Color.Yellow);
			
            spriteBatch.End();
        }


        #endregion
    }
}

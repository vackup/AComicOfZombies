using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktop.Base.Components
{
    public class FramePerSeconds : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        private SpriteFont _videoFont;
        private float _elapsedTime, _totalFrames, _fps;
        private Vector2 _textPosition;
        const string DISPLAY_TEXT = "FPS";
        
        public bool ShowFps { get; set; }

        public FramePerSeconds(Game game)
            : base(game)
        {
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            // Load the required font
            _videoFont = Game.Content.Load<SpriteFont>("Fonts/FancyFont");

            _textPosition = new Vector2(0,
                                        Game.GraphicsDevice.Viewport.Height - _videoFont.MeasureString(DISPLAY_TEXT).Y);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _elapsedTime += (float) gameTime.ElapsedGameTime.TotalSeconds;
            _totalFrames++;

            if (_elapsedTime >= 1.0f)
            {
                _fps = _totalFrames;
                _totalFrames = 0;
                _elapsedTime = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (ShowFps)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                _spriteBatch.DrawString(_videoFont, string.Format("{0}={1}", DISPLAY_TEXT, _fps),
                                               _textPosition,
                                               Color.Red,
                                               0f,
                                               Vector2.Zero,
                                               1.0f,
                                               SpriteEffects.None,
                                               0);
                _spriteBatch.End();
            }
            base.Draw(gameTime);
        }
    }
}
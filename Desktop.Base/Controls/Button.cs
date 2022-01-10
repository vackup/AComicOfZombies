using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Desktop.Base.ScreenSystem;

namespace Desktop.Base.Controls
{
    /// <summary>
    /// Button Control for XNA. Combine it with PanelControl or ScrollingPanelControl for menu like feel.
    /// </summary>
    public class Button : Control
    {
        #region Events
        public delegate void ClickHandler(Button sender);
        /// <summary>
        /// Occurs when [on clicked].
        /// </summary>
        /// <remarks></remarks>
        public event ClickHandler OnClicked;

        #endregion

        #region Enums
        public enum ButtonState
        {
            Normal,
            Clicked
        }
        public enum TextAlign
        {
            Left,
            Right,
            Centre
        }
        public enum FontSize
        {
            Small,
            Medium,
            Big
        }
        #endregion

        #region Attributes and Properties

        private SpriteFont _font;
        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        /// <remarks></remarks>
        public SpriteFont Font
        {
            get { return _font; }
            set { _font = value; _font.LineSpacing = 30; }
        }

        private bool _textVisible = true;
        /// <summary>
        /// Gets or sets a value indicating whether [text visible].
        /// </summary>
        /// <value><c>true</c> if [text visible]; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool TextVisible
        {
            get { return _textVisible; }
            set { _textVisible = value; }
        }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>The display text.</value>
        /// <remarks></remarks>
        public string DisplayText { get; set; }

        public Rectangle TextureRectangle { get; set; }

        /// <summary>
        /// Gets or sets the text rotation.
        /// </summary>
        /// <value>The text rotation.</value>
        /// <remarks></remarks>
        public float TextRotation { get; set; }

        private FontSize _textSize = FontSize.Medium;
        /// <summary>
        /// Gets or sets the size of the text.
        /// </summary>
        /// <value>The size of the text.</value>
        /// <remarks></remarks>
        public FontSize TextSize
        {
            get { return _textSize; }
            set { _textSize = value; }
        }

        private TextAlign _textAlignment = TextAlign.Centre;
        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        /// <value>The text alignment.</value>
        /// <remarks></remarks>
        public TextAlign TextAlignment
        {
            get { return _textAlignment; }
            set { _textAlignment = value; }
        }

        private bool _clickAreaSpecific;
        /// <summary>
        /// Gets or sets a value indicating whether [click area specific].
        /// </summary>
        /// <value><c>true</c> if [click area specific]; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool ClickAreaSpecific
        {
            get { return _clickAreaSpecific; }
            set { _clickAreaSpecific = value; }
        }

        private Rectangle _clickArea;
        /// <summary>
        /// The area where this button can be clicked, if this is not specified it uses the Normal Textures Bounds
        /// </summary>
        public Rectangle ClickArea
        {
            get
            {
                if (!_clickAreaSpecific)
                {
                    if (_txNormalButton != null && _clickArea == Rectangle.Empty)
                    {
                        _clickArea = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
                    }
                }
                return _clickArea;
            }
            set { _clickArea = value; _clickAreaSpecific = true; }
        }

        private Texture2D _txNormalButton;
        /// <summary>
        /// Gets or sets the normal button state texture.
        /// </summary>
        /// <value>The normal button state texture.</value>
        /// <remarks></remarks>
        public Texture2D NormalButtonTexture
        {
            get { return _txNormalButton; }
            set { _txNormalButton = value; }
        }

        private Texture2D _txClickedButton;
        /// <summary>
        /// Gets or sets the clicked button state texture.
        /// </summary>
        /// <value>The clicked button state texture.</value>
        /// <remarks></remarks>
        public Texture2D ClickedButtonTexture
        {
            get { return _txClickedButton; }
            set { _txClickedButton = value; }
        }

        /// <summary>
        /// Gets or sets the Current ButtonState. 
        /// </summary>
        /// <value>The Current ButtonState.</value>
        private ButtonState CurrentButtonState { get; set; }

        /// <summary>
        /// Gets or sets the Previous ButtonState. 
        /// </summary>
        /// <value>The Previous ButtonState.</value>
        private ButtonState PreviousButtonState { get; set; }

        private int _height = 50;
        public int Height
        {
            get
            {
                if (_height == 0)
                    _height = TextHeight + 10;
                return _height;
            }
            set { _height = value; }
        }
        private int _width = 50;
        public int Width
        {
            get
            {
                if (_width == 0)
                    _width = TextWidth + 10;
                return _width;
            }
            set { _width = value; }
        }

        private Color _clickedForeground = Color.Black;
        /// <summary>
        /// Gets or sets the clicked forground Color.
        /// </summary>
        /// <value>The clicked forground Color.</value>
        /// <remarks></remarks>
        public Color ClickedForeground
        {
            get { return _clickedForeground; }
            set { _clickedForeground = value; }
        }

        private Color _foreground = Color.Black;
        /// <summary>
        /// Gets or sets the forground Color.
        /// </summary>
        /// <value>The forground Color.</value>
        /// <remarks></remarks>
        public Color Foreground
        {
            get { return _foreground; }
            set { _foreground = value; }
        }

        /// <summary>
        /// Gets the width of the text.
        /// </summary>
        public int TextWidth
        {
            get
            {
                if (string.IsNullOrEmpty(DisplayText))
                    return 0;
                return (int)(Font.MeasureString(DisplayText).X * ScaleText());
            }
        }
        /// <summary>
        /// Gets the height of the text.
        /// </summary>
        public int TextHeight
        {
            get
            {
                if (string.IsNullOrEmpty(DisplayText))
                    return 0;
                return (int)(Font.MeasureString(DisplayText).Y * ScaleText());
            }
        }

        public Vector2 TapPosition = Vector2.Zero;
        private Vector2 _lastDrawOffset = Vector2.Zero;
        
        /// <summary>
        /// Gets or sets the tag.
        /// Used to store extra information we need to retrieve when button gets clicked.
        /// </summary>
        /// <value>The tag.</value>
        /// <remarks></remarks>
        public object Tag { get; set; }
        #endregion
        
        #region Init & Content

        public void Initialize()
        {
            PreviousButtonState = CurrentButtonState = ButtonState.Normal;
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        public void LoadContent()
        {
            //_content = _Game.Content;

            //Check whether we have all the required textures and fonts available.
            if (NormalButtonTexture == null)
            {
                throw new Exception("No Texture provided for Button.");
            }
            if (ClickedButtonTexture == null)
                ClickedButtonTexture = NormalButtonTexture;

            if (!string.IsNullOrEmpty(DisplayText))
                if (Font == null)
                {
                    throw new Exception("No Font provided for Button.");
                }

            //_sBatch = new SpriteBatch(_Game.GraphicsDevice);

        }
        #endregion

        #region Game Events

        public override void HandleInput(ScreenSystem.InputHelper input)
        {
            HandleInput(input.CurrentMouseState);

            //Handle inputs
            base.HandleInput(input);
        }

        public override void HandleInput(ScreenSystem.InputHelper input, Matrix traslationMatrix)
        {
            HandleInput(input.CurrentMouseState, traslationMatrix);

            //Handle inputs
            base.HandleInput(input, traslationMatrix);
        }

        private void HandleInput(MouseState touches, Matrix traslationMatrix)
        {
            PreviousButtonState = CurrentButtonState;

            if (touches.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && 
                HandleTap(new Vector2(touches.X, touches.Y), traslationMatrix))
            {
                CurrentButtonState = ButtonState.Clicked;
                return;
            }

            CurrentButtonState = ButtonState.Normal;
        }

        public override void Update(GameTime gameTime)
        {
            //TapPosition gets set from HandleInput if tapped in current button's click area.
            //If We have TapPosition then record the time, this time will be used to render the button's clicked texture.
            //if (TapPosition != Vector2.Zero)
            //{
            //    _lastTap = gameTime.TotalGameTime;
            //    TapPosition = Vector2.Zero;
            //}

            //If we are in Clicked state and we have shown clicked texture for specified period of time then get back to normal state.
            //Also raise the event now.
            //If we don't do this then button won't look like button as, when user tap on button it raise the event and 
            //clicked texture won't be shown for enough amount of time to be visible.
            //if (CurrentButtonState == ButtonState.Clicked && gameTime.TotalGameTime.Subtract(_lastTap).Milliseconds > TimeToShowClickedTexture)
            //if (CurrentButtonState == ButtonState.Clicked && PreviousButtonState == ButtonState.Normal)
            if (PreviousButtonState == ButtonState.Clicked && CurrentButtonState == ButtonState.Normal)
            {
                //PreviousButtonState = CurrentButtonState;
                //CurrentButtonState = ButtonState.Normal;
                if (OnClicked != null)
                    OnClicked(this);
            }

            base.Update(gameTime);
        }

        public override void Draw(DrawContext context)
        {
            if (Visible)
            {
                if (context.SpriteBatch == null)
                {
                    //spriteBatch = _sBatch;
                    //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    Draw(context);
                    //spriteBatch.End();
                }
                else
                {
                    //Get Texture to render based on the ButtonState.
                    Texture2D toPaint = StateToTexture();
                    //This gets updated if button is in ScrollablePanel
                    Vector2 pos = context.DrawOffset;
                    _lastDrawOffset = context.DrawOffset;

                    //Draw Texture
                    var spriteBatch = context.SpriteBatch;

                    if (toPaint != null)
                    {
                        spriteBatch.Draw(toPaint, new Rectangle((int)pos.X, (int)pos.Y, Width, Height), TextureRectangle, Color.White);
                    }

                    //Draw Text
                    DrawText(spriteBatch, context.DrawOffset);

                }
            }

            base.Draw(context);
        }

        private void DrawText(SpriteBatch spriteBatch, Vector2 drawOffset)
        {
            //Draw Text if Visible
            if (!TextVisible) return;

            //We should have some display text to render
            if (string.IsNullOrEmpty(DisplayText)) return;

            //Get Position based on the TextAlignment in horizontal side.
            var pos = new Vector2(StartPositionX(drawOffset), drawOffset.Y + Height / 2 - TextHeight / 2);

            //If clicked state then draw with ClickedForeground else with normal texture.
            //spriteBatch.DrawString(Font, DisplayText, pos,
            //                       CurrentButtonState == ButtonState.Clicked ? ClickedForeground : Foreground,
            //                       TextRotation, Vector2.Zero, ScaleText(), SpriteEffects.None, 0);

            TextHelper.DrawShadowedString(spriteBatch, Font, DisplayText, pos,
                                   CurrentButtonState == ButtonState.Clicked ? ClickedForeground : Foreground,
                                   TextRotation, ScaleText());
        }

        /// <summary>
        /// Called when the Size property is read and sizeValid is false. Call base.ComputeSize() to compute the
        /// size (actually the lower-right corner) of all child controls.
        /// </summary>
        override public Vector2 ComputeSize()
        {
            return new Vector2(_width, _height);
        }
        #endregion

        #region Helpers

        /// <summary>
        /// States to texture.
        /// </summary>
        /// <returns>Texture for the current button state.</returns>
        private Texture2D StateToTexture()
        {
            Texture2D toPaint = null;
            switch (CurrentButtonState)
            {
                case ButtonState.Normal: toPaint = _txNormalButton; break;
                case ButtonState.Clicked: toPaint = _txClickedButton; break;
            }
            return toPaint;
        }
        /// <summary>
        /// Scales the text.
        /// </summary>
        /// <returns>Size of text according to TextSize property</returns>
        private float ScaleText()
        {
            switch (TextSize)
            {
                case FontSize.Small:
                    return 0.75f;
                case FontSize.Medium:
                    return 1;
                case FontSize.Big:
                    return 1.25f;
                default:
                    return 1;
            }
        }
        /// <summary>
        /// Starts the position X.
        /// </summary>
        /// <param name="drawOffset">The draw offset.</param>
        /// <returns>Start Position for X direction based on text alignment property</returns>
        private float StartPositionX(Vector2 drawOffset)
        {
            switch (TextAlignment)
            {
                case TextAlign.Left:
                    return drawOffset.X + 50;
                case TextAlign.Right:
                    return drawOffset.X + Width - TextWidth - 50;
                case TextAlign.Centre:
                    return drawOffset.X + Width / 2 - TextWidth / 2;
                default:
                    return drawOffset.X + Width / 2 - TextWidth / 2;
            }

        }

        /// <summary>
        /// Handles the input.
        /// </summary>
        /// <param name="touches"></param>
        public void HandleInput(MouseState touches)
        {
            HandleInput(touches, Matrix.Identity);
        }

        private bool HandleTap(Vector2 tapPosition, Matrix transformationMatrix)
        {
            //Note the TapPosition to be used in game methods.
            TapPosition = tapPosition;

            //Find if the Tap in inside the button's click area.
            var rect = ClickAreaSpecific
                                 ? ClickArea
                                 : new Rectangle((int) _lastDrawOffset.X, (int) _lastDrawOffset.Y, Width, Height);

            //var mousePos = new Point((int)TapPosition.X, (int)TapPosition.Y);

            var mouseWorldPosition = Vector2.Transform(new Vector2(TapPosition.X, TapPosition.Y),
                                    Matrix.Invert(transformationMatrix));
            
            //return rect.Contains(mousePos);
            return rect.Contains((int)mouseWorldPosition.X, (int)mouseWorldPosition.Y);
        }
        #endregion

    }

}

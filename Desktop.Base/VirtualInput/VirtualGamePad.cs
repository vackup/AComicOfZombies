using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Desktop.Base.VirtualInput
{
    public class VirtualGamePad
    {
		//private static VirtualGamePad _instance;
		private float _thumbStickRadius = 20*20;	
		private bool _visible;
		private List<VirtualButtonDefinition> _buttonsDefinitions;
		private ThumbStickDefinition _leftThumbDefinition,_rightThumbDefinition;
		private Color _alphaColor = Color.DarkGray;		
		private int _buttons;
		private Vector2 _leftStick, _rightStick;
		
		public VirtualGamePad()
		{
            //_instance = new VirtualGamePad();

			_visible = true;
			_buttonsDefinitions = new List<VirtualButtonDefinition>();
			
			// Set the transparency Level
			_alphaColor.A = 100;
	
			Reset();
		}

        //private static VirtualGamePad Instance 
        //{
        //    get { return _instance ?? (_instance = new VirtualGamePad()); }
        //}
		
		private void Reset()
		{
			_buttons = 0;
			_leftStick = Vector2.Zero;
			_rightStick = Vector2.Zero;
			
			// reset thumbsticks
			if (_leftThumbDefinition != null) 
			{
				_leftThumbDefinition.Offset = Vector2.Zero;
			}
			if (_rightThumbDefinition != null) 
			{
				_rightThumbDefinition.Offset = Vector2.Zero;
			}
		}
		
		public bool Visible 
		{
			get 
			{
                return _visible;
			}
			set 
			{
				Reset();
				_visible = value;
			}
		}
		
        public static GamePadCapabilities GetCapabilities(PlayerIndex playerIndex)
        {
            var capabilities = new GamePadCapabilities
                                                   {
                                                       IsConnected = (playerIndex == PlayerIndex.One),
                                                       HasAButton = true,
                                                       HasBButton = true,
                                                       HasXButton = true,
                                                       HasYButton = true,
                                                       HasBackButton = true,
                                                       HasLeftXThumbStick = true,
                                                       HasLeftYThumbStick = true,
                                                       HasRightXThumbStick = true,
                                                       HasRightYThumbStick = true
                                                   };

            return capabilities;
        }

        //public static void Update()
        //{
        //    Instance.TouchesBegan(TouchPanel.GetState());
        //}

        public VirtualGamePadState GetState(PlayerIndex playerIndex)
        {
            /* if (playerIndex != PlayerIndex.One) 
			{
				throw new NotSupportedException("Only one player!");
			}*/

            TouchesBegan(Mouse.GetState());

            return new VirtualGamePadState((VirtualButtons) _buttons, _leftStick, _rightStick);
        }

        //public static bool SetVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor)
        //{	
        //    if (playerIndex != PlayerIndex.One) 
        //    {
        //        throw new NotSupportedException("Only one player!");
        //    }
			
        //    //SystemSound.Vibrate.PlaySystemSound();
        //    return true;
        //}
		
		public ThumbStickDefinition LeftThumbStickDefinition
		{
			get 
			{
				return _leftThumbDefinition;
			}
			set
			{
				_leftThumbDefinition = value;
			}
		}
		
		public ThumbStickDefinition RightThumbStickDefinition
		{
			get 
			{
				return _rightThumbDefinition;
			}
			set
			{
				_rightThumbDefinition = value;
			}
		}
		
		private bool CheckButtonHit(VirtualButtonDefinition theVirtualButton, Vector2 location)
		{
			var buttonRect = new Rectangle((int) theVirtualButton.Position.X,(int)theVirtualButton.Position.Y,theVirtualButton.TextureRect.Width, theVirtualButton.TextureRect.Height);

            return buttonRect.Contains((int)location.X, (int)location.Y); 
		}
		
		private bool CheckThumbStickHit(ThumbStickDefinition theStick, Vector2 location)
		{
			var stickPosition = theStick.Position + theStick.Offset;
			var thumbRect = new Rectangle((int) stickPosition.X,(int)stickPosition.Y,theStick.TextureRect.Width, theStick.TextureRect.Height);
            return thumbRect.Contains((int)location.X, (int)location.Y);
		}

        internal void TouchesBegan(MouseState touch)//, MonoTouch.UIKit.UIEvent e)
        {
            // Reset State		
            Reset();

            // Check where is the touch
            //UITouch[] touchesArray = touches.ToArray<UITouch>();
            //foreach (UITouch touch in touchesArray)
            //foreach (var touch in touches)
            //{
                //Vector2 location = new Vector2(touch.LocationInView(touch.View));
                Vector2 location = new Vector2(touch.X, touch.Y);

                // Check where is the touch
                bool hitInButton = false;

                if (Visible)
                {
                    foreach (VirtualButtonDefinition button in _buttonsDefinitions)
                    {
                        hitInButton |= UpdateButton(button, location);
                    }
                }
                if (!hitInButton)
                {
                    // check the left thumbstick
                    if (Visible && (_leftThumbDefinition != null) && (CheckThumbStickHit(_leftThumbDefinition, location)))
                    {
                        _leftThumbDefinition.InitialHit = location;
                    }
                    //else
                    //{
                    //    // check the right thumbstick
                    //    if (Visible && (_rightThumbDefinition != null) && (CheckThumbStickHit(_rightThumbDefinition, location)))
                    //    {
                    //        _rightThumbDefinition.InitialHit = location;
                    //    }
                    //    else // Handle mouse 
                    //    {
                    //        //Mouse.SetPosition((int)location.X, (int)location.Y);
                    //        throw new NotImplementedException("Mouse.SetPosition");
                    //    }
                    //}

                }
            //}
        }

        //internal void TouchesCancelled(TouchCollection touches)//, MonoTouch.UIKit.UIEvent e)
        //{
        //    // do nothing
        //}

        //internal void TouchesMoved(TouchCollection touches)//, MonoTouch.UIKit.UIEvent e)
        //{
        //    //UITouch[] touchesArray = touches.ToArray<UITouch>();
        //    //foreach (UITouch touch in touchesArray)
        //    foreach (var touch in touches)
        //    {
        //        //Vector2 location = new Vector2(touch.LocationInView(touch.View));
        //        Vector2 location = touch.Position;

        //        // Check if touch any button
        //        bool hitInButton = false;
        //        if (Visible)
        //        {
        //            foreach (VirtualButtonDefinition button in _buttonsDefinitions)
        //            {
        //                hitInButton |= UpdateButton(button, location);
        //            }
        //        }

        //        if (!hitInButton)
        //        {
        //            if (Visible && (_leftThumbDefinition != null) && (CheckThumbStickHit(_leftThumbDefinition, location)))
        //            {
        //                Vector2 movement = location - LeftThumbStickDefinition.InitialHit;

        //                // Keep the stick in the "hole" 
        //                float radius = (movement.X * movement.X) + (movement.Y * movement.Y);

        //                if (radius <= _thumbStickRadius)
        //                {
        //                    _leftThumbDefinition.Offset = movement;
        //                    _leftStick = new Vector2(movement.X / 20, movement.Y / -20);
        //                }
        //            }
        //            else
        //            {
        //                // reset left thumbstick
        //                if (_leftThumbDefinition != null)
        //                {
        //                    _leftThumbDefinition.Offset = Vector2.Zero;
        //                    _leftStick = Vector2.Zero;
        //                }

        //                if (Visible && (_rightThumbDefinition != null) && (CheckThumbStickHit(_rightThumbDefinition, location)))
        //                {
        //                    Vector2 movement = location - _rightThumbDefinition.InitialHit;

        //                    // Keep the stick in the "hole" 
        //                    float radius = (movement.X * movement.X) + (movement.Y * movement.Y);

        //                    if (radius <= _thumbStickRadius)
        //                    {
        //                        _rightThumbDefinition.Offset = movement;
        //                        _rightStick = new Vector2(movement.X / 20, movement.Y / -20);
        //                    }
        //                }
        //                //else
        //                //{
        //                //    // reset right thumbstick
        //                //    if (_rightThumbDefinition != null)
        //                //    {
        //                //        _rightThumbDefinition.Offset = Vector2.Zero;
        //                //        _rightStick = Vector2.Zero;
        //                //    }

        //                //    // Handle the mouse
        //                //    //Mouse.SetPosition((int)location.X, (int)location.Y);
        //                //    throw new NotImplementedException("Mouse.SetPosition");
        //                //}
        //            }
        //        }
        //    }
        //}

        //internal void TouchesEnded(TouchCollection touches)//, MonoTouch.UIKit.UIEvent e)
        //{
        //    //UITouch[] touchesArray = touches.ToArray<UITouch>();
        //    //foreach (UITouch touch in touchesArray)
        //    foreach (var touch in touches)
        //    {
        //        //Vector2 location = new Vector2(touch.LocationInView(touch.View).X, touch.LocationInView(touch.View).Y);
        //        Vector2 location = touch.Position;

        //        // Check where is the touch
        //        if (Visible)
        //        {
        //            foreach (VirtualButtonDefinition button in _buttonsDefinitions)
        //            {
        //                if (CheckButtonHit(button, location))
        //                {
        //                    _buttons &= ~(int)button.Type;
        //                }
        //            }
        //            if ((_leftThumbDefinition != null) && (CheckThumbStickHit(_leftThumbDefinition, location)))
        //            {
        //                LeftThumbStickDefinition.Offset = Vector2.Zero;
        //                _leftStick = Vector2.Zero;
        //            }
        //            if ((_rightThumbDefinition != null) && (CheckThumbStickHit(_rightThumbDefinition, location)))
        //            {
        //                _rightThumbDefinition.Offset = Vector2.Zero;
        //                _rightStick = Vector2.Zero;
        //            }
        //        }
        //    }
        //}		
		
		private bool UpdateButton (VirtualButtonDefinition virtualButton, Vector2 location)
		{
			var hitInButton = CheckButtonHit (virtualButton, location);
			
			if (hitInButton) 
			{
				_buttons |= (int)virtualButton.Type;
			}
			return hitInButton;
		}
		 
		#region render virtual gamepad
		
		public List<VirtualButtonDefinition> ButtonsDefinitions
		{
			get 
			{
				return _buttonsDefinitions;
			}
		}
		
		public void Draw(GameTime gameTime, SpriteBatch batch )
		{		
			Render(gameTime,batch);		
		}
		
		internal void Render(GameTime gameTime, SpriteBatch batch)
		{
			// render buttons
			foreach (VirtualButtonDefinition button in _buttonsDefinitions)
			{
				RenderButton(button, batch);
			}			
			
			// Render the thumbsticks
			if (_leftThumbDefinition != null)
			{
				RenderThumbStick(_leftThumbDefinition, batch);
			}
			if (_rightThumbDefinition != null)
			{
				RenderThumbStick(_rightThumbDefinition, batch);
			}
		}
		
		private void RenderButton(VirtualButtonDefinition theVirtualButton, SpriteBatch batch)
		{
			if (batch == null)
			{
				throw new InvalidOperationException("SpriteBatch not set.");
			}
			batch.Draw(theVirtualButton.Texture,theVirtualButton.Position,theVirtualButton.TextureRect,_alphaColor);
		}
		
		private void RenderThumbStick(ThumbStickDefinition theStick, SpriteBatch batch)
		{
			if (batch == null)
			{
				throw new InvalidOperationException("SpriteBatch not set.");
			}
			batch.Draw(theStick.Texture,theStick.Position + theStick.Offset,theStick.TextureRect,_alphaColor);
		}
		#endregion
	}
	
}

using Microsoft.Xna.Framework;

namespace Desktop.Base.VirtualInput
{
	public struct VirtualGamePadState
    {
        private VirtualGamePadThumbSticks _thumbs;
        private VirtualButtons _virtualButtons;
		private VirtualGamePadDPad _dPad;
		private VirtualGamePadTriggers _triggers;

		internal VirtualGamePadState(VirtualButtons virtualButtons, Vector2 LeftStick, Vector2 RightStick)
		{
			_virtualButtons = virtualButtons;
			_thumbs = new VirtualGamePadThumbSticks(LeftStick,RightStick);
		}
		
        public VirtualGamePadButtons Buttons
        {
            get
            {
                return new VirtualGamePadButtons(_virtualButtons);
            }
        }
 
        public bool IsConnected
        {
            get
            {
                return true;
            }
        }
       
        public VirtualGamePadThumbSticks ThumbSticks
        {
            get
            {
                return this._thumbs;
            }
        }
     
        public bool IsButtonDown(VirtualButtons virtualButton)
        {
            return ((_virtualButtons & virtualButton) == virtualButton);
        }

        public bool IsButtonUp(VirtualButtons virtualButton)
        {
            return !this.IsButtonDown(virtualButton);
        }
			
		public VirtualGamePadDPad DPad 
		{ 
			get
			{
				return _dPad;
			}
		}
		
		public VirtualGamePadTriggers Triggers 
		{ 
			get
			{
				return _triggers;
			}
		}
    }

}


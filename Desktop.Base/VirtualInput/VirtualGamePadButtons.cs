namespace Desktop.Base.VirtualInput
{
    public struct VirtualGamePadButtons
    {
        private VirtualButtons _virtualButtons;
		
        public VirtualGamePadButtons(VirtualButtons virtualButtons)
        {
            _virtualButtons = virtualButtons;
        }

        public VirtualButtonState A
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.A) == VirtualButtons.A) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }
        public VirtualButtonState B
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.B) == VirtualButtons.B) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }
        public VirtualButtonState Back
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.Back) == VirtualButtons.Back) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }
        public VirtualButtonState X
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.X) == VirtualButtons.X) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }
        public VirtualButtonState Y
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.Y) == VirtualButtons.Y) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }
        public VirtualButtonState Start
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.Start) == VirtualButtons.Start) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }
        public VirtualButtonState LeftShoulder
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.LeftShoulder) == VirtualButtons.LeftShoulder) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }
        public VirtualButtonState LeftStick
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.LeftStick) == VirtualButtons.LeftStick) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }
        public VirtualButtonState RightShoulder
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.RightShoulder) == VirtualButtons.RightShoulder) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }
        public VirtualButtonState RightStick
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.RightStick) == VirtualButtons.RightStick) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }
        public VirtualButtonState BigVirtualButton
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.BigButton) == VirtualButtons.BigButton) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }

        public VirtualButtonState DPadLeft
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.DPadLeft) == VirtualButtons.DPadLeft) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }

        public VirtualButtonState DPadRight
        {
            get
            {
                return ((_virtualButtons & VirtualButtons.DPadRight) == VirtualButtons.DPadRight) ? VirtualButtonState.Pressed : VirtualButtonState.Released;
            }
        }
    }

}


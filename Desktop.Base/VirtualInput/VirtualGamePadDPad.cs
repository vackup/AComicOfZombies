namespace Desktop.Base.VirtualInput
{
	public struct VirtualGamePadDPad
	{
		public VirtualGamePadDPad (
         VirtualButtonState upValue,
         VirtualButtonState downValue,
         VirtualButtonState leftValue,
         VirtualButtonState rightValue )
		{
		}
		
		public VirtualButtonState Down 
		{ 
			get
			{
				return VirtualButtonState.Released;
			}
		}
		
		public VirtualButtonState Left 
		{ 
			get
			{
				return VirtualButtonState.Released;
			}
		}
		
		public VirtualButtonState Right 
		{ 
			get
			{
				return VirtualButtonState.Released;
			}
		}
		
		public VirtualButtonState Up 
		{ 
			get
			{
				return VirtualButtonState.Released;
			}
		}
	}
}

using Microsoft.Xna.Framework;

namespace Desktop.Base.VirtualInput
{
    public struct VirtualGamePadThumbSticks
    {
        private readonly Vector2 _left;
        private readonly Vector2 _right;

		public VirtualGamePadThumbSticks(Vector2 leftPosition, Vector2 rightPosition)
		{
			_left = leftPosition;
			_right = rightPosition;
		}
		
        public Vector2 Left
        {
            get
            {
                return _left;
            }
        }
		
        public Vector2 Right
        {
            get
            {
                return _right;
            }
        }
    }

}


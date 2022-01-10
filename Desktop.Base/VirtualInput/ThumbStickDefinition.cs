using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktop.Base.VirtualInput
{
	public class ThumbStickDefinition
	{
		public Texture2D Texture {get;set;}
		public Vector2 Position {get;set;}
		public Rectangle TextureRect {get;set;}
		internal Vector2 InitialHit {get;set;}
		internal Vector2 Offset {get;set;}
	}
}

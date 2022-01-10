using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktop.Base.VirtualInput
{
	public class VirtualButtonDefinition
	{
		public Texture2D Texture {get;set;}
		public Vector2 Position {get;set;}
		public VirtualButtons Type {get;set;}
		public Rectangle TextureRect {get;set;}
	}
}

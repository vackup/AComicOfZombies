//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace Platformer.Helpers
//{
//    public class RectangleOverlay
//    {
//#if WINDOWS_PHONE || WINDOWS || XBOX
//        Texture2D dummyTexture;
//        Color Colori;
//#endif

//        public RectangleOverlay(Color colori, GraphicsDevice graphicsDevice)
//        {
//#if WINDOWS_PHONE || WINDOWS || XBOX
//            Colori = colori;

//            dummyTexture = new Texture2D(graphicsDevice, 1, 1);
//            dummyTexture.SetData(new Color[] { Color.White });
//#endif
//        }

//        public void Draw(SpriteBatch spriteBatch, Rectangle dummyRectangle)
//        {
//#if WINDOWS_PHONE || WINDOWS || XBOX
//            //spriteBatch.Draw(dummyTexture, dummyRectangle, Colori);
//#endif
//        }
//    }
//}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Platformer.Levels
{
    public class Layer
    {
        // These properties store the background texture of the layer and its scroll speed.
        private readonly Texture2D[] _textures;
        public float ScrollRate { get; private set; }

        /// <summary>
        /// This constructor accepts a content manager, a base path to the background asset, and the scroll 
        /// speed of the background layer. Note that each layer has only three segments. 
        /// It loads each segment of the background in the Textures array, and then sets the scroll speed.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="basePath"></param>
        /// <param name="scrollRate"></param>
        /// <param name="segmentsCount"></param>
        public Layer(ContentManager content, string basePath, float scrollRate, int segmentsCount)
        {
            // Todo: ver si no conviene ponerlo en GlobalParameters y hacer como los contenidos de los personajes
#if !IPHONE
            const string backgroundSufix = "@2x";
#else
            const string backgroundSufix = "";
#endif

            _textures = new Texture2D[segmentsCount];
            for (int i = 0; i < segmentsCount; ++i)
                _textures[i] = content.Load<Texture2D>(string.Format("{0}_{1}{2}", basePath, i, backgroundSufix));

            ScrollRate = scrollRate;
        }

        /// <summary>
        /// This method first calculates which of the background segments to draw, and 
        /// then draws them offset by the previously calculated amount.
        /// It is assumed that two segments are enough to cover any screen. 
        /// Si fuera algo mas grande, deberiamos poner mas spriteBatch.Draw para dibujar
        /// mas texturas que cubran toda la pantalla.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="cameraPosition"></param>
        public void Draw(SpriteBatch spriteBatch, float cameraPosition)
        {
            // Assume each segment is the same width.
            var segmentWidth = _textures[0].Width;

            // Calculate which segments to draw and how much to offset them.
            var x = cameraPosition * ScrollRate;
            var leftSegment = (int)Math.Floor(x / segmentWidth);
            var rightSegment = leftSegment + 1;
            x = (x / segmentWidth - leftSegment) * -segmentWidth;

            // Todo: crear material propio para WP como hice con los contenidos de los personajes
            // Hay que hacer esto xq los graficos de iphone retina son de 960*640 y WP tiene una pantalla de 800*480
            // y hay que acomodar la altura
#if WINDOWS_PHONE || WINDOWS
            const float desplazamientoY = -130.0f;
#else
            const float desplazamientoY = 0.0f;
#endif

            spriteBatch.Draw(_textures[leftSegment % _textures.Length], new Vector2((float)Math.Floor(x), desplazamientoY), Color.White);
            spriteBatch.Draw(_textures[rightSegment % _textures.Length], new Vector2((float)Math.Floor(x + segmentWidth), desplazamientoY), Color.White);
        }
    }
}

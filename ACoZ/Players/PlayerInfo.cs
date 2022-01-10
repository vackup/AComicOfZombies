using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Platformer.Helpers;

namespace Platformer.Players
{
    public class PlayerInfo
    {
        private readonly string _textureFile;
        public Texture2D Texture { get; private set; }
        public PlayerType Type { get; private set; }

        public PlayerInfo(PlayerType playerType)
        {
            Type = playerType;

            switch (playerType)
            {
                case PlayerType.GordoMercenario:
                    _textureFile = GlobalParameters.GORDO_MERCENARIO_INFO_TEXTURE;
                    break;
                case PlayerType.Obama:
                    _textureFile = GlobalParameters.OBAMA_INFO_TEXTURE;
                    break;
                default:
                    throw new Exception("Wrong player type");

            }
        }

        public void Init(ContentManager contentManager)
        {
             Texture = contentManager.Load<Texture2D>(_textureFile);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (Texture == null)
            {
                throw new Exception("Objeto no inicializado, llame a init antes de dibujar");
            }

            spriteBatch.Draw(Texture, position, Color.White);
        }
    }
}

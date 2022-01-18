using System;
using ACoZ.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ACoZ.Players
{
    public class PlayerInfo
    {
        private readonly string _textureFile;
        public Texture2D Texture { get; private set; }
        public PlayerType Type { get; private set; }

        public PlayerInfo(PlayerType playerType)
        {
            this.Type = playerType;

            switch (playerType)
            {
                case PlayerType.GordoMercenario:
                    this._textureFile = GlobalParameters.GORDO_MERCENARIO_INFO_TEXTURE;
                    break;
                case PlayerType.Obama:
                    this._textureFile = GlobalParameters.OBAMA_INFO_TEXTURE;
                    break;
                default:
                    throw new Exception("Wrong player type");

            }
        }

        public void Init(ContentManager contentManager)
        {
             this.Texture = contentManager.Load<Texture2D>(this._textureFile);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (this.Texture == null)
            {
                throw new Exception("Objeto no inicializado, llame a init antes de dibujar");
            }

            spriteBatch.Draw(this.Texture, position, Color.White);
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyJsonData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Graphics
{
    class Sprite
    {

        public Texture2D Texture { get; set; }
        public Color TintColor { get; set; } = Color.White;
        public float Scale { get; set; }
        public Image spriteInfo { get; set; }
        public Sprite(Texture2D texture, float scale, Image spriteInfo)
        {
            Texture = texture;
            this.Scale = scale;
            this.spriteInfo = spriteInfo;
        }

        public Sprite(Texture2D texture, float scale, Image spriteInfo, Color tintColor)
        {
            Texture = texture;
            this.Scale = scale;
            this.spriteInfo = spriteInfo;
            this.TintColor = tintColor;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(Texture, position, new Rectangle(spriteInfo.X, spriteInfo.Y, spriteInfo.W, spriteInfo.H),TintColor, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
        }
    }
}

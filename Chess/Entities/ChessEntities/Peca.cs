using Chess.Enums;
using Chess.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyJsonData.Models;

namespace Chess.Entities.ChessEntities
{
    abstract class Peca : IGameEntity
    {
        public Posicao Posicao { get; set; }
        public Cor Cor { get; set; }
        public int QtdMovimentos { get; set; }
        public Tabuleiro Tabuleiro { get; set; }
        public int DrawOrder { get; set; }
        public Vector2 Pos { get; set; }
        public abstract string textureName { get; set; }

        public Image spriteInfo { get; set; }

        public Sprite _sprite;

        public Texture2D _spriteSheet;

        public Atlas _atlas;

        public float _scaleRatio;

        public Vector2 AnchorPoint = new Vector2();

        public Rectangle CollisionBox;

        public Peca(Cor cor, Tabuleiro tabuleiro, Texture2D spriteSheet, Atlas atlas, float textureScaleRatio)
        {
            Posicao = null;
            Cor = cor;
            QtdMovimentos = 0;
            Tabuleiro = tabuleiro;
            this.Pos = new Vector2(-300,-300);
            this._spriteSheet = spriteSheet;
            this._atlas = atlas;
            this._scaleRatio = textureScaleRatio;
        }

        public void updateSpriteInfo()
        {
            this.spriteInfo = _atlas.Textures[0].Images.Find(image => image.Name == this.textureName);
            this._sprite = new Sprite(_spriteSheet, _scaleRatio, this.spriteInfo);
            AnchorPoint.X = spriteInfo.W / 2;
            AnchorPoint.Y = (spriteInfo.H / 2) + 40;
        }

        public void IncrementarQtdMovimentos()
        {
            QtdMovimentos++;
        }

        public void DecrementarQtdMovimentos()
        {
            QtdMovimentos--;
        }

        public bool ExitemMovimentosPossiveis()
        {
            bool[,] aux = MovimentosPossiveis();

            for (int i = 0; i < Tabuleiro.Linhas; i++)
            {
                for (int j = 0; j < Tabuleiro.Colunas; j++)
                {
                    if (aux[i, j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool MovimentoPossivel(Posicao pos)
        {
            return MovimentosPossiveis()[pos.Linha, pos.Coluna];
        }

        public abstract bool[,] MovimentosPossiveis();

        public void Update(GameTime gameTime)
        {
            this.CollisionBox = new Rectangle((int)Pos.X - (int)AnchorPoint.X/2, (int)Pos.Y - (int)AnchorPoint.Y/2, spriteInfo.W/2, spriteInfo.H/2);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            _sprite.Draw(spriteBatch, Pos - AnchorPoint * _scaleRatio);

            
        }

        private void DrawHitboxes(SpriteBatch spriteBatch)
        {
           Texture2D boxTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);

            boxTexture.SetData(new[] { Color.White });

            spriteBatch.Draw(boxTexture, CollisionBox, Color.White);
        }
    }
}

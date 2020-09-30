using Chess.Exceptions;
using Chess.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyJsonData.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chess.Entities.ChessEntities
{
    class Tabuleiro :  IGameEntity
    {
        public int Linhas { get; set; }
        public int Colunas { get; set; }
        public Peca[,] Pecas { get; set; }
        public int DrawOrder { get; set; }
        public Vector2 BoardPos { get; set; }
        public Image spriteInfoA { get; set; }
        public Image spriteInfoB { get; set; }
        private float _textureScaleRatio { get; set; }

        private Sprite _spriteA;

        private Sprite _spriteB;

        public Vector2 AnchorPoint = new Vector2();

        public bool[,] PosicoesPossiveis { get; set; }
        public Peca PecaSelecionada { get; set; }
        public Vector2 mousePos { get; set; } = new Vector2(0,0);


        public Tabuleiro(int linhas, int colunas, Texture2D spriteSheet, Atlas atlas, Vector2 boardPos, float textureScaleRatio, int drawOrder)
        {
            this.DrawOrder = drawOrder;
            this.BoardPos = boardPos;
            this.spriteInfoA = atlas.Textures[0].Images.Find(image => image.Name == "tile_A");
            _spriteA = new Sprite(spriteSheet, textureScaleRatio, this.spriteInfoA);

            this.spriteInfoB = atlas.Textures[0].Images.Find(image => image.Name == "tile_B");
            _spriteB = new Sprite(spriteSheet, textureScaleRatio, this.spriteInfoB);
            Linhas = linhas;
            Colunas = colunas;
            Pecas = new Peca[linhas, colunas];
            AnchorPoint.X = spriteInfoA.W / 2;
            AnchorPoint.Y = spriteInfoA.H / 2;
            _textureScaleRatio = textureScaleRatio;
            PosicoesPossiveis = new bool[Linhas, Colunas];
        }

        public Peca Peca(int linha, int coluna)
        {
            return Pecas[linha, coluna];
        }

        public Peca Peca(Posicao pos)
        {
            return Pecas[pos.Linha, pos.Coluna];
        }

        public bool ExistePeca(Posicao pos)
        {
            ValidarPosicao(pos);
            return Peca(pos) != null;
        }

        public void ColocarPeca(Peca p, Posicao pos)
        {
            if (ExistePeca(pos)) throw new TabuleiroException("Já existe uma peça nessa posição.");
            Pecas[pos.Linha, pos.Coluna] = p;
            p.Posicao = pos;
        }

        public Peca RetirarPeca(Posicao pos)
        {
            if (this.Peca(pos) == null)
            {
                return null;
            }
            Peca aux = this.Peca(pos);
            aux.Posicao = null;
            Pecas[pos.Linha, pos.Coluna] = null;
            return aux;
        }

        public bool PosicaoValida(Posicao pos)
        {
            if (pos.Linha < 0 || pos.Linha >= Linhas || pos.Coluna < 0 || pos.Coluna >= Colunas) return false;
            return true;
        }

        public void ValidarPosicao(Posicao pos)
        {
            if (!PosicaoValida(pos))
            {
                throw new TabuleiroException("Posição Inválida!");
            }
        }

        public void Update(GameTime gameTime)
        {

            foreach (Peca peca in Pecas)
            {
                if (peca != null)
                {
                    peca.Update(gameTime);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 Pos = BoardPos;
            Sprite auxSprite;

            Vector2 auxPos = new Vector2(-16 * _textureScaleRatio, 8 * _textureScaleRatio);
            Vector2 auxPos2 = new Vector2(16 * _textureScaleRatio, 8 * _textureScaleRatio);
            bool aux = true;

            for (int j = 0; j < Linhas; j++)
            {
                
                for (int i = 0; i < Colunas; i++)
                {
                    if (aux) auxSprite = _spriteB;
                    else auxSprite = _spriteA;

                    auxSprite.TintColor = PosicoesPossiveis[j, i] ? Color.CornflowerBlue : Color.White;

                    auxSprite.Draw(spriteBatch, Pos - AnchorPoint * _textureScaleRatio);
                    if (Pecas[j, i] != null)
                    {
                        if (PecaSelecionada == Pecas[j,i])
                        {
                            Pecas[j, i].Pos = mousePos;
                        } else Pecas[j, i].Pos = Pos;

                        Pecas[j, i].Draw(spriteBatch, gameTime);
                    }

                    Pos += auxPos;
                    aux = !aux;
                }
                aux = !aux;
                Pos = BoardPos + auxPos2 * (j + 1);
            }


            foreach (Peca peca in Pecas)
            {
                if (peca != null)
                {
                    peca.Draw(spriteBatch, gameTime);
                }
                
            }
        }
    }
}

using Chess.Enums;
using Chess.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Chess.Entities.ChessEntities.Pecas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Chess.Graphics;
using MyJsonData.Models;
using Microsoft.Xna.Framework.Input;

namespace Chess.Entities.ChessEntities
{
    class PartidaDeXadrez : IGameEntity
    {
        public Tabuleiro Tab { get; set; }
        public int Turno { get; private set; }
        public Cor JogadorAtual { get; private set; }
        public bool Terminada { get; private set; }
        private HashSet<Peca> Pecas { get; set; }
        private HashSet<Peca> Capturadas { get; set; }
        public bool Xeque { get; private set; }
        public Peca VulveravelEnPassant { get; private set; }
        public int DrawOrder { get; set; }

        private Texture2D _spriteSheet;

        private Atlas _atlas;

        public float PieceScaleRatio { get; set; }

        public float BoardScaleRatio { get; set; }

        public PartidaDeXadrez(Texture2D spriteSheet, Atlas atlas, Vector2 boardPos, float boardScaleRatio,  float pieceScaleRatio,int drawOrder)
        {
            this.DrawOrder = drawOrder;
            Tab = new Tabuleiro(8, 8, spriteSheet, atlas, boardPos, boardScaleRatio, drawOrder);
            PieceScaleRatio = pieceScaleRatio;
            Turno = 1;
            JogadorAtual = Cor.Branca;
            Terminada = false;
            Xeque = false;
            VulveravelEnPassant = null;
            Pecas = new HashSet<Peca>();
            Capturadas = new HashSet<Peca>();
            _spriteSheet = spriteSheet;
            _atlas = atlas;
            ColocarPecas();
        }

        private Peca ExecutaMovimento(Posicao origem, Posicao destino)
        {
            Peca p = Tab.RetirarPeca(origem);
            p.IncrementarQtdMovimentos();

            Peca pecaCapturada = Tab.RetirarPeca(destino);
            Tab.ColocarPeca(p, destino);
            if (pecaCapturada != null)
            {
                Capturadas.Add(pecaCapturada);
            }

            // #jogada especial roque
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca T = Tab.RetirarPeca(origemT);
                T.IncrementarQtdMovimentos();
                Tab.ColocarPeca(T, destinoT);
            }

            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca T = Tab.RetirarPeca(origemT);
                T.IncrementarQtdMovimentos();
                Tab.ColocarPeca(T, destinoT);
            }

            // enpassant
            if (p is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == null)
                {
                    Posicao posP;
                    if (p.Cor == Cor.Branca)
                    {
                        posP = new Posicao(destino.Linha + 1, destino.Coluna);
                    }
                    else
                    {
                        posP = new Posicao(destino.Linha - 1, destino.Coluna);
                    }

                    pecaCapturada = Tab.RetirarPeca(posP);
                    Capturadas.Add(pecaCapturada);
                }
            }

            return pecaCapturada;
        }

        public void RealizaJogada(Posicao origem, Posicao destino)
        {
            Peca pecaCapturada = ExecutaMovimento(origem, destino);
            if (EstaEmXeque(JogadorAtual))
            {
                DesfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em xeque!");
            }

            Peca p = Tab.Peca(destino);

            // promoção
            if (p is Peao)
            {
                if ((p.Cor == Cor.Branca && destino.Linha == 0) || (p.Cor == Cor.Preta && destino.Linha == 7))
                {
                    p = Tab.RetirarPeca(destino);
                    Pecas.Remove(p);
                    Peca dama = new Dama(Tab, p.Cor, _spriteSheet, _atlas, PieceScaleRatio);
                    Tab.ColocarPeca(dama, destino);
                    Pecas.Add(dama);
                }
            }


            if (EstaEmXeque(Adversaria(JogadorAtual)))
            {
                Xeque = true;
            }
            else
            {
                Xeque = false;
            }

            if (TexteXequeMate(Adversaria(JogadorAtual)))
            {
                Terminada = true;
            }
            else
            {
                this.Turno++;
                MudaJogador();
            }


            // #jogada especial en passant
            if (p is Peao && (destino.Linha == origem.Linha - 2 || destino.Linha == origem.Linha + 2))
            {
                VulveravelEnPassant = p;
            }
            else
            {
                VulveravelEnPassant = null;
            }
        }

        private void DesfazMovimento(Posicao origem, Posicao destino, Peca pecaCapturada)
        {
            Peca p = Tab.RetirarPeca(destino);
            p.DecrementarQtdMovimentos();
            if (pecaCapturada != null)
            {
                Tab.ColocarPeca(pecaCapturada, destino);
                Capturadas.Remove(pecaCapturada);
            }

            Tab.ColocarPeca(p, origem);

            // #jogada especial roque
            if (p is Rei && destino.Coluna == origem.Coluna + 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna + 3);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna + 1);
                Peca T = Tab.RetirarPeca(destinoT);
                T.DecrementarQtdMovimentos();
                Tab.ColocarPeca(T, origemT);
            }

            if (p is Rei && destino.Coluna == origem.Coluna - 2)
            {
                Posicao origemT = new Posicao(origem.Linha, origem.Coluna - 4);
                Posicao destinoT = new Posicao(origem.Linha, origem.Coluna - 1);
                Peca T = Tab.RetirarPeca(destinoT);
                T.IncrementarQtdMovimentos();
                Tab.ColocarPeca(T, origemT);
            }

            // enpassant
            if (p is Peao)
            {
                if (origem.Coluna != destino.Coluna && pecaCapturada == VulveravelEnPassant)
                {
                    Peca peao = Tab.RetirarPeca(destino);
                    Posicao posP;
                    if (p.Cor == Cor.Branca)
                    {
                        posP = new Posicao(3, destino.Coluna);
                    }
                    else
                    {
                        posP = new Posicao(4, destino.Coluna);
                    }

                    Tab.ColocarPeca(peao, posP);
                }
            }
        }

        public void ValidaPosicaoOrigem(Posicao pos)
        {
            if (Tab.Peca(pos) == null)
            {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida.");
            }

            if (JogadorAtual != Tab.Peca(pos).Cor)
            {
                throw new TabuleiroException("A peça escolhida não é sua.");
            }

            if (!Tab.Peca(pos).ExitemMovimentosPossiveis())
            {
                throw new TabuleiroException("Não existem movimentos possíveis para esta peça.");
            }
        }

        public void ValidarPosicaoDestino(Posicao origem, Posicao destino)
        {
            if (!Tab.Peca(origem).MovimentoPossivel(destino))
            {
                throw new TabuleiroException("Posição de destino inválida.");
            }
        }

        public HashSet<Peca> PecasCapturadas(Cor cor)
        {
            HashSet<Peca> temp = new HashSet<Peca>();
            foreach (Peca x in Capturadas)
            {
                if (x.Cor == cor)
                {
                    temp.Add(x);
                }
            }

            return temp;
        }

        public HashSet<Peca> PecasEmJogo(Cor cor)
        {
            HashSet<Peca> temp = new HashSet<Peca>();
            foreach (Peca x in Pecas)
            {
                if (x.Cor == cor)
                {
                    temp.Add(x);
                }
            }

            temp.ExceptWith(PecasCapturadas(cor));
            return temp;
        }

        private Cor Adversaria(Cor cor)
        {
            return cor == Cor.Branca ? Cor.Preta : Cor.Branca;
        }

        private Peca Rei(Cor cor)
        {
            return PecasEmJogo(cor).OfType<Rei>().FirstOrDefault();
            //            foreach (Peca x in PecasEmJogo(cor)) {
            //                if (x is Rei) {
            //                    return x;
            //                }
            //            }
            //
            //            return null;
        }

        public bool EstaEmXeque(Cor cor)
        {
            Peca R = Rei(cor);
            if (R == null)
            {
                throw new TabuleiroException("Rei da cor " + cor + " inexistente.");
            }

            return PecasEmJogo(Adversaria(cor)).Select(x => x.MovimentosPossiveis())
                .Any(mat => mat[R.Posicao.Linha, R.Posicao.Coluna]);
        }

        public bool TexteXequeMate(Cor cor)
        {
            if (!EstaEmXeque(cor))
            {
                return false;
            }

            foreach (Peca x in PecasEmJogo(cor))
            {
                bool[,] mat = x.MovimentosPossiveis();
                for (int i = 0; i < Tab.Linhas; i++)
                {
                    for (int j = 0; j < Tab.Colunas; j++)
                    {
                        if (mat[i, j])
                        {
                            Posicao origem = x.Posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = ExecutaMovimento(origem, destino);
                            bool testeXeque = EstaEmXeque(cor);
                            DesfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXeque)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private void MudaJogador()
        {
            JogadorAtual = JogadorAtual == Cor.Branca ? Cor.Preta : Cor.Branca;
        }

        public void ColocarNovaPeca(char coluna, int linha, Peca peca)
        {
            Tab.ColocarPeca(peca, new PosicaoXadrez(coluna, linha).ToPosicao());
            Pecas.Add(peca);
            peca.Posicao = new PosicaoXadrez(coluna, linha).ToPosicao();
        }

        private void ColocarPecas()
        {
            ColocarNovaPeca('a', 1, new Torre(Tab, Cor.Branca, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('b', 1, new Cavalo(Tab, Cor.Branca, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('c', 1, new Bispo(Tab, Cor.Branca, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('d', 1, new Dama(Tab, Cor.Branca, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('e', 1, new Rei(Tab, Cor.Branca, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('f', 1, new Bispo(Tab, Cor.Branca, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('g', 1, new Cavalo(Tab, Cor.Branca, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('h', 1, new Torre(Tab, Cor.Branca, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('a', 2, new Peao(Tab, Cor.Branca, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('b', 2, new Peao(Tab, Cor.Branca, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('c', 2, new Peao(Tab, Cor.Branca, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('d', 2, new Peao(Tab, Cor.Branca, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('e', 2, new Peao(Tab, Cor.Branca, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('f', 2, new Peao(Tab, Cor.Branca, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('g', 2, new Peao(Tab, Cor.Branca, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('h', 2, new Peao(Tab, Cor.Branca, this, _spriteSheet, _atlas, PieceScaleRatio));

            ColocarNovaPeca('a', 8, new Torre(Tab, Cor.Preta, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('b', 8, new Cavalo(Tab, Cor.Preta, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('c', 8, new Bispo(Tab, Cor.Preta, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('d', 8, new Dama(Tab, Cor.Preta, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('e', 8, new Rei(Tab, Cor.Preta, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('f', 8, new Bispo(Tab, Cor.Preta, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('g', 8, new Cavalo(Tab, Cor.Preta, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('h', 8, new Torre(Tab, Cor.Preta, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('a', 7, new Peao(Tab, Cor.Preta, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('b', 7, new Peao(Tab, Cor.Preta, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('c', 7, new Peao(Tab, Cor.Preta, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('d', 7, new Peao(Tab, Cor.Preta, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('e', 7, new Peao(Tab, Cor.Preta, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('f', 7, new Peao(Tab, Cor.Preta, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('g', 7, new Peao(Tab, Cor.Preta, this, _spriteSheet, _atlas, PieceScaleRatio));
            ColocarNovaPeca('h', 7, new Peao(Tab, Cor.Preta, this, _spriteSheet, _atlas, PieceScaleRatio));
        }

        public void Update(GameTime gameTime)
        {
            Tab.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Tab.Draw(spriteBatch, gameTime);
        }

        public void PegarPeca(MouseState mouseState)
        {
            foreach (Peca p in Tab.Pecas)
            {
                if (p != null)
                {
                    if (p.CollisionBox.Contains(mouseState.Position))
                    {
                        Tab.PosicoesPossiveis = p.MovimentosPossiveis();
                        Tab.PecaSelecionada = p;

                        float auxX = p.CollisionBox.X - mouseState.X;
                        float auxY = p.CollisionBox.Y - mouseState.Y;
                        //Tab.mousePos = new Vector2(mouseState.X + p.CollisionBox.Width, mouseState.Y + p.CollisionBox.Height);
                        Tab.mousePos = new Vector2(mouseState.X - auxX, mouseState.Y - auxY);
                    }
                }
            }
        }
    }
}

﻿using Chess.Entities.ChessEntities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Chess.Input
{
    class InputHandler
    {
        private PartidaDeXadrez _partida;

        private MouseState _mouseState;

        public InputHandler(PartidaDeXadrez partida)
        {
            _partida = partida;
        }

        public void ProcessControls(GameTime gameTime)
        {
            _mouseState = Mouse.GetState();

            if (_mouseState.LeftButton == ButtonState.Pressed)
            {
                _partida.PegarPeca(_mouseState);
            }
            else if (_mouseState.LeftButton == ButtonState.Released)
            {
                _partida.SoltarPeca();
            }
        }
    }
}

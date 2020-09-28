using Chess.Entities;
using Chess.Entities.ChessEntities;
using Chess.Entities.ChessEntities.Pecas;
using Chess.Graphics;
using Chess.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyJsonData.Models;


namespace Chess
{
    public class Chess : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public const string GAME_TITLE = "Chess";

        public const int WINDOW_WIDTH = 600;
        public const int WINDOW_HEIGHT = 600;

        private Texture2D _spriteSheet;

        private EntityManager _entityManager;

        private Atlas _atlas;

        private InputHandler _inputHandler;

        public Chess()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _entityManager = new EntityManager();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            Window.Title = GAME_TITLE;

            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.SynchronizeWithVerticalRetrace = true;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _atlas = Content.Load<Atlas>("atlas");
            _spriteSheet = Content.Load<Texture2D>("atlas0");

            PartidaDeXadrez novaPartida = new PartidaDeXadrez(_spriteSheet, _atlas, new Vector2(WINDOW_WIDTH/2, 400), 1.1f, 0.5f, 0);

            _entityManager.AddEntity(novaPartida);

            _inputHandler = new InputHandler(novaPartida);
        }

        protected override void Update(GameTime gameTime)
        {


            base.Update(gameTime);

            _inputHandler.ProcessControls(gameTime);

            _entityManager.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            _entityManager.Draw(_spriteBatch, gameTime);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public Vector2 toIsometric(Vector2 pos)
        {
            float x, y;

            x = pos.X + pos.Y;
            y = (-0.5f * pos.X) + (0.5f * pos.Y);

            return new Vector2(x,y);
        }
    }
}

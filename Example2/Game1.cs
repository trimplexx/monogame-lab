using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Game2
{
    public class Game1 : Game
    {
        // Zmienne klasy
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D paddle1aTexture, paddle2aTexture, _backgroundTexture, _ballTexture, _explosionTexture;
        private Vector2 paddle1aPosition, paddle2aPosition, _ballPosition, _ballVelocity;
        private Vector2 _explosionPosition; // Pozycja wybuchu
        private const int PADDLE_SPEED = 5;
        private const float INITIAL_BALL_SPEED = 4f;
        private const float BALL_ACCELERATION = 2f;
        private SpriteFont _font;
        private float ballSpeed = INITIAL_BALL_SPEED;
        private bool _gameStarted = false;
        private int _currentFrameIndex = 0;
        private double _animationTimer = 0;
        private List<Rectangle> _ballFrames, _explosionFrames;
        private int scoreLeft = 0;
        private int scoreRight = 0;
        private bool _ballExploded = false;
        private int _currentExplosionFrameIndex = 0;
        private double _explosionTimer = 0;
        private Rectangle paddle1Rect;
        private Rectangle paddle2Rect;
        private Rectangle ballRect;
        private double _paddleHitCooldown = 0;
        private bool lastHitByPaddle1 = false;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            paddle1aPosition = new Vector2(0, _graphics.PreferredBackBufferHeight / 2);
            paddle2aPosition = new Vector2(_graphics.PreferredBackBufferWidth - 20,
                _graphics.PreferredBackBufferHeight / 2);
            _ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2 - 32,
                _graphics.PreferredBackBufferHeight / 2 - 32);
            _ballVelocity = new Vector2(ballSpeed, ballSpeed);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundTexture = Content.Load<Texture2D>("pongBackground"); // Ładowanie tekstury tła
            paddle1aTexture = Content.Load<Texture2D>("paddle1a");
            paddle2aTexture = Content.Load<Texture2D>("paddle2a");

            _font = Content.Load<SpriteFont>("File");

            // Wczytywanie tekstury zawierającej wszystkie klatki
            _ballTexture = Content.Load<Texture2D>("ball-anim");

            _explosionTexture = Content.Load<Texture2D>("explosion64");

            // Podział tekstury na klatki
            _ballFrames = new List<Rectangle>();
            for (int i = 0; i < 4; i++) // Liczba rzędów
            {
                for (int j = 0; j < 16; j++) // Liczba kolumn
                {
                    Rectangle frame = new Rectangle(j * 64, i * 64, 64, 64); // Wymiary klatki
                    _ballFrames.Add(frame);
                }
            }

            for (int j = 0; j < 7; j++) // Liczba klatek w ostatnim rzędzie
            {
                Rectangle frame = new Rectangle(j * 64, 4 * 64, 64, 64); // Wymiary klatki
                _ballFrames.Add(frame);
            }

            _explosionFrames = new List<Rectangle>();
            for (int i = 0; i < 5; i++) // Liczba rzędów
            {
                for (int j = 0; j < 5; j++) // Liczba kolumn
                {
                    Rectangle frame = new Rectangle(j * 64, i * 64, 64, 64); // Wymiary klatki
                    _explosionFrames.Add(frame);
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Escape))
            {
                this.Exit(); // Zamknięcie gry
            }
            
            // Sterowanie dla Gracza 1
            if (state.IsKeyDown(Keys.Q))
            {
                paddle1aPosition.Y -= PADDLE_SPEED;
            }

            if (state.IsKeyDown(Keys.A))
            {
                paddle1aPosition.Y += PADDLE_SPEED;
            }

            // Sterowanie dla Gracza 2
            if (state.IsKeyDown(Keys.P))
            {
                paddle2aPosition.Y -= PADDLE_SPEED;
            }

            if (state.IsKeyDown(Keys.L))
            {
                paddle2aPosition.Y += PADDLE_SPEED;
            }

            // Ograniczenie ruchu paletek do ekranu
            paddle1aPosition.Y = MathHelper.Clamp(paddle1aPosition.Y, 0,
                _graphics.PreferredBackBufferHeight - paddle1aTexture.Height);
            paddle2aPosition.Y = MathHelper.Clamp(paddle2aPosition.Y, 0,
                _graphics.PreferredBackBufferHeight - paddle2aTexture.Height);

            // Rozpoczęcie gry
            if (!_gameStarted)
            {
                if (state.IsKeyDown(Keys.Space))
                {
                    _gameStarted = true;
                }

                return;
            }

            // Poruszanie piłki
            _ballPosition += _ballVelocity;

            // Odbijanie piłki od krawędzi ekranu
            if (_ballPosition.Y < 0 ||
                (_ballPosition.Y + _ballFrames[_currentFrameIndex].Height) > Window.ClientBounds.Height)
                _ballVelocity.Y *= -1;

            // Odbijanie piłki od paletki
             ballRect = new Rectangle((int)_ballPosition.X, (int)_ballPosition.Y,
                _ballFrames[_currentFrameIndex].Width,
                _ballFrames[_currentFrameIndex].Height);
             paddle1Rect = new Rectangle((int)paddle1aPosition.X, (int)paddle1aPosition.Y,
                paddle1aTexture.Width, paddle1aTexture.Height);
             paddle2Rect = new Rectangle((int)paddle2aPosition.X, (int)paddle2aPosition.Y,
                paddle2aTexture.Width, paddle2aTexture.Height);

            if (paddle1Rect.Intersects(ballRect))
            {
                float relativeIntersectY = (_ballPosition.Y + (_ballFrames[_currentFrameIndex].Height / 2)) -
                                           (paddle1Rect.Y + (paddle1Rect.Height / 2));
                float normalizedRelativeIntersectionY = (relativeIntersectY /
                                                         ((paddle1Rect.Height +
                                                           _ballFrames[_currentFrameIndex].Height) / 2));
                float bounceAngle = normalizedRelativeIntersectionY * (-75f * MathHelper.Pi / 180);

                _ballVelocity.X = Math.Abs(_ballVelocity.X) * (float)Math.Cos(bounceAngle) + BALL_ACCELERATION;
                _ballVelocity.Y = ballSpeed * -(float)Math.Sin(bounceAngle);
                ballSpeed += BALL_ACCELERATION;
                _paddleHitCooldown = 0.5; 
                lastHitByPaddle1 = true;
            }

            if (paddle2Rect.Intersects(ballRect))
            {
                float relativeIntersectY = (_ballPosition.Y + (_ballFrames[_currentFrameIndex].Height / 2)) -
                                           (paddle2Rect.Y + (paddle2Rect.Height / 2));
                float normalizedRelativeIntersectionY = (relativeIntersectY /
                                                         ((paddle2Rect.Height +
                                                           _ballFrames[_currentFrameIndex].Height) / 2));
                float bounceAngle = normalizedRelativeIntersectionY * (-75f * MathHelper.Pi / 180);

                _ballVelocity.X = -Math.Abs(_ballVelocity.X) * (float)Math.Cos(bounceAngle) - BALL_ACCELERATION;
                _ballVelocity.Y = ballSpeed * -(float)Math.Sin(bounceAngle);
                ballSpeed += BALL_ACCELERATION;
                _paddleHitCooldown = 0.5; 
                lastHitByPaddle1 = false;
            }
            
            if (_paddleHitCooldown > 0)
            {
                _paddleHitCooldown -= gameTime.ElapsedGameTime.TotalSeconds;
                if (_paddleHitCooldown < 0)
                {
                    _paddleHitCooldown = 0;
                }
            }

            // Aktualizacja animacji piłki
            _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_animationTimer > 0.05) // Prędkość animacji
            {
                _currentFrameIndex = (_currentFrameIndex + 1) % 71; // Liczba klatek
                _animationTimer -= 0.05;
            }

            // Sprawdzenie kolizji z bokami ekranu
            if (_ballPosition.X < 0 || _ballPosition.X >
                _graphics.PreferredBackBufferWidth - _ballFrames[_currentFrameIndex].Width)
            {
                if (_ballPosition.X < 0)
                {
                    scoreRight++;
                    _ballExploded = true;
                    _explosionPosition = _ballPosition;
                    _gameStarted = false;
                }
                else
                {
                    scoreLeft++;
                    _ballExploded = true;
                    _explosionPosition = _ballPosition;
                    _gameStarted = false;
                }

                if (_ballExploded)
                {
                    // Pobieranie aktualnej klatki animacji wybuchu
                    Rectangle explosionFrame = _explosionFrames[_currentExplosionFrameIndex];

                    // Rysowanie aktualnej klatki animacji wybuchu w miejscu, gdzie była ostatnio piłka
                    _spriteBatch.Begin(); // Rozpoczęcie nowego bloku rysowania
                    _spriteBatch.Draw(_explosionTexture, _ballPosition, explosionFrame, Color.White);
                    _spriteBatch.End(); // Zakończenie bloku rysowania

                    // Aktualizacja timera i indeksu klatki animacji wybuchu
                    _explosionTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_explosionTimer >= 0.05) // Prędkość animacji wybuchu (w sekundach na klatkę)
                    {
                        _currentExplosionFrameIndex++;
                        if (_currentExplosionFrameIndex >=
                            _explosionFrames.Count) // Jeśli osiągnięto ostatnią klatkę animacji wybuchu
                        {
                            // Zresetuj stan animacji wybuchu i zacznij grę od nowa
                            _ballExploded = false;
                            _currentExplosionFrameIndex = 0;
                            _explosionTimer = 0;
                            Initialize();
                        }
                        else // Jeśli nie osiągnięto ostatniej klatki animacji wybuchu
                        {
                            // Zresetuj timer
                            _explosionTimer = 0;
                        }
                    }
                }

                // Ustawienie piłki na środku
                _ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2 - 32,
                    _graphics.PreferredBackBufferHeight / 2 - 32);

                ballSpeed = INITIAL_BALL_SPEED;
                _ballVelocity = new Vector2(ballSpeed, ballSpeed);
            }

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);

    _spriteBatch.Begin();
    _spriteBatch.Draw(_backgroundTexture,
        new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
    
    if (_paddleHitCooldown > 0 && lastHitByPaddle1)
    {
        _spriteBatch.Draw(paddle1aTexture, paddle1aPosition, Color.Green); // Zmiana koloru na zielony
    }
    else
    {
        _spriteBatch.Draw(paddle1aTexture, paddle1aPosition, Color.White); // Domyślny kolor
    }


    if (_paddleHitCooldown > 0 && !lastHitByPaddle1)
    {
        _spriteBatch.Draw(paddle2aTexture, paddle2aPosition, Color.Green); // Zmiana koloru na zielony
    }
    else
    {
        _spriteBatch.Draw(paddle2aTexture, paddle2aPosition, Color.White); // Domyślny kolor
    }



    // Rysowanie piłki lub animacji wybuchu
    if (!_ballExploded)
    {
        _spriteBatch.Draw(_ballTexture, _ballPosition, _ballFrames[_currentFrameIndex], Color.White);
    }
    else
    {
        Rectangle explosionFrame = _explosionFrames[_currentExplosionFrameIndex];
        _spriteBatch.Draw(_explosionTexture, _explosionPosition, explosionFrame, Color.White);

        _explosionTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (_explosionTimer >= 0.05)
        {
            _currentExplosionFrameIndex++;
            if (_currentExplosionFrameIndex >= _explosionFrames.Count)
            {
                _ballExploded = false;
                _currentExplosionFrameIndex = 0;
                _explosionTimer = 0;
            }
            else
            {
                _explosionTimer = 0;
            }
        }
    }

    // Rysowanie tekstu
    _spriteBatch.DrawString(_font, $"{scoreLeft}       {scoreRight}",
        new Vector2(_graphics.PreferredBackBufferWidth / 2 - 50, 20), Color.White);

    _spriteBatch.End();

    base.Draw(gameTime);
}

    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game1
{
    public class Game1 : Game
    {
        SpriteBatch _spriteBatch;
        Texture2D _backgroundTexture, _numbersTexture, _questionTexture;
        Rectangle _questionRectangle;
        Rectangle[] _numberRectangles = new Rectangle[8];
        Rectangle[,] _gridRectangles = new Rectangle[11, 11];
        int[,] _gridNumbers = new int[11, 11];
        int _selectedNumber = -1;
        float[,] _gridRotation = new float[11, 11];
        bool _mouseWasPressed;

        public Game1()
        {
            var graphics = new GraphicsDeviceManager(this);
            this.Window.AllowUserResizing = true;
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            IsMouseVisible = true; // Ustawienie widoczności kursora myszy
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundTexture = Content.Load<Texture2D>("background"); // Ładowanie tekstury tła
            _numbersTexture = Content.Load<Texture2D>("numbers"); // Ładowanie tekstury liczb
            _questionTexture = Content.Load<Texture2D>("question"); // Ładowanie tekstury znaku zapytania

            _questionRectangle = new Rectangle(10, 10, 100, 100); // Inicjalizacja obszaru znaku zapytania
            int posY = 140;
            for (int i = 0; i < 8; i++)
            {
                // Inicjalizacja obszarów liczb
                _numberRectangles[i] = new Rectangle(10, posY, 50, 50); 
                posY += 50;
            }

            posY = 10;
            int posX = 150;
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    // Inicjalizacja obszarów planszy
                    _gridRectangles[i, j] = new Rectangle(posX, posY, 50, 50); 
                    posX += 50;
                }

                posY += 50;
                posX = 150;
            }

            // Inicjalizacja planszy znakami zapytania
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    _gridNumbers[i, j] = -1;
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (_numberRectangles[i].Contains(mouseState.Position))
                    {
                        _selectedNumber = i; // Zaznaczenie wybranej liczby
                        break;
                    }
                }

                if (_questionRectangle.Contains(mouseState.Position))
                    _selectedNumber = -1; // Usunięcie zaznaczenia, jeśli kliknięto znak zapytania

                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 11; j++)
                    {
                        if (_gridRectangles[i, j].Contains(mouseState.Position))
                        {
                            if (_gridNumbers[i, j] == _selectedNumber && !_mouseWasPressed)
                                _gridRotation[i, j] += MathHelper.ToRadians(90); // Obróć element planszy o 90 stopni
                            else
                                _gridNumbers[i, j] = _selectedNumber; // Ustaw wybraną liczbę na planszy
                        }
                    }
                }
                // Ustawiamy zmienną na true, gdy przycisk myszy jest przytrzymywany
                _mouseWasPressed = true; 
            }
            else
            {
                // Ustawiamy zmienną na false, gdy przycisk myszy jest puszczony
                _mouseWasPressed = false; 
            }

            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                // Wyjście z gry po naciśnięciu klawisza Esc
                Exit(); 
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // Wczytanie obrazu tła
            _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

            int posX = 0;
            int posY = 0;

            var mouseState = Mouse.GetState();

            // Wyświetlenie dostępnych liczb
            for (int i = 0; i < 8; i++)
            {
                Rectangle sourceRectangle = new Rectangle(posX, posY, 100, 100);

                Color colorToDraw = Color.White;
                if (_numberRectangles[i].Contains(mouseState.Position))
                {
                    // Podświetlenie na żółto, jeśli kursor znajduje się nad liczbą
                    colorToDraw = Color.Yellow; 
                }

                _spriteBatch.Draw(_numbersTexture, _numberRectangles[i], sourceRectangle, colorToDraw);

                if (i == 3)
                {
                    posY += 100;
                    posX = 0;
                }
                else
                    posX += 100;
            }

            if (_selectedNumber >= 0)
            {
                // Wyświetlenie wybranej liczby
                Rectangle sourceRectangle = new Rectangle((_selectedNumber % 4) * 100, (_selectedNumber / 4) * 100, 100, 100);
                _spriteBatch.Draw(_numbersTexture, _questionRectangle, sourceRectangle, Color.White); 
            }
            else
            {
                Color colorToDraw = Color.White;
                // Wyświetlenie znaku zapytania, gdy nie wybrano żadnej liczby
                _spriteBatch.Draw(_questionTexture, _questionRectangle, colorToDraw); 
            }

            // Wyświetlenie planszy z liczbami lub znakami zapytania
            for (int i = 0; i < 11; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    Color colorToDraw = Color.White;

                    if (_gridNumbers[i, j] >= 0)
                    {
                        // Wyświetlenie wybranej liczby na planszy, uwzględniając jej rotację
                        Rectangle sourceRectangle = new Rectangle((_gridNumbers[i, j] % 4) * 100, (_gridNumbers[i, j] / 4) * 100, 100, 100);
                        _spriteBatch.Draw(_numbersTexture, destinationRectangle: new Rectangle(_gridRectangles[i, j].X + 25, _gridRectangles[i, j].Y + 25, _gridRectangles[i, j].Width, _gridRectangles[i, j].Height), sourceRectangle: sourceRectangle,
                            color: colorToDraw, rotation: _gridRotation[i, j], origin: new Vector2(50f), effects: SpriteEffects.None,
                            layerDepth: 0f); 
                    }
                    else
                    {
                        
                        // Wyświetlenie znaku zapytania na planszy, uwzględniając jego rotację
                        _spriteBatch.Draw(_questionTexture, new Rectangle(_gridRectangles[i, j].X + 25, _gridRectangles[i, j].Y + 25, _gridRectangles[i, j].Width, _gridRectangles[i, j].Height), null,
                            color: colorToDraw, rotation: MathHelper.ToRadians(0), new Vector2(50f), effects: SpriteEffects.None,
                            layerDepth: 0f); 
                    }
                }
            }

            _spriteBatch.End(); // Zakończenie rysowania

            base.Draw(gameTime);
        }
    }
}

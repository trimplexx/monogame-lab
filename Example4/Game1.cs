using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game4
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        // Efekt używany do renderowania prymitywów
        private BasicEffect _basicEffect;
        // Macierz widoku dla kamery
        private Matrix _view;
        // Bufor wierzchołków dla prymitywów
        private VertexBuffer _vertexBuffer;
        
        // Zmienne do zarządzania kamerą
        private float _cameraDistance;
        private Vector2 _cameraAngle;
        
        // Części robota
        private ArmComponent _arm;
        private ArmComponent _arm2;
        private ArmComponent _pincer1;
        private ArmComponent _pincer2;
        
        // Macierze rotacji dla części robota
        private Matrix _rotationMatrixX;
        private Matrix _rotationMatrixY;
        private Matrix _rotationMatrixZ;
        private Matrix _arm2RotationMatrixX;
        private Matrix _arm2RotationMatrixZ;

        // Kąt otwarcia szczypiec robota
        private float _pincerAngle;
        private Matrix _princerMatrixZ;
        
        public Game1()
        {
            // Inicjalizacja głównych składników gry
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            // Wczytywanie tekstur i inicjalizacja prymitywów
            var texture = Content.Load<Texture2D>("metal1");
            var texture2 = Content.Load<Texture2D>("metal2");
            var texture3 = Content.Load<Texture2D>("metal3");
            _arm = new ArmComponent(_graphics.GraphicsDevice, new Vector3(0.09f, 0.09f, 0.4f), texture);
            _arm2 = new ArmComponent(_graphics.GraphicsDevice, new Vector3(0.065f, 0.065f, 0.35f), texture2);
            _pincer1 = new ArmComponent(_graphics.GraphicsDevice, new Vector3(0.03f, 0.03f, 0.2f), texture3);
            _pincer2 = new ArmComponent(_graphics.GraphicsDevice, new Vector3(0.03f, 0.03f, 0.2f), texture3);
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            
            // Inicjalizacja efektu, widoku i bufora wierzchołków
            _basicEffect = new BasicEffect(_graphics.GraphicsDevice);
            
            _view = Matrix.CreateLookAt(new Vector3(0, 0, _cameraDistance), Vector3.Zero, Vector3.Up);
            
            _vertexBuffer = new VertexBuffer(_graphics.GraphicsDevice, typeof(VertexPositionColor), 6, BufferUsage.WriteOnly);
            VertexPositionColor[] vertices = new VertexPositionColor[6];
            
            // Ustawianie wierzchołków siatki
            vertices[0] = new VertexPositionColor(new Vector3(-3, 0, 0), Color.Red);
            vertices[1] = new VertexPositionColor(new Vector3(3, 0, 0), Color.Red);

            vertices[2] = new VertexPositionColor(new Vector3(0, -3, 0), Color.Green);
            vertices[3] = new VertexPositionColor(new Vector3(0, 3, 0), Color.Green);

            vertices[4] = new VertexPositionColor(new Vector3(0, 0, -3), Color.Blue);
            vertices[5] = new VertexPositionColor(new Vector3(0, 0, 3), Color.Blue);
            _vertexBuffer.SetData(vertices);
            
            // Inicjalizacja pozostałych zmiennych dla ruchów
            _cameraAngle.X = 0.28f;
            _cameraAngle.Y = -0.50f;
            _cameraDistance = 2f;
            _rotationMatrixX = Matrix.Identity;
            _rotationMatrixY = Matrix.Identity;
            _rotationMatrixZ = Matrix.Identity;
            _arm2RotationMatrixX = Matrix.Identity;
            _arm2RotationMatrixZ = Matrix.Identity;
            _princerMatrixZ = Matrix.Identity;
            _pincerAngle = MathHelper.PiOver4;
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            keyboardState = Keyboard.GetState();
            // Modyfikacja kątów dla kamery
            if (keyboardState.IsKeyDown(Keys.Left)) _cameraAngle.Y -= 0.01f;
            if (keyboardState.IsKeyDown(Keys.Right)) _cameraAngle.Y += 0.01f;
            if (keyboardState.IsKeyDown(Keys.Up)) _cameraAngle.X -= 0.01f;
            if (keyboardState.IsKeyDown(Keys.Down)) _cameraAngle.X += 0.01f;
            if (keyboardState.IsKeyDown(Keys.Q)) _cameraDistance -= 0.1f;
            if (keyboardState.IsKeyDown(Keys.A)) _cameraDistance += 0.1f;
            
            // Modyfikacja kątów dla pierwszego z ramień
            if (keyboardState.IsKeyDown(Keys.W)) _rotationMatrixY *= Matrix.CreateRotationY(-0.01f); // Prawo
            if (keyboardState.IsKeyDown(Keys.S)) _rotationMatrixY *= Matrix.CreateRotationY(0.01f);  // Lewo
            if (keyboardState.IsKeyDown(Keys.E)) _rotationMatrixX *= Matrix.CreateRotationX(0.01f);  // Góra
            if (keyboardState.IsKeyDown(Keys.D)) _rotationMatrixX *= Matrix.CreateRotationX(-0.01f); // Dół
            
            // Modyfikacja kątów dla drugiego z ramień
            if (keyboardState.IsKeyDown(Keys.T)) _arm2RotationMatrixX *= Matrix.CreateRotationX(0.01f);  // Góra
            if (keyboardState.IsKeyDown(Keys.G)) _arm2RotationMatrixX *= Matrix.CreateRotationX(-0.01f); // Dół
            if (keyboardState.IsKeyDown(Keys.R)) _arm2RotationMatrixZ *= Matrix.CreateRotationZ(-0.01f); // Prawo
            if (keyboardState.IsKeyDown(Keys.F)) _arm2RotationMatrixZ *= Matrix.CreateRotationZ(0.01f);  // Lewo
            
            // Modyfikacja kątów dla szypiec
            if (Keyboard.GetState().IsKeyDown(Keys.Y) && _pincerAngle < MathHelper.PiOver2) _pincerAngle += 0.01f; // Rozwieranie
            if (Keyboard.GetState().IsKeyDown(Keys.H) && _pincerAngle > 0) _pincerAngle -= 0.01f; // Zwieranie
            
            if (keyboardState.IsKeyDown(Keys.U)) _princerMatrixZ *= Matrix.CreateRotationZ(-0.01f); // Prawo
            if (keyboardState.IsKeyDown(Keys.J)) _princerMatrixZ *= Matrix.CreateRotationZ(0.01f);  // Lewo

            _view = Matrix.CreateRotationX(_cameraAngle.X) * Matrix.CreateRotationY(_cameraAngle.Y) * Matrix.CreateLookAt(new Vector3(0, 0, _cameraDistance), Vector3.Zero, Vector3.Up);
            
            base.Update(gameTime);
        }

protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);

    // Ustawia macierz świata efektu na macierz jednostkową (bez żadnych transformacji)
    _basicEffect.World = Matrix.Identity;
    // Ustawia widok dla efektu
    _basicEffect.View = _view;
    // Ustawia projekcję dla efektu, tworząc widok pola
    _basicEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, _graphics.GraphicsDevice.Viewport.AspectRatio, 1.0f, 100.0f);

    // Włącza kolorowanie wierzchołków
    _basicEffect.VertexColorEnabled = true;

    // Ustawia bufor wierzchołków urządzenia graficznego
    GraphicsDevice.SetVertexBuffer(_vertexBuffer);

    // Przechodzi przez wszystkie przejścia efektu
    foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
    {
        // Stosuje efekt
        pass.Apply();
        // Rysuje prymitywy
        GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 3);
    }

    // Wyłącza kolorowanie wierzchołków
    _basicEffect.VertexColorEnabled = false;
    // Ustawia nową macierz świata efektu
    _basicEffect.World = _rotationMatrixX * _rotationMatrixY * _rotationMatrixZ;
    // Rysuje ramię
    _arm.Draw(_basicEffect);

    // Tworzy i ustawia macierz świata dla drugiego ramienia
    Matrix arm2World = _arm2RotationMatrixZ * _arm2RotationMatrixX * Matrix.CreateTranslation(0, 0, 0.4f) * _basicEffect.World;
    _basicEffect.World = arm2World;
    // Rysuje drugie ramię
    _arm2.Draw(_basicEffect);

    // Tworzy i ustawia macierz świata dla pierwszego szczypca
    Matrix pincer1World = _princerMatrixZ * Matrix.CreateRotationY(_pincerAngle) * Matrix.CreateTranslation(0, 0, 0.35f) * arm2World;
    _basicEffect.World = pincer1World;
    // Rysuje pierwszy szczypec
    _pincer1.Draw(_basicEffect);

    // Tworzy i ustawia macierz świata dla drugiego szczypca
    Matrix pincer2World = _princerMatrixZ * Matrix.CreateRotationY(-_pincerAngle) * Matrix.CreateTranslation(0, 0, 0.35f) * arm2World;
    _basicEffect.World = pincer2World;
    // Rysuje drugi szczypec
    _pincer2.Draw(_basicEffect);
    
    base.Draw(gameTime);
}
    }
}
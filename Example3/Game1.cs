using System;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Game3
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _backgroundTexture;
        private bool _backgroundEnabled = true;
        private ButtonState _previousState = ButtonState.Released;
        
        private float _cameraDistance = 10.0f;
        private Vector2 _cameraAngle = Vector2.Zero;
        private Matrix _world;
        private Matrix _view;
        private Matrix _projection;
        
        private VertexPositionColor[] _gridVertices;
        private int _gridSize = 30;
        
        private bool _gridEnabled = true;
        private ButtonState _previousGridState = ButtonState.Released;
        
        private BasicEffect _basicEffect;

        // Skale planet
        private const float SunScale = 1.2f;
        private const float MercuryScale = 0.45f;
        private const float VenusScale = 0.85f;
        private const float EarthScale = 0.9f;
        private const float MoonScale = 0.35f;
        private const float MarsScale = 0.7f;

        // Odległości planet od słońca
        private const float MercuryDistance = 2.5f;
        private const float VenusDistance = 5f;
        private const float EarthDistance = 8f;
        private const float MoonDistance =  1f;
        private const float MarsDistance = 11f;
        
        private CubePrimitive _sun;
        private CubePrimitive _mercury;
        private CubePrimitive _venus;
        private CubePrimitive _earth;
        private CubePrimitive _moon;
        private CubePrimitive _mars;
        
        private float _rotationX = 0.0f;
        private float _rotationY = 0.0f;
        
        private float _mercuryRotation = 0.0f;
        private float _venusRotation = 0.0f;
        private float _earthRotation = 0.0f;
        private float _moonRotation = 0.0f;
        private float _marsRotation = 0.0f;

        private float _mercuryAxisRotation = 0.0f;
        private float _venusAxisRotation = 0.0f;
        private float _earthAxisRotation = 0.0f;
        private float _moonAxisRotation = 0.0f;
        private float _marsAxisRotation = 0.0f;
        
        private const float MercuryAxisRotationSpeed = 0.08f;
        private const float VenusAxisRotationSpeed = 0.045f;
        private const float EarthAxisRotationSpeed = 0.04f;
        private const float MoonAxisRotationSpeed = 0.08f;
        private const float MarsAxisRotationSpeed = 0.03f;
        
        private const float MercuryRotationSpeed = 0.05f; 
        private const float VenusRotationSpeed = 0.03f; 
        private const float EarthRotationSpeed = 0.015f;
        private const float MoonRotationSpeedAroundEarth = 0.04f;
        private const float MarsRotationSpeed = 0.01f;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            Window.AllowUserResizing = true;
        }
        
        protected override void Initialize()
        {
            _world = Matrix.Identity;
            _view = Matrix.CreateLookAt(new Vector3(0, 0, _cameraDistance), Vector3.Zero, Vector3.Up);

            float aspectRatio = _graphics.GraphicsDevice.Viewport.AspectRatio;
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.1f, 100.0f);

            // Inicjalizacja siatki
            _gridVertices = new VertexPositionColor[_gridSize * 4 * 2];
            int i = 0;
            for (int x = 0; x <= _gridSize; x++)
            {
                Vector3 startX = new Vector3(x - _gridSize / 2, 0, -_gridSize / 2);
                Vector3 endX = new Vector3(x - _gridSize / 2, 0, _gridSize / 2);
                Vector3 startZ = new Vector3(-_gridSize / 2, 0, x - _gridSize / 2);
                Vector3 endZ = new Vector3(_gridSize / 2, 0, x - _gridSize / 2);
                _gridVertices[i++] = new VertexPositionColor(startX, Color.White);
                _gridVertices[i++] = new VertexPositionColor(endX, Color.White);
                _gridVertices[i++] = new VertexPositionColor(startZ, Color.White);
                _gridVertices[i++] = new VertexPositionColor(endZ, Color.White);
            }
            
            _basicEffect = new BasicEffect(_graphics.GraphicsDevice);
            _basicEffect.VertexColorEnabled = true;
            
            // Inicjalizacja początkowego położenia kamery
            _cameraAngle.X = 0.42f;
            _cameraAngle.Y = -0.45f;
            _cameraDistance = 23f;
            
            // Inicjalizacja Planet
            _sun = new CubePrimitive(GraphicsDevice, SunScale, Color.Yellow);
            _mercury = new CubePrimitive(GraphicsDevice, MercuryScale, Color.IndianRed);
            _venus = new CubePrimitive(GraphicsDevice, VenusScale, Color.Orange);
            _earth = new CubePrimitive(GraphicsDevice, EarthScale, Color.Green);
            _moon = new CubePrimitive(GraphicsDevice, MoonScale, Color.Gray);
            _mars = new CubePrimitive(GraphicsDevice, MarsScale, Color.IndianRed);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _backgroundTexture = Content.Load<Texture2D>("stars");

        }

        protected override void Update(GameTime gameTime)
        {
            // Obsługa końca gry
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            // Obsługa przycisków wyłączania/włączania siatki/tła
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.B) && _previousState == ButtonState.Released)
            {
                _backgroundEnabled = !_backgroundEnabled;
            }
            _previousState = keyboardState.IsKeyDown(Keys.B) ? ButtonState.Pressed : ButtonState.Released;
            
            if (keyboardState.IsKeyDown(Keys.X) && _previousGridState == ButtonState.Released)
            {
                _gridEnabled = !_gridEnabled;
            }
            _previousGridState = keyboardState.IsKeyDown(Keys.X) ? ButtonState.Pressed : ButtonState.Released;
            
            // Sterowanie kamerą
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left)) _cameraAngle.Y -= 0.01f;
            if (keyboardState.IsKeyDown(Keys.Right)) _cameraAngle.Y += 0.01f;
            if (keyboardState.IsKeyDown(Keys.Up)) _cameraAngle.X -= 0.01f;
            if (keyboardState.IsKeyDown(Keys.Down)) _cameraAngle.X += 0.01f;
            if (keyboardState.IsKeyDown(Keys.Q)) _cameraDistance -= 0.1f;
            if (keyboardState.IsKeyDown(Keys.A)) _cameraDistance += 0.1f;

            _view = Matrix.CreateRotationX(_cameraAngle.X) * Matrix.CreateRotationY(_cameraAngle.Y) * Matrix.CreateLookAt(new Vector3(0, 0, _cameraDistance), Vector3.Zero, Vector3.Up);

            // Rotacja wokół własnej osi
            _rotationX += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _rotationY += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Aktualizacja pozycji planet
            _mercuryRotation += MercuryRotationSpeed;
            _venusRotation += VenusRotationSpeed;
            _earthRotation += EarthRotationSpeed;
            _marsRotation += MarsRotationSpeed;
            
            // Aktualizacja pozycji księżyca
            _moonRotation += MoonRotationSpeedAroundEarth;
            
            _mercuryAxisRotation += MercuryAxisRotationSpeed;
            _venusAxisRotation += VenusAxisRotationSpeed;
            _earthAxisRotation += EarthAxisRotationSpeed;
            _moonAxisRotation += MoonAxisRotationSpeed;
            _marsAxisRotation += MarsAxisRotationSpeed;
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            if (_backgroundEnabled)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(_backgroundTexture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                _spriteBatch.End();
            }

            _basicEffect.World = _world;
            _basicEffect.View = _view;
            _basicEffect.Projection = _projection;

            // Rysowanie siatki
            if (_gridEnabled)
            {
                foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _gridVertices, 0, _gridVertices.Length / 2);
                }
            }

            // Rysowanie słońca oraz pozostałych planet wraz z uwzględnieniem rotacji.
            _basicEffect.World = Matrix.CreateScale(SunScale) * Matrix.CreateRotationX(_rotationX) * Matrix.CreateRotationY(_rotationY) * Matrix.CreateTranslation(Vector3.Zero);
            _sun.Draw(_basicEffect);
            
            _basicEffect.World = Matrix.CreateScale(MercuryScale) * 
                                 Matrix.CreateRotationY(_mercuryRotation) * 
                                 Matrix.CreateRotationZ(_mercuryAxisRotation) *
                                 Matrix.CreateTranslation(new Vector3(MercuryDistance * (float)Math.Cos(_mercuryRotation), 0, MercuryDistance * 0.75f * (float)Math.Sin(_mercuryRotation)));
            _mercury.Draw(_basicEffect);

            _basicEffect.World = Matrix.CreateScale(VenusScale) *
                                 Matrix.CreateRotationY(_venusRotation) *
                                 Matrix.CreateRotationZ(_venusAxisRotation) *
                                 Matrix.CreateTranslation(new Vector3(VenusDistance * (float)Math.Cos(_venusRotation), 0, VenusDistance * 0.75f * (float)Math.Sin(_venusRotation)));
            _venus.Draw(_basicEffect);

            Vector3 earthPosition = new Vector3(EarthDistance * (float)Math.Cos(_earthRotation), 0, EarthDistance * 0.75f * (float)Math.Sin(_earthRotation));
            _basicEffect.World = Matrix.CreateScale(EarthScale) *
                                 Matrix.CreateRotationY(_earthRotation) *
                                 Matrix.CreateRotationZ(_earthAxisRotation) *
                                 Matrix.CreateTranslation(earthPosition);
            _earth.Draw(_basicEffect);

            Vector3 moonPosition = earthPosition + new Vector3(MoonDistance * (float)Math.Cos(_moonRotation), 0, MoonDistance * 0.75f * (float)Math.Sin(_moonRotation));
            _basicEffect.World = Matrix.CreateScale(MoonScale) *
                                 Matrix.CreateRotationY(_moonRotation) *
                                 Matrix.CreateRotationZ(_moonAxisRotation) *
                                 Matrix.CreateTranslation(moonPosition);
            _moon.Draw(_basicEffect);

            _basicEffect.World = Matrix.CreateScale(MarsScale) *
                                 Matrix.CreateRotationY(_marsRotation) *
                                 Matrix.CreateRotationZ(_marsAxisRotation) *
                                 Matrix.CreateTranslation(new Vector3(MarsDistance * (float)Math.Cos(_marsRotation), 0, MarsDistance * 0.75f * (float)Math.Sin(_marsRotation)));
            _mars.Draw(_basicEffect);

            base.Draw(gameTime);
        }
    }
}
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game4
{
    public class ArmComponent
    {
        private VertexPositionTexture[] _vertices;
        private GraphicsDevice _graphicsDevice;
        private short[] _indices;
        private Texture2D _texture;

        public ArmComponent(GraphicsDevice graphicsDevice, Vector3 size, Texture2D texture)
        {
            _graphicsDevice = graphicsDevice;
            // Wczytywanie textury prostopadłościanu
            _texture = texture;

            // Inicjalizacja tablicy wierzchołków
            _vertices = new VertexPositionTexture[24];
            
            /* Wypełnianie tablicy wierzchołków
            *  Wierzchołki są tworzone dla każdej ściany boksie
            *  Każda ściana ma 4 wierzchołki
            * Współrzędne tekstur są ustawione tak, aby tekstura była odpowiednio naciągnięta na każdą ścianę 
            */
            _vertices[0] = new VertexPositionTexture(new Vector3(-size.X / 2, -size.Y / 2, 0), new Vector2(0, 0));
            _vertices[1] = new VertexPositionTexture(new Vector3(-size.X / 2, -size.Y / 2, size.Z), new Vector2(0, 1));
            _vertices[2] = new VertexPositionTexture(new Vector3(-size.X / 2, size.Y / 2, 0), new Vector2(1, 0));
            _vertices[3] = new VertexPositionTexture(new Vector3(-size.X / 2, size.Y / 2, size.Z), new Vector2(1, 1));
            
            _vertices[4] = new VertexPositionTexture(new Vector3(size.X / 2, -size.Y / 2, 0), new Vector2(0, 0));
            _vertices[5] = new VertexPositionTexture(new Vector3(size.X / 2, -size.Y / 2, size.Z), new Vector2(0, 1));
            _vertices[6] = new VertexPositionTexture(new Vector3(size.X / 2, size.Y / 2, 0), new Vector2(1, 0));
            _vertices[7] = new VertexPositionTexture(new Vector3(size.X / 2, size.Y / 2, size.Z), new Vector2(1, 1));
            
            _vertices[8] = new VertexPositionTexture(new Vector3(-size.X / 2, -size.Y / 2, 0), new Vector2(0, 0));
            _vertices[9] = new VertexPositionTexture(new Vector3(-size.X / 2, -size.Y / 2, size.Z), new Vector2(0, 1));
            _vertices[10] = new VertexPositionTexture(new Vector3(size.X / 2, -size.Y / 2, 0), new Vector2(1, 0));
            _vertices[11] = new VertexPositionTexture(new Vector3(size.X / 2, -size.Y / 2, size.Z), new Vector2(1, 1));
            
            _vertices[12] = new VertexPositionTexture(new Vector3(-size.X / 2, size.Y / 2, 0), new Vector2(0, 0));
            _vertices[13] = new VertexPositionTexture(new Vector3(-size.X / 2, size.Y / 2, size.Z), new Vector2(0, 1));
            _vertices[14] = new VertexPositionTexture(new Vector3(size.X / 2, size.Y / 2, 0), new Vector2(1, 0));
            _vertices[15] = new VertexPositionTexture(new Vector3(size.X / 2, size.Y / 2, size.Z), new Vector2(1, 1));
            
            _vertices[16] = new VertexPositionTexture(new Vector3(-size.X / 2, -size.Y / 2, size.Z), new Vector2(0, 0));
            _vertices[17] = new VertexPositionTexture(new Vector3(-size.X / 2, size.Y / 2, size.Z), new Vector2(0, 1));
            _vertices[18] = new VertexPositionTexture(new Vector3(size.X / 2, -size.Y / 2, size.Z), new Vector2(1,0));
            _vertices[19] = new VertexPositionTexture(new Vector3(size.X / 2, size.Y / 2, size.Z), new Vector2(1, 1));
            
            _vertices[20] = new VertexPositionTexture(new Vector3(-size.X / 2, -size.Y / 2, 0), new Vector2(0, 0));
            _vertices[21] = new VertexPositionTexture(new Vector3(-size.X / 2, size.Y / 2, 0), new Vector2(0, 1));
            _vertices[22] = new VertexPositionTexture(new Vector3(size.X / 2, -size.Y / 2, 0), new Vector2(1, 0));
            _vertices[23] = new VertexPositionTexture(new Vector3(size.X / 2, size.Y / 2, 0), new Vector2(1, 1));

            /* Inicjalizacja tablicy indeksów
            *  Każde 3 indeksy tworzą jeden trójkąt
            *  Każda ściana składa się z 2 trójkątów 
            */
            _indices = new short[]
            {
                0, 2, 1,
                1, 2, 3,

                4, 5, 6,
                5, 7, 6,

                8, 10, 9,
                9, 10, 11,

                12, 13, 14,
                13, 15, 14,

                16, 18, 17,
                17, 18, 19,

                20, 21, 22,
                21, 23, 22
            };
        }

        public void Draw(BasicEffect effect)
        {
            // Zapisuje stary stan RasterizerState
            var oldState = _graphicsDevice.RasterizerState;
            // Tworzy nowy stan RasterizerState, brak pomijania trójkątów niezależnie czy są skierowane do kamery
            var rasterizerState = new RasterizerState { CullMode = CullMode.None };
            // Ustawia nowy stan RasterizerState
            _graphicsDevice.RasterizerState = rasterizerState;

            // Włącza teksturę
            effect.TextureEnabled = true;
            // Ustawia teksturę
            effect.Texture = _texture;

            // Iteruje przez wszystkie przejścia efektu
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                // Stosuje efekt
                pass.Apply();
                // Rysuje prymitywy
                _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length, _indices, 0, _indices.Length / 3);
            }
            
            effect.TextureEnabled = false;
            // Przywraca stary stan RasterizerState
            _graphicsDevice.RasterizerState = oldState;
        }
    }
}
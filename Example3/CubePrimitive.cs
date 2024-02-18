using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game3
{
    // Klasa przeznaczona do rysowania sześcianów
    public class CubePrimitive
    {
        private VertexPositionColor[] _vertices;
        private GraphicsDevice _graphicsDevice;
        private short[] _indices;

        public CubePrimitive(GraphicsDevice graphicsDevice, float size, Color color)
        {
            _graphicsDevice = graphicsDevice;
            
            _vertices = new VertexPositionColor[8];
            _vertices[0] = new VertexPositionColor(new Vector3(-size / 2, -size / 2, -size / 2), color);
            _vertices[1] = new VertexPositionColor(new Vector3(-size / 2, -size / 2, size / 2), color);
            _vertices[2] = new VertexPositionColor(new Vector3(-size / 2, size / 2, -size / 2), color);
            _vertices[3] = new VertexPositionColor(new Vector3(-size / 2, size / 2, size / 2), color);
            _vertices[4] = new VertexPositionColor(new Vector3(size / 2, -size / 2, -size / 2), color);
            _vertices[5] = new VertexPositionColor(new Vector3(size / 2, -size / 2, size / 2), color);
            _vertices[6] = new VertexPositionColor(new Vector3(size / 2, size / 2, -size / 2), color);
            _vertices[7] = new VertexPositionColor(new Vector3(size / 2, size / 2, size / 2), color);

            _indices = new short[] 
            {
                // lewa
                0, 2, 1,
                1, 2, 3,

                // prawa
                4, 5, 6,
                5, 7, 6,

                // przód
                0, 1, 4,
                1, 5, 4,

                // tył
                2, 6, 3,
                3, 6, 7,

                // dół
                0, 4, 2,
                2, 4, 6,

                // góra
                1, 3, 5,
                3, 7, 5
            };
        }

        public void Draw(BasicEffect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length, _indices, 0, _indices.Length / 3);
            }
        }
    }
}

using OpenTK.Mathematics;

namespace BetaMinecraft.Graphics
{
    public readonly struct Vertex
    {
        public Vector3 Position { get; }

        public Vector2 UV0 { get; }

        public Vertex(float x, float y, float z, float u, float v)
        {
            Position = new Vector3(x, y, z);
            UV0 = new Vector2(u, v);
        }

        public Vertex(Vector3 position, Vector2 uv0)
        {
            Position = position;
            UV0 = uv0;
        }
    }
}

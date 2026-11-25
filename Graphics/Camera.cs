using OpenTK.Mathematics;

namespace Minecraft.Graphics
{
    public class Camera : Transformable
    {
        public float fieldOfView = 80f;

        public float nearPlaneDistance = 0.1f;

        public float farPlaneDistance = 1000f;

        public Matrix4 ViewMatrix => WorldMatrix.Inverted();
    }
}

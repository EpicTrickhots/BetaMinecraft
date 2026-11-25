using Minecraft.Graphics;

namespace Minecraft.Core.Player
{
    public abstract class PlayerController : Transformable
    {
        public Camera camera;

        public PlayerController()
        {
            camera = new Camera();
            Add(camera);
        }
    }
}

using Minecraft.Core;
using Minecraft.Core.Player;
using Minecraft.Generator;
using Minecraft.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Minecraft.Screens
{
    public class GameScreen : IScreen
    {
        ChunkGenerator generator = new ChunkGenerator(new TestGenerator());

        PlayerController controller = new DebugController();

        private static Matrix4 currentCameraViewMatrix;

        private static Matrix4 currentCameraProjectionMatrix;

        public void Start()
        {
            generator.Initialize();
        }

        public void Update()
        {
            generator.viewPosition = controller.Position;
            generator.Update();
        }

        public void RenderFrame()
        {
            Camera camera = controller.camera;
            currentCameraProjectionMatrix = camera.ViewMatrix;
            currentCameraProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                camera.fieldOfView * MathHelper.DegToRad,
                MinecraftWindow.ViewportAspect,
                camera.nearPlaneDistance,
                camera.farPlaneDistance
            );

            Shader shader = Resources.shaders["terrain"];
            shader.Use();

            GL.UniformMatrix4(shader.uniformLocations["uView"], true, ref currentCameraViewMatrix);
            GL.UniformMatrix4(shader.uniformLocations["uProj"], true, ref currentCameraProjectionMatrix);

            foreach(Chunk c in generator.renderedChunks)
            {

            }
        }
    }
}

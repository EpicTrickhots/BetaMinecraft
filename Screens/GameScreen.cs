using System;
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
        ChunkGenerator generator = new ChunkGenerator(new VoxelTerrainGenerator(2837));

        PlayerController controller = new DebugController();

        private static Matrix4 currentTransformationMatrix;

        public void Start()
        {
            generator.Initialize();
        }

        public void Update()
        {
            generator.viewPosition = controller.Position;
            generator.Update();
            controller.Update();
        }

        public void RenderFrame()
        {
            Camera camera = controller.camera;
            currentTransformationMatrix = camera.ViewMatrix * Matrix4.CreatePerspectiveFieldOfView(
                camera.fieldOfView * MathHelper.DegToRad,
                MinecraftWindow.ViewportAspect,
                camera.nearPlaneDistance,
                camera.farPlaneDistance
            );

            Resources.textures["grass"].Use(TextureUnit.Texture0);

            Shader shader = Resources.shaders["terrain"];
            shader.Use();

            GL.UniformMatrix4(shader.uniformLocations["uTransformation"], true, ref currentTransformationMatrix);

            foreach(Vector3i c in generator.renderedChunks)
            {
                if(generator.cachedChunks.TryGetValue(c, out Chunk? chunk))
                {
                    if (chunk.mesh == null)
                        return;

                    GL.BindVertexArray(chunk.mesh.VertexArrayObject);
                    GL.DrawElements(PrimitiveType.Triangles, chunk.mesh.indices.Length, DrawElementsType.UnsignedInt, 0);
                }
            }
        }
    }
}

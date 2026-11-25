using Minecraft.Generator;

namespace Minecraft.Screens
{
    public class GameScreen : IScreen
    {
        ChunkGenerator chunkGenerator = new ChunkGenerator(new TestGenerator());

        public void Start()
        {
            chunkGenerator.Initialize();
        }

        public void Update()
        {
            chunkGenerator.Update();
        }

        public void RenderFrame()
        {

        }
    }
}

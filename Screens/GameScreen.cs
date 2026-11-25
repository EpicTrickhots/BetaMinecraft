using Minecraft.Core.Player;
using Minecraft.Generator;

namespace Minecraft.Screens
{
    public class GameScreen : IScreen
    {
        ChunkGenerator chunkGenerator = new ChunkGenerator(new TestGenerator());

        PlayerController controller = new DebugController();

        public void Start()
        {
            chunkGenerator.Initialize();
        }

        public void Update()
        {
            chunkGenerator.viewPosition = controller.Position;
            chunkGenerator.Update();
        }

        public void RenderFrame()
        {

        }
    }
}

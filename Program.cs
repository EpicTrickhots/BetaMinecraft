using Minecraft.Core;

namespace Minecraft
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using(MinecraftWindow game = new MinecraftWindow())
            {
                game.Run();
            }
        }
    }
}

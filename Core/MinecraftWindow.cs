using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BetaMinecraft.Core
{
    internal class MinecraftWindow() 
        : GameWindow(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Title = "Minecraft",
            WindowState = WindowState.Maximized,
            ClientSize = (1200, 800)
        })
    {
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(Color4.Black);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SwapBuffers();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
        }
    }
}

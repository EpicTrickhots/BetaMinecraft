using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Minecraft.Screens;

namespace Minecraft.Core
{
    internal class MinecraftWindow() 
        : GameWindow(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Title = "Minecraft",
            WindowState = WindowState.Maximized,
            ClientSize = (1200, 800),
            Vsync = VSyncMode.On
        })
    {
        public readonly IScreen[] screens = [ new GameScreen() ];

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.Viewport(0, 0, FramebufferSize.X, FramebufferSize.Y);
            GL.ClearColor(Color4.Blue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            SwapBuffers();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}

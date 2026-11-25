using Minecraft.InputHandling;
using Minecraft.Graphics;
using Minecraft.Screens;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;
using System.IO;

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
        public readonly IScreen screen = new GameScreen();

        public static float ViewportAspect { get; private set; }

        protected override void OnLoad()
        {
            base.OnLoad();

            UpdateAspect();

            GL.Viewport(0, 0, FramebufferSize.X, FramebufferSize.Y);
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            CreateApplicationResources();
            screen.Start();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            Input.keyboard = KeyboardState;
            Input.mouse = MouseState;

            screen.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            screen.RenderFrame();

            SwapBuffers();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            UpdateAspect();
        }

        private void UpdateAspect()
        {
            ViewportAspect = FramebufferSize.Y > 0 ? FramebufferSize.X / (float)FramebufferSize.Y : 1f;
        }

        private void CreateApplicationResources()
        {
            Shader terrainShader = Shader.Load(Path.Combine(AppContext.BaseDirectory, @"Datafiles\Shaders"), "terrain");
            Resources.shaders.Add("terrain", terrainShader);
        }
    }
}

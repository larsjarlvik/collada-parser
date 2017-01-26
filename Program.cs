using System;
using ColladaParser.Shaders;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser
{
	public class Program : GameWindow
	{
		private DefaultShader defaultShader;
		private Model cube;

		private Matrix4 projectionMatrix;
		private Matrix4 modelViewMatrix;

		public Program()
			: base(1280, 720,
			new GraphicsMode(), "ColladaParser", 0,
			DisplayDevice.Default, 3, 3,
			GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug) { }

		protected override void OnLoad (System.EventArgs e)
		{
			VSync = VSyncMode.On;

			defaultShader = new DefaultShader();
			cube = new Model();
			cube.Bind(defaultShader.ShaderProgram);

			GL.Enable(EnableCap.DepthTest);
			GL.ClearColor(Color.Black);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			var rotation = Matrix4.CreateRotationY((float)e.Time);

			Matrix4.Mult(ref rotation, ref modelViewMatrix, out modelViewMatrix);
			GL.UniformMatrix4(defaultShader.ModelViewMatrix, false, ref modelViewMatrix);

			if (Keyboard[OpenTK.Input.Key.Escape])
				Exit();
		}

		protected override void OnResize(EventArgs e)
		{
			var aspectRatio = (float)Width / (float)Height;

			Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, aspectRatio, 1, 100, out projectionMatrix);
			modelViewMatrix = Matrix4.LookAt(new Vector3(0, 3, 5), new Vector3(0, 0, 0), new Vector3(0, 1, 0));

			GL.UniformMatrix4(defaultShader.ProjectionMatrix, false, ref projectionMatrix);
			GL.UniformMatrix4(defaultShader.ModelViewMatrix, false, ref modelViewMatrix);

			GL.Viewport(0, 0, Width, Height);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			cube.Render();

			SwapBuffers();
		}

		public static void Main()
		{
			using (Program example = new Program())
			{
				example.Run(30);
			}
		}
	}
}
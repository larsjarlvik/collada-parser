using System;
using ColladaParser.Collada;
using ColladaParser.Collada.Model;
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

		private ColladaModel model;

		private Matrix4 projectionMatrix;
		private Matrix4 modelViewMatrix;

		private float cameraDistance = 10.0f;

		public Program()
			: base(1280, 720,
			new GraphicsMode(), "ColladaParser", 0,
			DisplayDevice.Default, 3, 3,
			GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug) { }

		protected override void OnLoad (System.EventArgs e)
		{
			VSync = VSyncMode.On;

			defaultShader = new DefaultShader();

			model = ColladaLoader.Load("model");
			model.CreateVBOs();
			model.Bind(defaultShader.ShaderProgram);

			cube = new Model();
			cube.Bind(defaultShader.ShaderProgram);

			GL.Enable(EnableCap.DepthTest);
			GL.ClearColor(Color.Gray);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			var rotation = Matrix4.CreateRotationY((float)e.Time);

			Matrix4.Mult(ref rotation, ref modelViewMatrix, out modelViewMatrix);
			GL.UniformMatrix4(defaultShader.ModelViewMatrix, false, ref modelViewMatrix);

			if (Keyboard[OpenTK.Input.Key.Escape])
				Exit();

			if (Keyboard[OpenTK.Input.Key.W]) {
				cameraDistance -= 0.5f;
				OnResize(null);
			}
			if (Keyboard[OpenTK.Input.Key.S]) {
				cameraDistance += 0.5f;
				OnResize(null);
			}
		}

		protected override void OnResize(EventArgs e)
		{
			var aspectRatio = (float)Width / (float)Height;

			Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, aspectRatio, 1, 100, out projectionMatrix);
			modelViewMatrix = Matrix4.LookAt(new Vector3(0, cameraDistance * 1.5f, -cameraDistance * 2.0f), new Vector3(0, 2, 0), new Vector3(0, 1, 0));

			GL.UniformMatrix4(defaultShader.ProjectionMatrix, false, ref projectionMatrix);
			GL.UniformMatrix4(defaultShader.ModelViewMatrix, false, ref modelViewMatrix);

			GL.Viewport(0, 0, Width, Height);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			model.Render();

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
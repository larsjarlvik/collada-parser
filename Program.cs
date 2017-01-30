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
		private ColladaModel model;

		private Matrix4 projectionMatrix;
		private Matrix4 modelViewMatrix;

		private float cameraDistance = 20.0f;
		private float cameraRotation = 0.0f;

		private int FPS;

		public Program()
			: base(1280, 720,
			new GraphicsMode(32, 24, 0, 4), "ColladaParser", 0,
			DisplayDevice.Default, 3, 3,
			GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug) { }

		protected override void OnLoad (System.EventArgs e)
		{
			VSync = VSyncMode.Off;

			GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1.0f);
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.DepthTest);
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
			GL.ClearColor(Color.FromArgb(255, 24, 24, 24));

			defaultShader = new DefaultShader();

			model = ColladaLoader.Load("cowboy");
			model.CreateVBOs();
			model.LoadTextures();
			model.Bind(defaultShader.ShaderProgram, defaultShader.Texture, defaultShader.HaveTexture);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
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
			if (Keyboard[OpenTK.Input.Key.F]) {
				WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
			}
			
			cameraRotation += (float)e.Time;
			var camX = (float)Math.Sin(cameraRotation) * cameraDistance;
 			var camZ = (float)Math.Cos(cameraRotation) * cameraDistance;

			modelViewMatrix = Matrix4.LookAt(new Vector3(camX, cameraDistance * 0.5f, camZ), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
			GL.UniformMatrix4(defaultShader.ModelViewMatrix, false, ref modelViewMatrix);

			Title = $"Collada Parser (Vsync: {VSync}) FPS: {FPS}";
		}

		protected override void OnResize(EventArgs e)
		{
			var aspectRatio = (float)Width / (float)Height;

			Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, aspectRatio, 1, 100, out projectionMatrix);
			GL.UniformMatrix4(defaultShader.ProjectionMatrix, false, ref projectionMatrix);

			GL.Viewport(0, 0, Width, Height);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			FPS = (int)(1f / e.Time);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			model.Render();

			SwapBuffers();
		}

		public static void Main()
		{
			using (var program = new Program())
			{
				program.Run(30);
			}
		}
	}
}
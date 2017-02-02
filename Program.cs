using System;
using System.Diagnostics;
using ColladaParser.Collada;
using ColladaParser.Collada.Model;
using ColladaParser.Shaders;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace ColladaParser
{
	public class Program : GameWindow
	{
		private DefaultShader defaultShader;
		private ColladaModel model;

		private float cameraDistance = 20.0f;
		private float cameraRotation = 0.0f;

		private int FPS;
		private string modelName;
		private bool useBlend;

		private Multisampling multisampling;

		public Program(string modelName) : base(
			1280, 720,
			new GraphicsMode(32, 24, 0, 0), "ColladaParser", 0,
			DisplayDevice.Default, 3, 3,
			GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug) 
		{
			this.modelName = modelName;
			Keyboard.KeyRepeat = false;
		}

		protected override void OnLoad (System.EventArgs e)
		{
			VSync = VSyncMode.On;

			GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1.0f);
			GL.Enable(EnableCap.Texture2D);
			GL.Enable(EnableCap.DepthTest);
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
			GL.ClearColor(Color.FromArgb(255, 24, 24, 24));

			multisampling = new Multisampling(Width, Height, 8);
			defaultShader = new DefaultShader();

			model = ColladaLoader.Load(modelName);
			model.CreateVBOs();
			model.LoadTextures();
			model.Bind(defaultShader.ShaderProgram, 
				defaultShader.Texture, 
				defaultShader.HaveTexture,
				defaultShader.Ambient,
				defaultShader.Diffuse,
				defaultShader.Specular,
				defaultShader.Shininess);
		}

		protected override void OnKeyDown(KeyboardKeyEventArgs e) 
		{
			if(e.IsRepeat)
				return;

			if (Keyboard[OpenTK.Input.Key.Escape])
				Exit();

			if (Keyboard[OpenTK.Input.Key.F])
				WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;

			if (Keyboard[OpenTK.Input.Key.B]) {
				useBlend = !useBlend;

				if(useBlend) {
					GL.Disable(EnableCap.DepthTest);
					GL.Enable(EnableCap.Blend);
				} else {
					GL.Enable(EnableCap.DepthTest);
					GL.Disable(EnableCap.Blend);
				}
			}
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			if (Keyboard[OpenTK.Input.Key.W]) {
				cameraDistance -= 0.5f;
			}
			if (Keyboard[OpenTK.Input.Key.S]) {
				cameraDistance += 0.5f;
			}

			cameraRotation += (float)e.Time;
			var camX = (float)Math.Sin(cameraRotation) * cameraDistance;
 			var camZ = (float)Math.Cos(cameraRotation) * cameraDistance;

			Matrix.SetViewMatrix(defaultShader.ViewMatrix, new Vector3(camX, cameraDistance * 0.5f, camZ), new Vector3(0, 0, 0));
			Title = $"Collada Parser (Vsync: {VSync}) FPS: {FPS}";
		}

		protected override void OnResize(EventArgs e)
		{
			var aspectRatio = (float)Width / (float)Height;
			Matrix.SetProjectionMatrix(defaultShader.ProjectionMatrix, (float)Math.PI / 4, aspectRatio);
			
			GL.Viewport(0, 0, Width, Height);
			multisampling.RefreshBuffers(Width, Height);
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			FPS = (int)(1f / e.Time);
			multisampling.Bind();

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			model.Render();

			multisampling.Draw();
			SwapBuffers();
		}

		public static void Main(string[] args)
		{
			if(args.Length < 1) {
				Console.WriteLine("Model not specified!");
				Environment.Exit(-1);
			}

			using (var program = new Program(args[0]))
			{
				program.Run(30);
			}
		}
	}
}
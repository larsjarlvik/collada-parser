using System;
using System.IO;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Shaders
{
	public class BaseShader
	{
		public int ShaderProgram;

		public BaseShader(string name) 
		{
			var vertexShader = GL.CreateShader(ShaderType.VertexShader);
			var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

			GL.ShaderSource(vertexShader, loadShaderSource($"{name}.vs.glsl"));
			GL.CompileShader(vertexShader);
			checkCompileStatus($"Vertex Shader: {name}", vertexShader);

			GL.ShaderSource(fragmentShader, loadShaderSource($"{name}.fs.glsl"));
			GL.CompileShader(fragmentShader);
			checkCompileStatus($"Fragment Shader: {name}", fragmentShader);
			
			ShaderProgram = GL.CreateProgram();
			GL.AttachShader(ShaderProgram, fragmentShader);
			GL.AttachShader(ShaderProgram, vertexShader);
			GL.LinkProgram(ShaderProgram);
			GL.UseProgram(ShaderProgram);

			SetUniforms();
		}

		protected virtual void SetUniforms() 
		{

		}

		private static string loadShaderSource(string name) 
		{
			var assembly = Assembly.GetEntryAssembly();

			using (var stream = assembly.GetManifestResourceStream($"collada-parser.shaders.{name}"))
				using (var reader = new StreamReader(stream))
					return reader.ReadToEnd();
		}

		private void checkCompileStatus(string shaderName, int shader)
		{
			int compileStatus;

			GL.GetShader(shader, ShaderParameter.CompileStatus, out compileStatus);
			if (compileStatus != 1)
				throw new ApplicationException($"Filed to Compiler {shaderName}: {GL.GetShaderInfoLog(shader)}");
		}
	}
}

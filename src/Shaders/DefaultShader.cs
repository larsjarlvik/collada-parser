using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Shaders
{
    public class DefaultShader : BaseShader
	{
		public int ProjectionMatrix { get; private set; }
		public int ViewMatrix { get; private set; }
		public int Texture { get; private set; }
		public int HaveTexture { get; private set; }

		public int Ambient { get; private set; }
		public int Diffuse { get; private set; }
		public int Specular { get; private set; }
		public int Shininess { get; private set; }

		public DefaultShader(): base("default") {}

		protected override void SetUniforms()
		{
			ProjectionMatrix = GL.GetUniformLocation(ShaderProgram, "uProjectionMatrix");
			ViewMatrix = GL.GetUniformLocation(ShaderProgram, "uViewMatrix");
			Texture = GL.GetUniformLocation(ShaderProgram, "uTexture");
			HaveTexture = GL.GetUniformLocation(ShaderProgram, "uHaveTexture");

			Ambient = GL.GetUniformLocation(ShaderProgram, "uAmbient");
			Diffuse = GL.GetUniformLocation(ShaderProgram, "uDiffuse");
			Specular = GL.GetUniformLocation(ShaderProgram, "uSpecular");
			Shininess = GL.GetUniformLocation(ShaderProgram, "uShininess");
		}
	}
}
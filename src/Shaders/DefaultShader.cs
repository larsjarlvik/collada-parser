using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Shaders
{
    public class DefaultShader : BaseShader
	{
		public int ProjectionMatrix { get; private set; }
		public int ModelViewMatrix { get; private set; }

		public DefaultShader(): base("default") {}

		protected override void SetUniforms()
		{
			ProjectionMatrix = GL.GetUniformLocation(ShaderProgram, "projection_matrix");
			ModelViewMatrix = GL.GetUniformLocation(ShaderProgram, "modelview_matrix");
		}
	}
}
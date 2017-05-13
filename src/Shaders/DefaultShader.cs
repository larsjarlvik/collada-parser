using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Shaders
{
    public class DefaultShader : BaseShader
	{
		private const int JOINT_TRANSFORM_COUNT = 50;


		public int ProjectionMatrix { get; private set; }
		public int ViewMatrix { get; private set; }

		public int Texture { get; private set; }
		public int HaveTexture { get; private set; }
		public int Ambient { get; private set; }
		public int Diffuse { get; private set; }
		public int Specular { get; private set; }
		public int Shininess { get; private set; }

		public int IsAnimated { get; private set; }
		public int[] JointTransforms { get; private set; }

		public DefaultShader(): base("default") {}

		protected override void SetUniforms()
		{
			// Matrix
			ProjectionMatrix = GL.GetUniformLocation(ShaderProgram, "uProjectionMatrix");
			ViewMatrix = GL.GetUniformLocation(ShaderProgram, "uViewMatrix");

			// Texture & Lighting
			Texture = GL.GetUniformLocation(ShaderProgram, "uTexture");
			HaveTexture = GL.GetUniformLocation(ShaderProgram, "uHaveTexture");

			Ambient = GL.GetUniformLocation(ShaderProgram, "uAmbient");
			Diffuse = GL.GetUniformLocation(ShaderProgram, "uDiffuse");
			Specular = GL.GetUniformLocation(ShaderProgram, "uSpecular");
			Shininess = GL.GetUniformLocation(ShaderProgram, "uShininess");

			// Animations
			IsAnimated = GL.GetUniformLocation(ShaderProgram, "uIsAnimated");

			JointTransforms = new int[JOINT_TRANSFORM_COUNT];
			for (var i = 0; i < JOINT_TRANSFORM_COUNT; i ++)
				JointTransforms[i] = GL.GetUniformLocation(ShaderProgram, $"uJointTransforms[{i}]");
		}
	}
}
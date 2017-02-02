using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser
{
	public static class Matrix
	{
		public static void SetViewMatrix(int location, Vector3 eye, Vector3 target)
		{
			var viewMatrix = Matrix4.LookAt(eye, target, new Vector3(0, 1, 0));
			GL.UniformMatrix4(location, false, ref viewMatrix);
		}

		public static void SetProjectionMatrix(int location, float fieldOfView, float aspectRatio)
		{
			var projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, 1, 100);
			GL.UniformMatrix4(location, false, ref projectionMatrix);
		}
	}
}
using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Collada.Animation
{
	public class KeyFrameTransform
	{
		private Vector3 position;
		private Quaternion rotation;

		public Matrix4 Transform { get; set; }

		public KeyFrameTransform(Vector3 position, Quaternion rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}

		public KeyFrameTransform(Matrix4 transform)
		{
			this.Transform = transform;
		}
	}
}
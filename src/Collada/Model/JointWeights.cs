using System;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Collada.Model
{
	public class JointWeights
	{
		public Vector3 Ids { get; private set; }
		public Vector3 Weights { get; private set; }

		public JointWeights(Vector3 ids, Vector3 weights)
		{
			this.Ids = ids;
			this.Weights = weights;
		}
	}
}
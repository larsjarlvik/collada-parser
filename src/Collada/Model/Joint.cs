using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Collada.Model
{
	[DebuggerDisplay("{Name} ({Id})")]
	public class Joint
	{
		public int Id { get; private set; }
		public string Name { get; private set; }
		public List<Joint> Children { get; set; }
		public Matrix4 Transform { get; set; }

		public Joint(int id, string name, Matrix4 transform)
		{
			this.Id = id;
			this.Transform = transform;

			this.Name = name;
			this.Children = new List<Joint>();
		}
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Collada.Model
{
	[DebuggerDisplay("Name: {Name}")]
	public class Joint
	{
		private Matrix4 transform;

		public int Id { get; private set; }
		public string Name { get; private set; }
		public List<Joint> Children { get; set; }

		public Joint(int id, string name, Matrix4 transform)
		{
			this.Id = id;
			this.transform = transform;

			this.Name = name;
			this.Children = new List<Joint>();
		}
	}
}
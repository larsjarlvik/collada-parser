using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Collada.Animation
{
	public class KeyFrame
	{
		public float TimeStamp { get; private set; }

		public Dictionary<int, KeyFrameTransform> Transforms { get; private set; }

		public KeyFrame(float timeStamp)
		{
			this.TimeStamp = timeStamp;
			this.Transforms = new Dictionary<int, KeyFrameTransform>();
		}

	}
}
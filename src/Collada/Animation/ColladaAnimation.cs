using System;
using System.Collections.Generic;
using System.Diagnostics;
using ColladaParser.Collada.Model;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Collada.Animation
{
	public class ColladaAnimation 
	{
		private KeyFrame[] keyFrames;

		public ColladaAnimation(KeyFrame[] keyFrames)
		{
			this.keyFrames = keyFrames;
		}

		public void Animate(ColladaModel model, int isAnimatedLoc, int[] jointTransformsLoc) 
		{
			GL.Uniform1(isAnimatedLoc, 1);
		}
	}
}
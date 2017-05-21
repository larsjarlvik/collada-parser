using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

		public void SetAnimationState(int isAnimatedLoc, bool isAnimated)
		{
			GL.Uniform1(isAnimatedLoc, isAnimated ? 1 : 0);
		}

		public void SetKeyFrame(ColladaModel model, int keyFrame, int[] jointTransformsLoc) 
		{
			var geo = model.Geometries.First();

			ApplyPoseToJoint(geo.RootJoint, keyFrame, Matrix4.Identity);
			geo.BindJointTransforms(jointTransformsLoc, geo.RootJoint);
		}

		private void ApplyPoseToJoint(Joint joint, int keyFrame, Matrix4 parentTransform)
		{
			var localTransform = keyFrames[keyFrame].Transforms[joint.Id].Transform;
			var currentTransform = Matrix4.Mult(parentTransform, localTransform);

			foreach (var childJoint in joint.Children)
				ApplyPoseToJoint(childJoint, keyFrame, currentTransform);

			joint.Transform = currentTransform;
		}
	}
}
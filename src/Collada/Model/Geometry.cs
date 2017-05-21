using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Collada.Model
{
	public class Geometry
	{
		private int modelBuffer;
		private int positionBuffer;
		private int normalBuffer;
		private int textureBuffer;
		private int colorBuffer;
		private int jointWeightBuffer;
		private int jointIdBuffer;
		private int indexBuffer;

		private int numIndices;

		private Vector3[] vertices;
		private Vector3[] normals;
		private Vector2[] textures;
		private Vector3[] colors;

		// Animations
		private Vector3[] jointWeights;
		private Vector3[] jointIds;
		public Joint RootJoint { get; private set; }

		// Indices
		private int[] indices;

		public Geometry(Vector3[] vertices, Vector3[] normals, Vector2[] textures, Vector3[] colors, int[] indices)
		{
			this.vertices = vertices;
			this.normals = normals;
			this.textures = textures;
			this.colors = colors;
			this.indices = indices;
		}

		public void AppendJointInformation(JointWeights[] jointWeights, Joint rootJoint) 
		{
			this.jointWeights = jointWeights.Select(x => x.Weights).ToArray();
			this.jointIds = jointWeights.Select(x => x.Ids).ToArray();
			this.RootJoint = rootJoint;
		}

		public void CreateVBOs()
		{
			// Position
			positionBuffer = GenBuffer();
			GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
				new IntPtr(vertices.Length * Vector3.SizeInBytes),
				vertices, BufferUsageHint.StaticDraw);

			// Normals
			normalBuffer = GenBuffer();
			GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
				new IntPtr(normals.Length * Vector3.SizeInBytes),
				normals, BufferUsageHint.StaticDraw);

			// Textures
			if(textures != null) {
				textureBuffer = GenBuffer();
				GL.BufferData<Vector2>(BufferTarget.ArrayBuffer,
					new IntPtr(textures.Length * Vector2.SizeInBytes),
					textures, BufferUsageHint.StaticDraw);
			}

			// Colors
			if(colors != null) {
				colorBuffer = GenBuffer();
				GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
					new IntPtr(colors.Length * Vector3.SizeInBytes),
					colors, BufferUsageHint.StaticDraw);
			}

			// Joint Weights
			if(jointWeights != null) {
				jointWeightBuffer = GenBuffer();
				GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
					new IntPtr(jointWeights.Length * Vector3.SizeInBytes),
					jointWeights, BufferUsageHint.StaticDraw);
			}

			// Joint Ids
			if(jointIds != null) {
				jointIdBuffer = GenBuffer();
				GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
					new IntPtr(jointIds.Length * Vector3.SizeInBytes),
					jointIds, BufferUsageHint.StaticDraw);
			}

			// Indices
			GL.GenBuffers(1, out indexBuffer);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer,
				new IntPtr(sizeof(int) * indices.Length),
				indices, BufferUsageHint.StaticDraw);

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			numIndices = indices.Length;
		}

		public void Bind(int shaderProgram)
		{
			GL.GenVertexArrays(1, out modelBuffer);
			GL.BindVertexArray(modelBuffer);

			BindBuffer(shaderProgram, positionBuffer, 0, Vector3.SizeInBytes, "in_position");
			BindBuffer(shaderProgram, normalBuffer, 1, Vector3.SizeInBytes, "in_normal");

			if (textures != null)     BindBuffer(shaderProgram, textureBuffer, 2, Vector2.SizeInBytes, "in_texture");
			if (colors != null)       BindBuffer(shaderProgram, colorBuffer, 3, Vector3.SizeInBytes, "in_color");
			if (jointWeights != null) BindBuffer(shaderProgram, jointWeightBuffer, 4, Vector3.SizeInBytes, "in_joint_weights");
			if (jointIds != null)     BindBuffer(shaderProgram, jointIdBuffer, 5, Vector3.SizeInBytes, "in_joint_ids");

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
			GL.BindVertexArray(0);
		}

		public void BindJointTransforms(int[] jointTransformsLoc, Joint joint)
		{
			var transform = joint.Transform;
			GL.UniformMatrix4(jointTransformsLoc[joint.Id], true, ref transform);

			foreach (var child in joint.Children) {
				BindJointTransforms(jointTransformsLoc, child);
			}
		}

		public void Render() 
		{
			GL.BindVertexArray(modelBuffer);
			GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);
		}

		private int GenBuffer()
		{
			int buffer;

			GL.GenBuffers(1, out buffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);

			return buffer;
		}

		private void BindBuffer(int shaderProgram, int buffer, int index, int size, string name)
		{
			GL.EnableVertexAttribArray(index);
			GL.BindBuffer(BufferTarget.ArrayBuffer, buffer);
			GL.VertexAttribPointer(index, 3, VertexAttribPointerType.Float, true, size, 0);
			GL.BindAttribLocation(shaderProgram, index, name);
		}
	}
}
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
		private Joint jointHierarchy;

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

		public void AppendJointInformation(JointWeights[] jointWeights, Joint jointHierarchy) 
		{
			this.jointWeights = jointWeights.Select(x => x.Weights).ToArray();
			this.jointIds = jointWeights.Select(x => x.Ids).ToArray();
			this.jointHierarchy = jointHierarchy;
		}

		public void CreateVBOs()
		{
			// Position
			GL.GenBuffers(1, out positionBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
			GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
				new IntPtr(vertices.Length * Vector3.SizeInBytes),
				vertices, BufferUsageHint.StaticDraw);

			// Normals
			GL.GenBuffers(1, out normalBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, normalBuffer);
			GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
				new IntPtr(normals.Length * Vector3.SizeInBytes),
				normals, BufferUsageHint.StaticDraw);

			// Textures
			if(textures != null) {
				GL.GenBuffers(1, out textureBuffer);
				GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuffer);
				GL.BufferData<Vector2>(BufferTarget.ArrayBuffer,
					new IntPtr(textures.Length * Vector2.SizeInBytes),
					textures, BufferUsageHint.StaticDraw);
			}

			// Colors
			if(colors != null) {
				GL.GenBuffers(1, out colorBuffer);
				GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
				GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
					new IntPtr(colors.Length * Vector3.SizeInBytes),
					colors, BufferUsageHint.StaticDraw);
			}

			// Joint Weights
			if(jointWeights != null) {
				GL.GenBuffers(1, out jointWeightBuffer);
				GL.BindBuffer(BufferTarget.ArrayBuffer, jointWeightBuffer);
				GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
					new IntPtr(jointWeights.Length * Vector3.SizeInBytes),
					jointWeights, BufferUsageHint.StaticDraw);
			}

			// Joint Ids
			if(jointIds != null) {
				GL.GenBuffers(1, out jointIdBuffer);
				GL.BindBuffer(BufferTarget.ArrayBuffer, jointIdBuffer);
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

			GL.EnableVertexAttribArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
			GL.BindAttribLocation(shaderProgram, 0, "in_position");

			GL.EnableVertexAttribArray(1);
			GL.BindBuffer(BufferTarget.ArrayBuffer, normalBuffer);
			GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
			GL.BindAttribLocation(shaderProgram, 1, "in_normal");

			if (textures != null) {
				GL.EnableVertexAttribArray(2);
				GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuffer);
				GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, true, Vector2.SizeInBytes, 0);
				GL.BindAttribLocation(shaderProgram, 2, "in_texture");
			}

			if (colors != null) {
				GL.EnableVertexAttribArray(3);
				GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
				GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
				GL.BindAttribLocation(shaderProgram, 3, "in_color");
			}

			if (jointWeights != null){
				GL.EnableVertexAttribArray(4);
				GL.BindBuffer(BufferTarget.ArrayBuffer, jointWeightBuffer);
				GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
				GL.BindAttribLocation(shaderProgram, 4, "in_joint_weights");
			}

			if (jointIds != null){
				GL.EnableVertexAttribArray(5);
				GL.BindBuffer(BufferTarget.ArrayBuffer, jointIdBuffer);
				GL.VertexAttribPointer(5, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
				GL.BindAttribLocation(shaderProgram, 5, "in_joint_ids");
			}

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
			GL.BindVertexArray(0);
		}

		public void Render() 
		{
			GL.BindVertexArray(modelBuffer);
			GL.DrawElements(PrimitiveType.Triangles, numIndices, DrawElementsType.UnsignedInt, IntPtr.Zero);
		}
	}
}
using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Collada.Model
{
    public class Geometry
	{
		private int modelBuffer;
		private int positionBuffer;
		private int normalBuffer;
		private int indexBuffer;

		private int numIndices;

		private Vector3[] vertices;
		private Vector3[] normals;
		private Vector2[] textures;
		private int[] indices;

		public Geometry(Vector3[] vertices, Vector3[] normals, Vector2[] textures, int[] indices)
		{
			this.vertices = vertices;
			this.normals = normals;
			this.textures = textures;
			this.indices = indices;
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
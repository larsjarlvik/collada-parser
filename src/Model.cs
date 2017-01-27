using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser
{
	public class Model
	{
		private int modelBuffer;
		private int positionBuffer;
		private int normalBuffer;
		private int colorBuffer;
		private int indexBuffer;

		private Vector3[] positionVboData = new Vector3[]{
			new Vector3(-1.0f, -1.0f,  1.0f),
			new Vector3( 1.0f, -1.0f,  1.0f),
			new Vector3( 1.0f,  1.0f,  1.0f),
			new Vector3(-1.0f,  1.0f,  1.0f),
			new Vector3(-1.0f, -1.0f, -1.0f),
			new Vector3( 1.0f, -1.0f, -1.0f), 
			new Vector3( 1.0f,  1.0f, -1.0f),
			new Vector3(-1.0f,  1.0f, -1.0f) };

		private Vector3[] colorVboData = new Vector3[]{
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(1.0f, 0.0f, 0.0f), 
			new Vector3(1.0f, 0.0f, 0.0f),
			new Vector3(1.0f, 0.0f, 0.0f) };

		private int[] indicesVboData = new int[]{
			0, 1, 2, 2, 3, 0,
			3, 2, 6, 6, 7, 3,
			7, 6, 5, 5, 4, 7,
			4, 0, 3, 3, 7, 4,
			0, 1, 5, 5, 4, 0,
			1, 5, 6, 6, 2, 1 };

		public Model()
		{
			createVBOs();
		}

		private void createVBOs()
		{
			// Position
			GL.GenBuffers(1, out positionBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
			GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
				new IntPtr(positionVboData.Length * Vector3.SizeInBytes),
				positionVboData, BufferUsageHint.StaticDraw);

			// Normals
			GL.GenBuffers(1, out normalBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, normalBuffer);
			GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
				new IntPtr(positionVboData.Length * Vector3.SizeInBytes),
				positionVboData, BufferUsageHint.StaticDraw);

			// Colors
			GL.GenBuffers(1, out colorBuffer);
			GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
			GL.BufferData<Vector3>(BufferTarget.ArrayBuffer,
				new IntPtr(colorVboData.Length * Vector3.SizeInBytes),
				colorVboData, BufferUsageHint.StaticDraw);

			// Indices
			GL.GenBuffers(1, out indexBuffer);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
			GL.BufferData(BufferTarget.ElementArrayBuffer,
				new IntPtr(sizeof(uint) * indicesVboData.Length),
				indicesVboData, BufferUsageHint.StaticDraw);

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
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

			GL.EnableVertexAttribArray(2);
			GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
			GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, true, Vector3.SizeInBytes, 0);
			GL.BindAttribLocation(shaderProgram, 2, "in_color");

			GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
			GL.BindVertexArray(0);
		}

		public void Render() 
		{
			GL.BindVertexArray(modelBuffer);
			GL.DrawElements(PrimitiveType.Triangles, indicesVboData.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
		}
	}
}

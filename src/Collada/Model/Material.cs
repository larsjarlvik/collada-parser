using System;
using System.IO;
using ColladaParser.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Collada.Model
{
	public class Material 
	{
		private const int BITMAP_HEADER_LENGTH = 54;
		private const int BITMAP_SIZE = 1024;

		private const int GL_LINEAR = 0x2601;
		private const int GL_LINEAR_MIPMAP_LINEAR = 0x2703;
		private const float GL_CLAMP_TO_EDGE = 0x812F;

		private bool haveTexture = false;


		private string fileName;
		private int textureId;

		public Material(string fileName)
		{
			this.fileName = fileName;
		}
			
		public unsafe void LoadTexture(string texturePath)
		{
			if (fileName == null)
				return;

			var imageStream = SourceLoader.AsStream($"{texturePath}.{fileName}");
			var buffer = new byte[BITMAP_SIZE * BITMAP_SIZE * 3];

			imageStream.Seek(BITMAP_HEADER_LENGTH, SeekOrigin.Begin);
			imageStream.Read(buffer, 0, BITMAP_SIZE * BITMAP_SIZE * 3);

			fixed (byte* p = buffer)
			{
				var ptr = (IntPtr)p;

				textureId = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, textureId);
				GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
				
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, BITMAP_SIZE, BITMAP_SIZE, 0, PixelFormat.Bgr, PixelType.UnsignedByte, ptr);

				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new [] { GL_LINEAR });
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new [] { GL_LINEAR_MIPMAP_LINEAR });

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, GL_CLAMP_TO_EDGE);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, GL_CLAMP_TO_EDGE);

				GL.BindTexture(TextureTarget.Texture2D, 0);
			}

			haveTexture = true;
		}

		public void Bind(int textureLocation, int haveTextureLocation)
		{
			if (haveTexture) {
				GL.BindTexture(TextureTarget.Texture2D, textureId);
				GL.Uniform1(textureLocation, 0);
				GL.Uniform1(haveTextureLocation, 1);
			} else {
				GL.Uniform1(haveTextureLocation, 0);
			}
		}
	}
}
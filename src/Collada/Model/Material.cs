using System;
using System.IO;
using System.Linq;
using System.Text;
using ColladaParser.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ColladaParser.Collada.Model
{
    public class Material 
	{
		private const int BITMAP_HEADER_LENGTH = 54;

		private const int GL_LINEAR = 0x2601;
		private const int GL_LINEAR_MIPMAP_LINEAR = 0x2703;
		private const float GL_CLAMP_TO_EDGE = 0x812F;

		private int textureWidth;
		private int textureHeight;
		private int textureId;

		private string textureName;

		// Material properties
		public string TextureName { 
			get { return textureName; }
			set {
				haveTexture = value != null;
				textureName = value;
			}
		}

		public bool haveTexture { get; private set; }
		public Vector4 Diffuse { get; set; }
		public Vector4 Specular { get; set; }
		public float Shininess { get; set; }


		private int parseHeader(byte[] header) 
		{
			var fileType = Encoding.ASCII.GetString(header.Take(2).ToArray());
			if (fileType != "BM")
				throw new ApplicationException($"Texture has invalid file type, expected BM got {fileType}!");

			var compression = BitConverter.ToInt32(header, 30);
			if (compression != 0)
				throw new ApplicationException("Compressed bitmaps are not supported!");

			textureWidth = BitConverter.ToInt32(header, 18);
			textureHeight = BitConverter.ToInt32(header, 22);

			return BitConverter.ToInt32(header, 10); // Start of image data
		}
			
		public unsafe void LoadTexture(string texturePath)
		{
			if (TextureName == null)
				return;

			var imageStream = SourceLoader.AsStream($"{texturePath}.{TextureName}");
			if (imageStream == null)
				throw new ApplicationException($"Texture resource '{texturePath}.{TextureName}' not found!");
			
			// Read bitmap header
			var header = new byte[BITMAP_HEADER_LENGTH];
			imageStream.Read(header, 0, BITMAP_HEADER_LENGTH);
			var start = parseHeader(header);

			// Read bitmap data
			var buffer = new byte[textureWidth * textureHeight * 3];
			imageStream.Seek(start, SeekOrigin.Begin);
			imageStream.Read(buffer, 0, textureWidth * textureHeight * 3);

			fixed (byte* p = buffer)
			{
				var ptr = (IntPtr)p;

				textureId = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, textureId);
				GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
				
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, textureWidth, textureHeight, 0, PixelFormat.Bgr, PixelType.UnsignedByte, ptr);

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
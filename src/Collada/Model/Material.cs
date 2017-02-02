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
		public Vector4 Ambient { get; set; }
		public Vector4 Diffuse { get; set; }
		public Vector4 Specular { get; set; }
		public float Shininess { get; set; }

		private PixelFormat pixelFormat;

		private int parseHeader(byte[] header) 
		{
			var fileType = Encoding.ASCII.GetString(header.Take(2).ToArray());
			if (fileType != "BM")
				throw new ApplicationException($"Texture has invalid file type, expected BM got {fileType}!");

			var format = BitConverter.ToInt32(header, 30);
			if (format != 0 && format != 3)
				throw new ApplicationException("Unsupported bitmap format!");

			pixelFormat = format == 0 ? PixelFormat.Bgr : PixelFormat.Bgra;

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
			var pixelSize = pixelFormat == PixelFormat.Bgr ? 3 : 4;
			var buffer = new byte[textureWidth * textureHeight * pixelSize];
			imageStream.Seek(start, SeekOrigin.Begin);
			imageStream.Read(buffer, 0, textureWidth * textureHeight * pixelSize);

			if (pixelFormat == PixelFormat.Bgra)
				reorganizeBuffer(buffer);

			fixed (byte* p = buffer)
			{
				var ptr = (IntPtr)p;

				textureId = GL.GenTexture();
				GL.BindTexture(TextureTarget.Texture2D, textureId);
				GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
				
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textureWidth, textureHeight, 0, pixelFormat, PixelType.UnsignedByte, ptr);

				GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new [] { GL_LINEAR });
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new [] { GL_LINEAR_MIPMAP_LINEAR });

				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, GL_CLAMP_TO_EDGE);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, GL_CLAMP_TO_EDGE);

				GL.BindTexture(TextureTarget.Texture2D, 0);
			}

			haveTexture = true;
		}

		public void Bind(int textureLoc, int haveTextureLoc, int ambientLoc, int diffuseLoc, int specularLoc, int shininessLoc)
		{
			if (haveTexture) {
				GL.BindTexture(TextureTarget.Texture2D, textureId);
				GL.Uniform1(textureLoc, 0);
				GL.Uniform1(haveTextureLoc, 1);
			} else {
				GL.Uniform1(haveTextureLoc, 0);
			}

			GL.Uniform3(ambientLoc, Ambient.X, Ambient.Y, Ambient.Z);
			GL.Uniform3(diffuseLoc, Diffuse.X, Diffuse.Y, Diffuse.Z);
			GL.Uniform3(specularLoc, Specular.X, Specular.Y, Specular.Z);
			GL.Uniform1(shininessLoc, Shininess);
		}

		private void reorganizeBuffer(byte[] buffer)
		{
			for(var i = 0; i < buffer.Count() - 4; i += 4) {
				var src = buffer.Skip(i).Take(4).ToArray();
				buffer[i]     = src[1];
				buffer[i + 1] = src[2];
				buffer[i + 2] = src[3];
				buffer[i + 3] = src[0];
			}
		}
	}
}


using OpenTK;

namespace ColladaParser.Collada.Model
{
	public class Vertex {
		
		private const int NO_INDEX = -1;
		
		public Vector3 Position { get; set; }
		public int TextureIndex { get; set; }
		public int NormalIndex { get; set; }
		public int Index { get; private set; }
		public Vertex DuplicateVertex { get; set; }

		public bool IsSet => TextureIndex != NO_INDEX && NormalIndex != NO_INDEX;
		
		public Vertex(int index, Vector3 position)
		{
			Index = index;
			TextureIndex = NO_INDEX;
			NormalIndex = NO_INDEX;
			Position = position;
		}
		
		public bool hasSameTextureAndNormal(int textureIndexOther, int normalIndexOther)
		{
			return textureIndexOther == TextureIndex && normalIndexOther == NormalIndex;
		}
	}
}

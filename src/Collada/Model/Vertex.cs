

using OpenTK;

namespace ColladaParser.Collada.Model
{
	public class Vertex 
	{	
		private const int NO_INDEX = -1;
		
		public Vector3 Position { get; set; }
		public int TextureIndex { get; set; }
		public int NormalIndex { get; set; }
		public int ColorIndex { get; set; }
		public int Index { get; private set; }
		public Vertex DuplicateVertex { get; set; }

		public bool IsSet => NormalIndex != NO_INDEX;
		
		public Vertex(int index, Vector3 position)
		{
			Index = index;
			NormalIndex = NO_INDEX;
			TextureIndex = NO_INDEX;
			ColorIndex = NO_INDEX;
			Position = position;
		}
		
		public bool HasSameInformation(int normalIndexOther, int textureIndexOther, int colorIndexOther)
		{
			return 
				textureIndexOther == TextureIndex && 
				normalIndexOther == NormalIndex &&
				colorIndexOther == ColorIndex;
		}
	}
}

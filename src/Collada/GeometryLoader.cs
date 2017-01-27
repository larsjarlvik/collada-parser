using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ColladaParser.Collada.Model;
using OpenTK;

namespace ColladaParser.Collada
{
	public class GeometryLoader
	{
		private static XNamespace ns = "{http://www.collada.org/2005/11/COLLADASchema}";

		public List<Vertex> Vertices { get; set; }
		public List<Vector3> Normals { get; set; }
		public List<Vector2> Textures { get; set; }
		public List<int> PolyList { get; set; }

		private XElement mesh;


		public GeometryLoader(XElement mesh)
		{
			Vertices = new List<Vertex>();
			Normals = new List<Vector3>();
			Textures = new List<Vector2>();
			PolyList = new List<int>();

			this.mesh = mesh;
		}

		public Geometry Load()
		{
			var positionId = mesh
				.Element($"{ns}vertices")
				.Element($"{ns}input")
				.Attribute("source").Value.TrimStart(new[]{ '#' });

			// Vertices
			Vertices = readVertices(mesh, positionId).ToList();

			// Normals
			var normals = mesh
				.Element($"{ns}polylist")
				.Elements($"{ns}input").FirstOrDefault(x => x.Attribute("semantic").Value == "NORMAL");
			if (normals != null) {
				var normalId = normals.Attribute("source").Value.TrimStart(new[]{ '#' });
				Normals = readNormals(mesh, normalId).ToList();
			}

			// Textures
			var texCoords = mesh
				.Element($"{ns}polylist")
				.Elements($"{ns}input").FirstOrDefault(x => x.Attribute("semantic").Value == "TEXCOORD");
			if (texCoords != null) {
				var texCoordId = texCoords.Attribute("source").Value.TrimStart(new[]{ '#' });
				Textures = readTextures(mesh, texCoordId).ToList();
			}

			assembleVertices(mesh);
			removeUnusedVertices();

			return convertDataToArrays();
		}
		

		private IEnumerable<Vertex> readVertices(XElement mesh, string id)
		{
			var data = mesh
				.Elements($"{ns}source").FirstOrDefault(x => x.Attribute("id").Value == id)
				.Element($"{ns}float_array");

			var count = int.Parse(data.Attribute("count").Value);
			var array = parseFloats(data.Value);

			for (int i = 0; i < count / 3; i++)
				yield return new Vertex(Vertices.Count, new Vector3(
					array[i * 3], 
					array[i * 3 + 2], 
					array[i * 3 + 1]));
		}

		private IEnumerable<Vector3> readNormals(XElement mesh, string id)
		{
			var data = mesh
				.Elements($"{ns}source").FirstOrDefault(x => x.Attribute("id").Value == id)
				.Element($"{ns}float_array");

			var count = int.Parse(data.Attribute("count").Value);
			var array = parseFloats(data.Value);

			for (int i = 0; i < count / 3; i++) 
				yield return new Vector3(
					array[i * 3],
					array[i * 3 + 2],
					array[i * 3 + 1]
				);
		}

		private IEnumerable<Vector2> readTextures(XElement mesh, string id)
		{
			var data = mesh
				.Elements($"{ns}source").FirstOrDefault(x => x.Attribute("id").Value == id)
				.Element($"{ns}float_array");

			var count = int.Parse(data.Attribute("count").Value);
			var array = parseFloats(data.Value);

			for (int i = 0; i < count / 2; i++)
				yield return new Vector2(
					array[i * 2],
					array[i * 2 + 1]
				);
		}
		
		private void assembleVertices(XElement mesh) 
		{
			var poly = mesh.Element($"{ns}polylist");
			var typeCount = poly.Elements($"{ns}input").Count();
			var indexData = parseInts(poly.Element($"{ns}p").Value);

			for(int i = 0; i < indexData.Count / typeCount; i++) {
				processVertex(indexData[i * typeCount], 
					indexData[i * typeCount + 1], 
					indexData[i * typeCount + 2]);
			}
		}

		private Vertex processVertex(int posIndex, int normIndex, int texIndex) {
			var currentVertex = Vertices[posIndex];
			
			if (!currentVertex.IsSet) {
				currentVertex.TextureIndex = texIndex;
				currentVertex.NormalIndex = normIndex;
				PolyList.Add(posIndex);
				return currentVertex;
			} else {
				return dealWithAlreadyProcessedVertex(currentVertex, texIndex, normIndex);
			}
		}

		private Vertex dealWithAlreadyProcessedVertex(Vertex previousVertex, int newTextureIndex, int newNormalIndex) 
		{
			if (previousVertex.hasSameTextureAndNormal(newTextureIndex, newNormalIndex)) {
				PolyList.Add(previousVertex.Index);
				return previousVertex;
			} 
			else {
				if (previousVertex.DuplicateVertex != null) {
					return dealWithAlreadyProcessedVertex(previousVertex.DuplicateVertex, newTextureIndex, newNormalIndex);
				} 
				else {
					var duplicateVertex = new Vertex(Vertices.Count, previousVertex.Position);

					duplicateVertex.TextureIndex = newTextureIndex;
					duplicateVertex.NormalIndex = newNormalIndex;
					previousVertex.DuplicateVertex = duplicateVertex;

					Vertices.Add(duplicateVertex);
					PolyList.Add(duplicateVertex.Index);

					return duplicateVertex;
				}
			}
		}

		private void removeUnusedVertices() 
		{
			foreach (var vertex in Vertices) {
				
				if (!vertex.IsSet) {
					vertex.TextureIndex = 0;
					vertex.NormalIndex = 0;
				}
			}
		}

		private Geometry convertDataToArrays() 
		{
			var verticesArray = new Vector3[Vertices.Count];
			var texturesArray = new Vector2[Vertices.Count];
			var normalsArray = new Vector3[Vertices.Count];

			for (int i = 0; i < Vertices.Count; i++) {
				Vertex currentVertex = Vertices[i];
				
				var position = currentVertex.Position;
				var textureCoord = Textures[currentVertex.TextureIndex];
				var normalVector = Normals[currentVertex.NormalIndex];

				verticesArray[i] = position;
				texturesArray[i] = textureCoord;
				normalsArray[i] = normalVector;

			}

			return new Geometry(verticesArray, normalsArray, texturesArray, PolyList.ToArray());
		}

		private static List<float> parseFloats(string input) 
		{
			return input.Split(' ' ).Select(x => float.Parse(x)).ToList();
		}

		private static List<int> parseInts(string input) 
		{
			return input.Split(' ' ).Select(x => int.Parse(x)).ToList();
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ColladaParser.Collada.Model;
using ColladaParser.Common;
using OpenTK;

namespace ColladaParser.Collada
{
	public class GeometryLoader
	{
		private static XNamespace ns = "{http://www.collada.org/2005/11/COLLADASchema}";

		private List<Vertex> Vertices;
		private List<Vector3> Normals;
		private List<Vector2> Textures;		
		private List<Vector3> Colors;	
		private List<int> PolyList;

		private XElement xMesh;


		public GeometryLoader(XElement xMesh)
		{
			Vertices = new List<Vertex>();
			PolyList = new List<int>();

			this.xMesh = xMesh;
		}

		public Geometry Load()
		{
			// Vertices
			var positionId = xMesh
				.Element($"{ns}vertices")
				.Element($"{ns}input")
				.Attribute("source").Value.TrimStart(new[]{ '#' });

			var polylist = readVecArray<Vector3>(positionId);
			foreach(var poly in polylist)
				Vertices.Add(new Vertex(Vertices.Count, poly));

			// Normals
			var normals = xMesh
				.Element($"{ns}polylist")
				.Elements($"{ns}input").FirstOrDefault(x => x.Attribute("semantic").Value == "NORMAL");
			if (normals != null) {
				var normalId = normals.Attribute("source").Value.TrimStart(new[]{ '#' });

				Normals = new List<Vector3>();
				Normals = readVecArray<Vector3>(normalId);
			}

			// Textures
			var texCoords = xMesh
				.Element($"{ns}polylist")
				.Elements($"{ns}input").FirstOrDefault(x => x.Attribute("semantic").Value == "TEXCOORD");
			if (texCoords != null) {
				var texCoordId = texCoords.Attribute("source").Value.TrimStart(new[]{ '#' });

				Textures = new List<Vector2>();
				Textures = readVecArray<Vector2>(texCoordId);
			}

			// Colors
			var colors = xMesh
				.Element($"{ns}polylist")
				.Elements($"{ns}input").FirstOrDefault(x => x.Attribute("semantic").Value == "COLOR");
			if (colors != null) {
				var colorId = colors.Attribute("source").Value.TrimStart(new[]{ '#' });

				Colors = new List<Vector3>();
				Colors = readVecArray<Vector3>(colorId);
			}

			assembleVertices();
			removeUnusedVertices();

			return convertDataToArrays();
		}

		private List<T> readVecArray<T>(string id)
		{
			var data = xMesh
				.Elements($"{ns}source").FirstOrDefault(x => x.Attribute("id").Value == id)
				.Element($"{ns}float_array");

			var count = int.Parse(data.Attribute("count").Value);
			var array = ArrayParsers.ParseFloats(data.Value);
			var result = new List<T>();

			if (typeof(T) == typeof(Vector3))
				for (var i = 0; i < count / 3; i++) 
					result.Add((T)(object)new Vector3(
						array[i * 3],
						array[i * 3 + 2],
						array[i * 3 + 1]
					));
			else if (typeof(T) == typeof(Vector2))
				for (var i = 0; i < count / 2; i++) 
					result.Add((T)(object)new Vector2(
						array[i * 2],
						array[i * 2 + 1]
					));
			
			return result;
		}
		
		private void assembleVertices() 
		{
			var poly = xMesh.Element($"{ns}polylist");
			var typeCount = poly.Elements($"{ns}input").Count();
			var id = ArrayParsers.ParseInts(poly.Element($"{ns}p").Value);

			for (int i = 0; i < id.Count / typeCount; i++) {
				var textureIndex = -1;
				var colorIndex = -1;
				var index = 0;

				var posIndex = id[i * typeCount + index]; index++;
				var normalIndex = id[i * typeCount + index]; index++;

				if (Textures != null) {
					textureIndex = id[i * typeCount + index]; index++;
				}

				if (Colors != null) {
					colorIndex = id[i * typeCount + index]; index++;
				}

				processVertex(posIndex, normalIndex, textureIndex, colorIndex);
			}
		}

		private void processVertex(int posIndex, int normalIndex, int textureIndex, int colorIndex) {
			var currentVertex = Vertices[posIndex];
			
			if (!currentVertex.IsSet) {
				currentVertex.NormalIndex = normalIndex;
				currentVertex.TextureIndex = textureIndex;
				currentVertex.ColorIndex = colorIndex;
				PolyList.Add(posIndex);
			} else {
				handleAlreadyProcessedVertex(currentVertex, normalIndex, textureIndex, colorIndex);
			}
		}

		private void handleAlreadyProcessedVertex(Vertex previousVertex, int newNormalIndex, int newTextureIndex, int newColorIndex) 
		{
			if (previousVertex.HasSameInformation(newNormalIndex, newTextureIndex, newColorIndex)) {
				PolyList.Add(previousVertex.Index);
				return;
			} 

			if (previousVertex.DuplicateVertex != null) {
				handleAlreadyProcessedVertex(previousVertex.DuplicateVertex, newNormalIndex, newTextureIndex, newColorIndex);
				return;
			} 

			var duplicateVertex = new Vertex(Vertices.Count, previousVertex.Position);

			duplicateVertex.NormalIndex = newNormalIndex;
			duplicateVertex.TextureIndex = newTextureIndex;
			duplicateVertex.ColorIndex = newColorIndex;
			previousVertex.DuplicateVertex = duplicateVertex;

			Vertices.Add(duplicateVertex);
			PolyList.Add(duplicateVertex.Index);
		}

		private void removeUnusedVertices() 
		{
			foreach (var vertex in Vertices) {
				if (!vertex.IsSet) {
					vertex.NormalIndex = 0;
					vertex.TextureIndex = 0;
					vertex.ColorIndex = 0;
				}
			}
		}

		private Geometry convertDataToArrays() 
		{
			var verticesArray = new Vector3[Vertices.Count];
			var normalsArray = new Vector3[Vertices.Count];

			Vector2[] texturesArray = null;
			Vector3[] colorsArray = null;

			if (Textures != null)
				texturesArray = new Vector2[Vertices.Count];

			if (Colors != null)
				colorsArray = new Vector3[Vertices.Count];

			for (int i = 0; i < Vertices.Count; i++) {
				Vertex currentVertex = Vertices[i];
				
				verticesArray[i] = currentVertex.Position;
				normalsArray[i] = Normals[currentVertex.NormalIndex];

				if(texturesArray != null) texturesArray[i] = Textures[currentVertex.TextureIndex];
				if(colorsArray != null) colorsArray[i] = Colors[currentVertex.ColorIndex];
			}

			return new Geometry(verticesArray, normalsArray, texturesArray, colorsArray, PolyList.ToArray());
		}
	}
}
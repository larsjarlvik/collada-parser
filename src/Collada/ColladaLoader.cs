using System;
using System.Linq;
using System.Xml.Linq;
using ColladaParser.Collada.Model;
using ColladaParser.Common;

namespace ColladaParser.Collada
{
    public static class ColladaLoader
	{
		private static XNamespace ns = "{http://www.collada.org/2005/11/COLLADASchema}";

		public static ColladaModel Load(string name)
		{
			using(var xml = SourceLoader.AsStream($"models.{name}.dae"))
			{
				var xRoot = XDocument.Load(xml);
				var model = new ColladaModel();

				// Parse Geometries
				var xMeshes = xRoot.Descendants($"{ns}mesh");
				if (!xMeshes.Any())
					throw new ApplicationException("Failed to find geometries!");

				foreach(var xMesh in xMeshes) {
					var geoLoader = new GeometryLoader(xMesh);
					var geometry = geoLoader.Load();
					
					model.Geometries.Add(geometry);
				}

				// Parse Materials
				var xMaterials = xRoot.Descendants($"{ns}material");
				foreach(var xMaterial in xMaterials) {
					var materialLoader = new MaterialLoader(xRoot, xMaterial);
					var material = materialLoader.Load();

					model.Materials.Add(material);
				}

				return model;
			}
		}
	}
}
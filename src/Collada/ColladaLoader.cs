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
				var root = XDocument.Load(xml);
				var model = new ColladaModel();

				// Parse Geometries
				var geoPaths = root.Descendants($"{ns}mesh");
				if (!geoPaths.Any())
					throw new ApplicationException("Failed to find geometries!");

				foreach(var geoPath in geoPaths) {
					var geoLoader = new GeometryLoader(geoPath);
					var geometry = geoLoader.Load();
					
					model.Geometries.Add(geometry);
				}

				// Parse Materials
				var matPaths = root.Descendants($"{ns}material");
				foreach(var matPath in matPaths) {
					var materialLoader = new MaterialLoader(root, matPath);
					var material = materialLoader.Load();

					model.Materials.Add(material);
				}


				return model;
			}
		}
	}
}
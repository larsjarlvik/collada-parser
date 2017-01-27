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
				var geometries = root.Descendants($"{ns}mesh");
				if (!geometries.Any())
					throw new ApplicationException("Failed to find geometries!");

				foreach(var geometry in geometries) {
					var geoLoader = new GeometryLoader(geometry);
					model.Geometries.Add(geoLoader.Load());
				}

				return model;
			}
		}
	}
}
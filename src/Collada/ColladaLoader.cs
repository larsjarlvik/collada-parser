using System;
using System.Linq;
using System.Xml.Linq;
using ColladaParser.Collada.Animation;
using ColladaParser.Collada.Model;
using ColladaParser.Common;

namespace ColladaParser.Collada
{
	public static class ColladaLoader
	{
		private static XNamespace ns = "{http://www.collada.org/2005/11/COLLADASchema}";

		public static ColladaModel LoadModel(string name)
		{
			using(var xml = SourceLoader.AsStream($"models.{name}.dae"))
			{
				var xRoot = XDocument.Load(xml);
				var model = new ColladaModel();

				// Parse Geometries
				var xMeshes = xRoot.Descendants($"{ns}mesh");
				if (!xMeshes.Any())
					throw new ApplicationException("Failed to find geometries!");

				foreach (var xMesh in xMeshes) {
					var geoLoader = new GeometryLoader(xMesh);
					var geometry = geoLoader.Load();
					
					model.Geometries.Add(geometry);
				}

				// Parse Materials
				var xMaterials = xRoot.Descendants($"{ns}material");
				foreach (var xMaterial in xMaterials) {
					var materialLoader = new MaterialLoader(xRoot, xMaterial);
					var material = materialLoader.Load();

					model.Materials.Add(material);
				}

				// Parse joint information
				var xController = xRoot.Descendants($"{ns}controller").FirstOrDefault();
				var xVisualScene = xRoot.Descendants($"{ns}visual_scene").FirstOrDefault();
				if (xController != null) {
					var jointLoader = new JointLoader(xController, xVisualScene);
					var jointHierarchy = jointLoader.LoadJointHierarchy();

					// TODO: Support multiple geometries for animation
					model.Geometries.First().AppendJointInformation(
						jointLoader.LoadJointWeights(),
						jointLoader.LoadJointHierarchy());
				}

				return model;
			}
		}

		public static ColladaAnimation LoadAnimation(string name)
		{
			using(var xml = SourceLoader.AsStream($"models.{name}.dae"))
			{
				var xRoot = XDocument.Load(xml);

				// Parse animation information
				var xAnimations = xRoot.Descendants($"{ns}animation").ToArray();
				if (xAnimations.Length > 0) {
					var keyFrameLoader = new KeyFrameLoader(xAnimations);

					return new ColladaAnimation(keyFrameLoader.LoadKeyFrames());
				}
			}

			return null;
		}
	}
}
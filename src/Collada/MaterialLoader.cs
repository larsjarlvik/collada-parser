using System.Linq;
using System.Xml.Linq;
using ColladaParser.Collada.Model;
using ColladaParser.Common;
using OpenTK;

namespace ColladaParser.Collada
{
	public class MaterialLoader
	{
		private static XNamespace ns = "{http://www.collada.org/2005/11/COLLADASchema}";

		private XDocument xRoot;
		private XElement xMaterial;
		private XElement xEffect;

		private Material material;

		public MaterialLoader(XDocument xRoot, XElement xMaterial)
		{
			this.xRoot = xRoot;
			this.xMaterial = xMaterial;
		}

		public Material Load()
		{
			this.material = new Material();

			var effectId = xMaterial
				.Descendants($"{ns}instance_effect").First()
				.Attribute("url").Value.TrimStart(new[] { '#' });
			
			xEffect = xRoot.Descendants($"{ns}effect")
				.First(x => x.Attribute("id").Value == effectId);

			setTexture();
			setPhong(xEffect);

			return material;
		}

		private void setTexture()
		{
			var imageId = xEffect.Descendants($"{ns}init_from").FirstOrDefault();
			if (imageId == null)
				return; // No textures
			
			material.TextureName = xRoot.Descendants($"{ns}library_images")
				.Elements($"{ns}image")
				.First(x => x.Attribute("id").Value == imageId.Value).Value;
		}

		private void setPhong(XElement effect)
		{
			var diffuse = effect.Descendants($"{ns}phong")
					.Descendants($"{ns}color")
					.FirstOrDefault(x => x.Attribute("sid").Value == "diffuse");

			var specular = effect.Descendants($"{ns}phong")
					.Descendants($"{ns}color")
					.FirstOrDefault(x => x.Attribute("sid").Value == "specular");

			var shininess = effect.Descendants($"{ns}phong")
					.Descendants($"{ns}float")
					.FirstOrDefault(x => x.Attribute("sid").Value == "shininess");


			if(diffuse != null) {
				var aDiffuse = ArrayParsers.ParseFloats(diffuse.Value);
				material.Diffuse = new Vector4(aDiffuse[0], aDiffuse[1], aDiffuse[2], aDiffuse[3]);
			}

			if(specular != null) {
				var aSpecular = ArrayParsers.ParseFloats(specular.Value);
				material.Specular = new Vector4(aSpecular[0], aSpecular[1], aSpecular[2], aSpecular[3]);
			}

			if(shininess != null) {
				material.Shininess = float.Parse(shininess.Value);
			}
		}
	}
}
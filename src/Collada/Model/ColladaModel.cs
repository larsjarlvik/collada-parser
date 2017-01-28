using System.Collections.Generic;

namespace ColladaParser.Collada.Model
{
    public class ColladaModel
	{
		public List<Geometry> Geometries { get; set; }
		public List<Material> Materials { get; set; }

		public ColladaModel()
		{
			Geometries = new List<Geometry>();
			Materials = new List<Material>();
		}
		
		public void CreateVBOs()
		{
			Geometries.ForEach(g => g.CreateVBOs());
		}

		public void LoadTextures()
		{
			Materials.ForEach(m => m.LoadTexture("models.textures"));
		}

		public void Bind(int shaderProgram, int textureLocation, int haveTextureLocation)
		{
			Geometries.ForEach(g => g.Bind(shaderProgram));
			Materials.ForEach(m => m.Bind(textureLocation, haveTextureLocation));
		}

		public void Render() 
		{
			Geometries.ForEach(g => g.Render());
		}
	}
}
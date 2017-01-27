using System.Collections.Generic;

namespace ColladaParser.Collada.Model
{
    public class ColladaModel
	{
		public List<Geometry> Geometries { get; set; }

		public ColladaModel()
		{
			Geometries = new List<Geometry>();
		}
		
		public void CreateVBOs()
		{
			Geometries.ForEach(g => g.CreateVBOs());
		}

		public void Bind(int shaderProgram)
		{
			Geometries.ForEach(g => g.Bind(shaderProgram));
		}

		public void Render() 
		{
			Geometries.ForEach(g => g.Render());
		}
	}
}
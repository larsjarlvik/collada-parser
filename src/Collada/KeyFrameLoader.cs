using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ColladaParser.Collada.Animation;
using ColladaParser.Common;
using OpenTK;

namespace ColladaParser.Collada
{
	public class KeyFrameLoader
	{
		private static XNamespace ns = "{http://www.collada.org/2005/11/COLLADASchema}";
		private readonly XElement[] xAnimations;

		public KeyFrameLoader(XElement[] xAnimations)
		{
			this.xAnimations = xAnimations;
		}
		
		public KeyFrame[] LoadKeyFrames()
		{
			var keyFrames = new List<KeyFrame>();
			var index = 0;

			foreach(var animation in xAnimations) {
				var target = animation.Element($"{ns}channel").Attribute("target").Value;
				var name = target.Substring(0, target.IndexOf("/"));

				var semantics = animation.Element($"{ns}sampler").Elements($"{ns}input");
				var inputId = semantics.First(x => x.Attribute("semantic").Value == "INPUT").Attribute("source").Value.TrimStart('#');
				var outputId = semantics.First(x => x.Attribute("semantic").Value == "OUTPUT").Attribute("source").Value.TrimStart('#');

				var inputsList = animation
					.Descendants($"{ns}source")
					.First(x => x.Attribute("id").Value == inputId)
					.Element($"{ns}float_array").Value;

				var outputsList = animation
					.Descendants($"{ns}source")
					.First(x => x.Attribute("id").Value == outputId)
					.Element($"{ns}float_array").Value;

				var tss = ArrayParsers.ParseFloats(inputsList);
				var trs = ArrayParsers.ParseFloats(outputsList);

				foreach (var ts in tss) {
					var tr = new Matrix4 (
						trs[0],  trs[1],  trs[2],  trs[3],
						trs[4],  trs[5],  trs[6],  trs[7],
						trs[8],  trs[9],  trs[10], trs[11],
						trs[12], trs[13], trs[14], trs[15]
					);

					var current = keyFrames.FirstOrDefault(x => x.TimeStamp == ts);
					if (current == null) {
						current = new KeyFrame(ts);
						keyFrames.Add(current);
					}
					
					current.Transforms.Add(index, new KeyFrameTransform(tr));
				}

				index++;
			}
			
			return keyFrames.ToArray();
		}
	}
}
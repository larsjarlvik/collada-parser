using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ColladaParser.Collada.Model;
using ColladaParser.Common;
using OpenTK;

namespace ColladaParser.Collada
{
	public class JointLoader
	{
		private static XNamespace ns = "{http://www.collada.org/2005/11/COLLADASchema}";
		private readonly XElement xController;
		private readonly XElement xVisualScene;

		public JointLoader(XElement xController, XElement xVisualScene)
		{
			this.xController = xController;
			this.xVisualScene = xVisualScene;
		}

		public JointWeights[] LoadJointWeights()
		{
			var vWeights = xController
				.Descendants($"{ns}vertex_weights").First();

			// Joint counts
			var vcountList = vWeights.Element($"{ns}vcount").Value;
			var vcounts = ArrayParsers.ParseInts(vcountList);

			var vList = vWeights.Element($"{ns}v").Value;
			var vs = ArrayParsers.ParseInts(vList);

			// Joint weights
			var weightId = vWeights
				.Elements($"{ns}input").First(x => x.Attribute("semantic").Value == "WEIGHT")
				.Attribute("source").Value.TrimStart(new[]{ '#' });

			var weightList = xController
				.Descendants($"{ns}source").First(x => x.Attribute("id").Value == weightId)
				.Element($"{ns}float_array").Value;

			var weights = ArrayParsers.ParseFloats(weightList);
			
			// TODO: Assuming collada structure JOINT then WEIGHT
			// Should be parsed from 
			var jointIds = vs.Where((x, i) => i % 2 == 0).ToList();
			var jointWeights = vs.Where((x, i) => i % 2 == 1).ToList();
			var joints = new List<JointWeights>();

			// Parse joint weights and id
			while(vcounts.Count > 0) {
				var vcount = vcounts.First();

				// Joint Weights
				var jw = jointWeights.Take(vcount < 3 ? vcount : 3).ToList();
				var jwVec = new Vector3(
					jw.Count > 0 ? weights[jw[0]] : 0,
					jw.Count > 1 ? weights[jw[1]] : 0,
					jw.Count > 2 ? weights[jw[2]] : 0
				);

				// Joint Ids
				var jid = jointIds.Take(vcount < 3 ? vcount : 3).ToList();
				var jidVec = new Vector3(
					jid.Count > 0 ? jid[0] : 0,
					jid.Count > 1 ? jid[1] : 0,
					jid.Count > 2 ? jid[2] : 0
				);

				joints.Add(new JointWeights(jidVec, NormalizeToOne(jwVec)));

				vcounts.RemoveAt(0);
				jointIds.RemoveRange(0, vcount);
				jointWeights.RemoveRange(0, vcount);
			}

			return joints.ToArray();
		}

		private Vector3 NormalizeToOne(Vector3 vec)
		{
			var sum = vec.X + vec.Y + vec.Z;
			return new Vector3(vec.X / sum, vec.Y / sum, vec.Z / sum);
		}

		public Joint LoadJointHierarchy() 
		{
			// library_visual_scenes
			var joints = LoadJoints();
			var root = xVisualScene
				.Descendants($"{ns}node").First(x => x.Attribute("type").Value == "JOINT");

			return BuiltJointHierarchy(root, joints.First().Name, joints);
		}

		private Joint BuiltJointHierarchy(XElement root, string name, Joint[] joints)
		{
			var joint = joints.First(x => x.Name == name);
			var children = root.Elements($"{ns}node");

			foreach (var child in children) {
				joint.Children.Add(BuiltJointHierarchy(child, child.Attribute("id").Value, joints));
			}

			return joint;
		}

		private Joint[] LoadJoints()
		{
			// Joint names
			var jointNamesId = xController
				.Descendants($"{ns}joints").First()
				.Elements($"{ns}input").First(x => x.Attribute("semantic").Value == "JOINT")
				.Attribute("source").Value.TrimStart(new[]{ '#' });

			var jointNamesList = xController
				.Descendants($"{ns}source").First(x => x.Attribute("id").Value == jointNamesId)
				.Element($"{ns}Name_array").Value;

			var jointNames = ArrayParsers.ParseStrings(jointNamesList);

			// Joint Matrices
			var jointMatricesId = xController
				.Descendants($"{ns}joints").First()
				.Elements($"{ns}input").First(x => x.Attribute("semantic").Value == "INV_BIND_MATRIX")
				.Attribute("source").Value.TrimStart(new[]{ '#' });

			var jointMatricesList = xController
				.Descendants($"{ns}source").First(x => x.Attribute("id").Value == jointMatricesId)
				.Element($"{ns}float_array").Value;

			var jointMatrices = ArrayParsers.ParseFloats(jointMatricesList);

			// Load
			var joints = new List<Joint>();
			var index = 0;

			while(jointMatrices.Count > 0) {
				joints.Add(new Joint(index, jointNames[index], new Matrix4(
					jointMatrices[0],  jointMatrices[1],  jointMatrices[2],  jointMatrices[3],
					jointMatrices[4],  jointMatrices[5],  jointMatrices[6],  jointMatrices[7],
					jointMatrices[8],  jointMatrices[9],  jointMatrices[10], jointMatrices[11],
					jointMatrices[12], jointMatrices[13], jointMatrices[14], jointMatrices[15])
				));

				jointMatrices.RemoveRange(0, 16);
				index ++;
			}

			return joints.ToArray();
		}
	}
}
using System.Collections.Generic;
using System.Linq;

namespace ColladaParser.Common
{
	public static class ArrayParsers
	{
		public static List<float> ParseFloats(string input) 
		{
			return input.Split(' ' ).Select(x => float.Parse(x)).ToList();
		}

		public static List<int> ParseInts(string input) 
		{
			return input.Split(' ' ).Select(x => int.Parse(x)).ToList();
		}
	}
}
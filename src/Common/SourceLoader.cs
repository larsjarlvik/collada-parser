using System.IO;
using System.Reflection;

namespace ColladaParser.Common
{
	public static class SourceLoader
	{
		public static Stream AsStream(string path)
		{
			var assembly = Assembly.GetEntryAssembly();
			return assembly.GetManifestResourceStream($"collada-parser.{path}");
		}

		public static string AsString(string path) 
		{
			var assembly = Assembly.GetEntryAssembly();

			using (var stream = AsStream(path))
				using (var reader = new StreamReader(stream))
					return reader.ReadToEnd();
		}
	}
}

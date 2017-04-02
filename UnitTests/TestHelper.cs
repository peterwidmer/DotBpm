using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class TestHelper
    {
        public static string GetEmbeddedResourceAsString(string embeddedFileName)
        {
            using (var stream = GetEmbeddedResource(embeddedFileName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static Stream GetEmbeddedResource(string embeddedFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            return assembly.GetManifestResourceStream(embeddedFileName);
        }
    }
}

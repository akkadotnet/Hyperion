using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace Hyperion.Tests.Generator
{
    internal static class Program
    {
        private static readonly Serializer Serializer = new Serializer();
        private static readonly string FullFrameworkName = Assembly
            .GetEntryAssembly()?
            .GetCustomAttribute<TargetFrameworkAttribute>()?
            .FrameworkName;

        private static readonly string FrameworkName = Regex.Replace(FullFrameworkName, "[^\\w\\._]", string.Empty);

        private static void Main(string[] args)
        {
            if (args.Length != 1 || args[0] != "generate")
            {
                return;
            }

            string outputPath = args.Length == 2 ? args[1] : CrossFrameworkInitializer.DefaultOutputPath;

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            CrossFrameworkClass crossFrameworkClass = CrossFrameworkInitializer.Init();
            var crossFrameworkMixedClass = CrossFrameworkInitializer.InitMixed();

            string fileName = $"test_file_{FrameworkName.ToLowerInvariant()}.tf";
            string fullPath = Path.Combine(outputPath, fileName);
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                Serializer.Serialize(crossFrameworkClass, fileStream);
            }

            fileName = $"mixed_test_file_{FrameworkName.ToLowerInvariant()}.tf";
            fullPath = Path.Combine(outputPath, fileName);
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                Serializer.Serialize(crossFrameworkMixedClass, fileStream);
            }
        }
    }
}
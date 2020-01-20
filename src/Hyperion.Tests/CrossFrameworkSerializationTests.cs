using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hyperion.Tests.Generator;
using Xunit;

namespace Hyperion.Tests
{
    public class CrossFrameworkSerializationTests
    {
        private readonly Serializer _serializer;
        private readonly CrossFrameworkClass _originalObject;

        public CrossFrameworkSerializationTests()
        {
            _serializer = new Serializer();
            _originalObject = CrossFrameworkInitializer.Init();
        }

        public static IEnumerable<object[]> SerializationFiles()
        {
            const string defaultOutputPath = CrossFrameworkInitializer.DefaultOutputPath;
            var testFiles = Directory.GetFiles(defaultOutputPath, "*.tf");
            return testFiles.Select(x => new object[] { x });
        }

        [Theory]
        [MemberData(nameof(SerializationFiles))]
        public void CanSerializeCrossFramework(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                var crossFrameworkClass = _serializer.Deserialize<CrossFrameworkClass>(fileStream);

                Assert.Equal(_originalObject, crossFrameworkClass);
            }
        }
    }
}

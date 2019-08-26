using System.IO;
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

        [Fact]
        public void CanSerializeCrossFramework()
        {
            const string defaultOutputPath = CrossFrameworkInitializer.DefaultOutputPath;
            var testFiles = Directory.GetFiles(defaultOutputPath, "*.tf");

            Assert.NotEmpty(testFiles);

            foreach (string testFile in testFiles)
            {
                using (var fileStream = new FileStream(testFile, FileMode.Open))
                {
                    var crossFrameworkClass = _serializer.Deserialize<CrossFrameworkClass>(fileStream);

                    Assert.Equal(_originalObject, crossFrameworkClass);
                }
            }
        }
    }
}

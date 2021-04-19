using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FluentAssertions;
using Hyperion.Tests.Generator;
using Xunit;
using Xunit.Abstractions;

namespace Hyperion.Tests
{
    public class CrossFrameworkSerializationTests
    {
        private readonly ITestOutputHelper _log;
        private readonly Serializer _serializer;
        private readonly CrossFrameworkClass _originalObject;
        private readonly CrossFrameworkMixedClass _originalMixedObject;

        public CrossFrameworkSerializationTests(ITestOutputHelper log)
        {
            _log = log;
            _originalObject = CrossFrameworkInitializer.Init();
            _originalMixedObject = CrossFrameworkInitializer.InitMixed();

            // Demonstrating the use of custom dll package name override
            // to convert netcore System.Drawing.Primitives to netfx 
            // System.Drawing package.
            #if NETFX
            _serializer = new Serializer(SerializerOptions.Default.WithPackageNameOverrides(
                new List<Func<string, string>>
                {
                    str => str.Contains("System.Drawing.Primitives") ? str.Replace(".Primitives", "") : str
                }));
            #elif NETCOREAPP
            _serializer = new Serializer();
            #endif
        }

        public static IEnumerable<object[]> SerializationFiles()
        {
            const string defaultOutputPath = CrossFrameworkInitializer.DefaultOutputPath;
            var testFiles = Directory.GetFiles(defaultOutputPath, "test_file_.*.tf");
            return testFiles.Select(x => new object[] { x });
        }

        public static IEnumerable<object[]> MixedSerializationFiles()
        {
            const string defaultOutputPath = CrossFrameworkInitializer.DefaultOutputPath;
            var testFiles = Directory.GetFiles(defaultOutputPath, "mixed_test_file_.*.tf");
            return testFiles.Select(x => new object[] { x });
        }

        [Theory]
        [MemberData(nameof(SerializationFiles))]
        public void CanSerializeCrossFramework(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                var crossFrameworkClass = _serializer.Deserialize<CrossFrameworkClass>(fileStream);
                _originalObject.Should()
                    .Be(crossFrameworkClass, $"[CrossFrameworkClass] {fileName} deserialization should work.");
            }
        }

        [Theory]
        [MemberData(nameof(MixedSerializationFiles))]
        public void CanSerializeComplexObjectCrossFramework(string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                var deserialized = _serializer.Deserialize<CrossFrameworkMixedClass>(fileStream);
                _originalMixedObject.Should()
                    .Be(deserialized, $"[CrossFrameworkMixedClass] {fileName} deserialization should work.");
            }
        }
    }
}

using System;
using System.IO;
using Hyperion.Extensions;
using Hyperion.Internal;
using Xunit;
using FluentAssertions;

namespace Hyperion.Tests
{
    public class UnsafeDeserializationExclusionTests
    {
        [Fact]
        public void CantDeserializeANaughtyType()
        {
            //System.Diagnostics.Process p = new Process();
            var serializer = new Hyperion.Serializer();
            var di =new System.IO.DirectoryInfo(@"c:\");

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(di, stream);
                stream.Position = 0;
                Assert.Throws<EvilDeserializationException>(() =>
                    serializer.Deserialize<DirectoryInfo>(stream));
            }
        }

        internal class ClassA
        { }
        
        internal class ClassB
        { }
        
        internal class ClassC
        { }
        
        [Fact]
        public void TypeFilterShouldThrowOnNaughtyType()
        {
            var typeFilter = TypeFilterBuilder.Create()
                .Include<ClassA>()
                .Include<ClassB>()
                .Build();

            var options = SerializerOptions.Default
                .WithTypeFilter(typeFilter);

            var serializer = new Serializer(options);
            
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(new ClassA(), stream);
                stream.Position = 0;
                Action act = () => serializer.Deserialize<ClassA>(stream);
                act.Should().NotThrow();
            }

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(new ClassB(), stream);
                stream.Position = 0;
                Action act = () => serializer.Deserialize<ClassB>(stream);
                act.Should().NotThrow();
            }
            
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(new ClassC(), stream);
                stream.Position = 0;
                Action act = () => serializer.Deserialize<ClassC>(stream);
                act.Should().Throw<UserEvilDeserializationException>();
            }
        }
    }
}
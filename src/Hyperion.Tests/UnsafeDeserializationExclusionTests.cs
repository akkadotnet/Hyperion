using System.IO;
using Hyperion.Extensions;
using Hyperion.Internal;
using Xunit;

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
    }
}
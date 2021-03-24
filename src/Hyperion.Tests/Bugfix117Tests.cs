
using System;
using System.Drawing;
using Xunit;
using Xunit.Abstractions;

namespace Hyperion.Tests{
    public class Bugfix117Tests : TestBase
    {
        [Fact]
        public void CanSerializeColor()
        {
            var expected = Color.Aquamarine;
            Serialize(expected);
            Reset();
            var actual = Deserialize<Color>();
            Assert.Equal(expected, actual);
        }
    }
}

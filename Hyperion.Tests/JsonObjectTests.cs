#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ISerializableTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Runtime.Serialization;
using Xunit;
using Newtonsoft.Json.Linq;

namespace Hyperion.Tests
{
    public class JsonObjectTests : TestBase
    {
        [Fact]
        public void CanSerializeSimpleJObject()
        {
            var expected = JObject.Parse(@"{""name"":""john""}");
            Serialize(expected);
            Reset();
            var actual = Deserialize<JObject>();
            Assert.Equal(JToken.DeepEquals(expected, actual), true);
        }
    }
}
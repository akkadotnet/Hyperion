#region copyright
// -----------------------------------------------------------------------
//  <copyright file="MarshaledValueSerializerFactory.cs" company="Akka.NET Team">
//      Copyright (C) 2017-2017 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using Hyperion.SerializerFactories;
using Xunit;

namespace Hyperion.Tests
{
    public class MarshalSerializerTests : TestBase
    {
        public struct MyStruct
        {
            public readonly int X;
            public readonly int Y;
            public readonly int Z;

            public MyStruct(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }

        public MarshalSerializerTests()
        {
            CustomInit(new Serializer(new SerializerOptions(
                serializerFactories: new[] { new MarshalSerializerFactory(), })));
        }

        [Fact]
        public void CanSerializerFullyValuedTypesUsingMarshaller()
        {
            var expected = new MyStruct(1337, 293256, 235034634);

            Serialize(expected);
            Reset();
            var actual = Deserialize<MyStruct>();
            Assert.Equal(expected, actual);
        }
    }
}
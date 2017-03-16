#region copyright
// -----------------------------------------------------------------------
//  <copyright file="CustomObjectTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using Xunit;

namespace Hyperion.Tests
{
    
    public class CustomObjectTests : TestBase
    {
        private class PrivateType
        {
            public int IntProp { get; set; }
        }

        private class DefaultArgumentCtorType
        {
            public DefaultArgumentCtorType(bool val1 = false, bool val2 = false)
            {
                Val1 = val1;
                Val2 = val2;
            }

            public bool Val1 { get; }

            public bool Val2 { get; }
        }

        [Fact]
        public void CanSerializePrivateType()
        {
            var expected = new PrivateType()
            {
                IntProp = 123,
            };
            Serialize(expected);
            Reset();
            var actual = Deserialize<PrivateType>();
            Assert.Equal(expected.IntProp, actual.IntProp);
        }

        [Fact]
        public void CanSerializeTypeObject()
        {
            var expected = typeof(ArgumentException);
            Serialize(expected);
            Reset();
            var actual = Deserialize<Type>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeNull()
        {
            var expected = new Something
            {

                Else = null
            };
            Serialize(expected);
            Reset();
            var actual = Deserialize<Something>();
            Assert.Equal(expected, actual);
        }

        [InlineData(true, false)]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [Theory]
        public void CanSerializeDefaultCtorArguments(bool val1, bool val2)
        {
            // need at least 1 value to be non-default
            var expected = new DefaultArgumentCtorType(val1, val2);
            Serialize(expected);
            Reset();
            var actual = Deserialize<DefaultArgumentCtorType>();
            Assert.Equal(expected.Val1, actual.Val1);
            Assert.Equal(expected.Val2, actual.Val1);
        }

        //this uses a lightweight serialization of exceptions to conform to .NET core's lack of ISerializable
        //all custom exception information will be lost.
        //only message, inner exception, stacktrace and the bare minimum will be preserved.
        [Fact]
        public void CanSerializeException()
        {
            var expected = new Exception("hello wire");
            Serialize(expected);
            Reset();
            var actual = Deserialize<Exception>();
            Assert.Equal(expected.StackTrace, actual.StackTrace);
            Assert.Equal(expected.Message, actual.Message);
        }

        [Fact]
        public void CanSerializePolymorphicObject()
        {
            var expected = new Something
            {
                Else = new OtherElse
                {
                    Name = "Foo",
                    More = "Bar"
                }
            };
            Serialize(expected);
            Reset();
            var actual = Deserialize<Something>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeStruct()
        {
            var expected = new StuctValue
            {
                Prop1 = "hello",
                Prop2 = 123,
            };


            Serialize(expected);
            Reset();
            var actual = Deserialize<StuctValue>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeObject()
        {
            var expected = new Something
            {
                BoolProp = true,
                Int32Prop = 123,
                NullableInt32PropHasValue = 888,
                StringProp = "hello",
            };


            Serialize(expected);
            Reset();
            var actual = Deserialize<Something>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeObjects()
        {
            var expected1 = new Something
            {
                StringProp = "First"
            };
            var expected2 = new Something
            {
                StringProp = "Second"
            };
            var expected3 = new Something
            {
                StringProp = "Last"
            };
            Serialize(expected1);
            Serialize(expected2);
            Serialize(expected3);
            Reset();
            Assert.Equal(expected1, Deserialize<Something>());
            Assert.Equal(expected2, Deserialize<Something>());
            Assert.Equal(expected3, Deserialize<Something>());
        }

        [Fact]
        public void CanSerializeTuple()
        {
            var expected = Tuple.Create("hello");
            Serialize(expected);
            Reset();
            var actual = Deserialize<Tuple<string>>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanEmptyObject()
        {
            var expected = new Empty();

            Serialize(expected);
            Reset();
            var actual = Deserialize<Empty>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeObjectsKnownTypes()
        {
            CustomInit(new Serializer(new SerializerOptions(knownTypes:new[] {typeof(Something)})));
            var expected1 = new Something
            {
                StringProp = "First"
            };
            var expected2 = new Something
            {
                StringProp = "Second"
            };
            var expected3 = new Something
            {
                StringProp = "Last"
            };
            Serialize(expected1);
            Serialize(expected2);
            Serialize(expected3);
            Reset();
            Assert.Equal(expected1, Deserialize<Something>());
            Assert.Equal(expected2, Deserialize<Something>());
            Assert.Equal(expected3, Deserialize<Something>());
        }
    }
}
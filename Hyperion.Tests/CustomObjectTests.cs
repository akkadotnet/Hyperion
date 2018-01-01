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

        [Fact]
        public void Serializer_should_work_with_non_public_types()
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
        public void Serializer_should_work_with_Type()
        {
            var expected = typeof(ArgumentException);
            Serialize(expected);
            Reset();
            var actual = Deserialize<Type>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Serializer_should_work_with_null()
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


        //this uses a lightweight serialization of exceptions to conform to .NET core's lack of ISerializable
        //all custom exception information will be lost.
        //only message, inner exception, stacktrace and the bare minimum will be preserved.
        [Fact]
        public void Serializer_should_work_with_Exceptions()
        {
            var expected = new Exception("hello wire");
            Serialize(expected);
            Reset();
            var actual = Deserialize<Exception>();
            Assert.Equal(expected.StackTrace, actual.StackTrace);
            Assert.Equal(expected.Message, actual.Message);
        }

        [Fact]
        public void Serializer_should_work_with_polymorphic_objects()
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
        public void Serializer_should_work_with_structs()
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
        public void Serializer_should_work_with_custom_classes()
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
        public void Serializer_should_work_with_custom_classes_2()
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
        public void Serializer_should_work_with_tuples()
        {
            var expected = Tuple.Create("hello");
            Serialize(expected);
            Reset();
            var actual = Deserialize<Tuple<string>>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Serializer_should_work_with_objects_without_any_fields()
        {
            var expected = new Empty();

            Serialize(expected);
            Reset();
            var actual = Deserialize<Empty>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Serializer_should_work_with_objects_marked_as_known_types()
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
#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ISerializableTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

#if SERIALIZATION
using System;
using System.Runtime.Serialization;
using Xunit;

namespace Hyperion.Tests
{
    public class ISerializableTests : TestBase
    {
        public class NonSerializedTest
        {
            public int Field1;

            [NonSerialized]
            public string Field2;
        }

        public class NotSerializableEvent : ISerializable
        {
            public NotSerializableEvent(bool foo) { }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                throw new NotImplementedException();
            }
        }

        public class Person : ISerializable
        {
            public Person(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public void GetObjectData(SerializationInfo info, StreamingContext streamingContext)
            {
                info.AddValue("FirstName", FirstName);
                info.AddValue("LastName", LastName);
            }
        }

        [Fact]
        public void CanIgnoreFieldsWithNonSerializedAttribute()
        {
            var expected = new NonSerializedTest() {
                Field1 = 235,
                Field2 = "non serialized text"
            };

            Serialize(expected);
            Reset();
            var actual = Deserialize<NonSerializedTest>();
            Assert.Equal(expected.Field1, actual.Field1);
            Assert.Null(actual.Field2);
        }

        [Fact(Skip="Not implemented yet")]
        public void CanSerializeClassesWithISerializable()
        {
            var expected = new Person("Scott", "Hanselman");

            Serialize(expected);
            Reset();
            var actual = Deserialize<Person>();
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
        }

        [Fact]
        public void ShouldThrowOnNonSerializableEvent()
        {
            var expected = new NotSerializableEvent(true);

            Assert.Throws<NotImplementedException>(() => Serialize(expected));
        }
    }
}
#endif
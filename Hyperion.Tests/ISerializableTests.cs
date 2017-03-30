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
        #region Contracts
        [Serializable]
        public class NonSerializedTest
        {
            public int Field1;

            [NonSerialized]
            public string Field2;
        }

        public class NonSerializedWithoutSerializableTest
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

        public sealed class Person : ISerializable
        {
            public Person(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }

            public Person(SerializationInfo info, StreamingContext context)
            {
                FirstName = (string)info.GetValue("FirstName", typeof(string));
                LastName = (string)info.GetValue("LastName", typeof(string));
            }

            public string FirstName { get; }

            public string LastName { get; }

            public void GetObjectData(SerializationInfo info, StreamingContext streamingContext)
            {
                info.AddValue("FirstName", FirstName);
                info.AddValue("LastName", LastName);
            }
        }

        public sealed class PersonWithoutConstructor : ISerializable
        {
            public PersonWithoutConstructor(string firstName)
            {
                FirstName = firstName;
            }

            public string FirstName { get; }

            public void GetObjectData(SerializationInfo info, StreamingContext streamingContext)
            {
                info.AddValue("FirstName", FirstName);
            }
        }

        public sealed class PersonWithIDeserializationCallback : ISerializable, IDeserializationCallback
        {
            public PersonWithIDeserializationCallback() {}

            public PersonWithIDeserializationCallback(SerializationInfo info, StreamingContext context)
            {
                FirstName = (string)info.GetValue("FirstName", typeof(string));
            }

            public string FirstName { get; set; }

            public void GetObjectData(SerializationInfo info, StreamingContext streamingContext)
            {
                info.AddValue("FirstName", FirstName);
            }

            public void OnDeserialization(object sender)
            {
                throw new Exception("OnDeserialization");
            }
        }
        #endregion

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

        [Fact]
        public void SkipNonSerializedAttributeIfNoSerilizedAttribute()
        {
            var input = new NonSerializedWithoutSerializableTest()
            {
                Field1 = 235,
                Field2 = "non serialized text"
            };

            var expected = new NonSerializedWithoutSerializableTest()
            {
                Field1 = 235,
                Field2 = null
            };

            Serialize(input);
            Reset();
            var actual = Deserialize<NonSerializedWithoutSerializableTest>();
            Assert.Equal(expected.Field1, actual.Field1);
            Assert.Equal(expected.Field2, actual.Field2);
        }

        [Fact]
        public void CanSerializeClassesWithISerializable()
        {
            var expected = new Person("Scott", "Hanselman");

            Serialize(expected);
            Reset();
            var actual = Deserialize<Person>();
            Assert.Equal(expected.FirstName, actual.FirstName);
            Assert.Equal(expected.LastName, actual.LastName);
        }

        [Fact(Skip="Not implemented yet")]
        public void ShouldNotThrowIfNoConstructorWithSerializationInfo()
        {
            var expected = new PersonWithoutConstructor("Scott");

            Serialize(expected);
            Reset();
            var actual = Deserialize<PersonWithoutConstructor>();
            Assert.Equal(expected.FirstName, actual.FirstName);
        }

        [Fact]
        public void ShouldThrowOnNonSerializableEvent()
        {
            var expected = new NotSerializableEvent(true);

            Assert.Throws<NotImplementedException>(() => Serialize(expected));
        }

        [Fact]
        public void SupportsIDeserializationCallback()
        {
            var expected = new PersonWithIDeserializationCallback { FirstName = "Scott" };

            Serialize(expected);
            Reset();
            var exception = Assert.Throws<Exception>(() => Deserialize<PersonWithIDeserializationCallback>());
            Assert.Equal("OnDeserialization", exception.Message);
        }
    }
}
#endif
#region copyright
// -----------------------------------------------------------------------
//  <copyright file="TestBase.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.IO;
using Xunit;

namespace Hyperion.Tests
{
    public class PreserveObjectReferencesTests
    {
        [Fact]
        public void When_preserve_object_references_is_off_globally_and_PreserveReferences_is_used_references_should_be_preserved()
        {
            var serializer = new Serializer(new SerializerOptions(preserveObjectReferences: false));
            var expected = new PreservedObject { First = "first", Second = 1234 };

            AssertPreserveObjectReferencesWorks(serializer, expected, checkReferenceEquality: true);
        }

        [Fact]
        public void When_preserve_object_references_is_off_globally_and_PreserveReferences_is_used_in_nested_objects_references_should_be_preserved()
        {
            var serializer = new Serializer(new SerializerOptions(preserveObjectReferences: false));
            var x = new PreservedObject { First = "first", Second = 1234 };
            var expected = new Parent(123, x);

            AssertPreserveObjectReferencesWorks(serializer, expected, checkReferenceEquality: false);
        }

        [Fact]
        public void When_preserve_object_references_is_on_globally_references_should_be_preserved_for_any_object()
        {
            var serializer = new Serializer(new SerializerOptions(preserveObjectReferences: true));
            var expected = new Something { Int32Prop = 1235, StringProp = "hello", BoolProp = true };

            AssertPreserveObjectReferencesWorks(serializer, expected, checkReferenceEquality: true);
        }

        [Fact]
        public void When_preserve_object_references_is_on_globally_and_PreserveReferences_is_disabled_references_should_not_be_preserved()
        {
            var serializer = new Serializer(new SerializerOptions(preserveObjectReferences: true));
            var expected = new UnpreservedObject { First = "first", Second = 1234 };

            var serializerSession = new SerializerSession(serializer);
            var deserializerSession = new DeserializerSession(serializer);

            using (var stream1 = new MemoryStream())
            using (var stream2 = new MemoryStream())
            {
                serializer.Serialize(expected, stream1, serializerSession);
                var payload1 = stream1.ToArray();
                stream1.Position = 0;
                var actual1 = serializer.Deserialize<UnpreservedObject>(stream1, deserializerSession);

                serializer.Serialize(expected, stream2, serializerSession);
                var payload2 = stream2.ToArray();
                stream2.Position = 0;
                var actual2 = serializer.Deserialize<UnpreservedObject>(stream1, deserializerSession);

                // all objects should be equal to each other before/after serialization
                Assert.Equal(expected, actual1);
                Assert.Equal(expected, actual2);
                Assert.Equal(actual1, actual2);
                Assert.NotSame(actual1, actual2);

                // payloads should be equal - even thou we have preserveObjectReferences = true,
                // for this type it's turned off
                Assert.Equal(payload1.Length, payload2.Length);
            }
        }

        private static void AssertPreserveObjectReferencesWorks<T>(Serializer serializer, T expected, bool checkReferenceEquality)
        {
            var serializerSession = new SerializerSession(serializer);
            var deserializerSession = new DeserializerSession(serializer);

            using (var stream1 = new MemoryStream())
            using (var stream2 = new MemoryStream())
            {
                serializer.Serialize(expected, stream1, serializerSession);
                var payload1 = stream1.ToArray();
                stream1.Position = 0;
                var actual1 = serializer.Deserialize<T>(stream1, deserializerSession);

                serializer.Serialize(expected, stream2, serializerSession);
                var payload2 = stream2.ToArray();
                stream2.Position = 0;
                var actual2 = serializer.Deserialize<T>(stream1, deserializerSession);

                // all objects should be equal to each other before/after serialization
                Assert.Equal(expected, actual1);
                Assert.Equal(expected, actual2);

                if (checkReferenceEquality)
                    Assert.Same(actual1, actual2);
                else
                    Assert.Equal(actual1, actual2);

                // payloads should not be equal - in fact second time, second payload should be smaller
                // since we reused metadata from session
                Assert.NotEqual(payload1, payload2);
                Assert.True(payload2.Length < payload1.Length, "on second serialization the result payload should be smaller");
            }
        }
    }

    public sealed class Parent : IEquatable<Parent>
    {
        public int Id { get; }

        public PreservedObject Inner { get; }

        public Parent(int id, PreservedObject inner)
        {
            Id = id;
            Inner = inner;
        }

        public bool Equals(Parent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Equals(Inner, other.Inner);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is Parent && Equals((Parent) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ (Inner != null ? Inner.GetHashCode() : 0);
            }
        }
    }

    [PreserveReferences]
    public sealed class PreservedObject : IEquatable<PreservedObject>
    {
        public string First { get; set; }
        public int Second { get; set; }

        public bool Equals(PreservedObject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(First, other.First) && Second == other.Second;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is PreservedObject && Equals((PreservedObject)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((First != null ? First.GetHashCode() : 0) * 397) ^ Second;
            }
        }
    }

    [PreserveReferences(false)]
    public sealed class UnpreservedObject : IEquatable<UnpreservedObject>
    {
        public string First { get; set; }
        public int Second { get; set; }

        public bool Equals(UnpreservedObject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(First, other.First) && Second == other.Second;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is UnpreservedObject && Equals((UnpreservedObject)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((First != null ? First.GetHashCode() : 0) * 397) ^ Second;
            }
        }
    }

}
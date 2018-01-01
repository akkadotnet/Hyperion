#region copyright
// -----------------------------------------------------------------------
//  <copyright file="TestBase.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using Xunit;

namespace Hyperion.Tests
{
    public class LocalPreserveObjectReferencesTests : TestBase
    {
        private readonly SerializerSession _serializerSession;
        private readonly DeserializerSession _deserializerSession;

        public LocalPreserveObjectReferencesTests() : base(new SerializerOptions(preserveObjectReferences: false))
        {
            _serializerSession = new SerializerSession(Serializer);
            _deserializerSession = new DeserializerSession(Serializer);
        }

        [Fact]
        public void PreserveReferencesAttributeShouldOverrideSerializerOptions()
        {
            var source = new PreservedObject { First = "first", Second = 123 };
            Serialize(source, _serializerSession);
            Reset();
            var actual1 = Deserialize<PreservedObject>(_deserializerSession);

            Assert.Equal(source, actual1);

            Serialize(source, _serializerSession);
            Reset();
            var actual2 = Deserialize<PreservedObject>(_deserializerSession);

            Assert.Equal(source, actual2);
        }
    }

    public class GlobalPreserveObjectReferencesTests : TestBase
    {
        private readonly SerializerSession _serializerSession;
        private readonly DeserializerSession _deserializerSession;

        public GlobalPreserveObjectReferencesTests() : base(new SerializerOptions(preserveObjectReferences: true))
        {
            _serializerSession = new SerializerSession(Serializer);
            _deserializerSession = new DeserializerSession(Serializer);
        }

        [Fact]
        public void PreserveObjectReferencesShouldWorkInSessionScope()
        {
            var source = new Something { Int32Prop = 123, StringProp = "hello" };
            Serialize(source, _serializerSession);
            Reset();
            var actual1 = Deserialize<Something>(_deserializerSession);

            Assert.Equal(source, actual1);

            Serialize(source, _serializerSession);
            Reset();
            var actual2 = Deserialize<Something>(_deserializerSession);

            Assert.Equal(source, actual2);
        }

        [Fact]
        public void PreserveReferencesAttributeShouldOverrideSerializerOptions()
        {
            var source = new UnpreservedObject { First = "first", Second = 123 };
            Serialize(source, _serializerSession);
            Reset();
            var actual1 = Deserialize<UnpreservedObject>(_deserializerSession);

            Assert.Equal(source, actual1);

            Serialize(source, _serializerSession);
            Reset();
            var actual2 = Deserialize<UnpreservedObject>(_deserializerSession);

            Assert.Equal(source, actual2);
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
            return obj is PreservedObject && Equals((PreservedObject) obj);
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
            return obj is UnpreservedObject && Equals((UnpreservedObject) obj);
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
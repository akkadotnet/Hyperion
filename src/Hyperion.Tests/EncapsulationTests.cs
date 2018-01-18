﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="EncapsulationTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Hyperion.Tests
{
    #region test support classes

    public class ClassWithPrivCtor
    {
        public static ClassWithPrivCtor Create(int value) => new ClassWithPrivCtor(value);

        public int Value { get; }

        private ClassWithPrivCtor(int value)
        {
            Value = value;
        }
    }

    internal class NonPublicClass
    {
        public int Value { get; }

        public NonPublicClass(int value)
        {
            Value = value;
        }
    }

#if SERIALIZATION
    [Serializable]
#endif
    public struct StructWithReadonlyFields
    {
        public readonly int Value;
        public string PublicValue;

        public StructWithReadonlyFields(int value, string publicValue)
        {
            Value = value;
            PublicValue = publicValue;
        }
    }

    #endregion

    public class EncapsulationTests : IDisposable
    {
        private readonly Serializer serializer;
        private readonly MemoryStream stream;

        public EncapsulationTests()
        {
            serializer = new Serializer();
            stream = new MemoryStream();
        }

        [Fact]
        public void CanSerializeObjectsWithNonPublicConstructor()
        {
            var actual = ClassWithPrivCtor.Create(55);
            var deserialized = Roundtrip(actual);

            Assert.Equal(actual.Value, deserialized.Value);
        }

        [Fact]
        public void CanSerializeObjectsWithNonPublicConstructorInCollections()
        {
            IList<ClassWithPrivCtor> actual = new List<ClassWithPrivCtor>
            {
                ClassWithPrivCtor.Create(123),
                ClassWithPrivCtor.Create(239),
                ClassWithPrivCtor.Create(345746)
            };

            var deserialized = Roundtrip(actual);

            Assert.Equal(actual.Count, deserialized.Count);
            Assert.Equal(actual[0].Value, deserialized[0].Value);
            Assert.Equal(actual[1].Value, deserialized[1].Value);
            Assert.Equal(actual[2].Value, deserialized[2].Value);
        }

        [Fact]
        public void CanSerializeNonPublicClasses()
        {
            var actual = new NonPublicClass(123);
            var deserialized = Roundtrip(actual);

            Assert.Equal(actual.Value, deserialized.Value);
        }

        [Fact]
        public void CanSerializeNonPublicClassesInCollections()
        {
            IList<NonPublicClass> actual = new List<NonPublicClass>
            {
                new NonPublicClass(123),
                new NonPublicClass(239),
                new NonPublicClass(345746)
            };

            var deserialized = Roundtrip(actual);

            Assert.Equal(actual.Count, deserialized.Count);
            Assert.Equal(actual[0].Value, deserialized[0].Value);
            Assert.Equal(actual[1].Value, deserialized[1].Value);
            Assert.Equal(actual[2].Value, deserialized[2].Value);
        }

        [Fact]
        public void CanSerializeStructsWithReadonlyFields()
        {
            var actual = new StructWithReadonlyFields(123, "hello");
            var deserialized = Roundtrip(actual);

            Assert.Equal(actual.Value, deserialized.Value);
        }

        [Fact]
        public void CanSerializeStructsWithReadonlyFieldsInCollections()
        {
            IList<StructWithReadonlyFields> actual = new List<StructWithReadonlyFields>
            {
                new StructWithReadonlyFields(123, "hello"),
                new StructWithReadonlyFields(239, "world"),
            };

            var deserialized = Roundtrip(actual);

            Assert.Equal(actual.Count, deserialized.Count);
            Assert.Equal(actual[0].Value, deserialized[0].Value);
            Assert.Equal(actual[1].Value, deserialized[1].Value);
            Assert.Equal(actual[0].PublicValue, deserialized[0].PublicValue);
            Assert.Equal(actual[1].PublicValue, deserialized[1].PublicValue);
        }

        private T Roundtrip<T>(T target)
        {
            serializer.Serialize(target, stream);
            stream.Position = 0;
            return serializer.Deserialize<T>(stream);
        }

        public void Dispose()
        {
            stream.Dispose();
        }
    }
}
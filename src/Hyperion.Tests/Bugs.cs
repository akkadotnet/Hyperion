#region copyright
// -----------------------------------------------------------------------
//  <copyright file="Bugs.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using FluentAssertions;
using Hyperion.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Hyperion.Tests
{

    public class Bugs : TestBase
    {
        private readonly ITestOutputHelper _output;

        public Bugs(ITestOutputHelper output)
        {
            _output = output;
        }
        
        #region issue 58

        public enum TrustLevel { Unknown, Suspicious, Partial, Fully }
        public interface IContainer<out T>
        {
            T Value { get; }
            TrustLevel TrustLevel { get; }
        }
        public class Container<T> : IContainer<T>
        {
            public T Value { get; }
            public TrustLevel TrustLevel { get; }

            public Container(T value, TrustLevel trustLevel)
            {
                Value = value;
                TrustLevel = trustLevel;
            }
        }
        public class ProvisioningResultMessage<T>
        {
            private T Result { get; }

            public ProvisioningResultMessage(T result)
            {
                Result = result;
            }
        }

        #endregion

        public class ByteMessage
        {
            public DateTime UtcTime { get; }
            public long LongValue { get; }
            public byte ByteValue { get; }

            public ByteMessage(DateTime utcTime, byte byteValue, long longValue)
            {
                UtcTime = utcTime;
                ByteValue = byteValue;
                LongValue = longValue;
            }

            public override bool Equals(object obj)
            {
                var msg = obj as ByteMessage;
                return msg != null && Equals(msg);
            }

            public bool Equals(ByteMessage other)
            {
                return UtcTime.Equals(other.UtcTime) && LongValue.Equals(other.LongValue) && ByteValue.Equals(other.ByteValue);
            }
        }

        [Fact]
        public void CanSerializeMessageWithByte()
        {
            var stream = new MemoryStream();
            var msg = new ByteMessage(DateTime.UtcNow, 1, 2);
            var serializer = new Serializer(SerializerOptions.Default
                .WithVersionTolerance(true)
                .WithPreserveObjectReferences(true));
            serializer.Serialize(msg, stream);
            stream.Position = 0;
            var res = serializer.Deserialize(stream);
        }

        /// <summary>
        /// Fix for https://github.com/akkadotnet/Hyperion/issues/144
        /// </summary>
        [Fact]
        public void CanFindTypeByManifest_WhenManifestContainsUnknownAssemblyVersion()
        {
            var serializer = new Serializer(SerializerOptions.Default
                .WithVersionTolerance(true)
                .WithPreserveObjectReferences(true));
            var type = typeof(ByteMessage);
            
            MemoryStream GetStreamForManifest(string manifest)
            {
                var stream = new MemoryStream();
                stream.WriteLengthEncodedByteArray(manifest.ToUtf8Bytes(), serializer.GetSerializerSession());
                stream.Position = 0;
                return stream;
            }
            
            // This is used in serialized manifest, should be something like 'Hyperion.Tests.Bugs+ByteMessage, Hyperion.Tests'
            var shortName = type.GetShortAssemblyQualifiedName();
            var shortNameStream = GetStreamForManifest(shortName);
            // Something like 'Hyperion.Tests.Bugs+ByteMessage, Hyperion.Tests, Version=0.9.11.0, Culture=neutral, PublicKeyToken=null'
            var fullName = type.AssemblyQualifiedName;
            var fullNameStream = GetStreamForManifest(fullName);
            // Set bad assembly version to make deserialization fail
            var fullNameWithUnknownVersion = fullName.Remove(fullName.IndexOf(", Version=")) + ", Version=999999, Culture=neutral, PublicKeyToken=null";
            var fullNameWithUnknownVersionStream = GetStreamForManifest(fullNameWithUnknownVersion);

            this.Invoking(_ => TypeEx.GetTypeFromManifestFull(shortNameStream, serializer.GetDeserializerSession()))
                .Should().NotThrow("When assembly short name is specified in manifest, should work");
            this.Invoking(_ => TypeEx.GetTypeFromManifestFull(fullNameStream, serializer.GetDeserializerSession()))
                .Should().NotThrow("When assembly fully qualified name specified and name is correct, should work even before fix");
            // This one was initially failing
            this.Invoking(_ => TypeEx.GetTypeFromManifestFull(fullNameWithUnknownVersionStream, serializer.GetDeserializerSession()))
                .Should().NotThrow("When assembly fully qualified name specified and unknown/wrong, type should be detected anyway");
        }

        [Fact]
        public void TypeEx_ToQualifiedAssemblyName_should_strip_version_correctly_for_mscorlib_substitution()
        {
            var version = TypeEx.ToQualifiedAssemblyName(
                "System.Collections.Immutable.ImmutableDictionary`2[[System.String, mscorlib,%core%],[System.Int32, mscorlib,%core%]]," +
                " System.Collections.Immutable, Version=1.2.1.0, PublicKeyToken=b03f5f7f11d50a3a",
                ignoreAssemblyVersion: true);

            var coreAssemblyName = typeof(TypeEx).GetField("CoreAssemblyName", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
            if (coreAssemblyName == null)
                throw new Exception($"CoreAssemblyName private static field does not exist in {nameof(TypeEx)} class anymore");
            
            version.Should().Be("System.Collections.Immutable.ImmutableDictionary`2" +
                                $"[[System.String, mscorlib{coreAssemblyName}],[System.Int32, mscorlib{coreAssemblyName}]], System.Collections.Immutable");
        }
        
        [Fact]
        public void TypeEx_ToQualifiedAssemblyName_should_strip_version_correctly_for_multiple_versions_specified()
        {
            var version = TypeEx.ToQualifiedAssemblyName(
                "System.Collections.Immutable.ImmutableList`1[[Foo.Bar, Foo, Version=2019.12.10.1]], " +
                "System.Collections.Immutable, Version=1.2.2.0, PublicKeyToken=b03f5f7f11d50a3a",
                ignoreAssemblyVersion: true);

            version.Should().Be("System.Collections.Immutable.ImmutableList`1[[Foo.Bar, Foo]], System.Collections.Immutable");
        }

        [Fact]
        public void CanSerialieCustomType_bug()
        {
            var stream = new MemoryStream();
            var serializer = new Serializer(SerializerOptions.Default
                .WithVersionTolerance(true)
                .WithPreserveObjectReferences(true));
            var root = new Recover(SnapshotSelectionCriteria.Latest);

            serializer.Serialize(root, stream);
            stream.Position = 0;
            var actual = serializer.Deserialize<Recover>(stream);
        }

        [Fact]
        public void CanSerializeImmutableGenericInterfaces()
        {
            var serializer = new Serializer(SerializerOptions.Default
                .WithVersionTolerance(true)
                .WithPreserveObjectReferences(true));
            var names = new List<Container<string>>
            {
                new Container<string>("Mr", TrustLevel.Partial),
                new Container<string>("Bob", TrustLevel.Partial),
                new Container<string>("Smith", TrustLevel.Suspicious),
                new Container<string>("Mrs", TrustLevel.Suspicious),
                new Container<string>("Jane", TrustLevel.Suspicious),
                new Container<string>("Smith", TrustLevel.Suspicious),
                new Container<string>("Master", TrustLevel.Fully),
                new Container<string>("Fred", TrustLevel.Fully),
                new Container<string>("Smith", TrustLevel.Fully),
                new Container<string>("Miss", TrustLevel.Partial),
                new Container<string>("Sandra", TrustLevel.Partial),
                new Container<string>("Smith", TrustLevel.Suspicious)
            };
            var message = new ProvisioningResultMessage<IEnumerable<Container<string>>>(names.ToImmutableArray());

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(message, stream);
                stream.Position = 0;
                var actual = serializer.Deserialize(stream);
            }
        }

        [Fact]
        public void CanSerializeUri()
        {
            var stream = new MemoryStream();
            var msg = new Uri("http://localhost:9202/", UriKind.RelativeOrAbsolute);
            var serializer = new Serializer(SerializerOptions.Default
                .WithVersionTolerance(true)
                .WithPreserveObjectReferences(true));
            serializer.Serialize(msg, stream);
            stream.Position = 0;
            var res = serializer.Deserialize(stream);

            Assert.Equal(msg, res);
            Assert.Equal(stream.Length, stream.Position);
        }

        #region Issue 117

        [Fact]
        public void CanSerializeColor()
        {
            var expected = Color.Aquamarine;
            Serialize(expected);
            Reset();
            var actual = Deserialize<Color>();
            Assert.Equal(expected, actual);
        }

        #endregion

        public class SnapshotSelectionCriteria
        {
            public static SnapshotSelectionCriteria Latest { get; set; } = new SnapshotSelectionCriteria()
            {
                Foo = "hello",
            };
            public string Foo { get; set; }
        }

#if SERIALIZATION
        [Serializable]
#endif
        public sealed class Recover
        {
            public static readonly Recover Default = new Recover(SnapshotSelectionCriteria.Latest);
            public Recover(SnapshotSelectionCriteria fromSnapshot, long toSequenceNr = long.MaxValue, long replayMax = long.MaxValue)
            {
                FromSnapshot = fromSnapshot;
                ToSequenceNr = toSequenceNr;
                ReplayMax = replayMax;
            }

            /// <summary>
            /// Criteria for selecting a saved snapshot from which recovery should start. Default is del youngest snapshot.
            /// </summary>
            public SnapshotSelectionCriteria FromSnapshot { get; private set; }

            /// <summary>
            /// Upper, inclusive sequence number bound. Default is no upper bound.
            /// </summary>
            public long ToSequenceNr { get; private set; }

            /// <summary>
            /// Maximum number of messages to replay. Default is no limit.
            /// </summary>
            public long ReplayMax { get; private set; }
        }

        delegate int TestDelegate(int x, int y);

        class Temp : IEquatable<Temp>
        {
            public object[] SubArray { get; set; }
            public int[] IntArray { get; set; }
            public int[,] IntIntArray { get; set; }
            public Poco Poco { get; set; }
            public string String { get; set; }
            public Dictionary<int,string> Dictionary { get; set; }
            public TestDelegate Delegate { get; set; }
            public IEnumerable<int> TestEnum { get; set; }
            public Exception Exception { get; set; }
            public ImmutableList<int> ImmutableList { get; set; }
            public ImmutableDictionary<int, string> ImmutableDictionary { get; set; }

            public bool Equals(Temp other)
            {
                if (other == null) 
                    throw new Exception("Equals failed.");
                if (ReferenceEquals(this, other))
                    throw new Exception("Equals failed.");
                if (IntIntArray.Rank != other.IntIntArray.Rank)
                    throw new Exception("Equals failed.");

                for (var i = 0; i < IntIntArray.Rank; ++i)
                {
                    for (var j = 0; j < IntIntArray.GetLength(i); ++j)
                    {
                        if (IntIntArray[j, i] != other.IntIntArray[j, i])
                            throw new Exception("Equals failed.");
                    }
                }

                if (Exception.GetType() != other.Exception.GetType()) 
                    throw new Exception("Equals failed.");
                if (Exception.Message != other.Exception.Message)
                    throw new Exception("Equals failed.");
                if(Exception.InnerException != null 
                   && Exception.InnerException.GetType() != other.Exception.InnerException.GetType())
                    throw new Exception("Equals failed.");

                for (var i = 0; i < SubArray.Length; i++)
                {
                    if (SubArray[i].GetType() != other.SubArray[i].GetType())
                        throw new Exception("Equals failed.");
                    
                    if (SubArray[i] is Array arr)
                    {
                        var oArr = (Array)other.SubArray[i];
                        for (var j = 0; j < arr.Length; ++j)
                        {
                            if (!arr.GetValue(j).Equals(oArr.GetValue(j)))
                                throw new Exception("Equals failed.");
                        }
                    } else if (!SubArray[i].Equals(other.SubArray[i]))
                        throw new Exception("Equals failed.");
                }

                foreach (var key in Dictionary.Keys)
                {
                    if (!Dictionary[key].Equals(other.Dictionary[key]))
                        throw new Exception("Equals failed.");
                }

                foreach (var key in ImmutableDictionary.Keys)
                {
                    if (!ImmutableDictionary[key].Equals(other.ImmutableDictionary[key]))
                        throw new Exception("Equals failed.");
                }

                if (other.Delegate(2, 2) != 4)
                    throw new Exception("Equals failed.");

                if(!IntArray.SequenceEqual(other.IntArray))
                    throw new Exception("Equals failed.");
                if(!Equals(Poco, other.Poco))
                    throw new Exception("Equals failed.");
                if (String != other.String)
                    throw new Exception("Equals failed.");
                if(!TestEnum.SequenceEqual(other.TestEnum))
                    throw new Exception("Equals failed.");
                if(!ImmutableList.SequenceEqual(other.ImmutableList))
                    throw new Exception("Equals failed.");

                return true;
            }

            public override bool Equals(object obj)
            {
                if (obj == null) throw new Exception("Equals failed.");
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) throw new Exception("Equals failed.");
                return Equals((Temp) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (SubArray != null ? SubArray.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (IntArray != null ? IntArray.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (IntIntArray != null ? IntIntArray.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Poco != null ? Poco.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (String != null ? String.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Dictionary != null ? Dictionary.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Delegate != null ? Delegate.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (TestEnum != null ? TestEnum.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (Exception != null ? Exception.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (ImmutableList != null ? ImmutableList.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        class Poco : IEquatable<Poco>
        {
            public Poco()
            { }

            public Poco(int intValue, string stringValue)
            {
                Int = intValue;
                String = stringValue;
            }

            public int Int { get; set; }
            public string String { get; set; }

            public bool Equals(Poco other)
            {
                if (ReferenceEquals(null, other)) 
                    throw new Exception("Equals failed.");
                if (ReferenceEquals(this, other)) 
                    throw new Exception("Equals failed.");
                if(Int != other.Int)
                    throw new Exception("Equals failed.");
                if(String != other.String)
                    throw new Exception("Equals failed.");
                return true;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) throw new Exception("Equals failed.");
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) throw new Exception("Equals failed.");
                return Equals((Poco) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Int * 397) ^ (String != null ? String.GetHashCode() : 0);
                }
            }
        }

        [Fact]
        public void WritesManifestEvenIfKnown()
        {
            var stream = new MemoryStream();
            var msg = new Temp
            {
                SubArray = new object[] { 1, (byte)2, new object[] { 3 } },
                IntArray = new [] {1, 2, 3, 4, 5},
                IntIntArray = new [,] {{1, 2}, {3,4}, {5,6}, {7,8}},
                Poco = new Poco(999, "666"),
                String = "huhu", 
                Dictionary = new Dictionary<int, string>
                {
                    { 666, "b" },
                    { 999, "testString" },
                    { 42, "iMaGiNe" }
                }, 
                Delegate = (x, y) => x * y,
                TestEnum = new[]{4,8,9,3,2},
                Exception = new ArgumentException("Test Exception", new IndexOutOfRangeException("-999")),
                ImmutableList = new [] {9, 4, 6, 2, 5}.ToImmutableList(),
                ImmutableDictionary = new Dictionary<int, string>
                {
                    { 666, "b" },
                    { 999, "testString" },
                    { 42, "iMaGiNe" }
                }.ToImmutableDictionary(),
            };
            var serializer = new Serializer(SerializerOptions.Default.WithKnownTypes(new[]
            {
                typeof(object[]),
                typeof(int[]),
                typeof(int[,]),
                typeof(Dictionary<int, string>),
                typeof(DictionaryEntry),
                typeof(KeyValuePair<int,string>),
                typeof(Temp), 
                typeof(TestDelegate),
                typeof(Enumerable),
                typeof(IEnumerable<int>),
                typeof(Exception),
                typeof(ArgumentException),
                typeof(IndexOutOfRangeException),
                typeof(FieldInfo),
                typeof(ImmutableList),
                typeof(ImmutableList<int>),
                typeof(ImmutableDictionary<int, string>),
                typeof(MethodInfo),
                typeof(PropertyInfo),
            }));
            serializer.Serialize(msg, stream);
            stream.Position = 0;
            var a = stream.ToArray();
            var text = string.Join("", a.Select(x => x < 32 || x > 126 ? "" : ((char)x).ToString()));
            _output.WriteLine(text);
            var res = (Temp)serializer.Deserialize(stream);
            Assert.DoesNotContain("System.Collections.Generic.Dictionary", text);
            Assert.Equal(msg, res);
        }

        #region Issue 348

        class FieldsToOrder
        {
#pragma warning disable CS0649
            public string A2;
            // ReSharper disable once InconsistentNaming
            public string a1;
#pragma warning restore CS0649
        }

        [Fact]
        public void ShouldOrderFieldsByOrdinal()
        {
            string[] expected = { "A2", "a1" };
            var actual = typeof(FieldsToOrder).GetFieldInfosForType().Select(x => x.Name).ToArray();
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}

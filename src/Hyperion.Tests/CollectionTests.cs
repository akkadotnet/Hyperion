#region copyright
// -----------------------------------------------------------------------
//  <copyright file="CollectionTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.IO;
using System.Linq;
using Xunit;
using Hyperion.SerializerFactories;
using Hyperion.ValueSerializers;
using Hyperion.Extensions;

namespace Hyperion.Tests
{
    public class CollectionTests : TestBase
    {
        [Fact]
        public void CanSerializeArrayOfTuples()
        {
            var expected = new[]
            {
                Tuple.Create(1, 2, 3),
                Tuple.Create(4, 5, 6),
                Tuple.Create(7, 8, 9)
            };
            Serialize(expected);
            Reset();
            var actual = Deserialize<Tuple<int, int, int>[]>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeByteArray()
        {
            var expected = new byte[]
            {
                1, 2, 3, 4
            };
            Serialize(expected);
            Reset();
            var actual = Deserialize<byte[]>();
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void CanSerializeDictionary()
        {
            var expected = new Dictionary<string, string>
            {
                ["abc"] = "def",
                ["ghi"] = "jkl,"
            };

            Serialize(expected);
            Reset();
            var actual = Deserialize<Dictionary<string, string>>();
            Assert.Equal(expected.ToList(), actual.ToList());
        }

        [Fact]
        public void CanSerializeIDictionaryInterface()
        {
            var expected = new Dictionary<string, string>
            {
                ["abc"] = "def",
                ["ghi"] = "jkl,"
            };

            Serialize(expected);
            Reset();
            var actual = Deserialize<IDictionary<string, string>>();
            Assert.Equal(expected.ToList(), actual.ToList());
        }

        [Fact]
        public void CanSerializeDictionaryKeysAndValuesByteChar()
        {
            var instance = new Dictionary<byte, char> {{0, 'z'}, {255, 'z'}, {3, char.MinValue}};
            Serialize(instance);
            Reset();
            var res = Deserialize<Dictionary<byte, char>>();
            Assert.Equal(instance.Count, res.Count);

            foreach (var kvp in instance)
            {
                Assert.True(res.ContainsKey(kvp.Key));
                Assert.Equal(kvp.Value, res[kvp.Key]);
            }
        }

        [Fact]
        public void CanSerializeDictionaryKeysAndValuesByteString()
        {
            var instance = new Dictionary<byte, string> {{0, "z"}, {255, "z"}, {3, null}};
            Serialize(instance);
            Reset();
            var res = Deserialize<Dictionary<byte, string>>();
            Assert.Equal(instance.Count, res.Count);

            foreach (var kvp in instance)
            {
                Assert.True(res.ContainsKey(kvp.Key));
                Assert.Equal(kvp.Value, res[kvp.Key]);
            }
        }


        [Fact]
        public void CanSerializeExpandoObject()
        {
            var obj = new ExpandoObject();
            var dict = (IDictionary<string, object>) obj;
            dict.Add("Test1", "Value1");
            dict.Add("Test2", 1);
            dict.Add("Test3", DateTime.Now);

            var nestedObj = new ExpandoObject();
            var nestedDict = (IDictionary<string, object>) nestedObj;
            nestedDict.Add("NestedTest1", "Value2");
            nestedDict.Add("NestedTest2", new[] {"v1", "v2"});

            dict.Add("Test4", nestedObj);


            Serialize(obj);
            Reset();
            var actual = Deserialize<ExpandoObject>() as IDictionary<string, object>;
            //TODO: Check values
        }

        [Fact]
        public void CanSerializeImmutableDictionary()
        {
            var map = ImmutableDictionary<string, object>.Empty;
            var serializer = new Serializer();

            using (var stream = new MemoryStream())
            {
                serializer.Serialize(map, stream);
                stream.Position = 0;
                var map2 = serializer.Deserialize(stream); // exception
            }
        }

        [Fact]
        public void CanSerializeIntArray()
        {
            var expected = Enumerable.Range(0, 10000).ToArray();
            Serialize(expected);
            Reset();
            var actual = Deserialize<int[]>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeList()
        {
            var expected = new[]
            {
                new Something
                {
                    BoolProp = true,
                    Else = new Else
                    {
                        Name = "Yoho"
                    },
                    Int32Prop = 999,
                    StringProp = "Yesbox!"
                },
                new Something(), new Something(), null
            }.ToList();

            Serialize(expected);
            Reset();
            var actual = Deserialize<List<Something>>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeArrayList()
        {
	        var expected = new ArrayList()
	        {
		        new Something
		        {
			        BoolProp = true,
			        Else = new Else
			        {
				        Name = "Yoho"
			        },
			        Int32Prop = 999,
			        StringProp = "Yesbox!"
		        },
		        "a", 123
	        };

	        Serialize(expected);
	        Reset();
	        var actual = Deserialize<ArrayList>();
	        Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeLinkedList()
        {
            var expected = new LinkedList<Something>(new[]
            {
                new Something
                {
                    BoolProp = true,
                    Else = new Else
                    {
                        Name = "Yoho"
                    },
                    Int32Prop = 999,
                    StringProp = "Yesbox!"
                },
                new Something(), new Something(), null
            });

            Serialize(expected);
            Reset();
            var actual = Deserialize<LinkedList<Something>>();
            Assert.Equal(expected, actual);
        }

        [Fact(Skip = "add support for multi dimentional arrays")]
        public void CanSerializeMultiDimentionalArray()
        {
            var expected = new double[3, 3, 3];
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    for (var k = 0; k < 3; k++)
                    {
                        expected[i, j, k] = i + j + k;
                    }
                }
            }
            Serialize(expected);
            Reset();
            var actual = Deserialize<double[,,]>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeObjectArray()
        {
            var expected = new[]
            {
                new Something
                {
                    BoolProp = true,
                    Else = new Else
                    {
                        Name = "Yoho"
                    },
                    Int32Prop = 999,
                    StringProp = "Yesbox!"
                },
                new Something(),
                new Something(), null
            };
            Serialize(expected);
            Reset();
            var actual = Deserialize<Something[]>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializePrimitiveArray()
        {
            var expected = new[] {DateTime.MaxValue, DateTime.MinValue, DateTime.Now, DateTime.Today};
            Serialize(expected);
            Reset();
            var actual = Deserialize<DateTime[]>();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CanSerializeSet()
        {
            var expected = new HashSet<Something>
            {
                new Something
                {
                    BoolProp = true,
                    Else = new Else
                    {
                        Name = "Yoho"
                    },
                    Int32Prop = 999,
                    StringProp = "Yesbox!"
                },
                new Something(),
                new Something(),
                null
            };

            Serialize(expected);
            Reset();
            var actual = Deserialize<HashSet<Something>>();
            Assert.Equal(expected.ToList(), actual.ToList());
        }

        [Fact]
        public void CanSerializeStack()
        {
            var expected = new Stack<Something>();
            expected.Push(new Something
            {
                BoolProp = true,
                Else = new Else
                {
                    Name = "Yoho"
                },
                Int32Prop = 999,
                StringProp = "Yesbox!"
            });


            expected.Push(new Something());

            expected.Push(new Something());

            Serialize(expected);
            Reset();
            var actual = Deserialize<Stack<Something>>();
            Assert.Equal(expected.ToList(), actual.ToList());
        }

        [Fact]
        public void CanSerializeArray2DOfInt()
        {
            var expected = new int[,] { { 1, 2 }, { 3, 4 }, { 5, 6 }, { 7, 8 } };   // OK
            Serialize(expected);
            Reset();
            var actual = Deserialize<int[,]>();
            for (int i = expected.GetLowerBound(0); i < expected.GetUpperBound(0); i++)
            {
                for (int j = expected.GetLowerBound(1); j < expected.GetUpperBound(1); j++)
                {
                    Assert.Equal(expected[i, j], actual[i, j]);
                }
            }
        }

        [Fact]
        public void CanSerializeArray2DOfObj()
        {
            var expected = new object[,] { { "Header1", 2 }, { "Header2", 4 }};   // OK
            Serialize(expected);
            Reset();
            var actual = Deserialize<object[,]>();
            for (int i = expected.GetLowerBound(0); i < expected.GetUpperBound(0); i++)
            {
                for (int j = expected.GetLowerBound(1); j < expected.GetUpperBound(1); j++)
                {
                    Assert.Equal(expected[i, j], actual[i, j]);
                }
            }
        }

        [Fact]
        public void CanSerializeArray3DOfInt()
        {
            int[,,] expected = new int[,,] { { { 1, 2, 3 }, { 4, 5, 6 } },
                                 { { 7, 8, 9 }, { 10, 11, 12 } } };
            Serialize(expected);
            Reset();
            var actual = Deserialize<int[,,]>();
            for (int i = expected.GetLowerBound(0); i < expected.GetUpperBound(0); i++)
            {
                for (int j = expected.GetLowerBound(1); j < expected.GetUpperBound(1); j++)
                {
                    for (int m = expected.GetLowerBound(2); j < expected.GetUpperBound(2); j++)
                    {
                        Assert.Equal(expected[i, j, m], actual[i, j, m]);
                    }
                }
            }
        }


        [Fact]
        public void Issue18()
        {
            var msg = new byte[] {1, 2, 3, 4};
            var serializer = new Serializer(new SerializerOptions(true, true));

            byte[] serialized;
            using (var ms = new MemoryStream())
            {
                serializer.Serialize(msg, ms);
                serialized = ms.ToArray();
            }

            byte[] deserialized;
            using (var ms = new MemoryStream(serialized))
            {
                deserialized = serializer.Deserialize<byte[]>(ms);
            }

            Assert.True(msg.SequenceEqual(deserialized));
        }

        #region test classes

        public class CustomAdd : IEnumerable<int>
        {
            public IImmutableList<int> Inner { get; }
            public int Count => Inner.Count;

            public CustomAdd(IImmutableList<int> inner)
            {
                this.Inner = inner;
            }

            public CustomAdd Add(int item) => new CustomAdd(Inner.Add(item));

            public IEnumerator<int> GetEnumerator() => Inner.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class CustomAddRange : IEnumerable<int>
        {
            public IImmutableList<int> Inner { get; }
            public int Count => Inner.Count;

            public CustomAddRange(IImmutableList<int> inner)
            {
                this.Inner = inner;
            }

            public CustomAddRange AddRange(string newLabel, int[] items) => new CustomAddRange(Inner.AddRange(items));

            public IEnumerator<int> GetEnumerator() => Inner.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        #endregion

        [Fact]
        public void CanSerializeCustomEnumerableWithNonStandardAddSignature()
        {
            var init = new CustomAdd(ImmutableList<int>.Empty);
            var expected = init.Add(1).Add(2);

            Serialize(expected);
            Reset();
            var actual = Deserialize<CustomAdd>();
            Assert.True(expected.SequenceEqual(actual));
        }

        [Fact]
        public void CanSerializeCustomEnumerableWithNonStandardAddRangeSignature()
        {
            var init = new CustomAddRange(ImmutableList<int>.Empty);
            var expected = init.AddRange("label", new []{ 1, 2, 3 });

            Serialize(expected);
            Reset();
            var actual = Deserialize<CustomAddRange>();
            Assert.True(expected.SequenceEqual(actual));
        }

		// things get trickier when unknown types come to scene
		public class NotKnown { }

		public abstract class BaseContainer : IList<NotKnown>, ICollection<NotKnown>, IEnumerable<NotKnown>, IEnumerable, IList, ICollection
		{
			private List<NotKnown> _inner;

			#region interfaces methods

			public IEnumerator<NotKnown> GetEnumerator()
				=> _inner.GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator()
				=> _inner.GetEnumerator();

			// Ambiguous Add method
			public virtual void Add(object content)
				=> _inner.Add((NotKnown)content);

			// Ambiguous Add method
			void ICollection<NotKnown>.Add(NotKnown item)
				=> _inner.Add(item);

			int IList.Add(object value)
				=> ((IList)_inner).Add((NotKnown)value);

			public bool Contains(object value)
				=> _inner.Contains((NotKnown)value);

			void IList.Clear()
				=> _inner.Clear();

			public int IndexOf(object value)
				=> _inner.IndexOf((NotKnown)value);

			public void Insert(int index, object value)
				=> _inner.Insert(index, (NotKnown)value);

			public void Remove(object value)
				=> _inner.Remove((NotKnown)value);

			void IList.RemoveAt(int index)
				=> _inner.RemoveAt(index);

			object IList.this[int index]
			{
				get { return _inner[index]; }
				set { _inner[index] = (NotKnown)value; }
			}

			bool IList.IsReadOnly => false;
			public bool IsFixedSize => false;
			void ICollection<NotKnown>.Clear()
				=> _inner.Clear();

			public bool Contains(NotKnown item)
				=> _inner.Contains(item);

			public void CopyTo(NotKnown[] array, int arrayIndex)
				=> _inner.CopyTo(array, arrayIndex);

			public bool Remove(NotKnown item)
				=> _inner.Remove(item);

			public void CopyTo(Array array, int index)
				=> ((ICollection)_inner).CopyTo(array, index);

			// nb: not a direct implementation!
			public int Count => _inner.Count;

			public object SyncRoot => ((ICollection)_inner).SyncRoot;
			public bool IsSynchronized => ((ICollection)_inner).IsSynchronized;

			int ICollection<NotKnown>.Count => _inner.Count;

			bool ICollection<NotKnown>.IsReadOnly => ((ICollection<NotKnown>)_inner).IsReadOnly;

			public int IndexOf(NotKnown item)
				=> _inner.IndexOf(item);

			public void Insert(int index, NotKnown item)
				=> _inner.Add(item);

			void IList<NotKnown>.RemoveAt(int index)
				=> _inner.RemoveAt(index);

			public NotKnown this[int index]
			{
				get { return _inner[index]; }
				set { _inner[index] = value; }
			}
			#endregion
		}

		public class DerivedContainer : BaseContainer, IList<NotKnown>, ICollection<NotKnown>, IEnumerable<NotKnown>, IEnumerable
		{
			// Ambiguous Add method
			public void Add(NotKnown item)
			{
				this.Add((object)item);
			}
		}

	    [Fact]
	    public void CanInstantiateSerializerForCollectionWithAmbiguousAddMethod()
	    {
		    var serializer = new Serializer(
			    new SerializerOptions(knownTypes: new List<Type> {typeof(DerivedContainer)},
				    serializerFactories: new List<ValueSerializerFactory> {new DerivedContainerSerializerFactory()}));
		    var init = new DerivedContainer();
		    serializer.Serialize(init, new MemoryStream());
		    // we're done if AmbiguousMatchException wasn't fired
	    }

	    public class DerivedContainerSerializerFactory : ValueSerializerFactory
		{
			public override bool CanSerialize(Serializer serializer, Type type)
				=> typeof(DerivedContainer) == type;

			public override bool CanDeserialize(Serializer serializer, Type type)
				=> typeof(DerivedContainer) == type;

			public override ValueSerializer BuildSerializer(Serializer serializer, Type type, ConcurrentDictionary<Type, ValueSerializer> typeMapping)
			{
				var os = new ObjectSerializer(type);
				typeMapping.TryAdd(type, os);
				ObjectReader reader = (stream, session) =>
				{
					var raw = stream.ReadString(session);
					return new DerivedContainer();
				};
				ObjectWriter writer = (stream, value, session) =>
				{
					StringSerializer.WriteValueImpl(stream, "test", session);
				};
				os.Initialize(reader, writer);
				return os;
			}
		}
	    
	    [Fact]
	    public void CanSerializeNullableIntArray()
	    {
		    SerializeAndAssert(new int?[]{1, 2, 3, 4, 5});
	    }

	    [Fact]
	    public void CanSerializeLongArray()
	    {
		    SerializeAndAssert(new []{1L, 2L, 3L, 4L, 5L});
	    }

	    [Fact]
	    public void CanSerializeNullableLongArray()
	    {
		    SerializeAndAssert(new long?[]{1L, 2L, 3L, 4L, 5L});
	    }

	    [Fact]
	    public void CanSerializeShortArray()
	    {
		    SerializeAndAssert(new short[]{1, 2, 3, 4, 5});
	    }
        
	    [Fact]
	    public void CanSerializeNullableShortArray()
	    {
		    SerializeAndAssert(new short?[]{1, 2, 3, 4, 5});
	    }
        
	    [Fact]
	    public void CanSerializeStringArray()
	    {
		    SerializeAndAssert(new []{"1", "2", "3", "4", "5"});
	    }
        
	    [Fact]
	    public void CanSerializeDateTimeOffsetArray()
	    {
		    SerializeAndAssert(new []
		    {
			    DateTimeOffset.Now, 
			    DateTimeOffset.UtcNow,
		    });
	    }
        
	    [Fact]
	    public void CanSerializeStructArray()
	    {
		    SerializeAndAssert(new IStructInterface[]
		    {
			    new PrimitiveStruct {Int = 1, Long = 1, String = "1"},
			    new PrimitiveStruct {Int = 2, Long = 2, String = "2"},
			    new PrimitiveStruct {Int = 3, Long = 3, String = "3"},
			    new PrimitiveStruct {Int = 4, Long = 4, String = "4"},
			    new PrimitiveStruct {Int = 5, Long = 5, String = "5"},
		    });
	    }
        
	    private interface IStructInterface
	    {
		    int Int { get; set; }
		    long Long { get; set; }
		    string String { get; set; }
	    }
	    
	    private struct PrimitiveStruct : IStructInterface
	    {
		    public int Int { get; set; }
		    public long Long { get; set; }
		    public string String { get; set; }
	    }	    
	    
	    private void SerializeAndAssert<T>(T[] expected)
	    {
		    Serialize(expected);
		    Reset();
		    var res = Deserialize<T[]>();
		    Assert.Equal(expected, res);
		    AssertMemoryStreamConsumed();
	    }
    }
}
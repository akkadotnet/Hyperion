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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.IO;
using System.Linq;
using Xunit;

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
    }
}
#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ImmutableCollectionsTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace Hyperion.Tests
{
    public class ImmutableCollectionTests : TestBase
    {
        #region internal classes

        class TestClass
        {
            // KeyValuePair<int, int> because Mono has broken structural equality on KeyValuePair<,>
            public IImmutableDictionary<int, int> Dictionary { get; }
            public IImmutableList<string> List { get; }
            public IImmutableQueue<string> Queue { get; }
            public IImmutableSet<string> SortedSet { get; }
            public IImmutableSet<string> HashSet { get; }
            public IImmutableStack<string> Stack { get; }

            public TestClass(IImmutableDictionary<int, int> dictionary, IImmutableList<string> list, IImmutableQueue<string> queue, IImmutableSet<string> sortedSet, IImmutableSet<string> hashSet, IImmutableStack<string> stack)
            {
                Dictionary = dictionary;
                List = list;
                Queue = queue;
                SortedSet = sortedSet;
                HashSet = hashSet;
                Stack = stack;
            }

            protected bool Equals(TestClass other)
            {
                return SeqEquals(Dictionary, other.Dictionary) 
                    && SeqEquals(List, other.List) 
                    && SeqEquals(Queue, other.Queue) 
                    && SeqEquals(SortedSet, other.SortedSet) 
                    && SeqEquals(HashSet, other.HashSet) 
                    && SeqEquals(Stack, other.Stack);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TestClass) obj);
            }

            private static bool SeqEquals<T>(IEnumerable<T> first, IEnumerable<T> second)
            {
                var f = first.GetEnumerator();
                var s = second.GetEnumerator();
                while (f.MoveNext() && s.MoveNext())
                {
                    var x = f.Current;
                    var y = s.Current;

                    if (!Equals(x, y))
                    {
                        return false;
                    }
                }

                if (f.MoveNext() || s.MoveNext())
                {
                    // collections are not equal in size
                    return false;
                }

                return true;
            }
        }

        #endregion

        [Fact]
        public void CanSerializeImmutableHashSet()
        {
            var expected = ImmutableHashSet.CreateRange(new[]
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
                new Something()
            });

            Serialize(expected);
            Reset();
            var actual = Deserialize<ImmutableHashSet<Something>>();
            Assert.Equal(expected.ToList(), actual.ToList());
        }

        [Fact]
        public void CanSerializeImmutableSortedSet()
        {
            var expected = ImmutableSortedSet.CreateRange(new[]
            {
                "abc",
                "abcd",
                "abcde"
            });

            Serialize(expected);
            Reset();
            var actual = Deserialize<ImmutableSortedSet<string>>();
            Assert.Equal(expected.ToList(), actual.ToList());
        }

        [Fact]
        public void CanSerializeImmutableDictionary()
        {
            var expected = ImmutableDictionary.CreateRange(new Dictionary<string, Something>
            {
                ["a1"] = new Something
                {
                    BoolProp = true,
                    Else = new Else
                    {
                        Name = "Yoho"
                    },
                    Int32Prop = 999,
                    StringProp = "Yesbox!"
                },
                ["a2"] = new Something(),
                ["a3"] = new Something(),
                ["a4"] = null
            });

            Serialize(expected);
            Reset();
            var actual = Deserialize<ImmutableDictionary<string, Something>>();
            Assert.Equal(expected.ToList(), actual.ToList());
        }

        [Fact]
        public void CanSerializeImmutableQueue()
        {
            var expected = ImmutableQueue.CreateRange(new[]
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
            });

            Serialize(expected);
            Reset();
            var actual = Deserialize<ImmutableQueue<Something>>();
            Assert.Equal(expected.ToList(), actual.ToList());
        }

        [Fact]
        public void CanSerializeImmutableStack()
        {
            var expected = ImmutableStack.CreateRange(new[]
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
                new Something()
            });

            Serialize(expected);
            Reset();
            var actual = Deserialize<ImmutableStack<Something>>();
            Assert.Equal(expected.ToList(), actual.ToList());
        }

        [Fact]
        public void CanSerializeImmutableArray()
        {
            var expected = ImmutableArray.CreateRange(new[]
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
            });

            Serialize(expected);
            Reset();
            var actual = Deserialize<ImmutableArray<Something>>();
            Assert.Equal(expected.ToList(), actual.ToList());
        }

        [Fact]
        public void CanSerializeImmutableList()
        {
            var expected = ImmutableList.CreateRange(new[]
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
            });

            Serialize(expected);
            Reset();
            var actual = Deserialize<ImmutableList<Something>>();
            Assert.Equal(expected.ToList(), actual.ToList());
        }

        [Fact]
        public void CanSerializeImmutableCollectionsReferencedThroughInterfaceInFields()
        {
            var expected = new TestClass(
                dictionary: ImmutableDictionary.CreateRange(new[]
                {
                    new KeyValuePair<int, int>(2, 1),
                    new KeyValuePair<int, int>(3, 2),
                }),
                list: ImmutableList.CreateRange(new[] { "c", "d" }),
                queue: ImmutableQueue.CreateRange(new[] { "e", "f" }),
                sortedSet: ImmutableSortedSet.CreateRange(new[] { "g", "h" }),
                hashSet: ImmutableHashSet.CreateRange(new[] { "i", "j" }),
                stack: ImmutableStack.CreateRange(new[] { "k", "l" }));

            Serialize(expected);
            Reset();
            var actual = Deserialize<TestClass>();
            Assert.Equal(expected, actual);
        }
    }
}
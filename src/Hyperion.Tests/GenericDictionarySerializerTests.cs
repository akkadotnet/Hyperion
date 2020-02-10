using System;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Hyperion.Tests
{
    public class GenericDictionarySerializerTests : TestBase
    {
        [Fact]
        public void CanSerializeDictionary()
        {
            var customDict = new CustomDictionary<string, int>(new Dictionary<string, int>()
            {
                ["key"] = 1
            });
            SerializeAndAssertEquivalent(customDict);
        }
        
        private void SerializeAndAssertEquivalent<T>(T expected)
        {
            Serialize(expected);
            Reset();
            var res = Deserialize<T>();
            res.Should().BeEquivalentTo(expected);
            AssertMemoryStreamConsumed();
        }

        /// <summary>
        /// Just a custom class wrapper for another <see cref="IDictionary{TKey,TValue}"/>
        /// </summary>
        class CustomDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        {
            private readonly IDictionary<TKey, TValue> _dictGeneric;

            /// <summary>
            /// For serialization
            /// </summary>
            public CustomDictionary() : this(new Dictionary<TKey, TValue>())
            {
            }

            public CustomDictionary(Dictionary<TKey, TValue> dict)
            {
                _dictGeneric = dict;
            }

            /// <inheritdoc />
            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return _dictGeneric.GetEnumerator();
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable) _dictGeneric).GetEnumerator();
            }

            /// <inheritdoc />
            public void Add(KeyValuePair<TKey, TValue> item)
            {
                _dictGeneric.Add(item);
            }

            /// <inheritdoc />
            public void Clear()
            {
                _dictGeneric.Clear();
            }

            /// <inheritdoc />
            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return _dictGeneric.Contains(item);
            }

            /// <inheritdoc />
            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                _dictGeneric.CopyTo(array, arrayIndex);
            }

            /// <inheritdoc />
            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                return _dictGeneric.Remove(item);
            }

            /// <inheritdoc />
            public int Count => _dictGeneric.Count;

            /// <inheritdoc />
            public bool IsReadOnly => _dictGeneric.IsReadOnly;

            /// <inheritdoc />
            public void Add(TKey key, TValue value)
            {
                _dictGeneric.Add(key, value);
            }

            /// <inheritdoc />
            public bool ContainsKey(TKey key)
            {
                return _dictGeneric.ContainsKey(key);
            }

            /// <inheritdoc />
            public bool Remove(TKey key)
            {
                return _dictGeneric.Remove(key);
            }

            /// <inheritdoc />
            public bool TryGetValue(TKey key, out TValue value)
            {
                return _dictGeneric.TryGetValue(key, out value);
            }

            /// <inheritdoc />
            public TValue this[TKey key]
            {
                get => _dictGeneric[key];
                set => _dictGeneric[key] = value;
            }

            /// <inheritdoc />
            public ICollection<TKey> Keys => _dictGeneric.Keys;

            /// <inheritdoc />
            public ICollection<TValue> Values => _dictGeneric.Values;
        }
    }
}
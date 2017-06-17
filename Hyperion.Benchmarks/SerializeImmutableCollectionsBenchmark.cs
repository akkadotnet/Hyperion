#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializeImmutableCollectionsBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using BenchmarkDotNet.Attributes;

namespace Hyperion.Benchmarks
{
    [Config(typeof(HyperionConfig))]
    public class SerializeImmutableCollectionsBenchmark
    {
        #region init
        private Serializer serializer;
        private MemoryStream stream;

        [Setup]
        public void Setup()
        {
            serializer = new Serializer();
            stream = new MemoryStream();
        }

        [Cleanup]
        public void Cleanup()
        {
            stream.Dispose();
        }
        #endregion

        [Benchmark] public void Serialize_ImmutableArray() => serializer.Serialize(ImmutableArray.CreateRange(new[] { "abc", "cbd0", "sdsd4", "4dfg", "adafd0xd" }), stream);
        [Benchmark] public void Serialize_ImmutableList() => serializer.Serialize(ImmutableList.CreateRange(new[] { "abc", "cbd0", "sdsd4", "4dfg", "adafd0xd" }), stream);
        [Benchmark] public void Serialize_ImmutableHashSet() => serializer.Serialize(ImmutableHashSet.CreateRange(new[] { "abc", "cbd0", "sdsd4", "4dfg", "adafd0xd" }), stream);
        [Benchmark] public void Serialize_ImmutableSortedSet() => serializer.Serialize(ImmutableSortedSet.CreateRange(new[] { "abc", "cbd0", "sdsd4", "4dfg", "adafd0xd" }), stream);
        [Benchmark] public void Serialize_ImmutableDictionary() => serializer.Serialize(ImmutableDictionary.CreateRange(new[]
        {
            new KeyValuePair<string, string>("key1", "value1"),
            new KeyValuePair<string, string>("key2", "value2"),
            new KeyValuePair<string, string>("key3", "value3"),
            new KeyValuePair<string, string>("key4", "value4"),
            new KeyValuePair<string, string>("key5", "value5"),
        }), stream);
    }
}
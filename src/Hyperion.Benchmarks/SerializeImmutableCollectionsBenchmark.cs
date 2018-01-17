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
using BenchmarkDotNet.Attributes;

namespace Hyperion.Benchmarks
{
    public class SerializeImmutableCollectionsBenchmark : HyperionBenchmark
    {
        [Benchmark] public void Immutable_Array() => SerializeAndDeserialize(ImmutableArray.CreateRange(new[] { "abc", "cbd0", "sdsd4", "4dfg", "adafd0xd" }));
        [Benchmark] public void Immutable_List() => SerializeAndDeserialize(ImmutableList.CreateRange(new[] { "abc", "cbd0", "sdsd4", "4dfg", "adafd0xd" }));
        [Benchmark] public void Immutable_Hash_Set() => SerializeAndDeserialize(ImmutableHashSet.CreateRange(new[] { "abc", "cbd0", "sdsd4", "4dfg", "adafd0xd" }));
        [Benchmark] public void Immutable_Sorted_Set() => SerializeAndDeserialize(ImmutableSortedSet.CreateRange(new[] { "abc", "cbd0", "sdsd4", "4dfg", "adafd0xd" }));
        [Benchmark] public void Immutable_Dictionary() => SerializeAndDeserialize(ImmutableDictionary.CreateRange(new[]
        {
            new KeyValuePair<string, string>("key1", "value1"),
            new KeyValuePair<string, string>("key2", "value2"),
            new KeyValuePair<string, string>("key3", "value3"),
            new KeyValuePair<string, string>("key4", "value4"),
            new KeyValuePair<string, string>("key5", "value5"),
        }));
    }
}
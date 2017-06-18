#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializeCollectionsBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Hyperion.Benchmarks
{
    public class SerializeCollectionsBenchmark : HyperionBenchmark
    {
        [Benchmark] public void Serialize_ByteArray() => Serialize(new byte[] { 123, 134, 11, 122, 1 });
        [Benchmark] public void Serialize_StringArray() => Serialize(new[] { "abc", "cbd0", "sdsd4", "4dfg", "sfsdf44g" });
        [Benchmark] public void Serialize_Dictionary() => Serialize(new Dictionary<string, string>
            {
                ["abc"] = "aaa",
                ["dsdf"] = "asdab",
                ["fms0"] = "sdftu"
            });
        [Benchmark] public void Serialize_ArrayList() => Serialize(new List<string> { "asdad", "asdabs3", "sfsdf44g", "asdf4r", "sfsdf44g" });
        [Benchmark] public void Serialize_LinkedList() => Serialize(new LinkedList<string>(new[] { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" }));
        [Benchmark] public void Serialize_HashSet() => Serialize(new HashSet<string>(new[] { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" }));
        [Benchmark] public void Serialize_SortedSet() => Serialize(new SortedSet<string>(new[] { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" }));
    }
}
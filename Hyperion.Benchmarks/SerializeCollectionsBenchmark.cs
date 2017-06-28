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
        [Benchmark] public void Byte_Array() => SerializeAndDeserialize(new byte[] { 123, 134, 11, 122, 1 });
        [Benchmark] public void String_Array() => SerializeAndDeserialize(new[] { "abc", "cbd0", "sdsd4", "4dfg", "sfsdf44g" });
        [Benchmark] public void Dictionary() => SerializeAndDeserialize(new Dictionary<string, string>
            {
                ["abc"] = "aaa",
                ["dsdf"] = "asdab",
                ["fms0"] = "sdftu"
            });
        [Benchmark] public void Array_List() => SerializeAndDeserialize(new List<string> { "asdad", "asdabs3", "sfsdf44g", "asdf4r", "sfsdf44g" });
        [Benchmark] public void Linked_List() => SerializeAndDeserialize(new LinkedList<string>(new[] { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" }));
        [Benchmark] public void Hash_Set() => SerializeAndDeserialize(new HashSet<string>(new[] { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" }));
        [Benchmark] public void Sorted_Set() => SerializeAndDeserialize(new SortedSet<string>(new[] { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" }));
    }
}
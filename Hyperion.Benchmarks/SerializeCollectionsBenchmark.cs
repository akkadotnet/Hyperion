#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializeCollectionsBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using System.IO;
using BenchmarkDotNet.Attributes;

namespace Hyperion.Benchmarks
{
    [Config(typeof(HyperionConfig))]
    public class SerializeCollectionsBenchmark
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

        [Benchmark] public void Serialize_ByteArray() => serializer.Serialize(new byte[] { 123, 134, 11, 122, 1 }, stream);
        [Benchmark] public void Serialize_StringArray() => serializer.Serialize(new[] { "abc", "cbd0", "sdsd4", "4dfg", "sfsdf44g" }, stream);
        [Benchmark] public void Serialize_Dictionary() => serializer.Serialize(new Dictionary<string, string>
            {
                ["abc"] = "aaa",
                ["dsdf"] = "asdab",
                ["fms0"] = "sdftu"
            }, stream);
        [Benchmark] public void Serialize_ArrayList() => serializer.Serialize(new List<string> { "asdad", "asdabs3", "sfsdf44g", "asdf4r", "sfsdf44g" }, stream);
        [Benchmark] public void Serialize_LinkedList() => serializer.Serialize(new LinkedList<string>(new[] { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" }), stream);
        [Benchmark] public void Serialize_HashSet() => serializer.Serialize(new HashSet<string>(new[] { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" }), stream);
        [Benchmark] public void Serialize_SortedSet() => serializer.Serialize(new SortedSet<string>(new[] { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" }), stream);
    }
}
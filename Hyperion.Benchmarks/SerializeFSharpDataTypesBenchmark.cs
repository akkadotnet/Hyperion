#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializeFSharpDataTypesBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.IO;
using BenchmarkDotNet.Attributes;
using Hyperion.FSharpTestTypes;
using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;

namespace Hyperion.Benchmarks
{
    [Config(typeof(HyperionConfig))]
    public class SerializeFSharpDataTypesBenchmark
    {
        #region init
        private Serializer serializer;
        private MemoryStream stream;

        private FSharpSet<long> set;
        private FSharpList<long> list;
        private TestRecord record;
        private DU2 du;

        [Setup]
        public void Setup()
        {
            serializer = new Serializer();
            stream = new MemoryStream();
            list = ListModule.OfArray(new[] {123, 2342355, 456456467578, 234234, -234281});
            set = SetModule.OfArray(new[] {123, 2342355, 456456467578, 234234, -234281});
            record = new TestRecord(
                name: "John Doe",
                aref: FSharpOption<string>.Some("ok"),
                connections: "test");
            du = DU2.NewC(DU1.NewB("test", 123));
        }

        [Cleanup]
        public void Cleanup()
        {
            stream.Dispose();
        }
        #endregion

        [Benchmark] public void Serialize_DiscriminatedUnion() => serializer.Serialize(du, stream);
        [Benchmark] public void Serialize_Record() => serializer.Serialize(record, stream);
        [Benchmark] public void Serialize_RecordWithMap() => serializer.Serialize(TestMap.createRecordWithMap, stream);
        [Benchmark] public void Serialize_FSharpList() => serializer.Serialize(list, stream);
        [Benchmark] public void Serialize_FSharpSet() => serializer.Serialize(set, stream);
    }
}
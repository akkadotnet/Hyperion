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
    public class SerializeFSharpDataTypesBenchmark : HyperionBenchmark
    {
        #region init
        
        private FSharpSet<long> set;
        private FSharpList<long> list;
        private TestRecord record;
        private DU2 du;
        private SDU1 sdu;

        protected override void Init()
        {
            base.Init();

            list = ListModule.OfArray(new[] {123, 2342355, 456456467578, 234234, -234281});
            set = SetModule.OfArray(new[] {123, 2342355, 456456467578, 234234, -234281});
            record = new TestRecord(
                name: "John Doe",
                aref: FSharpOption<string>.Some("ok"),
                connections: "test");
            du = DU2.NewC(DU1.NewB("test", 123));
            sdu = SDU1.NewB("hello", 123);
        }
        
        #endregion

        [Benchmark] public void Serialize_DiscriminatedUnion() => Serialize(du);
        [Benchmark] public void Serialize_Struct_DiscriminatedUnion() => Serialize(sdu);
        [Benchmark] public void Serialize_Record() => Serialize(record);
        [Benchmark] public void Serialize_RecordWithMap() => Serialize(TestMap.createRecordWithMap);
        [Benchmark] public void Serialize_FSharpList() => Serialize(list);
        [Benchmark] public void Serialize_FSharpSet() => Serialize(set);
    }
}
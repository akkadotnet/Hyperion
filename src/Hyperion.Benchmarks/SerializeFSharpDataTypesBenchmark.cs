#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializeFSharpDataTypesBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using BenchmarkDotNet.Attributes;
#if FSHARP

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

        [Benchmark] public void Discriminated_Union() => SerializeAndDeserialize(du);
        [Benchmark] public void Struct_Discriminated_Union() => SerializeAndDeserialize(sdu);
        [Benchmark] public void Record() => SerializeAndDeserialize(record);
        [Benchmark] public void Record_With_Map() => SerializeAndDeserialize(TestMap.createRecordWithMap);
        [Benchmark] public void FSharp_List() => SerializeAndDeserialize(list);
        [Benchmark] public void FSharp_Set() => SerializeAndDeserialize(set);
    }
}

#endif
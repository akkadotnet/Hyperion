#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializeComplexObjectsBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using Hyperion.Tests.Performance.Types;
using NBench;
namespace Hyperion.Tests.Performance.Serialization
{
    public class SerializeComplexObjectsBenchmark : PerfTestBase
    {
        private LargeStruct _testStruct;
        private TypicalPersonData _testObject;
        private CyclicA _cyclic;

        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            _testStruct = LargeStruct.Create();
            _testObject = TypicalPersonData.MakeRandom();

            var a = new CyclicA();
            var b = new CyclicB();
            a.B = b;
            b.A = a;

            _cyclic = a;
        }

        [PerfBenchmark(
            Description = "Benchmark struct serialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 1300000)]
        public void Serialize_Struct()
        {
            SerializeAndCount(_testStruct);
        }
        
        [PerfBenchmark(
            Description = "Benchmark big object serialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 160000)]
        public void Serialize_LargeObject()
        {
            SerializeAndCount(_testObject);
        }

        //TODO: PerfBenchmark.Skip doesn't work
        [PerfBenchmark(
            Description = "Benchmark cyclic reference serialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test,
            Skip = "FIXME: stack overflow")]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 230000)]
        public void Serialize_CyclicReferences()
        {
            SerializeAndCount(_cyclic);
        }
    }
}
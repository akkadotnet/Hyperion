#region copyright
// -----------------------------------------------------------------------
//  <copyright file="SerializeCollectionsBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using NBench;

namespace Hyperion.Tests.Performance.Serialization
{
    public class SerializeCollectionsBenchmark : PerfTestBase
    {
        [PerfBenchmark(
            Description = "Benchmark byte array serialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 4800000)]
        public void Serialize_ByteArray()
        {
            SerializeAndCount(new byte[] { 123, 134, 11, 122, 1 });
        }

        [PerfBenchmark(
            Description = "Benchmark string array serialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 300000)]
        public void Serialize_StringArray()
        {
            SerializeAndCount(new string[] { "abc", "cbd0", "sdsd4", "4dfg", "sfsdf44g" });
        }

        [PerfBenchmark(
            Description = "Benchmark dictionary serialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 360000)]
        public void Serialize_Dictionary()
        {
            var dictionary = new Dictionary<string, string>
            {
                ["abc"] = "aaa",
                ["dsdf"] = "asdab",
                ["fms0"] = "sdftu"
            };
            SerializeAndCount(dictionary);
        }

        [PerfBenchmark(
            Description = "Benchmark list serialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 550000)]
        public void Serialize_List()
        {
            SerializeAndCount(new List<string> { "asdad", "asdabs3", "sfsdf44g", "asdf4r", "sfsdf44g" });
        }

        [PerfBenchmark(
            Description = "Benchmark linked list serialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 500000)]
        public void Serialize_LinkedList()
        {
            var list = new LinkedList<string>(new[] { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" });
            SerializeAndCount(list);
        }

        [PerfBenchmark(
            Description = "Benchmark hash set serialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 170000)]
        public void Serialize_HashSet()
        {
            var set = new HashSet<string> { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" };
            SerializeAndCount(set);
        }

        [PerfBenchmark(
            Description = "Benchmark sorted set serialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 110000)]
        public void Serialize_SortedSet()
        {
            var set = new SortedSet<string> { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" };
            SerializeAndCount(set);
        }
    }
}
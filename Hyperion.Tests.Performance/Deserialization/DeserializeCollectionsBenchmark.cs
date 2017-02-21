#region copyright
// -----------------------------------------------------------------------
//  <copyright file="DeserializeCollectionsBenchmark.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using NBench;

namespace Hyperion.Tests.Performance.Deserialization
{
    public class ByteArrayDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(new byte[] { 123, 134, 11, 122, 1 });
        }

        [PerfBenchmark(
            Description = "Benchmark byte array deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 4500000)]
        public void Deserialize_ByteArray()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<byte[]>();
        }
    }

    public sealed class StringArrayDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(new string[] { "abc", "cbd0", "sdsd4", "4dfg", "sfsdf44g" });
        }

        [PerfBenchmark(
            Description = "Benchmark string array deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 650000)]
        public void Deserialize_StringArray()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<string[]>();
        }
    }

    public sealed class DictionaryDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(new Dictionary<string, string>
            {
                ["abc"] = "aaa",
                ["dsdf"] = "asdab",
                ["fms0"] = "sdftu"
            });
        }

        [PerfBenchmark(
            Description = "Benchmark dictionary deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 130000)]
        public void Deserialize_Dictionary()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<Dictionary<string, string>>();
        }
    }

    public sealed class ListDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(new List<string> { "asdad", "asdabs3", "sfsdf44g", "asdf4r", "sfsdf44g" });
        }

        [PerfBenchmark(
            Description = "Benchmark list deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 250000)]
        public void Deserialize_StringList()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<List<string>>();
        }
    }

    public sealed class LinkedListDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(new LinkedList<string>(new[] { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" }));
        }

        [PerfBenchmark(
            Description = "Benchmark linked list deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 150000)]
        public void Deserialize_LinkedList()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<LinkedList<string>>();
        }
    }

    public sealed class HashSetDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(new HashSet<string> { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" });
        }

        [PerfBenchmark(
            Description = "Benchmark hash set deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 50000)]
        public void Deserialize_HashSet()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<HashSet<string>>();
        }
    }

    public sealed class SortedSetDeserializationBenchmark : PerfTestBase
    {
        public override void Setup(BenchmarkContext context)
        {
            base.Setup(context);
            InitStreamWith(new SortedSet<string> { "asdad", "asdabs3", "dfsdf9", "asdf4r", "sfsdf44g" });
        }

        [PerfBenchmark(
            Description = "Benchmark sorted set deserialization",
            NumberOfIterations = StandardIterationCount,
            RunMode = RunMode.Throughput,
            RunTimeMilliseconds = StandardRunTime,
            TestMode = TestMode.Test)]
        [CounterThroughputAssertion(TestCounterName, MustBe.GreaterThan, 45000)]
        public void Deserialize_SortedSet()
        {
            Stream.Position = 0; // don't move it up to Setup, I don't know why it needed here to work
            DeserializeAndCount<SortedSet<string>>();
        }
    }
}
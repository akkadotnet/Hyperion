#region copyright
// -----------------------------------------------------------------------
//  <copyright file="Prolog.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.IO;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;

namespace Hyperion.Benchmarks
{
    public class HyperionConfig : ManualConfig
    {
        public HyperionConfig()
        {
            AddColumn(StatisticColumn.Mean, StatisticColumn.Min, StatisticColumn.Max, StatisticColumn.OperationsPerSecond);
            AddExporter(MarkdownExporter.GitHub);
            AddDiagnoser(MemoryDiagnoser.Default);
        }
    }

    [Config(typeof(HyperionConfig))]
    public abstract class HyperionBenchmark
    {
        #region init

        protected Serializer Serializer;
        protected MemoryStream Stream;
        
        [GlobalSetup]
        public void Setup()
        {
            Stream = new MemoryStream();
            Init();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Stream.Dispose();
            Clean();
        }

        protected virtual void Init()
        {
            Serializer = new Serializer();
        }

        protected virtual void Clean() { }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Serialize<T>(T elem)
        {
            Serializer.Serialize(elem, Stream);
            Stream.Position = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Deserialize<T>()
        {
            Serializer.Deserialize<T>(Stream);
            Stream.Position = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SerializeAndDeserialize<T>(T elem)
        {
            Serialize(elem);
            Deserialize<T>();
        }
    }
}
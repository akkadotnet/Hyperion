﻿#region copyright
// -----------------------------------------------------------------------
//  <copyright file="PerfTestBase.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using NBench;
using NBench.Sdk;

namespace Hyperion.Tests.Performance
{
    public abstract class PerfTestBase
    {
        public const string TestCounterName = "CallCounter";

        /// <summary>
        /// 100 milliseconds.
        /// </summary>
        public const int StandardRunTime = 100;

        /// <summary>
        /// 3 iterations.
        /// </summary>
        public const int StandardIterationCount = 3;

        protected MemoryStream Stream;
        protected Hyperion.Serializer Serializer;
        protected Counter TestCounter;
        private readonly SerializerOptions options;

        protected PerfTestBase(SerializerOptions options = null)
        {
            this.options = options;
            new TestRunner(null).SetProcessPriority(concurrent: false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void SerializeAndCount<T>(T value)
        {
            Serializer.Serialize(value, Stream);
            TestCounter.Increment();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void DeserializeAndCount<T>()
        {
            Serializer.Deserialize<T>(Stream);
            TestCounter.Increment();
        }

        protected void InitStreamWith<T>(T value)
        {
            Serializer.Serialize(value, Stream);
            Stream.Position = 0;
        }

        [PerfSetup]
        public virtual void Setup(BenchmarkContext context)
        {
            Serializer = options == null ? new Serializer() : new Serializer(options);
            Stream = new MemoryStream();
            TestCounter = context.GetCounter(TestCounterName);
        }

        [PerfCleanup]
        public virtual void Cleanup()
        {
            Stream?.Dispose();
        }
    }
}
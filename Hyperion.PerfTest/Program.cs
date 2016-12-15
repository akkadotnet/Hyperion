#region copyright
// -----------------------------------------------------------------------
//  <copyright file="Program.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Threading;
using Hyperion.PerfTest.Tests;

namespace Hyperion.PerfTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var t = new Thread(Run);
            t.Priority = ThreadPriority.Highest;
            t.IsBackground = true;

            t.Start();
            Console.ReadLine();
        }

        private static void Run()
        {
            var largeStructTest = new LargeStructTest();
            largeStructTest.Run(1000000);

            var guidArrayTest = new GuidArrayTest();
            guidArrayTest.Run(30000);

            var guidTest = new GuidTest();
            guidTest.Run(1000000);
            var typicalPersonArrayTest = new TypicalPersonArrayTest();
            typicalPersonArrayTest.Run(1000);

            var typicalPersonTest = new TypicalPersonTest();
            typicalPersonTest.Run(100000);

            var typicalMessageArrayTest = new TypicalMessageArrayTest();
            typicalMessageArrayTest.Run(10000);

            var typicalMessageTest = new TypicalMessageTest();
            typicalMessageTest.Run(1000000);

            Console.ReadLine();
        }
    }
}
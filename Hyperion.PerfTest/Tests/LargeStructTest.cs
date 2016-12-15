#region copyright
// -----------------------------------------------------------------------
//  <copyright file="LargeStructTest.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using Hyperion.PerfTest.Types;

namespace Hyperion.PerfTest.Tests
{
    internal class LargeStructTest : TestBase<LargeStruct>
    {
        protected override LargeStruct GetValue()
        {
            return LargeStruct.Create();
        }
    }
}
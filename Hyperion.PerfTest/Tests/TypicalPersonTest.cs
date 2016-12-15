#region copyright
// -----------------------------------------------------------------------
//  <copyright file="TypicalPersonTest.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using Hyperion.PerfTest.Types;

namespace Hyperion.PerfTest.Tests
{
    class TypicalPersonTest : TestBase<TypicalPersonData>
    {
        protected override TypicalPersonData GetValue()
        {
            return TypicalPersonData.MakeRandom();
        }
    }
}

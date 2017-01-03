#region copyright
// -----------------------------------------------------------------------
//  <copyright file="TypicalPersonArrayTest.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.Collections.Generic;
using Hyperion.PerfTest.Types;

namespace Hyperion.PerfTest.Tests
{
    class TypicalPersonArrayTest : TestBase<TypicalPersonData[]>
    {
        protected override TypicalPersonData[] GetValue()
        {
            var l = new List<TypicalPersonData>();
            for (int i = 0; i < 100; i++)
            {
                l.Add(TypicalPersonData.MakeRandom());
            }
            return l.ToArray();
        }
    }
}

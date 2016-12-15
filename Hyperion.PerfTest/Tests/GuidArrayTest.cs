#region copyright
// -----------------------------------------------------------------------
//  <copyright file="GuidArrayTest.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;

namespace Hyperion.PerfTest.Tests
{
    class GuidArrayTest : TestBase<Guid[]>
    {
        protected override Guid[] GetValue()
        {
            var l = new List<Guid>();
            for (int i = 0; i < 100; i++)
            {
                l.Add(Guid.NewGuid());
            }
            return l.ToArray();
        }
    }
}

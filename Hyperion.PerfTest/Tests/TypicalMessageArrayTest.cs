#region copyright
// -----------------------------------------------------------------------
//  <copyright file="TypicalMessageArrayTest.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Hyperion.PerfTest.Types;

namespace Hyperion.PerfTest.Tests
{
    class TypicalMessageArrayTest : TestBase<TypicalMessage[]>
    {
        protected override TypicalMessage[] GetValue()
        {
            var l = new List<TypicalMessage>();

            for (var i = 0; i < 100; i++)
            {
                var v = new TypicalMessage()
                {
                    StringProp = "hello",
                    GuidProp = Guid.NewGuid(),
                    IntProp = 123,
                    DateProp = DateTime.UtcNow
                };
                l.Add(v);
            }

            return l.ToArray();
        }
    }
}

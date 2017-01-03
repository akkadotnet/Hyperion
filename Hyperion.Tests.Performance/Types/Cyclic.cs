#region copyright
// -----------------------------------------------------------------------
//  <copyright file="Cyclic.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;

namespace Hyperion.Tests.Performance.Types
{
    [Serializable]
    public class CyclicA
    {
        public CyclicB B { get; set; }
    }

    [Serializable]
    public class CyclicB
    {
        public CyclicA A { get; set; }
    }
}
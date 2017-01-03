#region copyright
// -----------------------------------------------------------------------
//  <copyright file="TestMessage.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;

namespace Hyperion.Tests.Performance.Types
{
    public class TestMessage
    {
        public virtual string StringProp { get; set; }
        
        public virtual int IntProp { get; set; }
        
        public virtual Guid GuidProp { get; set; }
        
        public virtual DateTime DateProp { get; set; }
    }
}
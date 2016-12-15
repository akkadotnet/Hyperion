#region copyright
// -----------------------------------------------------------------------
//  <copyright file="TypicalMessage.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using ProtoBuf;
using ZeroFormatter;

namespace Hyperion.PerfTest.Types
{
    [ProtoContract]
    [Serializable]
    [ZeroFormattable]
    public class TypicalMessage
    {
        [ProtoMember(1)]
        [Index(0)]
        public virtual string StringProp { get; set; }

        [ProtoMember(2)]
        [Index(1)]
        public virtual int IntProp { get; set; }

        [ProtoMember(3)]
        [Index(2)]
        public virtual Guid GuidProp { get; set; }

        [ProtoMember(4)]
        [Index(3)]
        public virtual DateTime DateProp { get; set; }
    }
}

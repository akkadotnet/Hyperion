#region copyright
// -----------------------------------------------------------------------
//  <copyright file="PrimitivesTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using Xunit;

namespace Hyperion.Tests
{
    
    public class PrimitivesTest : TestBase
    {
        [Fact]
        public void CanSerializeTuple1()
        {
            SerializeAndAssert(Tuple.Create("abc"));
        }

        [Fact]
        public void CanSerializeTuple2()
        {
            SerializeAndAssert(Tuple.Create(1, 123));
        }

        [Fact]
        public void CanSerializeTuple3()
        {
            SerializeAndAssert(Tuple.Create(1, 2, 3));
        }

        [Fact]
        public void CanSerializeTuple4()
        {
            SerializeAndAssert(Tuple.Create(1, 2, 3, 4));
        }

        [Fact]
        public void CanSerializeTuple5()
        {
            SerializeAndAssert(Tuple.Create(1, 2, 3, 4, 5));
        }

        [Fact]
        public void CanSerializeTuple6()
        {
            SerializeAndAssert(Tuple.Create(1, 2, 3, 4, 5, 6));
        }

        [Fact]
        public void CanSerializeTuple7()
        {
            SerializeAndAssert(Tuple.Create(1, 2, 3, 4, 5, 6, 7));
        }

        [Fact]
        public void CanSerializeTuple8()
        {
            SerializeAndAssert(Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8));
        }

        [Fact]
        public void CanSerializeBool()
        {
            SerializeAndAssert(true);
        }

        [Fact]
        public void CanSerializeGuid()
        {
            SerializeAndAssert(Guid.NewGuid());
        }

        [Fact]
        public void CanSerializeDateTimeUtc()
        {
            SerializeAndAssert(DateTime.UtcNow);
        }

        [Fact]
        public void CanSerializeDateTimeLocal()
        {
            SerializeAndAssert(new DateTime(DateTime.Now.Ticks, DateTimeKind.Local));
        }

        [Fact]
        public void CanSerializeDateTimeOffsetNow()
        {
            SerializeAndAssert(DateTimeOffset.Now);
        }

        [Fact]
        public void CanSerializeDateTimeOffsetUtc()
        {
            SerializeAndAssert(DateTimeOffset.UtcNow);
        }

        [Fact]
        public void CanSerializeDecimal()
        {
            SerializeAndAssert(123m);
        }

        [Fact]
        public void CanSerializeDouble()
        {
            SerializeAndAssert(123d);
        }


        [Fact]
        public void CanSerializeByte()
        {
            SerializeAndAssert((byte) 123);
        }
        [Fact]
        public void CanSerializeSByte()
        {
            SerializeAndAssert((sbyte)123);
        }

        [Fact]
        public void CanSerializeInt16()
        {
            SerializeAndAssert((short) 123);
        }

        [Fact]
        public void CanSerializeInt64()
        {
            SerializeAndAssert(123L);
        }

        [Fact]
        public void CanSerializeInt32()
        {
            SerializeAndAssert(123);
        }

        [Fact]
        public void CanSerializeUInt16()
        {
            SerializeAndAssert((ushort)123);
        }

        [Fact]
        public void CanSerializeUInt64()
        {
            SerializeAndAssert((ulong)123);
        }

        [Fact]
        public void CanSerializeUInt32()
        {
            SerializeAndAssert((uint)123);
        }

        [Fact]
        public void CanSerializeLongString()
        {
            var s = new string('x',1000);
            SerializeAndAssert(s);
        }

        [Fact]
        public void CanSerializeString()
        {
            SerializeAndAssert("hello");
        }

        private void SerializeAndAssert(object expected)
        {
            Serialize(expected);
            Reset();
            var res = Deserialize<object>();
            Assert.Equal(expected, res);
            AssertMemoryStreamConsumed();
        }
    }
}
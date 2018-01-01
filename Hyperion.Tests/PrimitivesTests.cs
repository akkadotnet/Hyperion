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
        public void Serializer_should_work_with_Tuple1()
        {
            SerializeAndAssert(Tuple.Create("abc"));
        }

        [Fact]
        public void Serializer_should_work_with_Tuple2()
        {
            SerializeAndAssert(Tuple.Create(1, 123));
        }

        [Fact]
        public void Serializer_should_work_with_Tuple3()
        {
            SerializeAndAssert(Tuple.Create(1, 2, 3));
        }

        [Fact]
        public void Serializer_should_work_with_Tuple4()
        {
            SerializeAndAssert(Tuple.Create(1, 2, 3, 4));
        }

        [Fact]
        public void Serializer_should_work_with_Tuple5()
        {
            SerializeAndAssert(Tuple.Create(1, 2, 3, 4, 5));
        }

        [Fact]
        public void Serializer_should_work_with_Tuple6()
        {
            SerializeAndAssert(Tuple.Create(1, 2, 3, 4, 5, 6));
        }

        [Fact]
        public void Serializer_should_work_with_Tuple7()
        {
            SerializeAndAssert(Tuple.Create(1, 2, 3, 4, 5, 6, 7));
        }

        [Fact]
        public void Serializer_should_work_with_Tuple8()
        {
            SerializeAndAssert(Tuple.Create(1, 2, 3, 4, 5, 6, 7, 8));
        }

        [Fact]
        public void Serializer_should_work_with_Bool()
        {
            SerializeAndAssert(true);
        }

        [Fact]
        public void Serializer_should_work_with_Guid()
        {
            SerializeAndAssert(Guid.NewGuid());
        }

        [Fact]
        public void Serializer_should_work_with_DateTime_UTC()
        {
            SerializeAndAssert(DateTime.UtcNow);
        }

        [Fact]
        public void Serializer_should_work_with_DateTime_local()
        {
            SerializeAndAssert(new DateTime(DateTime.Now.Ticks, DateTimeKind.Local));
        }

        [Fact]
        public void Serializer_should_work_with_DateTimeOffset_now()
        {
            SerializeAndAssert(DateTimeOffset.Now);
        }

        [Fact]
        public void Serializer_should_work_with_DateTimeOffset_utc()
        {
            SerializeAndAssert(DateTimeOffset.UtcNow);
        }

        [Fact]
        public void Serializer_should_work_with_Decimal()
        {
            SerializeAndAssert(123m);
        }

        [Fact]
        public void Serializer_should_work_with_Double()
        {
            SerializeAndAssert(123d);
        }


        [Fact]
        public void Serializer_should_work_with_Byte()
        {
            SerializeAndAssert((byte) 123);
        }
        [Fact]
        public void Serializer_should_work_with_SByte()
        {
            SerializeAndAssert((sbyte)123);
        }

        [Fact]
        public void Serializer_should_work_with_Int16()
        {
            SerializeAndAssert((short) 123);
        }

        [Fact]
        public void Serializer_should_work_with_Int64()
        {
            SerializeAndAssert(123L);
        }

        [Fact]
        public void Serializer_should_work_with_Int32()
        {
            SerializeAndAssert(123);
        }

        [Fact]
        public void Serializer_should_work_with_UInt16()
        {
            SerializeAndAssert((ushort)123);
        }

        [Fact]
        public void Serializer_should_work_with_UInt64()
        {
            SerializeAndAssert((ulong)123);
        }

        [Fact]
        public void Serializer_should_work_with_UInt32()
        {
            SerializeAndAssert((uint)123);
        }

        [Fact]
        public void Serializer_should_work_with_long_String()
        {
            var s = new string('x', 100000);
            SerializeAndAssert(s);
        }

        [Fact]
        public void Serializer_should_work_with_String()
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
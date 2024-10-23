#region copyright
// -----------------------------------------------------------------------
//  <copyright file="PrimitivesTests.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Xunit;

namespace Hyperion.Tests
{
    
    public class PrimitivesTest : TestBase
    {
        public PrimitivesTest()
        {
        }

        protected PrimitivesTest(Func<Stream, Stream> streamFacade)
            : base(streamFacade)
        {
        }

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

        [Theory]
        [MemberData(nameof(DateTimeData))]
        public void CanSerializeDateTime(DateTime expected)
        {
            SerializeAndAssert(expected);
        }

        public static IEnumerable<object[]> DateTimeData()
        {
            yield return new object[] { DateTime.UtcNow };
            yield return new object[] { DateTime.Now };
            yield return new object[] { DateTime.MinValue };
            yield return new object[] { DateTime.MaxValue };
            var now = DateTime.Now;
            yield return new object[] { DateTime.SpecifyKind(now, DateTimeKind.Unspecified) };
            yield return new object[] { DateTime.SpecifyKind(now, DateTimeKind.Local) };
            yield return new object[] { DateTime.SpecifyKind(now, DateTimeKind.Utc) };
        }

        [Theory]
        [MemberData(nameof(DateTimeOffsetData))]
        public void CanSerializeDateTimeOffset(DateTimeOffset expected)
        {
            SerializeAndAssert(expected);
        }

        public static IEnumerable<object[]> DateTimeOffsetData()
        {
            yield return new object[] { DateTimeOffset.UtcNow };
            yield return new object[] { DateTimeOffset.Now };
            yield return new object[] { DateTimeOffset.MinValue };
            yield return new object[] { DateTimeOffset.MaxValue };
            yield return new object[] { new DateTimeOffset(DateTime.MinValue, TimeSpan.Zero) };
            yield return new object[] { new DateTimeOffset(DateTime.MaxValue, TimeSpan.Zero) };
            
            var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            var increment = TimeSpan.FromMinutes(10);
            var offset = TimeSpan.FromHours(-14) - increment;
            
            // Over the whole possible timezone offsets (-14 to 14), incremented by 10 minutes
            while (offset.TotalHours < 14.0)
            {
                offset += increment;
                yield return new object[] { new DateTimeOffset(now, offset) };
            }
            
            increment = TimeSpan.FromMinutes(1);
            offset = TimeSpan.FromHours(-1);
            
            // Over the span of -1 to 1 hours, incremented by 1 minute
            while (offset.TotalHours < 1.0)
            {
                offset += increment;
                yield return new object[] { new DateTimeOffset(now, offset) };
            }
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

        private void SerializeAndAssert<T>(T expected)
        {
            Serialize(expected);
            Reset();
            var res = Deserialize<T>();
            expected.Should().Be(res);
            AssertMemoryStreamConsumed();
        }
    }
}
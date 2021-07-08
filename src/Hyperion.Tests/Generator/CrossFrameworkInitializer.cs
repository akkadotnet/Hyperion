using System;
using System.Collections.Generic;
using System.Drawing;

namespace Hyperion.Tests.Generator
{
    public static class CrossFrameworkInitializer
    {
        public const string DefaultOutputPath = "../../../testfiles";

        public static CrossFrameworkMixedClass InitMixed()
        {
            return new CrossFrameworkMixedClass
            {
                Name = "Cookie",
                Sound = "Bark",
                FriendType = typeof(CrossFrameworkClass),
                Color = Color.Blue,
                Point = new Point(10, 10),
                PointF = new PointF(10, 10),
                Rectangle = new Rectangle(10, 10, 10, 10),
                RectangleF = new RectangleF(10, 10, 10, 10),
                Size = new Size(10, 10),
                SizeF = new SizeF(10, 10),
                Data = Init()
            };
        }

        public static CrossFrameworkClass Init()
        {
            return new CrossFrameworkClass()
            {
                Exception = new Exception("Test message", new ArgumentNullException("param", "Cannot be null")),
                DateTime = new DateTime(1944, 6, 6), // DDay
                Enum = CrossFrameworkEnum.Yatagan,
                String = "On June 6, 1944, more than 160,000 Allied troops landed along a 50-mile stretch of heavily-fortified French coastline",
                Struct = new CrossFrameworkStruct()
                {
                    Boolean = true,
                    Long = long.MaxValue,
                    Decimal = decimal.MinusOne,
                    Double = double.MaxValue,
                    Int = int.MaxValue,
                    Short = short.MaxValue,
                    ULong = ulong.MinValue,
                    Byte = byte.MaxValue,
                    Char = char.MaxValue,
                    Float = float.MinValue,
                    UShort = ushort.MinValue,
                    UInt = uint.MaxValue,
                    Sbyte = sbyte.MaxValue
                },
                Decimal = decimal.MaxValue,
                Float = float.MaxValue,
                Long = long.MinValue,
                Int = int.MinValue,
                Double = double.Epsilon,
                Char = char.MaxValue,
                Byte = byte.MaxValue,
                Sbyte = sbyte.MaxValue,
                Short = short.MaxValue,
                UInt = uint.MaxValue,
                ULong = ulong.MaxValue,
                UShort = ushort.MaxValue,
                Boolean = true
            };
        }
    }
}
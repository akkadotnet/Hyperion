using System;
using System.Collections.Generic;

namespace Hyperion.Tests.Generator
{
    public class CrossFrameworkClass
    {
        public sbyte Sbyte { get; set; }

        public short Short { get; set; }

        public int Int { get; set; }

        public long Long { get; set; }

        public byte Byte { get; set; }

        public ushort UShort { get; set; }

        public uint UInt { get; set; }

        public ulong ULong { get; set; }

        public char Char { get; set; }

        public float Float { get; set; }

        public double Double { get; set; }

        public decimal Decimal { get; set; }

        public bool Boolean { get; set; }

        public string String { get; set; }

        public DateTime DateTime { get; set; }

        public Exception Exception { get; set; }

        public CrossFrameworkEnum Enum { get; set; }

        public CrossFrameworkStruct Struct { get; set; }

        public Type Type { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is CrossFrameworkClass))
            {
                return false;
            }

            var objectToCompare = (CrossFrameworkClass) obj;


            //return Sbyte == objectToCompare.Sbyte
            //       && Short == objectToCompare.Short
            //       && Int == objectToCompare.Int
            //       && Long == objectToCompare.Long
            //       && Byte == objectToCompare.Byte
            //       && UShort == objectToCompare.UShort
            //       && UInt == objectToCompare.UInt
            //       && ULong == objectToCompare.ULong
            //       && Char == objectToCompare.Char
            //       && Math.Abs(Float - objectToCompare.Float) < float.Epsilon
            //       && Math.Abs(Double - objectToCompare.Double) < double.Epsilon
            //       && Decimal == objectToCompare.Decimal
            //       && Boolean == objectToCompare.Boolean
            //       && String == objectToCompare.String
            //       && DateTime == objectToCompare.DateTime
            //       && Exception == objectToCompare.Exception
            //       && Enum == objectToCompare.Enum
            //       && Struct.Equals(objectToCompare.Struct);

            return Equals(objectToCompare);

        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Sbyte.GetHashCode();
                hashCode = (hashCode * 397) ^ Short.GetHashCode();
                hashCode = (hashCode * 397) ^ Int;
                hashCode = (hashCode * 397) ^ Long.GetHashCode();
                hashCode = (hashCode * 397) ^ Byte.GetHashCode();
                hashCode = (hashCode * 397) ^ UShort.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) UInt;
                hashCode = (hashCode * 397) ^ ULong.GetHashCode();
                hashCode = (hashCode * 397) ^ Char.GetHashCode();
                hashCode = (hashCode * 397) ^ Float.GetHashCode();
                hashCode = (hashCode * 397) ^ Double.GetHashCode();
                hashCode = (hashCode * 397) ^ Decimal.GetHashCode();
                hashCode = (hashCode * 397) ^ Boolean.GetHashCode();
                hashCode = (hashCode * 397) ^ (String != null ? String.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ DateTime.GetHashCode();
                hashCode = (hashCode * 397) ^ (Exception != null ? Exception.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) Enum;
                hashCode = (hashCode * 397) ^ Struct.GetHashCode();
                return hashCode;
            }
        }

        private bool Equals(CrossFrameworkClass other)
        {
            return Sbyte == other.Sbyte
                   && Short == other.Short
                   && Int == other.Int
                   && Long == other.Long
                   && Byte == other.Byte
                   && UShort == other.UShort
                   && UInt == other.UInt
                   && ULong == other.ULong
                   && Char == other.Char
                   && Math.Abs(Float - other.Float) < float.Epsilon
                   && Math.Abs(Double - other.Double) < double.Epsilon
                   && Decimal == other.Decimal
                   && Boolean == other.Boolean
                   && string.Equals(String, other.String)
                   && DateTime.Equals(other.DateTime)
                   // && Equals(Exception, other.Exception) 
                   && Exception.Message == other.Exception.Message 
                   && Enum == other.Enum
                   && Struct.Equals(other.Struct);
        }
    }
}
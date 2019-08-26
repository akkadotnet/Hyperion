using System;

namespace Hyperion.Tests.Generator
{
    public struct CrossFrameworkStruct
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

        public override bool Equals(object obj)
        {
            if (!(obj is CrossFrameworkStruct))
            {
                return false;
            }

            var objectToCompare = (CrossFrameworkStruct)obj;

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
                return hashCode;
            }
        }

        private bool Equals(CrossFrameworkStruct other)
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
                   && Boolean == other.Boolean;
        }
    }
}
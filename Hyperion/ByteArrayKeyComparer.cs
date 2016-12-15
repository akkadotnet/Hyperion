#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ByteArrayKeyComparer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.Collections.Generic;

namespace Hyperion
{
    /// <summary>
    /// By default ByteArrayKey overrides "public bool Equals(object obj)" to do comparisons.
    /// But this causes boxing/allocations, so by having a custom comparer we can prevent that.
    /// </summary>
    public class ByteArrayKeyComparer : IEqualityComparer<ByteArrayKey>
    {
        public static readonly ByteArrayKeyComparer Instance = new ByteArrayKeyComparer();

        public bool Equals(ByteArrayKey x, ByteArrayKey y)
        {
            return ByteArrayKey.Compare(x.Bytes, y.Bytes);
        }

        public int GetHashCode(ByteArrayKey obj)
        {
            return obj.GetHashCode();
        }
    }
}

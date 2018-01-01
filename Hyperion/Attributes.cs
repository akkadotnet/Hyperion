#region copyright
// -----------------------------------------------------------------------
//  <copyright file="Serializer.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;

namespace Hyperion
{
    public abstract class HyperionAttribute : Attribute { }

    /// <summary>
    /// Mark class with this attribute in order to turn preserving all of its object references
    /// in scope of a single <see cref="SerializerSession"/> / <see cref="DeserializerSession"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class PreserveReferencesAttribute : HyperionAttribute
    {
        public PreserveReferencesAttribute() : this(true) { }

        public PreserveReferencesAttribute(bool enabled)
        {
            Enabled = enabled;
        }

        public bool Enabled { get; }
    }
}
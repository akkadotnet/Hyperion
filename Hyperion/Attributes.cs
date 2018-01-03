#region copyright
// -----------------------------------------------------------------------
//  <copyright file="Attributes.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System;

namespace Hyperion
{
    /// <summary>
    /// Just like `NonSerializedAttribute` Ignore attribute can be used to mark fields,
    /// which should not be attached as part of the payload.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreAttribute : Attribute
    {
    }
}
// -----------------------------------------------------------------------
//  <copyright file="TypeExSpecs.cs" company="Akka.NET Team">
//      Copyright (C) 2016-2018 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Hyperion.Extensions;
using Xunit;

namespace Hyperion.Tests.Extensions
{
    public class TypeExSpecs
    {
        [Fact]
        public void GetShortAssemblyQualifiedNameShouldStripAssemblyInfo()
        {
            var immutableInterlockedType = typeof(ImmutableInterlocked);
            var shortName = immutableInterlockedType.GetShortAssemblyQualifiedName();

            // shouldn't have any version data
            shortName.Should().Be("System.Collections.Immutable.ImmutableInterlocked, System.Collections.Immutable");
        }

        //[Fact]
        //public void GetShortAssemblyQualifiedNameShouldStripAssemblyInfoForGenericTypes()
        //{
        //    var immutableArrayType = typeof(ImmutableArray<int>);
        //    var shortName = immutableArrayType.GetShortAssemblyQualifiedName();

        //    // shouldn't have any version data
        //    shortName.Should().Be("System.Collections.Immutable.ImmutableInterlocked, System.Collections.Immutable");
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hyperion.Extensions;

namespace Hyperion
{
    /// <inheritdoc/>
    public sealed class TypeFilter : ITypeFilter
    {
        public ImmutableHashSet<string> FilteredTypes { get; }

        internal TypeFilter(IEnumerable<Type> types)
        {
            FilteredTypes = types.Select(t => t.GetShortAssemblyQualifiedName()).ToImmutableHashSet();
        }
        
        public bool IsAllowed(string typeName)
            => FilteredTypes.Any(t => t == typeName);
    }

    /// <summary>
    /// A disabled type filter that always returns true
    /// </summary>
    public sealed class DisabledTypeFilter : ITypeFilter
    {
        public static readonly DisabledTypeFilter Instance = new DisabledTypeFilter();
        
        private DisabledTypeFilter() { }
        
        public bool IsAllowed(string typeName) => true;
    }
}
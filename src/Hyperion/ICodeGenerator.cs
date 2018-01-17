#region copyright
// -----------------------------------------------------------------------
//  <copyright file="ICodeGenerator.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using Hyperion.ValueSerializers;

namespace Hyperion
{
    public interface ICodeGenerator
    {
        void BuildSerializer(Serializer serializer, ObjectSerializer objectSerializer);
    }
}
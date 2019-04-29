#region copyright
// -----------------------------------------------------------------------
//  <copyright file="AssemblyInfo.cs" company="Akka.NET Team">
//      Copyright (C) 2015-2016 AsynkronIT <https://github.com/AsynkronIT>
//      Copyright (C) 2016-2016 Akka.NET Team <https://github.com/akkadotnet>
//  </copyright>
// -----------------------------------------------------------------------
#endregion

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid("7af8d2b6-9f1f-4a1c-8673-48e533108385")]
[assembly: InternalsVisibleTo("Hyperion.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100d1b02d7d7ee028be7b02edc08abd7f95760f1cc3fee6c48936b9febf4d6f76da586b8932c3a87c5912ddc66028a1841063fa1d4d1dc2814b58deb14cce7d18ffe3d8475bc2fcba11c480f96bcc41355c8f0131c6c3b2c8005ae5744e29498875a487e0f8c977b87236a596680ace77082889305e1e9f2907147add28536c99d8")]

#if UNSAFE
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityTransparent]
[assembly: SecurityRules(SecurityRuleSet.Level1, SkipVerificationInFullTrust = true)]
#endif
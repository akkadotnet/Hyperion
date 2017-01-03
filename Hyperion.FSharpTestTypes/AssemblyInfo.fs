namespace System
open System
open System.Reflection
open System.Runtime.InteropServices

[<assembly: AssemblyTitleAttribute("Hyperion")>]
[<assembly: AssemblyProductAttribute("Hyperion")>]
[<assembly: AssemblyDescriptionAttribute("Binary serializer for POCO objects")>]
[<assembly: AssemblyCopyrightAttribute("Copyright 2016 Akka.NET Team")>]
[<assembly: AssemblyCompanyAttribute("Akka.NET Team")>]
[<assembly: ComVisibleAttribute(false)>]
[<assembly: CLSCompliantAttribute(true)>]
[<assembly: AssemblyVersionAttribute("0.9.0.0")>]
[<assembly: AssemblyFileVersionAttribute("0.9.0.0")>]
do ()

module internal AssemblyVersionInformation =
    let [<Literal>] Version = "0.9.0.0"

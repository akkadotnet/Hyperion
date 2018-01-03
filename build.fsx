#I @"tools/FAKE/tools"
#r "FakeLib.dll"

open System
open System.IO
open System.Text

open Fake
open Fake.DotNetCli

// Variables
let configuration = "Release"

// Directories
let output = __SOURCE_DIRECTORY__  @@ "build"
let outputTests = output @@ "TestResults"
let outputPerfTests = output @@ "perf"
let outputBinaries = output @@ "binaries"
let outputNuGet = output @@ "nuget"
let outputBinariesNet45 = outputBinaries @@ "net45"
let outputBinariesNetStandard = outputBinaries @@ "netstandard1.6"

let buildNumber = environVarOrDefault "BUILD_NUMBER" "0"
let versionSuffix = 
    match (getBuildParam "nugetprerelease") with
    | "dev" -> (if (not (buildNumber = "0")) then (buildNumber) else "") + "-beta"
    | _ -> ""

Target "Clean" (fun _ ->
    CleanDir output
    CleanDir outputTests
    CleanDir outputPerfTests
    CleanDir outputBinaries
    CleanDir outputNuGet
    CleanDir outputBinariesNet45
    CleanDir outputBinariesNetStandard

    CleanDirs !! "./**/bin"
    CleanDirs !! "./**/obj"
)

Target "RestorePackages" (fun _ ->
    let additionalArgs = if versionSuffix.Length > 0 then [sprintf "/p:VersionSuffix=%s" versionSuffix] else []  

    DotNetCli.Restore
        (fun p -> 
            { p with
                Project = "./Hyperion.sln"
                NoCache = false 
                AdditionalArgs = additionalArgs })
)

Target "Build" (fun _ ->
    let additionalArgs = if versionSuffix.Length > 0 then [sprintf "/p:VersionSuffix=%s" versionSuffix] else []  

    if (isWindows) then
        let projects = !! "./**/*.csproj" ++ "./**/*.fsproj"

        let runSingleProject project =
            DotNetCli.Build
                (fun p -> 
                    { p with
                        Project = project
                        Configuration = configuration 
                        AdditionalArgs = additionalArgs })

        projects |> Seq.iter (runSingleProject)
    else
        DotNetCli.Build
            (fun p -> 
                { p with
                    Project = "./Hyperion/Hyperion.csproj"
                    Framework = "netstandard1.6"
                    Configuration = configuration 
                    AdditionalArgs = additionalArgs })

        DotNetCli.Build
            (fun p -> 
                { p with
                    Project = "./Hyperion.Tests/Hyperion.Tests.csproj"
                    Framework = "netcoreapp1.0"
                    Configuration = configuration 
                    AdditionalArgs = additionalArgs })
)

Target "RunTests" (fun _ ->
    if (isWindows) then
        let projects = !! "./**/Hyperion.Tests.csproj"

        let runSingleProject project =
            DotNetCli.Test
                (fun p -> 
                    { p with
                        Project = project
                        Configuration = configuration })

        projects |> Seq.iter (runSingleProject)
    else
        DotNetCli.Test
            (fun p -> 
                { p with
                    Project = "./Hyperion.Tests/Hyperion.Tests.csproj"
                    Framework = "netcoreapp1.1"
                    Configuration = configuration })
)

Target "CopyOutput" (fun _ ->
    // .NET 4.5
    if (isWindows) then
        DotNetCli.Publish
            (fun p -> 
                { p with
                    Project = "./Hyperion/Hyperion.csproj"
                    Framework = "net45"
                    Output = outputBinariesNet45
                    Configuration = configuration })

    // .NET Core
    DotNetCli.Publish
        (fun p -> 
            { p with
                Project = "./Hyperion/Hyperion.csproj"
                Framework = "netstandard1.6"
                Output = outputBinariesNetStandard
                Configuration = configuration })
)

//--------------------------------------------------------------------------------
// Benchmarks
//--------------------------------------------------------------------------------

Target "Benchmarks" (fun _ ->
    () //TODO: complete BenchmarkDotNet setup
)

//--------------------------------------------------------------------------------
// Nuget targets 
//--------------------------------------------------------------------------------

Target "CreateNuget" (fun _ ->
    DotNetCli.Pack
        (fun p -> 
            { p with
                Project = "./Hyperion/Hyperion.csproj"
                Configuration = configuration
                AdditionalArgs = ["--include-symbols"]
                VersionSuffix = versionSuffix
                OutputPath = outputNuGet })
)

Target "PublishNuget" (fun _ ->
    let projects = !! "./build/nuget/*.nupkg" -- "./build/nuget/*.symbols.nupkg"
    let apiKey = getBuildParamOrDefault "nugetkey" ""
    let source = getBuildParamOrDefault "nugetpublishurl" ""
    let symbolSource = getBuildParamOrDefault "symbolspublishurl" ""
    let shouldPublishSymbolsPackages = not (symbolSource = "")

    if (not (source = "") && not (apiKey = "") && shouldPublishSymbolsPackages) then
        let runSingleProject project =
            DotNetCli.RunCommand
                (fun p -> 
                    { p with 
                        TimeOut = TimeSpan.FromMinutes 10. })
                (sprintf "nuget push %s --api-key %s --source %s --symbol-source %s" project apiKey source symbolSource)

        projects |> Seq.iter (runSingleProject)
    else if (not (source = "") && not (apiKey = "") && not shouldPublishSymbolsPackages) then
        let runSingleProject project =
            DotNetCli.RunCommand
                (fun p -> 
                    { p with 
                        TimeOut = TimeSpan.FromMinutes 10. })
                (sprintf "nuget push %s --api-key %s --source %s" project apiKey source)

        projects |> Seq.iter (runSingleProject)
)

//--------------------------------------------------------------------------------
// Help 
//--------------------------------------------------------------------------------

Target "Help" <| fun _ ->
    List.iter printfn [
      "usage:"
      "/build [target]"
      ""
      " Targets for building:"
      " * Build      Builds"
      " * Nuget      Create and optionally publish nugets packages"
      " * RunTests   Runs tests"
      " * Benchmarks Run BenchmarkDotNet performance tests"
      " * All        Builds, run tests, creates and optionally publish nuget packages"
      ""
      " Other Targets"
      " * Help       Display this help" 
      ""]

Target "HelpNuget" <| fun _ ->
    List.iter printfn [
      "usage: "
      "build Nuget [nugetkey=<key> [nugetpublishurl=<url>]] "
      "            [symbolspublishurl=<url>] "
      ""
      "In order to publish a nuget package, keys must be specified."
      "If a key is not specified the nuget packages will only be created on disk"
      "After a build you can find them in build/nuget"
      ""
      "For pushing nuget packages to nuget.org and symbols to symbolsource.org"
      "you need to specify nugetkey=<key>"
      "   build Nuget nugetKey=<key for nuget.org>"
      ""
      "For pushing the ordinary nuget packages to another place than nuget.org specify the url"
      "  nugetkey=<key>  nugetpublishurl=<url>  "
      ""
      "For pushing symbols packages specify:"
      "  symbolskey=<key>  symbolspublishurl=<url> "
      ""
      "Examples:"
      "  build Nuget                      Build nuget packages to the build/nuget folder"
      ""
      "  build Nuget versionsuffix=beta1  Build nuget packages with the custom version suffix"
      ""
      "  build Nuget nugetkey=123         Build and publish to nuget.org and symbolsource.org"
      ""
      "  build Nuget nugetprerelease=dev nugetkey=123 nugetpublishurl=http://abcsymbolspublishurl=http://xyz"
      ""]

//--------------------------------------------------------------------------------
//  Target dependencies
//--------------------------------------------------------------------------------

Target "BuildRelease" DoNothing
Target "All" DoNothing
Target "Nuget" DoNothing

// build dependencies
"Clean" ==> "RestorePackages" ==> "Build" ==> "CopyOutput" ==> "BuildRelease"

// tests dependencies
"Clean" ==> "RestorePackages" ==> "Build" ==> "RunTests"
"Clean" ==> "RestorePackages" ==> "Build"

// benchmark dependencies
"BuildRelease" ==> "Benchmarks"

// nuget dependencies
"Clean" ==> "RestorePackages" ==> "Build" ==> "CreateNuget"
"CreateNuget" ==> "PublishNuget"
"PublishNuget" ==> "Nuget"

// all
"BuildRelease" ==> "All"
"RunTests" ==> "All"
"Benchmarks" ==> "All"
"Nuget" ==> "All"

RunTargetOrDefault "Help"
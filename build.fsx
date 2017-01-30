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
    DotNetCli.Restore
        (fun p -> 
            { p with
                Project = "./Hyperion.sln"
                NoCache = false })
)

Target "Build" (fun _ ->
    if (isWindows) then
        let projects = !! "./**/*.csproj"

        let runSingleProject project =
            DotNetCli.Build
                (fun p -> 
                    { p with
                        Project = project
                        Configuration = configuration })

        projects |> Seq.iter (runSingleProject)
    else
        DotNetCli.Build
            (fun p -> 
                { p with
                    Project = "./Hyperion/Hyperion.csproj"
                    Framework = "netstandard1.6"
                    Configuration = configuration })

        DotNetCli.Build
            (fun p -> 
                { p with
                    Project = "./Hyperion.Tests/Hyperion.Tests.csproj"
                    Framework = "netcoreapp1.0"
                    Configuration = configuration })
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
                    Framework = "netcoreapp1.0"
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

Target "NBench" (fun _ ->
    if (isWindows) then
        let nbenchTestPath = findToolInSubPath "NBench.Runner.exe" "tools/NBench.Runner/lib/net45"
        let assembly = __SOURCE_DIRECTORY__ @@ "Hyperion.Tests.Performance/bin/Release/net451/Hyperion.Tests.Performance.dll"
        
        let spec = getBuildParam "spec"

        let args = new StringBuilder()
                |> append assembly
                |> append (sprintf "output-directory=\"%s\"" outputPerfTests)
                |> append (sprintf "concurrent=\"%b\"" true)
                |> append (sprintf "trace=\"%b\"" true)
                |> toText

        let result = ExecProcess(fun info -> 
            info.FileName <- nbenchTestPath
            info.WorkingDirectory <- (Path.GetDirectoryName (FullName nbenchTestPath))
            info.Arguments <- args) (System.TimeSpan.FromMinutes 15.0) (* Reasonably long-running task. *)
        if result <> 0 then failwithf "NBench.Runner failed. %s %s" nbenchTestPath args
)

Target "CreateNuget" (fun _ ->
    DotNetCli.Pack
        (fun p -> 
            { p with
                Project = "./Hyperion/Hyperion.csproj"
                Configuration = configuration
                AdditionalArgs = ["--include-symbols"]
                OutputPath = outputNuGet })
)

Target "PublishNuget" (fun _ ->
    let projects = !! "./build/nuget/*.nupkg" -- "./build/nuget/*.symbols.nupkg"
    let apiKey = getBuildParamOrDefault "nugetkey" ""
    let source = getBuildParamOrDefault "nugetpublishurl" ""
    let symbolSource = getBuildParamOrDefault "symbolspublishurl" ""

    let runSingleProject project =
        DotNetCli.RunCommand
            (fun p -> 
                { p with 
                    TimeOut = TimeSpan.FromMinutes 10. })
            (sprintf "nuget push %s --api-key %s --source %s --symbol-source %s" project apiKey source symbolSource)

    projects |> Seq.iter (runSingleProject)
)

//--------------------------------------------------------------------------------
// Help 
//--------------------------------------------------------------------------------

Target "Help" <| fun _ ->
    List.iter printfn [
      "usage:"
      "./build.ps1 [target]"
      ""
      " Targets for building:"
      " * Build      Builds"
      " * Nuget      Create and optionally publish nugets packages"
      " * RunTests   Runs tests"
      " * All        Builds, run tests, creates and optionally publish nuget packages"
      ""
      " Other Targets"
      " * Help       Display this help" 
      ""]

//--------------------------------------------------------------------------------
//  Target dependencies
//--------------------------------------------------------------------------------

Target "BuildRelease" DoNothing
Target "All" DoNothing

// build dependencies
"Clean" ==> "RestorePackages" ==> "Build" ==> "CopyOutput" ==> "BuildRelease"

// tests dependencies
"Clean" ==> "RestorePackages" ==> "Build" ==> "RunTests"
"Clean" ==> "RestorePackages" ==> "Build" ==> "NBench"

// nuget dependencies
"Clean" ==> "RestorePackages" ==> "Build" ==> "CreateNuget"

// all
"BuildRelease" ==> "All"
"RunTests" ==> "All"

RunTargetOrDefault "Help"
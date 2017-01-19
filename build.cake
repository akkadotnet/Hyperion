// Usings
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

// Arguments
var target = Argument<string>("target", "Default");
var apiUrl = Argument<string>("apiurl", null);
var apiKey = Argument<string>("apikey", null);
var releaseNote = ParseReleaseNotes("./RELEASE_NOTES.md");
var buildNumber = EnvironmentVariable("BUILD_NUMBER") ?? "0";
var version = Argument<string>("targetversion", string.Format("{0}.{1}", releaseNote.Version, buildNumber ));
var skipClean = Argument<bool>("skipclean", false);
var skipTests = Argument<bool>("skiptests", false);
//var nogit = Argument<bool>("nogit", false);

// Variables
var configuration = IsRunningOnWindows() ? "Release" : "MonoRelease";
var projectJsonFiles = GetFiles("./src/**/project.json");

// Directories
var nuget = Directory(".nuget");
var output = Directory("build");
var outputTests = Directory("TestResults");
var outputBinaries = output + Directory("binaries");
var outputBinariesNet451 = outputBinaries + Directory("net452");
var outputBinariesNetstandard = outputBinaries + Directory("netcoreapp1.0");
var outputNuGet = output + Directory("nuget");
var outputPerfResults = Directory("perfResults");

var root = @"./";
var src = @"./";
var product = "Hyperion";
//var authors = [ "Roger Johansson" ];
var copyright = "Copyright � 2016 Akka.NET Team";
var company = "Akka.NET Team";
var description = "Binary serializer for POCO objects";
//var tags = [ "serializer" ];
var toolDir = "tools";
var publishingError = false;


Task("AssemblyInfo")
  .Does(() => 
{
    var assemblyInfosPaths = GetFiles(src + "Hyperion/**/AssemblyInfo.cs");
    
    foreach (var assemblyInfoPath in assemblyInfosPaths) {
        CreateAssemblyInfo(assemblyInfoPath.FullPath, new AssemblyInfoSettings {
            Product = product,
            Version = version,
            FileVersion = version,
            //InformationalVersion = semVersion,
            InformationalVersion = version,
            Copyright = copyright
        });
    }
});


Task("Clean")
  .Does(() =>
{
    //Clean artifact directories.
    CleanDirectories(new DirectoryPath[] {
      output, outputBinaries, outputNuGet,
      outputBinariesNet451, outputBinariesNetstandard
    });

    if(!skipClean) {
        // Clean output directories.
        CleanDirectories("./**/bin/" + configuration);
        CleanDirectories("./**/obj/" + configuration);
    }
});

Task("Restore-NuGet-Packages")
  .Description("Restores dependencies")
  .Does(() =>
{
    DotNetCoreRestore("./", new DotNetCoreRestoreSettings
    {
        Verbose = false,
        Verbosity = DotNetCoreRestoreVerbosity.Warning,
        Sources = new [] {
          "https://www.myget.org/F/xunit/api/v3/index.json",
          "https://dotnet.myget.org/F/dotnet-core/api/v3/index.json",
          "https://dotnet.myget.org/F/cli-deps/api/v3/index.json",
          "https://api.nuget.org/v3/index.json",
        }
    });
});


Task("Build")
  .Description("Builds the solution")
  .Does(() =>
{
  DotNetCoreBuild("./**/project.json", new DotNetCoreBuildSettings {
      Configuration = configuration,
      Verbose = false
    });
});


Task("CleanTests")
    .Does(() =>
    {
        CleanDirectories(new DirectoryPath[] { outputTests });
    });

Task("RunTests")
  .Description("Executes xUnit tests")
  .WithCriteria(!skipTests)
  .IsDependentOn("CleanTests")
  .Does(() =>
{
    var projects = GetFiles("./Hyperion.Tests/*.xproj");
  //   - GetFiles("./tests/**/*.Performance.xproj");
  // var projects = GetFiles("./tests/**/*.xproj")
  //   - GetFiles("./tests/**/*.Performance.xproj");

    foreach(var project in projects)
    {
        if (IsRunningOnWindows()) {
            DotNetCoreTest(project.GetDirectory().FullPath, new DotNetCoreTestSettings
            {
                Configuration = configuration,
                NoBuild = true,
                Verbose = false,
                ArgumentCustomization = args =>
                  args.Append("-xml").Append(outputTests.Path.CombineWithFilePath(project.GetFilenameWithoutExtension()).FullPath + ".xml")
            });
        }
        else {
            var name = project.GetFilenameWithoutExtension();
            var dirPath = project.GetDirectory().FullPath;
            var xunit = GetFiles(dirPath + "/bin/" + configuration + "/net452/*/dotnet-test-xunit.exe").First().FullPath;
            var testfile = GetFiles(dirPath + "/bin/" + configuration + "/net452/*/" + name + ".dll").First().FullPath;

            using(var process = StartAndReturnProcess("mono", new ProcessSettings{ Arguments = xunit + " " + testfile }))
            {
                process.WaitForExit();
                if (process.GetExitCode() != 0)
                {
                    throw new Exception("Mono tests failed!");
                }
            }
        }
    }
});



Task("Copy-Files")
    .Does(() =>
{
    // .NET 4.5
    DotNetCorePublish("./Hyperion", new DotNetCorePublishSettings
    {
        Framework = "net452",
        OutputDirectory = outputBinariesNet451,
        Configuration = configuration,
        NoBuild = true,
        Verbose = false
    });

    // .NET Core
    DotNetCorePublish("./Hyperion", new DotNetCorePublishSettings
    {
        Framework = "netcoreapp1.0",
        OutputDirectory = outputBinariesNetstandard,
        Configuration = configuration,
        NoBuild = true,
        Verbose = false
    });

    // Copy license
    CopyFileToDirectory("./LICENSE", outputBinariesNet451);
    CopyFileToDirectory("./LICENSE", outputBinariesNetstandard);
});


Task("Package-NuGet")
  .Description("Generates NuGet packages for each project that contains a nuspec")
  .Does(() =>
{
    var projects = GetFiles("./src/**/*.xproj");
    foreach(var project in projects)
    {
        DotNetCorePack(project.GetDirectory().FullPath, new DotNetCorePackSettings {
            Configuration = configuration,
            OutputDirectory = outputNuGet
        });
    }

    // Cake - .NET 4.5
    NuGetPack("./Hyperion/nuspec/Hyperion.nuspec", new NuGetPackSettings {
        Version = version,
        ReleaseNotes = releaseNote.Notes.ToList(),
        BasePath = outputBinariesNet451,
        OutputDirectory = outputNuGet,
        Symbols = false,
        NoPackageAnalysis = true
    });

    // Cake - .NET Core
    NuGetPack("./Hyperion/nuspec/Hyperion.Core.nuspec", new NuGetPackSettings {
        Version = version,
        ReleaseNotes = releaseNote.Notes.ToList(),
        BasePath = outputBinariesNetstandard,
        OutputDirectory = outputNuGet,
        Symbols = false,
        NoPackageAnalysis = true
    });
});


Task("Publish-NuGet")
  .Does(() =>
{
    if(string.IsNullOrWhiteSpace(apiKey)){
        throw new InvalidOperationException("No NuGet API key provided.");
    }

    if(string.IsNullOrEmpty(apiUrl)) {
        throw new InvalidOperationException("Could not resolve NuGet API url.");
    }

    var packages = GetFiles(outputNuGet.Path.FullPath + "/*.nupkg");
    foreach(var package in packages)
    {
        NuGetPush(package, new NuGetPushSettings {
            Source = apiUrl,
            ApiKey = apiKey
        });
    }
})
.OnError(exception =>
{
    Information("Publish-NuGet Task failed, but continuing with next Task...");
    publishingError = true;
});

Task("BuildRelease")
    .IsDependentOn("Clean")
    .IsDependentOn("AssemblyInfo")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Build");

Task("CreateNuget")
    .IsDependentOn("Copy-Files")
    .IsDependentOn("Package-Nuget");

Task("PublishNuget")
    .IsDependentOn("Publish-NuGet")
    .Finally(() => 
    {
        if(publishingError)
        {
            throw new Exception("An error occurred during the publishing of Cake.  All publishing tasks have been attempted.");
        }
    });

Task("All")
    .IsDependentOn("BuildRelease")
    .IsDependentOn("RunTests")
    .IsDependentOn("CreateNuget");

RunTarget(target);
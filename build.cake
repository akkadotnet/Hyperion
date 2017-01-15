// Usings
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

// Arguments
var target = Argument<string>("target", "Default");
var source = Argument<string>("source", null);
var apiKey = Argument<string>("apikey", null);
var releaseNote = ParseReleaseNotes("./RELEASE_NOTES.md");
var buildNumber = EnvironmentVariable("BUILD_NUMBER") ?? "0";
var version = Argument<string>("targetversion", string.Format("{0}.{1}", releaseNote.Version, buildNumber ));
var skipClean = Argument<bool>("skipclean", false);
var skipTests = Argument<bool>("skiptests", false);
var nogit = Argument<bool>("nogit", false);

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

var root = @".\";
var product = "Hyperion";
//var authors = [ "Roger Johansson" ];
var copyright = "Copyright � 2016 Akka.NET Team";
var company = "Akka.NET Team";
var description = "Binary serializer for POCO objects";
//var tags = [ "serializer" ]
//var configuration = "Release";
var toolDir = "tools";
var publishingError = false;
//var CloudCopyDir = toolDir @@ "CloudCopy";
//var AzCopyDir = toolDir @@ "AzCopy"

Task("AssemblyInfo")
  .Does(() => 
{
    CreateAssemblyInfo(root + "SharedAssemblyInfo.cs", new AssemblyInfoSettings {
        Product = product,
        Version = version,
        FileVersion = version,
        //InformationalVersion = semVersion,
        InformationalVersion = version,
        Copyright = copyright
    });
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


// Task("CleanTests")
//   .Does(() =>
// {
//   CleanDirectories(new DirectoryPath[] { outputTests });
// });

Task("RunTests")
  .Description("Executes xUnit tests")
  .WithCriteria(!skipTests)
  //.IsDependentOn("CleanTests")
  .Does(() =>
{
  var projects = GetFiles("./Hyperion.Tests/*.xproj");
  //   - GetFiles("./tests/**/*.Performance.xproj");
  // var projects = GetFiles("./tests/**/*.xproj")
  //   - GetFiles("./tests/**/*.Performance.xproj");

  foreach(var project in projects)
  {
    DotNetCoreTest(project.GetDirectory().FullPath, new DotNetCoreTestSettings
    {
      Configuration = configuration,
      OutputDirectory = outputTests,
      Verbose = false
    });
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
  .IsDependentOn("Package-Nuget")
  .Does(() =>
{
  if(string.IsNullOrWhiteSpace(apiKey)){
    throw new CakeException("No NuGet API key provided.");
  }

  var packages = GetFiles(outputNuGet.Path.FullPath + "/*" + version + ".nupkg");
  foreach(var package in packages)
  {
    NuGetPush(package, new NuGetPushSettings {
      Source = source,
      ApiKey = apiKey
    });
  }
})
.OnError(exception =>
{
    Information("Publish-MyGet Task failed, but continuing with next Task...");
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
  .IsDependentOn("Publish-NuGet");

RunTarget(target);
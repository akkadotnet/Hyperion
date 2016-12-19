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
var version = Argument<string>("targetversion", $"{releaseNote.Version}.{buildNumber}-beta");
var skipClean = Argument<bool>("skipclean", false);
var skipTests = Argument<bool>("skiptests", false);
var nogit = Argument<bool>("nogit", false);

// Variables
var configuration = IsRunningOnWindows() ? "Release" : "MonoRelease";
var projectJsonFiles = GetFiles("./src/**/project.json");

// Directories
var nuget = Directory(".nuget");
var output = Directory("build");
var outputBinaries = output + Directory("binaries");
var outputBinariesNet451 = outputBinaries + Directory("net451");
var outputBinariesNetstandard = outputBinaries + Directory("netstandard1.3");
var outputPackages = output + Directory("packages");
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
    output, outputBinaries, outputPackages, outputNuGet,
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
  var settings = new DotNetCoreRestoreSettings
  {
    Verbose = false,
    Verbosity = DotNetCoreRestoreVerbosity.Warning
  };

  DotNetCoreRestore("./", settings);
});


Task("Compile")
  .Description("Builds the solution")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore-NuGet-Packages")
  .Does(() =>
{
  DotNetCoreBuild("./**/project.json", new DotNetCoreBuildSettings {
      Configuration = configuration,
      Verbose = false
    });
});


Task("Test")
  .Description("Executes xUnit tests")
  .WithCriteria(!skipTests)
  .IsDependentOn("Compile")
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
          Verbose = false
        });
  }
});


Task("Package-NuGet")
  .Description("Generates NuGet packages for each project that contains a nuspec")
  .Does(() =>
{
  var projects = GetFiles("./src/**/*.xproj");
  foreach(var project in projects)
  {
    var settings = new DotNetCorePackSettings {
      Configuration = configuration,
      OutputDirectory = outputNuGet
    };

    DotNetCorePack(project.GetDirectory().FullPath, settings);
  }
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
});




// Task("Default")
//   .IsDependentOn("Clean");

Task("BuildRelease")
  .IsDependentOn("Clean")
  .IsDependentOn("AssemblyInfo")
  .IsDependentOn("Compile")
  .IsDependentOn("Test");

Task("CreateNuget")
  .IsDependentOn("Package-Nuget");

RunTarget(target);
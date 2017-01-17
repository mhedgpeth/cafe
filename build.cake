#addin "Cake.Docker"
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "FullBuild");
var configuration = Argument("configuration", "Debug");
var buildNumber = Argument("buildNumber", "0");

var cafeDirectory = Directory("./src/cafe");
var cafeProject = cafeDirectory + File("project.json");
var cafeUnitTestProject = Directory("./test/cafe.Test/project.json");
var cafeIntegrationTestProject = Directory("./test/cafe.IntegrationTest/project.json");

var buildSettings = new DotNetCoreBuildSettings { VersionSuffix = buildNumber, Configuration = configuration };

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/cafe/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore(cafeProject);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild(cafeProject, buildSettings);
});

Task("UnitTest")
    .Does(() =>
    {
        DotNetCoreRestore(cafeUnitTestProject);      
        DotNetCoreBuild(cafeUnitTestProject, buildSettings);
        DotNetCoreTest(cafeUnitTestProject);
    });

Task("IntegrationTest")
    .Does(() =>
    {
        DotNetCoreRestore(cafeIntegrationTestProject);      
        DotNetCoreBuild(cafeIntegrationTestProject, buildSettings);
        DotNetCoreTest(cafeIntegrationTestProject);
    });

Task("Publish")
    .Does(() => 
    {
        Information("Publishing {0}", configuration);
        DotNetCorePublish(cafeProject, new DotNetCorePublishSettings { Runtime = "win10-x64", Configuration = configuration, VersionSuffix = buildNumber });
        DotNetCorePublish(cafeProject, new DotNetCorePublishSettings { Runtime = "centos.7-x64", Configuration = configuration, VersionSuffix = buildNumber });
        DotNetCorePublish(cafeProject, new DotNetCorePublishSettings { Runtime = "ubuntu.16.04-x64", Configuration = configuration, VersionSuffix = buildNumber });
    });

var cafeWindowsContainerImage = "cafe:windows";

Task("Build-WindowsImage")
    .IsDependentOn("Publish")
    .Does(() => {
        DockerBuild(new DockerBuildSettings { File = cafeDirectory + File("Dockerfile-windows"), Tag = new[] { cafeWindowsContainerImage } }, cafeDirectory);
    });

Task("Run-CafeServerDockerContainer")
    .IsDependentOn("Build-WindowsImage")
    .Does(() => {
        DockerRun(new DockerRunSettings { Interactive = true }, cafeWindowsContainerImage, "server", new string[0]);
    });


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("IncrementalBuild")
    .IsDependentOn("Build")
    .IsDependentOn("UnitTest")
    .IsDependentOn("IntegrationTest");

Task("FullBuild")
    .IsDependentOn("Clean")
    .IsDependentOn("IncrementalBuild")
    .IsDependentOn("Build-WindowsImage");

//////////////////////////////////////////////////////////////////////
// TESTING TARGETS
//////////////////////////////////////////////////////////////////////

var cafeWindowsPublishDirectory = buildDir + Directory("netcoreapp1.1/win10-x64/publish");


Task("ShowStatus")
    .Does(() => {
        var processSettings =  new ProcessSettings() { Arguments = "status" }.UseWorkingDirectory(cafeWindowsPublishDirectory);
        Information("Running cafe.exe from {0}", cafeWindowsPublishDirectory);
        var exitCode = StartProcess(cafeWindowsPublishDirectory + File("cafe.exe"), processSettings);
        Information("Exit code: {0}", exitCode);
    }); 

Task("RunChef")
    .Does(() => {
        var processSettings =  new ProcessSettings() { Arguments = "chef run" }.UseWorkingDirectory(cafeWindowsPublishDirectory);
        Information("Running cafe.exe from {0}", cafeWindowsPublishDirectory);
        var exitCode = StartProcess(cafeWindowsPublishDirectory + File("cafe.exe"), processSettings);
        Information("Exit code: {0}", exitCode);
    }); 


Task("RunServer")
    .Does(() => {
        RunCafe("server"); 
    });

var oldVersion = "12.16.42";

Task("DownloadOldVersion")
    .Does(() =>
    {
        RunCafe("chef download {0}", oldVersion);
    });

Task("InstallOldVersion")
    .Does(() =>
    {
        RunCafe("chef install {0}", oldVersion);
    });

Task("BootstrapPolicy")
    .Does(() =>
    {
        RunCafe(@"chef bootstrap policy: webserver group: production config: C:\Users\mhedg\.chef\client.rb validator: C:\Users\mhedg\.chef\cafe-demo-validator.pem");
    });

public void RunCafe(string argument, params string[] formatParameters) 
{
  var arguments = string.Format(argument, formatParameters);
  var processSettings =  new ProcessSettings { Arguments = arguments}.UseWorkingDirectory(cafeWindowsPublishDirectory);
  Information("Running cafe.exe from {0}", cafeWindowsPublishDirectory);
  var exitCode = StartProcess(cafeWindowsPublishDirectory + File("cafe.exe"), processSettings);
  Information("Exit code: {0}", exitCode);
}


//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

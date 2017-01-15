#addin "Cake.Docker"
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

var cafeProject = "./src/cafe/project.json";
var cafeUnitTestProject = "./test/cafe.Test/project.json";
var cafeIntegrationTestProject = "./test/cafe.IntegrationTest/project.json";

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
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore(cafeProject);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild(cafeProject);
});

Task("UnitTest")
    .Does(() =>
    {
        DotNetCoreRestore(cafeUnitTestProject);      
        DotNetCoreTest(cafeUnitTestProject);
    });

Task("IntegrationTest")
    .Does(() =>
    {
        DotNetCoreRestore(cafeIntegrationTestProject);      
        DotNetCoreTest(cafeIntegrationTestProject);
    });

Task("Publish")
    .Does(() => 
    {
        Information("Publishing {0}", configuration);
        DotNetCorePublish(cafeProject, new DotNetCorePublishSettings { Runtime = "win10-x64", Configuration = configuration });
        DotNetCorePublish(cafeProject, new DotNetCorePublishSettings { Runtime = "centos.7-x64", Configuration = configuration });
        DotNetCorePublish(cafeProject, new DotNetCorePublishSettings { Runtime = "ubuntu.16.04-x64", Configuration = configuration });
    });

Task("Build-WindowsImage")
    .IsDependentOn("Publish")
    .Does(() => {
        DockerBuild(new DockerBuildSettings { File = "./src/cafe/Dockerfile-windows", Tag = new[] {"cafe:windows"} }, "./src/cafe");
    });

Task("Run-CafeServerDockerContainer")
    .IsDependentOn("Build-WindowsImage")
    .Does(() => {
        DockerRun(new DockerRunSettings { Interactive = true }, "cafe:windows", "server", new string[0]);
    });


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("UnitTest")
    .IsDependentOn("IntegrationTest")
    .IsDependentOn("Build-WindowsImage");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

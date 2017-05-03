#addin "Cake.Docker"
#addin "Cake.Compression"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "FullBuild");
var configuration = Argument("configuration", "Debug");
var buildNumber = Argument("buildNumber", "0");

var version = "0.8.0." + buildNumber;

var cafeDirectory = Directory("./src/cafe");
var cafeProject = cafeDirectory + File("cafe.csproj");
var cafeUnitTestProject = Directory("./test/cafe.Test/cafe.Test.csproj");
var cafeIntegrationTestProject = Directory("./test/cafe.IntegrationTest/cafe.IntegrationTest.csproj");

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
    CleanDirectory(archiveDirectory);
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
        DotNetCorePublish(cafeProject, new DotNetCorePublishSettings { Runtime = "win7-x64", Configuration = configuration, VersionSuffix = buildNumber });
        DotNetCorePublish(cafeProject, new DotNetCorePublishSettings { Runtime = "win8-x64", Configuration = configuration, VersionSuffix = buildNumber });
        // Later: DotNetCorePublish(cafeProject, new DotNetCorePublishSettings { Runtime = "centos.7-x64", Configuration = configuration, VersionSuffix = buildNumber });
        // Later: DotNetCorePublish(cafeProject, new DotNetCorePublishSettings { Runtime = "ubuntu.16.04-x64", Configuration = configuration, VersionSuffix = buildNumber });
    });

var archiveDirectory =  Directory("archive");

Task("Archive")
    .Does(() => 
    {
        Information("Archiving {0}", configuration);
        CreateDirectory(archiveDirectory);
        Zip(cafeWindowsPublishDirectory, archiveDirectory  + File("cafe-win10-x64-" + version + ".zip"));
        Zip(buildDir + Directory("netcoreapp1.1/win7-x64/publish"), archiveDirectory  + File("cafe-win7-x64-" + version + ".zip"));
        Zip(buildDir + Directory("netcoreapp1.1/win8-x64/publish"), archiveDirectory  + File("cafe-win8-x64-" + version + ".zip"));
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
    .IsDependentOn("IntegrationTest")
    .IsDependentOn("Publish")
    .IsDependentOn("Archive");

Task("FullBuild")
    .IsDependentOn("Clean")
    .IsDependentOn("IncrementalBuild");

//////////////////////////////////////////////////////////////////////
// TESTING TARGETS
//////////////////////////////////////////////////////////////////////

var cafeWindowsPublishDirectory = buildDir + Directory("netcoreapp1.1/win10-x64/publish");

Task("RunCafeWithNoArguments")
    .Does(() => {
        RunCafe("");
    });


Task("ShowHelp")
    .Does(() => {
        RunCafe("help");
    });


Task("ShowChefStatus")
    .Does(() => {
        RunCafe("chef status");
    });

Task("ShowInspecStatus")
    .Does(() => {
        RunCafe("inspec status");
    });


Task("ShowJobStatus")
    .Does(() => {
        RunCafe("job status");
    });

Task("RunChef")
    .Does(() => {
        RunCafe("chef run");
    }); 


Task("RunServer")
    .Does(() => {
        RunCafe("server"); 
    });

var oldInspecVersion = "1.6.0";
var newInspecVersion = "1.7.1";
var oldChefVersion = "12.16.42";
var newChefVersion = "12.17.44";

Task("InspecDownloadOldVersion")
    .Does(() =>
    {
        RunCafe("inspec download {0}", oldInspecVersion);
    });

Task("InspecInstallOldVersion")
    .Does(() =>
    {
        RunCafe("inspec install {0}", oldInspecVersion);
    });

Task("InspecCheckOldVersion")
    .Does(() =>
    {
        RunCafe("inspec version? {0}.1", oldInspecVersion);
    });


Task("InspecDownloadNewVersion")
    .Does(() =>
    {
        RunCafe("inspec download {0}", newInspecVersion);
    });

Task("InspecInstallNewVersion")
    .Does(() =>
    {
        RunCafe("inspec install {0}", newInspecVersion);
    });

Task("InspecCheckNewVersion")
    .Does(() =>
    {
        RunCafe("inspec version? {0}.1", newInspecVersion);
    });


var oldVersion = "12.16.42";


Task("ChefDownloadOldVersion")
    .Does(() =>
    {
        RunCafe("chef download {0}", oldChefVersion);
    });

Task("ChefInstallOldVersion")
    .Does(() =>
    {
        RunCafe("chef install {0}", oldChefVersion);
    });

Task("ChefCheckOldVersion")
    .Does(() =>
    {
        RunCafe("chef version? {0}.1", oldChefVersion);
    });


Task("ChefDownloadNewVersion")
    .Does(() =>
    {
        RunCafe("chef download {0}", newChefVersion);
    });

Task("ChefInstallNewVersion")
    .Does(() =>
    {
        RunCafe("chef install {0}", newChefVersion);
    });


Task("ChefCheckNewVersion")
    .Does(() =>
    {
        RunCafe("chef version? {0}.1", newChefVersion);
    });

Task("BootstrapPolicy")
    .Does(() =>
    {
        RunCafe(@"chef bootstrap policy: cafe-demo group: qa config: C:\Users\mhedg\.chef\client.rb validator: C:\Users\mhedg\.chef\cafe-demo-validator.pem");
    });

Task("RegisterService")
    .Does(() =>
    {
        RunCafe(@"service register");
    });

Task("UnregisterService")
    .Does(() =>
    {
        RunCafe(@"service unregister");
    });

Task("PauseChef")
    .Does(() =>
    {
        RunCafe(@"chef pause");
    });

Task("ResumeChef")
    .Does(() =>
    {
        RunCafe(@"chef resume");
    });

Task("StopService")
    .Does(() =>
    {
        RunCafe(@"service stop");
    });


Task("StartService")
    .Does(() =>
    {
        RunCafe(@"service start");
    });

Task("CheckCafeVersion")
    .Does(() =>
    {
        RunCafe("version? {0}", version);
    });

var cafeOldVersion = "0.5.4";

Task("DownloadCafeOldVersion")
    .Does(() => 
    {
        RunCafe("download {0}", cafeOldVersion);
    });

Task("InstallCafeOldVersion")
    .Does(() => 
    {
        RunCafe("install {0}", cafeOldVersion);
    });

public void RunCafe(string argument, params string[] formatParameters) 
{
  var arguments = string.Format(argument, formatParameters);
  var processSettings =  new ProcessSettings { Arguments = arguments}.UseWorkingDirectory(cafeWindowsPublishDirectory);
  Information("Running cafe.exe from {0}", cafeWindowsPublishDirectory);
  var exitCode = StartProcess(cafeWindowsPublishDirectory + File("cafe.exe"), processSettings);
  Information("Exit code: {0}", exitCode);
  if (exitCode < 0) throw new Exception(string.Format("cafe.exe exited with code: {0}", exitCode));
}

Task("AcceptanceTest")
    .IsDependentOn("RunCafeWithNoArguments")
    .IsDependentOn("ShowHelp")
    .IsDependentOn("RegisterService")
    .IsDependentOn("CheckCafeVersion")
    .IsDependentOn("InspecDownloadOldVersion")
    .IsDependentOn("InspecInstallOldVersion")
    .IsDependentOn("InspecCheckOldVersion")
    .IsDependentOn("ChefDownloadOldVersion")
    .IsDependentOn("ChefInstallOldVersion")
    .IsDependentOn("ChefCheckOldVersion")
    .IsDependentOn("BootstrapPolicy")
    .IsDependentOn("ShowChefStatus")
    .IsDependentOn("ShowInspecStatus")
    .IsDependentOn("ShowJobStatus")
    .IsDependentOn("InspecDownloadNewVersion")
    .IsDependentOn("InspecInstallNewVersion")
    .IsDependentOn("InspecCheckNewVersion")
    .IsDependentOn("ChefDownloadNewVersion")
    .IsDependentOn("ChefInstallNewVersion")
    .IsDependentOn("ChefCheckNewVersion")
    .IsDependentOn("RunChef")
    .IsDependentOn("PauseChef")
    .IsDependentOn("ResumeChef")
    .IsDependentOn("StopService")
    .IsDependentOn("UnregisterService");

Task("RunServerInDocker")
    .Does(() => {
        var settings = new DockerRunSettings
        {
            Interactive = true,
            Rm = true
        };
        DockerRun(settings, "cafe:windows", string.Empty, new string[0]);
    });

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);

#addin "Cake.Docker"
#addin "Cake.Compression"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "FullBuild");
var configuration = Argument("configuration", "Debug");
var buildNumber = Argument("buildNumber", "0");

var version = "0.9.0." + buildNumber;

var cafeDirectory = Directory("./src/cafe");
var cafeProject = cafeDirectory + File("cafe.csproj");
var cafeUpdaterDirectory = Directory("./src/cafe.Updater");
var cafeUpdaterProject = cafeUpdaterDirectory + File("cafe.Updater.csproj");
var cafeUpdaterBuildDir = cafeUpdaterDirectory + Directory("bin") + Directory(configuration);
var cafeUnitTestProject = Directory("./test/cafe.Test/cafe.Test.csproj");
var cafeCommandLineUnitTestProject = Directory("./test/cafe.CommandLine.Test/cafe.CommandLine.Test.csproj");
var cafeIntegrationTestProject = Directory("./test/cafe.IntegrationTest/cafe.IntegrationTest.csproj");

var buildSettings = new DotNetCoreBuildSettings { VersionSuffix = buildNumber, Configuration = configuration };

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/cafe/bin") + Directory(configuration);

var stagingDirectory = Directory("staging");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(archiveDirectory);
    CleanDirectory(cafeUpdaterBuildDir);
    CleanDirectory(stagingDirectory);
});

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore(cafeProject);
    DotNetCoreRestore(cafeUpdaterProject);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild(cafeProject, buildSettings);
    DotNetCoreBuild(cafeUpdaterProject, buildSettings);
});

Task("UnitTest")
    .Does(() =>
    {
        DotNetCoreRestore(cafeUnitTestProject);
        DotNetCoreRestore(cafeCommandLineUnitTestProject);      
        DotNetCoreBuild(cafeUnitTestProject, buildSettings);
        DotNetCoreBuild(cafeCommandLineUnitTestProject, buildSettings);
        DotNetCoreTest(cafeUnitTestProject);
        DotNetCoreTest(cafeCommandLineUnitTestProject);
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
        PublishProject(cafeProject);
        PublishProject(cafeUpdaterProject);
    });

public void PublishProject(string project) 
{
    DotNetCorePublish(project, new DotNetCorePublishSettings { Runtime = "win10-x64", Configuration = configuration, VersionSuffix = buildNumber });
    DotNetCorePublish(project, new DotNetCorePublishSettings { Runtime = "win7-x64", Configuration = configuration, VersionSuffix = buildNumber });
    DotNetCorePublish(project, new DotNetCorePublishSettings { Runtime = "win8-x64", Configuration = configuration, VersionSuffix = buildNumber });
}


var archiveDirectory =  Directory("archive");

Task("Stage")
    .Does(() =>
    {
        StageRelease("win10");
        StageRelease("win7");
        StageRelease("win8");
    });

var windows10StagingDirectory = stagingDirectory + Directory("cafe-win10-x64-" + version);

public void StageRelease(string runtimeIdentifier) 
{
    CreateDirectory(stagingDirectory);
    var versionStagingDirectory = stagingDirectory + Directory("cafe-" + runtimeIdentifier + "-x64-" + version);
    CreateDirectory(versionStagingDirectory);
    var updaterStagingDirectory = versionStagingDirectory + Directory("updater");
    CreateDirectory(updaterStagingDirectory);

    var cafeParentDirectory = buildDir + Directory("netcoreapp1.1");
    
    CopyDirectory(cafeParentDirectory + Directory(runtimeIdentifier + "-x64") + Directory("publish"), versionStagingDirectory);
    CopyDirectory(cafeUpdaterBuildDir + Directory("netcoreapp1.1") + Directory(runtimeIdentifier + "-x64") + Directory("publish"), updaterStagingDirectory);
}

Task("Archive")
    .Does(() => 
    {
        Information("Archiving {0}", configuration);
        CreateDirectory(archiveDirectory);
        ZipStagedRelease("win10");
        ZipStagedRelease("win7");
        ZipStagedRelease("win8");
    });

public void ZipStagedRelease(string runtimeIdentifier)
{
    var archiveName = "cafe-" + runtimeIdentifier + "-x64-" + version;
    Zip(stagingDirectory + Directory(archiveName), archiveDirectory + File(archiveName + ".zip"));    
}

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
    .IsDependentOn("Stage")
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
    .IsDependentOn("CafeUpdaterRegister")
    .Does(() =>
    {
        RunCafe(@"service register");
    });

Task("UnregisterService")
    .IsDependentOn("CafeUpdaterUnregister")
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
    .IsDependentOn("CafeUpdaterStopService")
    .Does(() =>
    {
        RunCafe(@"service stop");
    });


Task("StartService")
    .IsDependentOn("CafeUpdaterStartService")
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

Task("CafeUpdaterRegister")
    .Does(() =>
    {
        RunCafeUpdater("service register");
    });

Task("CafeUpdaterUnregister")
    .Does(() =>
    {
        RunCafeUpdater("service unregister");
    });

Task("CafeUpdaterStartService")
    .Does(() =>
    {
        RunCafeUpdater("service start");
    });

Task("CafeUpdaterStopService")
    .Does(() =>
    {
        RunCafeUpdater("service stop");
    });

var windows10Release = "cafe-win10-x64-" + version;
var windows10ReleaseZipFile = File(windows10Release + ".zip");

Task("StageUpgrade")
    .Does(() =>
    {
        CopyFile(archiveDirectory + windows10ReleaseZipFile, stagingDirectory + Directory(windows10Release) + Directory("updater") + Directory("staging") + windows10ReleaseZipFile);
    });

Task("WaitForUpgradeToFinish")
    .Does(() => 
    {
        RunCafeUpdater("wait installer: {0}", windows10ReleaseZipFile);
    });

Task("UpgradeToSameVersion")
    .IsDependentOn("StageUpgrade")
    .IsDependentOn("WaitForUpgradeToFinish");


public void RunCafe(string argument, params string[] formatParameters) 
{
  var arguments = string.Format(argument, formatParameters);
  var processSettings =  new ProcessSettings { Arguments = arguments}.UseWorkingDirectory(windows10StagingDirectory);
  Information("Running cafe.exe from {0}", windows10StagingDirectory);
  var exitCode = StartProcess(windows10StagingDirectory + File("cafe.exe"), processSettings);
  Information("Exit code: {0}", exitCode);
  if (exitCode < 0) throw new Exception(string.Format("cafe.exe exited with code: {0}", exitCode));
}

var windows10UpdaterStagingDirectory = windows10StagingDirectory + Directory("updater");

public void RunCafeUpdater(string argument, params string[] formatParameters) 
{
  var arguments = string.Format(argument, formatParameters);
  var processSettings =  new ProcessSettings { Arguments = arguments}.UseWorkingDirectory(windows10UpdaterStagingDirectory);
  Information("Running cafe.Updater.exe from {0}", windows10UpdaterStagingDirectory);
  var exitCode = StartProcess(windows10UpdaterStagingDirectory + File("cafe.Updater.exe"), processSettings);
  Information("Exit code: {0}", exitCode);
  if (exitCode < 0) throw new Exception(string.Format("cafe.Updater.exe exited with code: {0}", exitCode));
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
    .IsDependentOn("UpgradeToSameVersion")
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

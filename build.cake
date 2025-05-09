#tool "nuget:?package=GitVersion.CommandLine&version=5.10.3"
#tool "nuget:?package=JetBrains.dotCover.CommandLineTools&version=2023.3.1"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var solution = "./ATMSystem.sln";
var skipTests = Argument("skip-tests", false);

Task("Clean")
    .Does(() =>
    {
        CleanDirectory("./src/ATMSystem/bin");
        CleanDirectory("./src/ATMSystem/obj");
        CleanDirectory("./src/ATMSystem.Tests/bin");
        CleanDirectory("./src/ATMSystem.Tests/obj");
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        DotNetRestore(solution);
    });

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetBuild(solution, new DotNetBuildSettings
        {
            Configuration = configuration,
            NoRestore = true
        });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var testSettings = new DotNetTestSettings
        {
            Configuration = configuration,
            NoBuild = true,
            NoRestore = true
        };

        // var coverletSettings = new CoverletSettings
        // {
        //     CollectCoverage = true,
        //     CoverletOutputFormat = CoverletOutputFormat.cobertura,
        //     CoverletOutputDirectory = Directory("./coverage"),
        //     CoverletOutputName = "coverage",
        //     Threshold = 90,
        //     ThresholdType = ThresholdType.Line
        // };

        // DotNetTest("./src/ATMSystem.Tests/ATMSystem.Tests.csproj", testSettings, coverletSettings);
        if (!skipTests)
        {
            DotNetTest("./src/ATMSystem.Tests/ATMSystem.Tests.csproj", testSettings);
        }
    });

Task("Default")
    .IsDependentOn("Test");

RunTarget(target); 
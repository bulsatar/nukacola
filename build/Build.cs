using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Pack);

    // EXTERNAL ACCESSIBLE PARAMETERS
    //---------------------------------------------
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    [Parameter("Nuget Version Prefix - defaults to Version Prefix in csproj")]
    public string VersionPrefix = "";
    [Parameter("Nuget Version Suffix")]
    public string VersionSuffix = "";
    [Parameter("Build Number - defaults to 'local'")]
    public string BuildNumber = "";
    [Parameter("Build Output Path - defaults to ./output")]
    public string BuildOutputPath;
    //---------------------------------------------

    [Solution] readonly Solution Solution;

    AbsolutePath OutputDirectory => string.IsNullOrWhiteSpace(BuildOutputPath) ? RootDirectory / "artifacts" : (AbsolutePath)BuildOutputPath;
    AbsolutePath TempBuildDir => RootDirectory / "tempbuild";

    Target Clean => _ => _
        .Executes(() =>
        {
            Solution.AllProjects.ForEach(project => CleanProject(project));
            EnsureCleanDirectory(OutputDirectory);
            EnsureCleanDirectory(TempBuildDir);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .When(!string.IsNullOrWhiteSpace(VersionPrefix), buildsetting => buildsetting.SetVersionPrefix(VersionPrefix))
                .When(!string.IsNullOrWhiteSpace(VersionSuffix), buildsetting => buildsetting.SetVersionSuffix(VersionSuffix))
                .SetAuthors("Adrian Hoffman")               
                .EnableNoRestore()
            );
        });

    Target UnitTest => ttarget => ttarget
       .DependsOn(Compile)
       .Executes(() =>
       {
           DotNetTest(test => test
               .SetProjectFile(Solution.GetProject("UnitTests"))
               .SetConfiguration(Configuration)
               .EnableNoRestore()
               .EnableNoBuild()
           );
       });

    //dotnet doesn't need the pack command but is useful when outputting artifacts to specific directories.
    Target Pack => ttarget => ttarget
        .DependsOn(UnitTest)
        .Executes(() =>
        {
            PackageProject(Solution.GetProject("NukaCola"));
        });

    private void CleanProject(Project project)
    {
        if (project.Name == "_build")
            return;
        project.Path.Parent.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
    }


    private void PackageProject(Project project)
    {
        ZipProject(project);
        PackageNuget(project);
    }

    private void ZipProject(Project project)
    {
        if (project.Name == "_build" || project.Name == "UnitTests")
            return;

        string filename = project.Name + BuildNumber.ToString() + ".zip";
        AbsolutePath builddir = TempBuildDir / project.Name;
        AbsolutePath zipartifact = OutputDirectory / filename;

        DotNetPublish(pack => pack
               .SetProject(project)
               .When(!string.IsNullOrWhiteSpace(VersionPrefix), buildsetting => buildsetting.SetVersionPrefix(VersionPrefix))
               .When(!string.IsNullOrWhiteSpace(VersionSuffix), buildsetting => buildsetting.SetVersionSuffix(VersionSuffix))
               .SetAuthors("Adrian Hoffman")
               .SetOutput(builddir)
               .EnableNoRestore()
               .EnableNoBuild()
               .SetConfiguration(Configuration)
           );

        CompressionTasks.CompressZip(builddir, zipartifact);
    }

    private void PackageNuget(Project project)
    {
        if (project.Name == "_build" || project.Name == "UnitTests")
            return;

        AbsolutePath artifactdir = OutputDirectory;
        DotNetPack(pack => pack
               .SetProject(project)
               .When(!string.IsNullOrWhiteSpace(VersionPrefix), buildsetting => buildsetting.SetVersionPrefix(VersionPrefix))
               .When(!string.IsNullOrWhiteSpace(VersionSuffix), buildsetting => buildsetting.SetVersionSuffix(VersionSuffix))
               .SetAuthors("Adrian Hoffman")
               .SetOutputDirectory(artifactdir)
               .EnableNoRestore()
               //.EnableNoBuild()
               .SetConfiguration(Configuration)
           );
    }
}

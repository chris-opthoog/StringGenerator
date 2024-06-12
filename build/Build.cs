using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Serilog;
using System;
using System.IO;
using System.Linq;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;


[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild {
    public static int Main() => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter]
    [Secret]
    readonly string NugetApiKey;

    [Solution] readonly Solution Solution;

    [GitRepository] readonly GitRepository GitRepository;

    [GitVersion(Framework = "net6.0", NoFetch = true)] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath NugetOutputDirectory => OutputDirectory / "nuget";
    AbsolutePath ArtifactsDirectory => OutputDirectory / "artifacts";


    Target Clean => _ => _
        .Executes(() => {

            Log.Information(NugetOutputDirectory);

            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            OutputDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() => {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() => {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)

                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)

                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() => {

            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)

                .EnableNoBuild()
                .EnableNoRestore()
            );
        });

    Target Pack => _ => _
        .DependsOn(Test)
        .Executes(() => {

            NugetOutputDirectory.CreateOrCleanDirectory();

            DotNetPack(s => s
              .SetProject(Solution.GetProject("StringGenerator"))
              .SetConfiguration(Configuration)

              .SetVersion(GitVersion.NuGetVersionV2)
              .SetPackageProjectUrl("https://github.com/chris-opthoog/StringGenerator")
              .SetDescription("Generate random strings. Use the CryptoStringGenerator if using for passwords.")
              .SetAuthors(new string[] { "Chris Opthoog (chris.opthoog@myob.com)" })
              .EnableNoBuild()
              .EnableNoRestore()
              .SetNoDependencies(true)
              .SetOutputDirectory(NugetOutputDirectory));
        });

    Target Dist => _ => _
        .DependsOn(Test)
        .Executes(() => {

            ArtifactsDirectory.CreateOrCleanDirectory();

            DotNetPublish(s => s
                .SetProject(Solution.GetProject("StringGenerator.Client"))
                .SetConfiguration(Configuration)
                .SetOutput(ArtifactsDirectory)
                .EnableNoBuild()
                .EnableNoRestore()
            );
        });

    Target Push => _ => _
       .DependsOn(Pack)

       .Requires(() => Configuration.Equals(Configuration.Release))
       .Executes(() => {

            NugetOutputDirectory.GlobFiles("*.nupkg")
               .Where(x => !x.Contains("symbols.nupkg"))
               .ForEach(x => {
                   DotNetNuGetPush(s => s
                       .SetTargetPath(x)
                       .SetSource("https://api.nuget.org/v3/index.json")
                       .SetApiKey(NugetApiKey)
                       .SetSkipDuplicate(true)
                   );
               });
       });
}

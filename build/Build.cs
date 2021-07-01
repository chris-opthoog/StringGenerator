using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using System.Linq;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    [GitRepository] readonly GitRepository GitRepository;

    [GitVersion(Framework = "net5.0", NoFetch = true)] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
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
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() => {

            DotNetPack(s => s
              .SetProject(Solution.GetProject("StringGenerator"))
              .SetConfiguration(Configuration)
              .EnableNoBuild()
              .EnableNoRestore()
              .SetNoDependencies(true)
              .SetOutputDirectory(OutputDirectory));
        });

    Target Push => _ => _
       .DependsOn(Pack)
       
       .Requires(() => Configuration.Equals(Configuration.Release))
       .Executes(() => {
           GlobFiles(OutputDirectory, "*.nupkg")
               .NotEmpty()
               .Where(x => !x.EndsWith("symbols.nupkg"))
               .ForEach(x => {
                   DotNetNuGetPush(s => s
                       .SetTargetPath(x)
                       .SetSource("https://api.nuget.org/v3/index.json")
                       .SetApiKey("oy2n7jlt3zyxau36rzmaop3czplkmeye5p7dtwx4mxe5wy")
                       .SetSkipDuplicate(true)
                   );
               });
       });

}

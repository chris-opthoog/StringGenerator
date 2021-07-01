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
class Build : NukeBuild {
    public static int Main() => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter]
    [Secret]
    readonly string NugetApiKey;

    [Solution] readonly Solution Solution;

    [GitRepository] readonly GitRepository GitRepository;

    [GitVersion(Framework = "net5.0", NoFetch = true)] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() => {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
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

            DotNetPack(s => s
              .SetProject(Solution.GetProject("StringGenerator"))
              .SetConfiguration(Configuration)

              .SetAssemblyVersion(GitVersion.AssemblySemVer)
              .SetFileVersion(GitVersion.AssemblySemFileVer)
              .SetInformationalVersion(GitVersion.InformationalVersion)

              .EnableNoBuild()
              .EnableNoRestore()
              .SetNoDependencies(true)
              .SetOutputDirectory(OutputDirectory));
        });

    Target Push => _ => _
       .DependsOn(Pack)

       .Requires(() => Configuration.Equals(Configuration.Release))
       .Executes(() => {

           var pathEnvVar = System.Environment.GetEnvironmentVariable("PATH");
           Logger.Info(pathEnvVar);

           var v = System.Environment.GetEnvironmentVariable("NUGET_API_KEY");
           Logger.Info(v);

           GlobFiles(OutputDirectory, "*.nupkg")
               .NotEmpty()
               .Where(x => !x.EndsWith("symbols.nupkg"))
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

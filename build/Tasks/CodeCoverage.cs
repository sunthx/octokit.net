using System.Collections.Generic;
using System.Linq;
using Cake.Codecov;
using Cake.Common;
using Cake.Common.Build;
using Cake.Common.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

[Dependency(typeof(Build))]
public sealed class CodeCoverage : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        var coverageFiles = new List<FilePath>();
        foreach (var project in context.Projects.Where(x => x.UnitTests))
        {
            context.Information("Executing Code Coverage for Project {0}...", project.Name);

            var dotNetCoreCoverage = context.CodeCoverage
                .CombineWithFilePath(project.Name + "-netcoreapp2.0.xml");
            coverageFiles.Add(dotNetCoreCoverage);

            context.Coverlet(project, new CoverletToolSettings()
            {
                Configuration = context.Configuration,
                Framework = "netcoreapp2.0",
                Output = dotNetCoreCoverage.FullPath
            });

            if (context.IsRunningOnWindows())
            {
                var dotNetFrameworkCoverage = context.CodeCoverage
                    .CombineWithFilePath(project.Name + "-net452.xml");
                coverageFiles.Add(dotNetFrameworkCoverage);

                context.Coverlet(project, new CoverletToolSettings
                {
                    Configuration = context.Configuration,
                    Framework = "net452",
                    Output = dotNetFrameworkCoverage.FullPath
                });
            }

            if (context.AppVeyor)
            {
                context.Information("Uploading Coverage Files: {0}", string.Join(",", coverageFiles.Select(path => path.GetFilename().ToString())));

                var userProfilePath = context.Environment.GetEnvironmentVariable("USERPROFILE");
                var codecovPath = new DirectoryPath(userProfilePath)
                    .CombineWithFilePath(".nuget\\packages\\codecov\\1.0.5\\tools\\codecov.exe");

                context.Tools.RegisterFile(codecovPath);

                var buildVersion = string.Format("{0}.build.{1}",
                    context.EnvironmentVariable("GitVersion_SemVer"),
                    context.BuildSystem().AppVeyor.Environment.Build.Version
                );

                var settings = new CodecovSettings
                {
                    Files = coverageFiles.Select(path => path.FullPath).ToArray(),
                    EnvironmentVariables = new Dictionary<string, string> { { "APPVEYOR_BUILD_VERSION", buildVersion } }
                };

                context.Codecov(settings);
            }
        }
    }
}
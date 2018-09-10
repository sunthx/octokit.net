using System.Linq;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Core.IO;
using Cake.Coverlet;
using Cake.Frosting;

[Dependency(typeof(Build))]
public sealed class CodeCoverage : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        foreach (var project in context.Projects.Where(x => x.UnitTests))
        {
            context.Information("Executing Code Coverage Project {0}...", project.Name);
            var dotNetCoreTestSettings = context.GetTestSettings();
            dotNetCoreTestSettings.Framework = "netcoreapp2.0";

            context.DotNetCoreTest(project.Path.FullPath, dotNetCoreTestSettings, new CoverletSettings()
            {
                CollectCoverage = true,
                CoverletOutputFormat = CoverletOutputFormat.opencover,
                CoverletOutputDirectory = DirectoryPath.FromString(@".\coverage-results\"),
                CoverletOutputName = $"{project.Name}-netcoreapp2.0"
            });

            if (context.IsRunningOnWindows())
            {
                dotNetCoreTestSettings.Framework = "net452";

                context.DotNetCoreTest(project.Path.FullPath, dotNetCoreTestSettings, new CoverletSettings()
                {
                    CollectCoverage = true,
                    CoverletOutputFormat = CoverletOutputFormat.opencover,
                    CoverletOutputDirectory = DirectoryPath.FromString(@".\coverage-results\"),
                    CoverletOutputName = $"{project.Name}-net452.0"
                });
            }
        }
    }
}
using System.Linq;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNetCore;
using Cake.Core.IO;
using Cake.Coverlet;
using Cake.Frosting;

[Dependency(typeof(Build))]
public sealed class UnitTests : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        foreach (var project in context.Projects.Where(x => x.UnitTests))
        {
            context.Information("Executing Unit Tests Project {0}...", project.Name);
            context.DotNetCoreTest(project.Path.FullPath, context.GetTestSettings());
        }
    }
}

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
                CoverletOutputName = $"{project.Name}"
            });
        }
    }
}
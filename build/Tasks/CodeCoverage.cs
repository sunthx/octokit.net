using System.Linq;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Frosting;

[Dependency(typeof(Build))]
public sealed class CodeCoverage : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        foreach (var project in context.Projects.Where(x => x.UnitTests))
        {
            context.Information("Executing Code Coverage Project {0}...", project.Name);

            context.Coverlet(project, new CoverletToolSettings()
            {
                Configuration = context.Configuration,
                Framework = "netcoreapp2.0",
                Output = @".\coverage-results\" + project.Name + "-netcoreapp2.0.xml"
            });

            if (context.IsRunningOnWindows())
            {
                context.Coverlet(project, new CoverletToolSettings
                {
                    Configuration = context.Configuration,
                    Framework = "net452",
                    Output = @".\coverage-results\" + project.Name + "-net452.xml"
                });
            }
        }
    }
}
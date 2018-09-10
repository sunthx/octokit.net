using System.Collections.Generic;
using System.Linq;
using Cake.Codecov;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Frosting;

[Dependency(typeof(Build))]
public sealed class CodeCoverage : FrostingTask<Context>
{
    public override void Run(Context context)
    {
        var outputs = new List<string>();
        foreach (var project in context.Projects.Where(x => x.UnitTests))
        {
            context.Information("Executing Code Coverage Project {0}...", project.Name);

            var output = @".\coverage-results\" + project.Name + "-netcoreapp2.0.xml";
            outputs.Add(output);

            context.Coverlet(project, new CoverletToolSettings()
            {
                Configuration = context.Configuration,
                Framework = "netcoreapp2.0",
                Output = output
            });

            if (context.IsRunningOnWindows())
            {
                output = @".\coverage-results\" + project.Name + "-net452.xml";
                outputs.Add(output);

                context.Coverlet(project, new CoverletToolSettings
                {
                    Configuration = context.Configuration,
                    Framework = "net452",
                    Output = output
                });
            }

            if (context.AppVeyor)
            {
                context.Codecov(outputs);
            }
        }
    }
}
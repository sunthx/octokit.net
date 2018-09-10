using System.Collections.Generic;
using System.Linq;
using Cake.Codecov;
using Cake.Common;
using Cake.Common.Diagnostics;
using Cake.Core.IO;
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

            var output = context.CodeCoverage
                .CombineWithFilePath(project.Name + "-netcoreapp2.0.xml")
                .MakeAbsolute(context.Environment)
                .ToString();
            outputs.Add(output);

            context.Coverlet(project, new CoverletToolSettings()
            {
                Configuration = context.Configuration,
                Framework = "netcoreapp2.0",
                Output = output
            });

            if (context.IsRunningOnWindows())
            {
                output = context.CodeCoverage
                    .CombineWithFilePath(project.Name + "-net452.xml")
                    .MakeAbsolute(context.Environment)
                    .ToString();
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
                var userProfilePath = context.Environment.GetEnvironmentVariable("userprofile");
                var codecovPath = new DirectoryPath(userProfilePath)
                    .CombineWithFilePath(".nuget\\packages\\codecov\\1.0.5\\tools\\codecov.exe");

                context.Tools.RegisterFile(codecovPath);

                context.Codecov(outputs);
            }
        }
    }
}
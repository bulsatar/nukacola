using Nuke.Common;
using Nuke.Common.Tooling;
using System.Collections.Generic;

namespace Tool.Deploy.AwsSamCli
{
    public static partial class AwsSamTasks
    {
        public static string SamPath =>
            ToolPathResolver.TryGetEnvironmentExecutable("SAM_EXE") ??
            ToolPathResolver.GetPathExecutable("sam");

        internal static void CustomLogger(OutputType type, string output)
        {
            if (type == OutputType.Err)
            {
                Logger.Error(output);
                return;
            }

            Logger.Normal(output);
        }

        public static IReadOnlyCollection<Output> AwsSam(string cliTarget, string arguments, string workingDirectory = null, bool waitForExit = false, bool assertZeroExitCode = true)
        {
            string command = $"sam {cliTarget} {arguments}";
            using var process = ProcessTasks.StartProcess(SamPath, command, workingDirectory);

            if (assertZeroExitCode)
                process.AssertZeroExitCode();
            if (waitForExit)
                process.WaitForExit();

            return process.Output;
        }

        public static IReadOnlyCollection<Output> AwsSamDeploy(AwsSamDeploySettings settings = null)
        {
            settings ??= new AwsSamDeploySettings();
            using var process = ProcessTasks.StartProcess(settings);

            process.AssertZeroExitCode();

            return process.Output;
        }
        public static IReadOnlyCollection<Output> AwsSamDeploy(Configure<AwsSamDeploySettings> configurator)
        {
            return AwsSamDeploy(configurator(new AwsSamDeploySettings()));
        }

        public static IReadOnlyCollection<Output> AwsSamBuild(AwsSamBuildSettings settings = null)
        {
            settings ??= new AwsSamBuildSettings();
            using var process = ProcessTasks.StartProcess(settings);

            process.AssertZeroExitCode();

            return process.Output;
        }
        public static IReadOnlyCollection<Output> AwsSamBuild(Configure<AwsSamBuildSettings> configurator)
        {
            return AwsSamBuild(configurator(new AwsSamBuildSettings()));
        }

    }
}

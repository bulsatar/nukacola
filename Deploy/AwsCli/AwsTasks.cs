using Nuke.Common;
using Nuke.Common.Tooling;
using System.Collections.Generic;


namespace Tool.Deploy.AwsCli
{
    public static partial class AwsCliTasks
    {

        public static string AwsPath =>
            ToolPathResolver.TryGetEnvironmentExecutable("AWS_EXE") ??
            ToolPathResolver.GetPathExecutable("aws");

        internal static void CustomLogger(OutputType type, string output)
        {
            if (type == OutputType.Err)
            {
                Logger.Error(output);
                return;
            }

            Logger.Normal(output);
        }

        public static IReadOnlyCollection<Output> AwsCli(string cliTarget, string arguments, string workingDirectory = null, bool waitForExit = false, bool assertZeroExitCode = true)
        {
            string command = $"aws {cliTarget} {arguments}";
            using var process = ProcessTasks.StartProcess(AwsPath, command, workingDirectory);
            
            if (assertZeroExitCode)
                process.AssertZeroExitCode();
            if (waitForExit)
                process.WaitForExit();           

            return process.Output;
        }

      



    }
}

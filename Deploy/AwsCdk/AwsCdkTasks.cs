using Nuke.Common;
using Nuke.Common.Tooling;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Deploy.AwsCdk
{
    public static partial class AwsCdkTasks
    {
        public static string CdkPath =>
            ToolPathResolver.TryGetEnvironmentExecutable("CDK_EXE") ??
            ToolPathResolver.GetPathExecutable("cdk");

        internal static void CustomLogger(OutputType type, string output)
        {
            if (type == OutputType.Err)
            {
                Logger.Error(output);
                return;
            }

            Logger.Normal(output);
        }

        public static IReadOnlyCollection<Output> AwsCdkSynth(AwsCdkSynthSettings settings)
        {
            settings ??= new AwsCdkSynthSettings();
            using var process = ProcessTasks.StartProcess(settings);

            process.AssertZeroExitCode();

            return process.Output;
        }

        public static IReadOnlyCollection<Output> AwsCdkSynth(Configure<AwsCdkSynthSettings> configurator)
        {
            return AwsCdkSynth(configurator(new AwsCdkSynthSettings()));
        }


        public static IReadOnlyCollection<Output> AwsCdkDeploy(AwsCdkDeploySettings settings)
        {
            settings ??= new AwsCdkDeploySettings();
            using var process = ProcessTasks.StartProcess(settings);

            process.AssertZeroExitCode();

            return process.Output;
        }

        public static IReadOnlyCollection<Output> AwsCdkDeploy(Configure<AwsCdkDeploySettings> configurator)
        {
            return AwsCdkDeploy(configurator(new AwsCdkDeploySettings()));
        }
    }
}

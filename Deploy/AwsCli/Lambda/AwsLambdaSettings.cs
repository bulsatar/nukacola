using Nuke.Common.Tooling;
using System;

namespace Tool.Deploy.AwsCli
{
    public class AwsLambdaSettings : ToolSettings
    {
        public override string ProcessToolPath => base.ProcessToolPath ?? AwsCliTasks.AwsPath;
        public override Action<OutputType, string> ProcessCustomLogger => AwsCliTasks.CustomLogger;

        protected override Arguments ConfigureProcessArguments(Arguments arguments)
        {
            arguments
                .Add("lambda");
            return base.ConfigureProcessArguments(arguments);
        }
    }
}

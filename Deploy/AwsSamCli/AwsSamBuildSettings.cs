using Nuke.Common.Tooling;
using System;

namespace Tool.Deploy.AwsSamCli
{
    [Serializable]
    public class AwsSamBuildSettings : ToolSettings
    {
        public override string ProcessToolPath => base.ProcessToolPath ?? AwsSamTasks.SamPath;
        public override Action<OutputType, string> ProcessCustomLogger => AwsSamTasks.CustomLogger;

        //public virtual string SamTemplateName { get; internal set; }
        public virtual string SamTemplatePathAndName { get; internal set; }

        protected override Arguments ConfigureProcessArguments(Arguments arguments)
        {
            arguments
                .Add("build")
                .Add("-t {value}", SamTemplatePathAndName);

            return base.ConfigureProcessArguments(arguments);
        }
    }

    public static class AwsSamBuildSettingsExtensions
    {
        public static T SetSamTemplatePathAndName<T>(this T toolSettings, string value) where T : AwsSamBuildSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.SamTemplatePathAndName = value;
            return toolSettings;
        }
    }
}

using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tool.Deploy.AwsSamCli
{
    [Serializable]
    public class AwsSamDeploySettings : ToolSettings
    {
        public override string ProcessToolPath => base.ProcessToolPath ?? AwsSamTasks.SamPath;
        public override Action<OutputType, string> ProcessCustomLogger => AwsSamTasks.CustomLogger;

        public virtual string StackName { get; internal set; }
        public virtual string BucketName { get; internal set; }
        public virtual bool NoFailOnEmptyChangeset { get; internal set; } = false;
        public virtual IList<string> Capabilities => CapabilitiesInternal;
        internal IList<string> CapabilitiesInternal { get; set; } = new List<string>();

        public virtual IReadOnlyDictionary<string, string> ParameterPairs => ParameterPairsInternal.AsReadOnly();
        internal Dictionary<string, string> ParameterPairsInternal { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        protected override Arguments ConfigureProcessArguments(Arguments arguments)
        {
            arguments
                .Add("deploy")
                .Add("--stack-name {value}", StackName)
                .Add("--capabilities {value}", CapabilitiesInternal, separator: ' ')
                .Add("--s3-bucket {value}", BucketName)
                .Add("--no-fail-on-empty-changeset", NoFailOnEmptyChangeset)
                .Add("--parameter-overrides {value}", GetParameterOverrides());

            return base.ConfigureProcessArguments(arguments);
        }

        
        private string GetParameterOverrides()
        {
            IEnumerable<string> overrides = ParameterPairsInternal.Select(x => $"ParameterKey={x.Key},ParameterValue=\"{x.Value}\"");

            return string.Join(" ", overrides);
        }

    }

    public static class AwsSamDeploySettingsExtensions
    {
        
        public static T SetBucketName<T>(this T toolSettings, string value) where T : AwsSamDeploySettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.BucketName = value;
            return toolSettings;
        }
        public static T SetStackName<T>(this T toolSettings, string value) where T : AwsSamDeploySettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.StackName = value;
            return toolSettings;
        }
        public static T SetNoFailOnEmptyChangeset<T>(this T toolSettings) where T : AwsSamDeploySettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.NoFailOnEmptyChangeset = true;
            return toolSettings;
        }
        public static T AddCapability<T>(this T toolSettings, string value) where T : AwsSamDeploySettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.CapabilitiesInternal.Add(value);
            return toolSettings;
        }
        public static T AddParameter<T>(this T toolSettings, string parameterName, string parameterValue) where T : AwsSamDeploySettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.ParameterPairsInternal.Add(parameterName, parameterValue);
            return toolSettings;
        }
    }
}

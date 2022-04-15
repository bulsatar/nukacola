using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Tool.Deploy.Utilities;

namespace Tool.Deploy.AwsCdk
{
    [Serializable]
    public class AwsCdkDeploySettings : ToolSettings
    {
        public override string ProcessToolPath => base.ProcessToolPath ?? AwsCdkTasks.CdkPath;
        public override Action<OutputType, string> ProcessCustomLogger => AwsCdkTasks.CustomLogger;

        public bool ForceStackDeploy { get; set; } = false;
        public bool DeployAllStacks { get; set; } = false;
        public virtual IReadOnlyDictionary<string, string> ContextPairs => ContextPairsInternal.AsReadOnly();
        internal Dictionary<string, string> ContextPairsInternal { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public virtual IEnumerable<string> Stacks => StacksInternal.AsReadOnly();
        internal List<string> StacksInternal { get; set; } = new List<string>();

        protected override Arguments ConfigureProcessArguments(Arguments arguments)
        {
            if (DeployAllStacks)
                StacksInternal = new List<string>();

            arguments
                .Add("deploy")
                .Add(StacksInternal)
                .When(ForceStackDeploy, setting => setting.Add("--force"))
                .When(DeployAllStacks, setting => setting.Add("--all"))
                .Add("--require-approval never")
                .AddKeyValuePairs("--context [<key>=\"<value>\"]", ContextPairsInternal);

            return base.ConfigureProcessArguments(arguments);
        }
    }

    public static class AwsCdkDeploySettingsExtensions
    {
        public static T SetContext<T>(this T toolSettings, string contextKey, string contextValue) where T : AwsCdkDeploySettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.ContextPairsInternal.Add(contextKey, contextValue);
            return toolSettings;
        }
        public static T SetForceStackDeploy<T>(this T toolSettings) where T : AwsCdkDeploySettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.ForceStackDeploy = true;
            return toolSettings;
        }

        public static T SetDeployAllStacks<T>(this T toolSettings) where T : AwsCdkDeploySettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.DeployAllStacks = true;
            return toolSettings;
        }

        /// <summary>
        /// Only list each stack if need to deploy separately. Use SetDeployAllStacks instead if no need to separate stack deployments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toolSettings"></param>
        /// <returns></returns>
        public static T AddStack<T>(this T toolSettings, string stackName) where T : AwsCdkDeploySettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.StacksInternal.Add(stackName);
            return toolSettings;
        }
    }
}

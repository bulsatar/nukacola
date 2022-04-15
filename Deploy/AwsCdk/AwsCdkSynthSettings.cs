using Nuke.Common.Tooling;
using Nuke.Common.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using Tool.Deploy.Utilities;

namespace Tool.Deploy.AwsCdk
{
    [Serializable]
    public class AwsCdkSynthSettings : ToolSettings
    {
        public override string ProcessToolPath => base.ProcessToolPath ?? AwsCdkTasks.CdkPath;
        public override Action<OutputType, string> ProcessCustomLogger => AwsCdkTasks.CustomLogger;
        public virtual IReadOnlyDictionary<string, string> ContextPairs => ContextPairsInternal.AsReadOnly();
        internal Dictionary<string, string> ContextPairsInternal { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        public virtual IEnumerable<string> Stacks => StacksInternal.AsReadOnly();
        internal List<string> StacksInternal { get; set; } = new List<string>();

        protected override Arguments ConfigureProcessArguments(Arguments arguments)
        {
            arguments
                .Add("synth")
                .Add(StacksInternal)
                .AddKeyValuePairs("--context [<key>=\"<value>\"]", ContextPairsInternal);

            return base.ConfigureProcessArguments(arguments);
        }
    }

    public static class AwsCdkSynthSettingsExtensions
    {
        public static T SetContext<T>(this T toolSettings, string contextKey, string contextValue) where T : AwsCdkSynthSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.ContextPairsInternal.Add(contextKey, contextValue);
            return toolSettings;
        }

        /// <summary>
        /// Current cdk synth structure creates templates for all, then deletes unused. If there are systemic dependencies between stacks, do not use.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toolSettings"></param>
        /// <param name="stackName"></param>
        /// <returns></returns>
        public static T AddStack<T>(this T toolSettings, string stackName) where T : AwsCdkSynthSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.StacksInternal.Add(stackName);
            return toolSettings;
        }

    }
}

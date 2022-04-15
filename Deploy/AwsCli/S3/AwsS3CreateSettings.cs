using Nuke.Common.Tooling;
using System;


namespace Tool.Deploy.AwsCli
{

    [Serializable]
    public class AwsS3CreateSettings : ToolSettings
    {
        public override string ProcessToolPath => base.ProcessToolPath ?? AwsCliTasks.AwsPath;
        public override Action<OutputType, string> ProcessCustomLogger => AwsCliTasks.CustomLogger;

        public virtual string BucketName { get; internal set; }
        public virtual string Region { get; internal set; }


        protected override Arguments ConfigureProcessArguments(Arguments arguments)
        {
            arguments
                .Add("s3api")
                .Add("create-bucket")
                .Add("--bucket {value}", BucketName)
                .Add("--region {value}", Region)               
                .Add("--create-bucket-configuration LocationConstraint={value}", Region);

            return base.ConfigureProcessArguments(arguments);
        }

    }

    public static class AwsS3SettingsExtensions
    {
        public static T SetBucketName<T>(this T toolSettings, string bucketName) where T : AwsS3CreateSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.BucketName = bucketName;
            return toolSettings;
        }
        public static T SetRegion<T>(this T toolSettings, string region) where T : AwsS3CreateSettings
        {
            toolSettings = toolSettings.NewInstance();
            toolSettings.Region = region;
            return toolSettings;
        }

        
    }
}

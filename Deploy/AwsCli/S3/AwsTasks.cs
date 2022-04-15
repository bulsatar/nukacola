using Nuke.Common.Tooling;
using System;
using System.Collections.Generic;


namespace Tool.Deploy.AwsCli
{
    public static partial class AwsCliTasks
    {        

        public static IReadOnlyCollection<Output> AwsS3Api(AwsS3CreateSettings settings = null)
        {
            settings ??= new AwsS3CreateSettings();
            using var process = ProcessTasks.StartProcess(settings);

            process.AssertZeroExitCode();

            return process.Output;
        }
        public static IReadOnlyCollection<Output> AwsS3Api(Configure<AwsS3CreateSettings> configurator)
        {
            return AwsS3Api(configurator(new AwsS3CreateSettings()));
        }

        /// <summary>
        /// Process with check for existing bucket and bucket status before attempting to create a new bucket
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public static IReadOnlyCollection<Output> AwsS3ApiCreateBucket(AwsS3CreateSettings settings)
        {
            BucketStatus status = AwsS3ApiBucketExists(settings.BucketName);
            if (status == BucketStatus.ExistsAndAccessible)
                status = BucketStatus.Ready;

            IReadOnlyCollection<Output> outputs = null;
            if (status == BucketStatus.DoesNotExist)
            {
                outputs = AwsS3Api(settings);

                status = BucketStatus.Ready;
            }
            
            Utilities.CliHelper.OutputMessageToScreen($"S3 bucket {settings.BucketName} status: {status}");
            if (status != AwsCliTasks.BucketStatus.Ready)
                throw new Exception("Error occurred attempting to prepare S3 bucket. See output above for details");

            return outputs;
        }

        public static IReadOnlyCollection<Output> AwsS3ApiCreateBucket(Configure<AwsS3CreateSettings> configurator)
        {
            return AwsS3ApiCreateBucket(configurator(new AwsS3CreateSettings()));
        }

       


    }
}

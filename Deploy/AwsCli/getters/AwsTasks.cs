using System;
using System.Collections.Generic;
using System.Linq;
using Tool.Deploy.AwsCli.dtos;
using Tool.Deploy.Utilities;
using Nuke.Common.Tooling;
using Newtonsoft.Json;

namespace Tool.Deploy.AwsCli
{
    public static partial class AwsCliTasks
    {
        public enum BucketStatus
        {
            Ready = 0,
            DoesNotExist = 1,
            ExistsAndAccessible = 2,
            ExistsAndNotAccessible = 3
        }
        public static BucketStatus AwsS3ApiBucketExists(string bucketName)
        {
            string command = $"s3api head-bucket --bucket {bucketName}";
            using var process = ProcessTasks.StartProcess(AwsPath, command);
            process.WaitForExit();
            IReadOnlyCollection<Nuke.Common.Tooling.Output> outputs = process.Output;

            if (outputs.Count > 0 && outputs.ToList()[1].Text.Contains("404"))
                return BucketStatus.DoesNotExist;
            if (outputs.Count > 0 && outputs.ToList()[1].Text.Contains("403"))
                return BucketStatus.ExistsAndNotAccessible;


            return BucketStatus.ExistsAndAccessible;
        }

        public static SqsQueue AwsSqsGetQueue(string queueName)
        {
            string url = AwsGetSqsQueueUrl(queueName);
            string command = $"sqs get-queue-attributes --queue-url {url} --attribute-names QueueArn CreatedTimestamp LastModifiedTimestamp MessageRetentionPeriod MaximumMessageSize";
            SqsQueue sqsQueue = QueryFor<SqsQueue>(command, queueName);

            return sqsQueue;
        }

        public static string AwsGetSqsQueueUrl(string queueName)
        {
            string command = $"sqs get-queue-url --queue-name {queueName}";
            SqsQueueUrl url = QueryFor<SqsQueueUrl>(command, queueName);

            return url.QueueUrl;
        }

        public static StateMachine AwsGetStateMachine(string name)
        {
            string command = $"stepfunctions list-state-machines";
            StepFunctions stepFunctions = QueryFor<StepFunctions>(command, name);
            StateMachine stateMachine = stepFunctions.StateMachines.FirstOrDefault(x => x.Name == name);

            return stateMachine;
        }

        public static Lambda AwsGetLambda(string name)
        {
            string command = $"lambda get-function --function-name {name}";
            Lambda lambda = QueryFor<Lambda>(command, name);

            return lambda;
        }

        /// <summary>
        /// Returns a given cloud stack description. Will throw error if stack name not found
        /// </summary>
        /// <param name="stackName"></param>
        /// <returns></returns>
        public static CloudStack AwsGetStackDescriptions(string stackName)
        {
            string command = $"cloudformation describe-stacks --stack-name {stackName}";
            CloudStacks stacks = QueryFor<CloudStacks>(command, stackName);

            return stacks.Stacks[0];
        }


        private static T QueryFor<T>(string command, string name)
        {
            using var process = ProcessTasks.StartProcess(AwsPath, command);
            process.WaitForExit();

            IReadOnlyCollection<Nuke.Common.Tooling.Output> outputs = process.Output;
            string output = outputs.GetFullOutputText();

            if (outputs.ToList()[0].Type == OutputType.Err)
                throw new Exception($"Error querying for {name}. See error: \n {output}");

            return JsonConvert.DeserializeObject<T>(output);
        }
    }
}

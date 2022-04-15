
namespace Tool.Deploy.AwsCli.dtos
{

    public class Attributes
    {
        public string QueueArn { get; set; }
        public string CreatedTimestamp { get; set; }
        public string LastModifiedTimestamp { get; set; }
        public string MessageRetentionPeriod { get; set; }
        public string MaximumMessageSize { get; set; }
    }

    public class SqsQueue
    {
        public Attributes Attributes { get; set; }
    }

    public class SqsQueueUrl
    {
        public string QueueUrl { get; set; }
    }
}

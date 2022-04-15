using Newtonsoft.Json;
using System;


namespace Tool.Deploy.AwsCli.dtos
{
    public class Lambda
    {
        public Configuration Configuration { get; set; }
        public Code Code { get; set; }
        public Tags Tags { get; set; }
    }
    public class TracingConfig
    {
        public string Mode { get; set; }
    }

    public class Configuration
    {
        public string FunctionName { get; set; }
        public string FunctionArn { get; set; }
        public string Runtime { get; set; }
        public string Role { get; set; }
        public string Handler { get; set; }
        public int CodeSize { get; set; }
        public string Description { get; set; }
        public int Timeout { get; set; }
        public int MemorySize { get; set; }
        public DateTime LastModified { get; set; }
        public string CodeSha256 { get; set; }
        public string Version { get; set; }
        public TracingConfig TracingConfig { get; set; }
        public string RevisionId { get; set; }
        public string State { get; set; }
        public string LastUpdateStatus { get; set; }
    }

    public class Code
    {
        public string RepositoryType { get; set; }
        public string Location { get; set; }
    }

    public class Tags
    {
        [JsonProperty("aws:cloudformation:stack-name")]
        public string AwsCloudformationStackName { get; set; }

        [JsonProperty("lambda:createdBy")]
        public string LambdaCreatedBy { get; set; }

        [JsonProperty("aws:cloudformation:stack-id")]
        public string AwsCloudformationStackId { get; set; }

        [JsonProperty("aws:cloudformation:logical-id")]
        public string AwsCloudformationLogicalId { get; set; }
    }

    


}

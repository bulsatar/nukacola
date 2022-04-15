using System;
using System.Collections;
using System.Collections.Generic;

namespace Tool.Deploy.AwsCli.dtos
{
    public class Output
    {
        public string Description { get; set; }
        public string OutputKey { get; set; }
        public string OutputValue { get; set; }
    }

    public class CloudStacks
    {
        public List<CloudStack> Stacks { get; set; } = new List<CloudStack>();
    }

    public class CloudStack
    {
        public string StackId { get; set; }
        public string Description { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Output> Outputs { get; set; }
        public object StackStatusReason { get; set; }
        public DateTime CreationTime { get; set; }
        public List<string> Capabilities { get; set; }
        public string StackName { get; set; }
        public string StackStatus { get; set; }
        public bool DisableRollback { get; set; }
    }

    public class Tag
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}

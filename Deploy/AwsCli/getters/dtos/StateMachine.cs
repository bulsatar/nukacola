using System.Collections.Generic;

namespace Tool.Deploy.AwsCli.dtos
{
    public class StateMachine
    {
        public string StateMachineArn { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double CreationDate { get; set; }
    }

    public class StepFunctions
    {
        public List<StateMachine> StateMachines { get; set; }
    }
}

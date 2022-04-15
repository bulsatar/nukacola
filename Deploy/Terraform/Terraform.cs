using Nuke.Common.Tooling;
using System;
using System.Collections.Generic;

namespace Tool.Deploy.Terraform
{
    public class Terraform
    {

        public static string TerraformPath =>
            ToolPathResolver.TryGetEnvironmentExecutable("TERRAFORM_EXE") ??
            ToolPathResolver.GetPathExecutable("terraform");
        public static IReadOnlyCollection<Output> Init(string workingDirectory)
        {
            using var process = ProcessTasks.StartProcess(TerraformPath, "init", workingDirectory);
            process.AssertZeroExitCode();
            return process.Output;
        }

        public static IReadOnlyCollection<Output> SetWorkspace(string workingDirectory, string workspace = null)
        {
            if (string.IsNullOrWhiteSpace(workspace) || workspace == "local")
                workspace = Environment.UserName;
            using var process = ProcessTasks.StartProcess(TerraformPath, $"workspace select {workspace} || {TerraformPath} workspace new {workspace}", workingDirectory);
            //process.AssertZeroExitCode();   cannot assert zero because an error will popup if workspace doesn't exist stopping execution.
            return process.Output;
        }

        public static IReadOnlyCollection<Output> Plan(string workingDirectory, string options = "")
        {
            string command = $"plan {options}";
            using var process = ProcessTasks.StartProcess(TerraformPath, command, workingDirectory);
            process.AssertZeroExitCode();
            return process.Output;
        }

        public static IReadOnlyCollection<Output> Apply(string workingDirectory, string options = "")
        {
            string command = $"apply -auto-approve {options}";
            using var process = ProcessTasks.StartProcess(TerraformPath, command, workingDirectory);
            process.AssertZeroExitCode();
            return process.Output;
        }

    }
}

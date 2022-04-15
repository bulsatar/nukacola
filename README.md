# NukaCola
NukaCola is a set of extensions for the Nuke c# build system

# Build
Nuke build tool is provided but not needed to build locally. If build tool is preferred:
- From Visual Studio - rebuild and run the _build project
- From command line - navigate to solution folder - run "build"
- From command line - with nuke installed, navigate to solution folder - run "nuke"


# Prerequisites
Before you use this package you must install the relevant underlying sdk. You can find instructions for how to do that here:
 - aws cli - https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2-windows.html
 - aws sdk - https://aws.amazon.com/sdk-for-net/
 - terraform - https://www.terraform.io/downloads.html

You will also need to setup a local set of credentials if you have not already set them up.


For a detailed overview of AWS credential files: https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-files.html


# How to Use
This package was developed to extend the Nuke system to also handle deployments to the cloud via Aws, Sam templates, or Terraform.
To use, access the nuget from either a local repo or artifact stream, add a new deployment target in the Nuke Build.cs (or wherever your build 
targets reside in your build project), and use the AwsCli, AwsSamCli, and Terraform classes to define your deployment. 

*the ci build should not call the deploy target to maintain separation of concerns. Build.cs file should have both a "Pack" and a "Deploy" target, however the "Deploy" 
target can and should be dependent on the pack target. Or the path to the pre-built package should be passed into the deploy target as a variable*

### Sam and Aws Cli Example:
Here is an example of a simple lambda deployment using a mix of Aws cli for managing the S3 bucket and Sam cli for deploying the lambda. 
Add the target to the Build.cs (or wherever your build targets reside in your build project). 

 
```
    Target SamDeploy => ttarget => ttarget
       .DependsOn(Pack)
       .Executes(() =>
       {
           string stackname = Aws.GetSafeName($"{DeployEnv}-deploytesting-{DeployRegion}");
           string bucketname = stackname + "-s3";
           AwsCliTasks.AwsS3ApiCreateBucket(settings => settings
                .SetBucketName(bucketname)
                .SetRegion(DeployRegion)               
           );

           // s3 arn format should be standard
           string bucketarn = $"arn:aws:s3:::{bucketname}";
           string templatepath = $"{Solution.GetProject("deploytesting").Directory}\\serverless.template";
           AwsSamTasks.AwsSamBuild(settings => settings
                .SetSamTemplatePathAndName(templatepath)
           );

           AwsSamTasks.AwsSamDeploy(settings => settings
                .SetBucketName(bucketname)
                .SetNoFailOnEmptyChangeset()
                .SetStackName(stackname)
                .AddCapability("CAPABILITY_IAM")
                .AddCapability("CAPABILITY_AUTO_EXPAND")
                .AddParameter("AwsRole", DeployRoleArn)
                .AddParameter("FunctionName", stackname + "-get")
           );
           

       });
```

### Cdk Examples:
Here is an example of a simple cdk synth and deploy call.  
*Currently only the context options are exposed and integrated*

```
 Target DeployAutoCodeScheduler => ttarget => ttarget
        .DependsOn(Pack)
       .Executes(() =>
       {
            string stackname = $"{DeployEnv}-{DeployAccount}-testapp".GetAwsSafeName();
            string functionName = (stackname + "-fn").GetAwsSafeName();
            string queueName = (stackname + "-q").GetAwsSafeName();
            string eventName = (stackname + "-ebt").GetAwsSafeName();
            string roleName = (stackname + "-iam").GetAwsSafeName();
            string policyName = (stackname + "-policy").GetAwsSafeName();

            AwsCdkTasks.AwsCdkSynth(settings => settings
                .SetProcessWorkingDirectory(Solution.Directory)
                .SetContext("zipFile", GeneratedZip)
                .SetContext("stackName", stackname)
                .SetContext("functionName", functionName)
                .SetContext("eventName", eventName)
                .SetContext("queueName", queueName)
                .SetContext("roleName", roleName)
                .SetContext("policyName", policyName)
                .SetContext("account", DeployAccount)
                .SetContext("environment", DeployEnv)
                .SetContext("region", DeployRegion)
                .SetContext("kms", DeployKmsArn)
                .SetContext("vpcId", DeployVpcId)
            );
            AwsCdkTasks.AwsCdkDeploy(settings => settings
                .SetProcessWorkingDirectory(Solution.Directory)
            );           
       });
```

### Terraform Example:
The Terraform tool is not yet setup for using a fluent interface. Because Terraform takes the management of variables, arns, and such out of the c# scope the calls
to terraform are simpler. However that means all of that must be managed independently
Here is an example of the current interface:
```
    Target TerraformDeploy => ttarget => ttarget
        .DependsOn(Pack)
        .Executes(() =>
        {
            AbsolutePath workingdir = "sonekind of path management to your terraform templates";
            Tool.Deploy.Terraform.Terraform.Init(workingdir);
            Tool.Deploy.Terraform.Terraform.SetWorkspace(workingdir, "somekind of workspace management");
            Tool.Deploy.Terraform.Terraform.Plan(workingdir, "somekind of options management");
            Tool.Deploy.Terraform.Terraform.Apply(workingdir, "somekind of options management");
        });
```

# References
Aws Cli
 - https://awscli.amazonaws.com/v2/documentation/api/latest/index.html
 
Sam Cli
 - https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-sam-cli-command-reference.html

Terraform
 - https://www.terraform.io/

# Contribute
There are hundreds of calls that can be made on the Aws Cli. I don't know if all of them are needed but many could be added. The handful of Sam Cli items should also be carried over
and established in the existing fluent interface. There must also be a way to expand the Terraform calls into a fluent interface so a giant bulk of parameter management can be done 
through build variables instead of through Terraform template manipulation.
using Newtonsoft.Json.Linq;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nuke.Common.Tools.DotNet;

namespace Tool.Deploy.Utilities
{
    public class BuildHelpers
    {
        public static void UpdateJsonFile(string filepath, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                return;

            Console.WriteLine($"writing {key}:{value} to {filepath}");
            string jsonString = File.ReadAllText(filepath);
            JObject jObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString) as JObject;
            JToken jToken = jObject.SelectToken(key);
            if (jToken != null)
                jToken.Replace(value);
            string updatedJsonString = jObject.ToString();
            File.WriteAllText(filepath, updatedJsonString);
        }

        public static void UpdateJsonFiles(IEnumerable<string> filePaths, string key, string value)
        {
            filePaths.ToList().ForEach(x => UpdateJsonFile(x, key, value));
        }

        public static T GetValueFromJsonFile<T>(string filepath, string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return default;

            string jsonString = File.ReadAllText(filepath);
            JObject jObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString) as JObject;
            JToken jToken = jObject.SelectToken(key);

            return jToken.Value<T>();
        }

        public static string ZipProject(Project project, string buildNumber, AbsolutePath buildDir, AbsolutePath outputDir, Configuration config)
        {
            string filename = project.Name + buildNumber + ".zip";
            AbsolutePath builddir = buildDir / project.Name;
            AbsolutePath zipartifact = outputDir / filename;

            DotNetTasks.DotNetPublish(pack => pack
                   .SetProject(project)
                   .SetOutput(builddir)
                   .AddProperty("buildNumber", buildNumber)
                   .EnableNoRestore()
                   .EnableNoBuild()
                   .SetConfiguration(config)
               );

            CompressionTasks.CompressZip(builddir, zipartifact);

            return zipartifact;
        }
    }
}

using Nuke.Common.Tooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tool.Deploy.Utilities
{
    public static class ArgumentExtensions
    {
        public enum RepeatFormat
        {
            RepeatKeyValuePairs,
            RepeatEntireCommand
        }
        /// <summary>
        /// Adds each key value pairs with the designated format 
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="commandformat">use [...<key>...<value>...] to designate format. examples: 
        /// --context [<key>=<value>]
        /// --parameter-overrides [ParameterKey=<key>,ParameterValue=\"<value>\"]</param>
        /// <param name="keyValuePairs"></param>
        /// <param name="repeatFormat">Designates if content in [] needs repeated (RepeatKeyValuePairs) or if entire command format needs repeated (RepeatEntireCommand) for each key value pair</param>
        /// <param name="keyValuePairSeparator">when RepeatKeyValuePairs is chosen, separates each key value pair in the generated string</param>
        /// <returns></returns>
        public static Arguments AddKeyValuePairs(this Arguments arguments, string commandformat, 
            Dictionary<string, string> keyValuePairs, 
            RepeatFormat repeatFormat = RepeatFormat.RepeatEntireCommand, 
            string keyValuePairSeparator = " ")
        {
            if (keyValuePairs == null || keyValuePairs.Count == 0)
                return arguments;
            if (!IsValidFormat(commandformat))
                throw new ArgumentException("invalid format for AddKeyValuePairs");

            if (repeatFormat == RepeatFormat.RepeatEntireCommand)
                PopulateRepeatedCommand(ref arguments, commandformat, keyValuePairs);
            if (repeatFormat == RepeatFormat.RepeatKeyValuePairs)
                PopulateRepeatedKeyValuePairs(ref arguments, commandformat, keyValuePairs, keyValuePairSeparator);            

            return arguments;
        }

        public static Arguments Add(this Arguments arguments, IEnumerable<string> items)
        {
            if (items == null || items.Count() == 0)
                return arguments;

            items.ToList().ForEach(x => arguments.Add(x));

            return arguments;
        }

        private static void PopulateRepeatedCommand(ref Arguments arguments, string commandformat, 
            Dictionary<string, string> keyValuePairs)
        {
            GetCommandParts(commandformat, out string command, out string repeatpart);

            foreach (KeyValuePair<string, string> kp in keyValuePairs)
            {
                string replacement = repeatpart.Replace("<key>", kp.Key)
                    .Replace("<value>", kp.Value)
                    .Replace("[","")
                    .Replace("]","");
                arguments.Add(command, replacement);
            }
        }

        private static void PopulateRepeatedKeyValuePairs(ref Arguments arguments, string commandformat, 
            Dictionary<string, string> keyValuePairs, string keyValuePairSeparator)
        {
            GetCommandParts(commandformat, out string command, out string repeatpart);
            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<string, string> kp in keyValuePairs)
            {
                builder.Append(repeatpart.Replace("<key>", kp.Key)
                    .Replace("<value>", kp.Value)
                    .Replace("[", "")
                    .Replace("]", ""));
                builder.Append(keyValuePairSeparator);
            }

            arguments.Add(command, builder.ToString().Trim());
        }

        private static void GetCommandParts(string commandformat, out string command, out string repeatpart)
        {
            int firstIndex = commandformat.IndexOf('[');
            int lastIndex = commandformat.LastIndexOf(']') + 1;
            repeatpart = commandformat.Substring(firstIndex, lastIndex - firstIndex);
            command = commandformat.Replace(repeatpart, "{value}");

        }

        private static bool IsValidFormat(string commandformat)
        {
            return (commandformat.Contains("<key>") 
                && commandformat.Contains("<value>") 
                && commandformat.Contains("[") 
                && commandformat.Contains("]"));
        }
    }
}

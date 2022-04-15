using Nuke.Common.Tooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuke.Tool.Utilities
{
    public static class OutputExtensions
    {
        public static string GetFullOutputText(this IReadOnlyCollection<Output> outputs, char separator = ' ')
        {
            IEnumerable<string> alltext = outputs.ToList().Select(x => x.Text);
            return string.Join(separator, alltext);
        }
    }
}

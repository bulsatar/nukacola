using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tool.Deploy.Utilities
{
    public static class StringExtensions
    {

        public static string GetAwsSafeName(this string name)
        {
            name = Regex.Replace(name, @"([^A-Za-z0-9-])", "");
            name = name.Substring(0, Math.Min(63, name.Length));
            return name.ToLower();
        }
    }
}

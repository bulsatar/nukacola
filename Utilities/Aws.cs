using System;
using System.Collections.Generic;
using System.Text;

namespace Nuke.Tool.Utilities
{
    public static class Aws
    {
        public static string GetSafeName(string name)
        {
            return name.Replace("_", "").Replace(".", "").ToLower();
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nuke.Common.Tooling;
using System;
using System.Collections.Generic;
using Tool.Deploy.Utilities;

namespace UnitTests
{
    [TestClass]
    public class utilitytests
    {
        [TestMethod]
        public void GetSafeName_valid()
        {
            string test = "safe-name";
            Assert.IsTrue(test.GetAwsSafeName() == "safe-name");
        }

        [TestMethod]
        public void GetSafeName_invalidchar()
        {
            string test = "not_safe-name";
            Assert.IsTrue(test.GetAwsSafeName() == "notsafe-name");
        }

        [TestMethod]
        public void GetSafeName_invalidcaps()
        {
            string test = "Not_safe-name";
            Assert.IsTrue(test.GetAwsSafeName() == "notsafe-name");
        }

        [TestMethod]
        public void GetSafeName_invalidlength()
        {
            string test = "Notsafe-nameakjdfjasdfiasdfisadfiasdfiiasdfiasdifiasdfiiasdfiajd";
            Assert.IsTrue(test.GetAwsSafeName() == "notsafe-nameakjdfjasdfiasdfisadfiasdfiiasdfiasdifiasdfiiasdfiaj");
        }



        [TestMethod]
        public void ArgumentExtensions_AddKeyValuePairs_validRepeatEntireCommand()
        {
            string format = "--context [<key>=<value>]";
            Dictionary<string, string> pairs = new Dictionary<string, string>() { { "testkey", "testvalue" }, { "testkey2", "testvalue2" } };
            Arguments arguments = new Arguments();
            arguments.AddKeyValuePairs(format, pairs, ArgumentExtensions.RepeatFormat.RepeatEntireCommand);

            Assert.IsTrue(arguments.RenderForExecution() == "--context testkey=testvalue --context testkey2=testvalue2");
        }

        [TestMethod]
        public void ArgumentExtensions_AddKeyValuePairs_validRepeatKeyValuePairs()
        {
            string format = "--context [<key>=<value>]";
            Dictionary<string, string> pairs = new Dictionary<string, string>() { { "testkey", "testvalue" }, { "testkey2", "testvalue2" } };
            Arguments arguments = new Arguments();
            arguments.AddKeyValuePairs(format, pairs, ArgumentExtensions.RepeatFormat.RepeatKeyValuePairs);

            Assert.IsTrue(arguments.RenderForExecution() == "--context \"testkey=testvalue testkey2=testvalue2\"");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "invalid format for AddKeyValuePairs")]
        public void ArgumentExtensions_AddKeyValuePairs_invalidRepeatEntireCommand()
        {
            string format = "--context [=<value>]";
            Dictionary<string, string> pairs = new Dictionary<string, string>() { { "testkey", "testvalue" }, { "testkey2", "testvalue2" } };
            Arguments arguments = new Arguments();
            arguments.AddKeyValuePairs(format, pairs, ArgumentExtensions.RepeatFormat.RepeatEntireCommand);
        }

        [TestMethod]
        public void ArgumentExtensions_AddKeyValuePairs_empty()
        {
            string format = "--context [<key>=<value>]";
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            Arguments arguments = new Arguments();
            arguments.AddKeyValuePairs(format, pairs, ArgumentExtensions.RepeatFormat.RepeatKeyValuePairs);

            Assert.IsTrue(arguments.RenderForExecution() == "");
        }

        [TestMethod]
        public void ArgumentExtensions_AddKeyValuePairs_null()
        {
            string format = "--context [<key>=<value>]";
            Dictionary<string, string> pairs = null;
            Arguments arguments = new Arguments();
            arguments.AddKeyValuePairs(format, pairs, ArgumentExtensions.RepeatFormat.RepeatKeyValuePairs);

            Assert.IsTrue(arguments.RenderForExecution() == "");
        }

        [TestMethod]
        public void ArgumentExtensions_Addmultiple_valid()
        {
            List<string> list = new List<string>() { "test1", "test2" };
            Arguments arguments = new Arguments();
            arguments.Add(list);

            Assert.IsTrue(arguments.RenderForExecution() == "test1 test2");
        }

        [TestMethod]
        public void ArgumentExtensions_Addmultiple_empty()
        {
            List<string> list = new List<string>() ;
            Arguments arguments = new Arguments();
            arguments.Add(list);

            Assert.IsTrue(arguments.RenderForExecution() == "");
        }

        [TestMethod]
        public void ArgumentExtensions_Addmultiple_null()
        {
            List<string> list = null;
            Arguments arguments = new Arguments();
            arguments.Add(list);

            Assert.IsTrue(arguments.RenderForExecution() == "");
        }

    }
}

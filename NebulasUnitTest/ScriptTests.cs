using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;


namespace NebulasUnitTest
{
    [TestClass]
    public class ScriptTests
    {
        Nebulas.Scripting.RubyCore mRubyCore;

        public ScriptTests()
        {
            mRubyCore = new Nebulas.Scripting.RubyCore();
            mRubyCore.Initialize();
        }
        
        [TestMethod]
        public void IronRubySanity()
        {
            String result = "hello world";
            result = mRubyCore.SanityTest(result, "changed");
            Assert.IsTrue(result == "changed");
        }

        [TestMethod]
        public void StringExecution()
        {
            Assert.AreEqual(null, mRubyCore.GetAsInteger("test_variable"));
            mRubyCore.SetVariable("test_variable", 0);
            mRubyCore.ExecString("self.test_variable = 42");
            Assert.AreEqual(42, mRubyCore.GetAsInteger("test_variable"));
        }

        [TestMethod]
        public void FileExecution()
        {
            Assert.AreEqual(null, mRubyCore.GetAsInteger("test_variable"));
            mRubyCore.ExecFile("simple_test.rb");
            Assert.AreEqual(42, mRubyCore.GetAsInteger("test_variable"));
        }

        [TestMethod]
        public void StringPatching()
        {
            Nebulas.Events.Event evt = new Nebulas.Events.Event();
            evt.SetProperty("patch_test_variable", "42");
            Assert.AreEqual(null, mRubyCore.GetAsInteger("event['patch_test_variable']"));
            mRubyCore.SetContext(evt);
            Dictionary<String, String> prop = (Dictionary<String, String>) mRubyCore.GetVariable("event");
            Assert.AreEqual("42", prop["patch_test_variable"]);
            mRubyCore.ExecString("self.event[\"patch_test_variable\"] = '72'");
            //Assert.AreEqual(72, mRubyCore.GetAsInteger("patch_test_variable"));
            mRubyCore.ExtractContext(evt);
            Assert.AreEqual("72", evt.GetProperty("patch_test_variable"));
        }

        [TestMethod]
        public void FilePatching()
        {
            Nebulas.Events.Event evt = new Nebulas.Events.Event();
            evt.SetProperty("patch_test2_variable", "42");
            Assert.AreEqual(null, mRubyCore.GetAsInteger("event.patch_test2_variable"));
            mRubyCore.SetContext(evt);
            //Assert.AreEqual(42, mRubyCore.GetAsInteger("event.patch_test2_variable"));
            Dictionary<String, String> prop = (Dictionary<String, String>)mRubyCore.GetVariable("event");
            Assert.AreEqual("42", prop["patch_test2_variable"]);
            mRubyCore.ExecFile("file_patch_test.rb");
            //Assert.AreEqual("72", mRubyCore.GetAsInteger("event.patch_test2_variable"));
            mRubyCore.ExtractContext(evt);
            Assert.AreEqual("72", evt.GetProperty("patch_test2_variable"));
        }

        [TestMethod]
        public void AutoExec()
        {
            Assert.AreEqual(null, mRubyCore.GetAsInteger("patch_test3_variable"));
            mRubyCore.Exec("self.patch_test3_variable = 80");
            Assert.AreEqual(80, mRubyCore.GetAsInteger("patch_test3_variable"));
            mRubyCore.Exec("patch_test.rb");
            //Not a typo, use patch 2:
            Assert.AreEqual(72, mRubyCore.GetAsInteger("patch_test2_variable"));
        }

        [TestMethod]
        public void VariablePersistance()
        {
            Assert.AreEqual(null, mRubyCore.GetAsInteger("persist_test_variable"));
            mRubyCore.Exec("self.persist_test_variable = 2");
            Assert.AreEqual(2, mRubyCore.GetAsInteger("persist_test_variable"));
            mRubyCore.Exec("self.persist_test_variable = 4");
            Assert.AreEqual(4, mRubyCore.GetAsInteger("persist_test_variable"));
            mRubyCore.Exec("persist_test.rb");
            Assert.AreEqual(6, mRubyCore.GetAsInteger("persist_test_variable"));
        }

        [TestMethod]
        public void LocalVariables()
        {
            Assert.AreEqual(null, mRubyCore.GetLocal("local_var"));
            mRubyCore.SetLocal("local_var", "five");
            Assert.AreEqual("five", mRubyCore.GetLocal("local_var"));
            mRubyCore.Run("local_var = \"six\"");
            Assert.AreEqual("six", mRubyCore.GetLocal("local_var"));
            mRubyCore.Run("secret_var = \"5\"");
            Assert.AreEqual("5", mRubyCore.GetLocal("secret_var"));
        }

        [TestMethod]
        public void DictionaryExportImport()
        {
            Dictionary<String, String> data = new Dictionary<string, string>();
            data.Add("str", "One");
            data.Add("num", "72");
            mRubyCore.ExportDictionary("dict", data);

            mRubyCore.Run("dict[\"str\"] = \"Two\"");
            mRubyCore.Run("dict[\"num\"] = 42");

            Dictionary<String, String> result = mRubyCore.ImportDictionary("dict");
            String str;
            String num;
            result.TryGetValue("str", out str);
            result.TryGetValue("num", out num);
            Assert.AreEqual("Two", str);
            Assert.AreEqual("42", num);

        }
    }
}

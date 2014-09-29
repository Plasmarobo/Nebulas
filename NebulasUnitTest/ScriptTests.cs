using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            mRubyCore.ExecFile("scripts/simple_test.rb");
            Assert.AreEqual(42, mRubyCore.GetAsInteger("test_variable"));
        }

        [TestMethod]
        public void StringPatching()
        {
            Nebulas.Events.Event evt = new Nebulas.Events.Event();
            evt.SetProperty("patch_test_variable", "42");
            Assert.AreEqual(null, mRubyCore.GetAsInteger("patch_test_variable"));
            mRubyCore.SetContext(evt);
            Assert.AreEqual(42, mRubyCore.GetAsInteger("patch_test_variable"));
            mRubyCore.ExecString("self.patch_test_variable = 72");
            Assert.AreEqual(72, mRubyCore.GetAsInteger("patch_test_variable"));
            mRubyCore.ExtractContext(evt);
            Assert.AreEqual(72, evt.GetProperty("patch_test_variable"));
        }

        [TestMethod]
        public void FilePatching()
        {
            Nebulas.Events.Event evt = new Nebulas.Events.Event();
            evt.SetProperty("patch_test2_variable", "42");
            Assert.AreEqual(null, mRubyCore.GetAsInteger("patch_test2_variable"));
            mRubyCore.SetContext(evt);
            Assert.AreEqual(42, mRubyCore.GetAsInteger("patch_test2_variable"));
            mRubyCore.ExecFile("scripts/patch_test.rb");
            Assert.AreEqual(72, mRubyCore.GetAsInteger("patch_test2_variable"));
            mRubyCore.ExtractContext(evt);
            Assert.AreEqual(72, evt.GetProperty("patch_test2_variable"));
        }

        [TestMethod]
        public void AutoExec()
        {
            Assert.AreEqual(null, mRubyCore.GetAsInteger("patch_test3_variable"));
            mRubyCore.Exec("self.patch_test3_variable = 80");
            Assert.AreEqual(80, mRubyCore.GetAsInteger("patch_test3_variable"));
            mRubyCore.Exec("scripts/patch_test.rb");
            //Not a typo, use patch 2:
            Assert.AreEqual(72, mRubyCore.GetAsInteger("patch_test2_variable"));
        }
    }
}

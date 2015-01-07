using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;
using Nebulas.Network;
using System.Diagnostics;

namespace NebulasUnitTest
{
    /// <summary>
    /// Tests network loopback channel
    /// </summary>
    [TestClass]
    public class NetworkTest
    {

        Client mClient;
        Server mServer;
        public NetworkTest()
        {
            
            //mClient = new Client("127.0.0.1");
            //mServer = new Server();
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        protected Process StartProgram(String target)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            // Enter in the command line arguments, everything you would enter after the executable name itself
            //start.Arguments = arguments;
            // Enter the executable to run, including the complete path
            start.FileName = target;
            // Do you want to show a console window?
            start.WindowStyle = ProcessWindowStyle.Normal;
            start.CreateNoWindow = false;
            return Process.Start(start);
        }

        [TestMethod]
        public void ServerTest()
        {
            this.mServer = new Server();
            Process p = StartProgram("NebulasDummyClient.exe");
            Assert.IsTrue(this.mServer.Test());
            mServer.Destroy();
            p.Close();
        }
        [TestMethod]
        public void ClientTest()
        {
            Nebulas.Network.Client mClient = new Client("127.0.0.1");
            Process p = StartProgram("NebulasDummyServer.exe");
            mClient.Test();
            mClient.Destroy();
            p.Close();
        }


        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NebulasDummyServer
{
    class Program
    {
        static Nebulas.Network.Server mServer;

        static void Main(string[] args)
        {
            mServer = new Nebulas.Network.Server();
            mServer.Test();
        }
    }
}

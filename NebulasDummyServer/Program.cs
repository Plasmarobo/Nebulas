using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nebulas.Network;

namespace NebulasDummyServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server mServer = new Server();
            mServer.Test();
        }
    }
}

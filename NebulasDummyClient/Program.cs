using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nebulas.Network;

namespace NebulasDummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Client mClient = new Client("127.0.0.1");
            mClient.Test();
        }
    }
}

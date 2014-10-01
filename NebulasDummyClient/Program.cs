using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NebulasDummyClient
{
    class Program
    {
        static Nebulas.Network.Client mClient;
        static void Main(string[] args)
        {
            mClient = new Nebulas.Network.Client("127.0.0.1");
            mClient.Test();
        }
    }
}

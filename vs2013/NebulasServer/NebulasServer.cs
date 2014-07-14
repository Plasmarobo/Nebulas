using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;


namespace NebulasServer
{
    public class NebulasClient : NetConnection
    {
        String mClientAuth;
        String mClientName;
    }
    public class NebulasServer : IDisposable
    {
        static String mNetAppName = "Nebulas";
        NetPeerConfiguration mConfig;
        NetServer mServer;
        List<NebulasClient> mClients;
        Nebulas.Events.EventStream mEventMaster;
        DateTime mCurrentTimestamp;
        
        public NebulasServer()
        {
            mConfig = new NetPeerConfiguration(mNetAppName);
            mConfig.Port = 87578;
            mClients = new List<NebulasClient>();
            mServer = new NetServer(mConfig);
            mServer.Start();
            
        }
        public void Run();


        public void Listen()
        {
            NetIncomingMessage msg;
            while ((msg = mServer.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(msg.ReadString());
                        break;
                    default:
                        Console.WriteLine("Unhandled type: " + msg.MessageType);
                        break;
                }
                mServer.Recycle(msg);
            }
        }
        public void SendUpdates()
        {
            NetOutgoingMessage sendMsg = mServer.CreateMessage();
            sendMsg.Write("Hello");
            sendMsg.Write(42);
            mServer.SendMessage(sendMsg, mClients[0], NetDeliveryMethod.ReliableOrdered);
        }
        public void AuthenticateClient()
        {

        }
        public void Thing()
        {
        }
    }
}

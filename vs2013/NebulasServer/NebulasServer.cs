using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NebulasServer
{
    public class NebulasClient
    {

    }
    public class NebulasServer : IDisposable
    {
        static String mNetAppName = "Nebulas";
        NetPeerConfiguration mConfig;
        NetServer mServer;
        public NebulasServer()
        {
            mConfig = new NetPeerConfiguration(mNetAppName);
            mConfig.Port = 87578;

            mServer = new NetServer(mConfig);
            mServer.Start();
        }
        public void Run();
    }

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
    public void SendUpdates()
    {
        NetOutgoingMessage sendMsg = mServer.CreateMessage();
        sendMsg.Write("Hello");
        sendMsg.Write(42);
 
        mServer.SendMessage(sendMsg, recipient, NetDeliveryMethod.ReliableOrdered);
    }
    public void AuthenticateClient()
    {

    }
    public void 
    
}

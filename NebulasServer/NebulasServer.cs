using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;


namespace NebulasServer
{
    public class GameUser
    {
        String mClientAuth;
        String mClientName;
        NetConnection mLink;

        public GameUser(NetConnection link)
        {
            mLink = link;
            mClientAuth = "Error_Auth";
            mClientName = "Error_Name";
        }

        public GameUser(String name, String auth, NetConnection link)
        {
            mClientAuth = auth;
            mClientName = name;
            mLink = link;
        }

        public NetConnection Link()
        {
            return mLink;
        }

    }
    public class GameServer : IDisposable
    {
        static String mNetAppName = "Nebulas";
        NetPeerConfiguration mConfig;
        NetServer mServer;
        List<GameUser> mClients;
        Nebulas.Events.EventStream mEventMaster;
        DateTime mCurrentTimestamp;
        
        public GameServer()
        {
            mConfig = new NetPeerConfiguration(mNetAppName);
            mConfig.Port = 8578;
            mClients = new List<GameUser>();
            mServer = new NetServer(mConfig);
            mServer.Start();
        }
        public void Run()
        {

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
        }
        public void SendUpdates()
        {
            NetOutgoingMessage sendMsg = mServer.CreateMessage();
            sendMsg.Write("Hello");
            
            sendMsg.Write(42);
            mServer.SendMessage(sendMsg, mClients[0].Link(), NetDeliveryMethod.ReliableOrdered);
        }
        public void AuthenticateClient()
        {

        }
        public void Thing()
        {
        }

        public void Dispose()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Nebulas
{
    namespace Network
    {

        enum PacketTypes
        {
            LOGIN,
        }
        
        public class Identifiers
        {
            public static String Application = "Nebulas";
            public static Int16 Port = 8998;
        }

        public class Client
        {
            // Client Object
            NetClient mClient;
            NetPeerConfiguration mConfig;
            string mServerIP;
            // Create timer that tells client, when to send update
            System.Timers.Timer update;

            // Indicates if program is running
            bool IsRunning = true;

            public Client(string ip)
            {
                mServerIP = ip;
                mConfig = new NetPeerConfiguration(Nebulas.Network.Identifiers.Application);
                mClient = new NetClient(mConfig);
            }

            public Client(System.Net.IPAddress ip)
            {
                mServerIP = ip.ToString();
                mConfig = new NetPeerConfiguration(Nebulas.Network.Identifiers.Application);
                mClient = new NetClient(mConfig);
            }

            public void Test()
            {
                // Create new outgoing message
                NetOutgoingMessage outmsg = mClient.CreateMessage();


                //LoginPacket lp = new LoginPacket("Katu");

                // Start client
                mClient.Start();

                // Write byte ( first byte informs server about the message type ) ( This way we know, what kind of variables to read )
                outmsg.Write((byte)PacketTypes.LOGIN);

                // Write String "Name" . Not used, but just showing how to do it
                outmsg.Write("USERNAME");

                // Connect client, to ip previously requested from user 
                mClient.Connect(mServerIP, Nebulas.Network.Identifiers.Port, outmsg);


                Console.WriteLine("Client Started");

                // Set timer to tick every 50ms
                update = new System.Timers.Timer(50);

                // When time has elapsed ( 50ms in this case ), call "update_Elapsed" funtion
                update.Elapsed += new System.Timers.ElapsedEventHandler(update_Elapsed);

                // Funtion that waits for connection approval info from server
                WaitForStartingInfo();
                SendEcho();
                // Start the timer
                update.Start();

                // While..running
                while (IsRunning)
                {
                    // Just loop this like madman
                    //GetInputAndSendItToServer();
                    System.Threading.Thread.Yield();

                }
            }

            /// <summary>
            /// Every 50ms this is fired
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void update_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                // Check if server sent new messages
                CheckServerMessages();

                // Draw the world
                //DrawGameState();
            }

            // Before main looping starts, we loop here and wait for approval message
            private void WaitForStartingInfo()
            {
                // When this is set to true, we are approved and ready to go
                bool CanStart = false;

                // New incomgin message
                NetIncomingMessage inc;

                // Loop untill we are approved
                while (!CanStart)
                {

                    // If new messages arrived
                    if ((inc = mClient.ReadMessage()) != null)
                    {
                        // Switch based on the message types
                        switch (inc.MessageType)
                        {
                            case NetIncomingMessageType.StatusChanged:
                                NetConnectionStatus status = (NetConnectionStatus)inc.ReadByte();
                                string reason = inc.ReadString();
                                Console.WriteLine("New status: " + status + " (" + reason + ")");

                                if (status == NetConnectionStatus.Connected)
                                {
                                    CanStart = true;
                                }
                                else 
                                {
                                    //Catastrophic failure
                                    return;
                                }
                                break;
                            
                            // All manually sent messages are type of "Data"
                            case NetIncomingMessageType.Data:

                                // Read the first byte
                                // This way we can separate packets from each others
                                if (inc.ReadString() == "echo")
                                {

                                    Console.WriteLine("Echo Received");
                                }
                                break;

                            default:
                                // Should not happen and if happens, don't care
                                Console.WriteLine(inc.ReadString() + " Strange message");
                                break;
                        }
                        mClient.Recycle(inc);
                    }
                }
            }


            /// <summary>
            /// Check for new incoming messages from server
            /// </summary>
            private void CheckServerMessages()
            {
                // Create new incoming message holder
                NetIncomingMessage inc;

                // While theres new messages
                //
                // THIS is exactly the same as in WaitForStartingInfo() function
                // Check if its Data message
                // If its WorldState, read all the characters to list
                while ((inc = mClient.ReadMessage()) != null)
                {
                    if (inc.MessageType == NetIncomingMessageType.Data)
                    {
                        /*if (inc.ReadByte() == (byte)PacketTypes.WORLDSTATE)
                        {
                            Console.WriteLine("World State uppaus");
                            GameStateList.Clear();
                            int jii = 0;
                            jii = inc.ReadInt32();
                            for (int i = 0; i < jii; i++)
                            {
                                Character ch = new Character();
                                inc.ReadAllProperties(ch);
                                GameStateList.Add(ch);
                            }
                        }*/
                    }
                }
            }

            public void SendEcho()
            {
                // Create new message
                NetOutgoingMessage outmsg = mClient.CreateMessage();

                // Write byte = Set "MOVE" as packet type
                outmsg.Write("echo");

                // Send it to server
                mClient.SendMessage(outmsg, NetDeliveryMethod.ReliableOrdered);


            }


        }

        public class Server
        {
            // Server object
            NetServer mServer;
            // Configuration object
            NetPeerConfiguration mConfig;

            public Server()
            {
                mConfig = new NetPeerConfiguration(Nebulas.Network.Identifiers.Application);
                mConfig.Port = Nebulas.Network.Identifiers.Port;

                mConfig.MaximumConnections = 200;
                mConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                mServer = new NetServer(mConfig);
                mServer.Start();
            }

            public void Test()
            {
                
                // Object that can be used to store and read messages
                NetIncomingMessage inc;

                // Check time
                DateTime time = DateTime.Now;

                // Create timespan of 30ms
                TimeSpan timetopass = new TimeSpan(0, 0, 0, 0, 30);

                // Write to con..
                Console.WriteLine("Waiting for new connections and updateing world state to current ones");

                // Main loop
                // This kind of loop can't be made in XNA. In there, its basically same, but without while
                // Or maybe it could be while(new messages)
                while (true)
                {
                    // Server.ReadMessage() Returns new messages, that have not yet been read.
                    // If "inc" is null -> ReadMessage returned null -> Its null, so dont do this :)
                    if ((inc = mServer.ReadMessage()) != null)
                    {
                        // Theres few different types of messages. To simplify this process, i left only 2 of em here
                        Console.WriteLine(inc.MessageType.ToString());
                        switch (inc.MessageType)
                        {
                            // If incoming message is Request for connection approval
                            // This is the very first packet/message that is sent from client
                            // Here you can do new player initialisation stuff
                            case NetIncomingMessageType.ConnectionApproval:

                                // Read the first byte of the packet
                                // ( Enums can be casted to bytes, so it be used to make bytes human readable )
                                if (inc.ReadByte() == (byte)PacketTypes.LOGIN)
                                {
                                    Console.WriteLine("Incoming LOGIN");

                                    // Approve clients connection ( Its sort of agreenment. "You can be my client and i will host you" )
                                    inc.SenderConnection.Approve();

                                    // Create message, that can be written and sent
                                    NetOutgoingMessage outmsg = mServer.CreateMessage();

                                    // first we write byte
                                    outmsg.Write("echo");
                                    // Reliably means, that each packet arrives in same order they were sent. Its slower than unreliable, but easyest to understand
                                    mServer.SendMessage(outmsg, inc.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);

                                    // Debug
                                    Console.WriteLine("Approved new connection and updated the world status");
                                }
                                else 
                                {
                                    inc.SenderConnection.Deny();
                                }

                                break;
                            // Data type is all messages manually sent from client
                            // ( Approval is automated process )
                            case NetIncomingMessageType.Data:

                                // Read first byte
                                //if (inc.ReadByte() == (byte)PacketTypes.MOVE)
                                {
                                    Console.WriteLine("Sending Echo");
                                    NetOutgoingMessage outmsg = mServer.CreateMessage();
                                    outmsg.Write("echo");
                                    // Send messsage to clients ( All connections, in reliable order, channel 0)
                                    mServer.SendMessage(outmsg, mServer.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                                }
                                break;
                            case NetIncomingMessageType.StatusChanged:
                                // In case status changed
                                // It can be one of these
                                // NetConnectionStatus.Connected;
                                // NetConnectionStatus.Connecting;
                                // NetConnectionStatus.Disconnected;
                                // NetConnectionStatus.Disconnecting;
                                // NetConnectionStatus.None;

                                // NOTE: Disconnecting and Disconnected are not instant unless client is shutdown with disconnect()
                                Console.WriteLine(inc.SenderConnection.ToString() + " status changed. " + (NetConnectionStatus)inc.SenderConnection.Status);
                                if (inc.SenderConnection.Status == NetConnectionStatus.Disconnected || inc.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                                {

                                }
                                break;
                            default:
                                // As i statet previously, theres few other kind of messages also, but i dont cover those in this example
                                // Uncommenting next line, informs you, when ever some other kind of message is received
                                Console.WriteLine("Unhandled Message");
                                break;
                        }
                        mServer.Recycle(inc);
                    } // If New messages

                    // if 30ms has passed
                    /*if ((time + timetopass) < DateTime.Now)
                    {
                        // If there is even 1 client
                        if (mServer.ConnectionsCount != 0)
                        {
                            // Create new message
                            NetOutgoingMessage outmsg = mServer.CreateMessage();

                            // Write byte
                            outmsg.Write((byte)PacketTypes.WORLDSTATE);

                            // Write Int
                            outmsg.Write(GameWorldState.Count);

                            // Iterate throught all the players in game
                            foreach (Character ch2 in GameWorldState)
                            {

                                // Write all properties of character, to the message
                                outmsg.WriteAllProperties(ch2);
                            }

                            // Message contains
                            // byte = Type
                            // Int = Player count
                            // Character obj * Player count

                            // Send messsage to clients ( All connections, in reliable order, channel 0)
                            Server.SendMessage(outmsg, Server.Connections, NetDeliveryMethod.ReliableOrdered, 0);
                        }
                        // Update current time
                        time = DateTime.Now;
                    }*/

                    // While loops run as fast as your computer lets. While(true) can lock your computer up. Even 1ms sleep, lets other programs have piece of your CPU time
                    //System.Threading.Thread.Sleep(1);
                    System.Threading.Thread.Yield();
                }
            }
        }
    }
}


// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Dx.Runtime;
using Ninject;
using Tychaia.Data;

namespace Tychaia.Network
{
    public class TychaiaTCPNetwork : INetworkProvider
    {
        private TcpListener m_Tcp;
        private object m_Lock = new object();

        public TychaiaTCPNetwork(ILocalNode node, bool isServer, IPAddress address, int port)
        {
            this.Node = node;
            this.IsServer = isServer;
            this.IPAddress = address;
            this.DiscoveryPort = port;
            this.MessagingPort = this.IsServer ? 9092 : 9093;
        }

        public bool IsServer
        {
            get;
            private set;
        }

        public ILocalNode Node
        {
            get;
            private set;
        }

        public ID ID
        {
            get
            {
                return this.Node.ID;
            }
        }

        public int DiscoveryPort
        {
            get;
            private set;
        }

        public int MessagingPort
        {
            get;
            private set;
        }

        public IPAddress IPAddress
        {
            get;
            private set;
        }

        public bool IsFirst
        {
            get
            {
                return this.IsServer;
            }
        }

        public static void SetupKernel(IKernel kernel, bool isServer, IPAddress address, int port)
        {
            kernel.Unbind<INetworkProvider>();
            kernel.Bind<INetworkProvider>().To<TychaiaTCPNetwork>()
                .WithConstructorArgument("isServer", isServer)
                .WithConstructorArgument("address", address)
                .WithConstructorArgument("port", port);
        }

        public void Join(ID id)
        {
            // This network gets all of it's information from the constructor, which
            // is dependency injected.  Thus we don't use the ID for discovery.
            
            // If this is a server, then we need to start listening on the incoming discovery
            // port.  This port is only used so that clients can indicate that they want to
            // join the distribute network (after which they then kick off calling methods
            // on GameState).
            if (this.IsServer)
            {
                this.m_Tcp = new TcpListener(new IPEndPoint(this.IPAddress, this.DiscoveryPort));
                this.m_Tcp.Start();
                this.HandleDiscovery();
                Console.WriteLine(this.IPAddress + ":" + this.DiscoveryPort);
                return;
            }
            
            // Otherwise we are a client, and we need to establish a connection to the server and
            // send a "JOIN" message.
            var client = new TcpClient();
            client.Connect(new IPEndPoint(this.IPAddress, this.DiscoveryPort));
            var stream = client.GetStream();
            var serializer = new TychaiaDataSerializer();
            var sendMessage = new TCPMessage
            {
                Mode = TCPMessage.ModeMessageJoin,
                ID = this.ID.GetBytes().ToArray(),
                Port = this.MessagingPort
            };
            serializer.SerializeWithLengthPrefix(stream, sendMessage, typeof(TCPMessage), ProtoBuf.PrefixStyle.Base128, 1);
            var receiveMessage = new TCPMessage();
            serializer.DeserializeWithLengthPrefix(stream, receiveMessage, typeof(TCPMessage), ProtoBuf.PrefixStyle.Base128, 1);
            switch (receiveMessage.Mode)
            {
                case TCPMessage.ModeMessageAccept:
                    this.Node.Contacts.Add(new Contact(
                        new ID(receiveMessage.ID),
                        new IPEndPoint(this.IPAddress, receiveMessage.Port)));
                    break;
                default:
                    throw new InvalidOperationException();
            }
            
            client.Close();
        }

        public void Leave()
        {
            // TODO: Notify the server that we're leaving in terms of distributed networking.
        }
        
        private void HandleDiscovery() 
        {
            var serverThread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        // Wait for an incoming connection.
                        var client = this.m_Tcp.AcceptTcpClient();
                        var stream = client.GetStream();
                        
                        var clientThread = new Thread(() =>
                        {
                            var serializer = new TychaiaDataSerializer();
                            var connected = true;
                            try
                            {
                                while (connected)
                                {
                                    // Read the next Protobuf message.
                                    var message = new TCPMessage();
                                    serializer.DeserializeWithLengthPrefix(stream, message, typeof(TCPMessage), ProtoBuf.PrefixStyle.Base128, 1);
                                    
                                    lock (this.m_Lock)
                                    {
                                        switch (message.Mode)
                                        {
                                            case TCPMessage.ModeMessageJoin:
                                                var sendMessage = new TCPMessage
                                                {
                                                    Mode = TCPMessage.ModeMessageAccept,
                                                    ID = this.ID.GetBytes().ToArray(),
                                                    Port = this.MessagingPort
                                                };
                                                serializer.SerializeWithLengthPrefix(stream, sendMessage, typeof(TCPMessage), ProtoBuf.PrefixStyle.Base128, 1);
                                                var endpoint = (IPEndPoint)client.Client.RemoteEndPoint;
                                                this.Node.Contacts.Add(new Contact(
                                                    new ID(message.ID),
                                                    new IPEndPoint(endpoint.Address, message.Port)));
                                                break;
                                            case TCPMessage.ModeUnset:
                                                // Client disconnected (usually happens once it's notified us
                                                // that it's in the network).
                                                connected = false;
                                                break;
                                            default:
                                                throw new InvalidOperationException();
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                if (e is ThreadAbortException || e is ObjectDisposedException)
                                    return;
                                Console.WriteLine(e);
                            }
                        });
                        clientThread.Name = "Tychaia Server Client Discovery Thread";
                        clientThread.IsBackground = true;
                        clientThread.Start();
                    }
                }
                catch (Exception e)
                {
                    if (e is ThreadAbortException || e is ObjectDisposedException)
                        return;
                    Console.WriteLine(e);
                }
            });
            serverThread.Name = "Tychaia Server Incoming Connection Thread";
            serverThread.IsBackground = true;
            serverThread.Start();
        }
    }
}

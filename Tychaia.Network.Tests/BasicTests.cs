// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Net;
using System.Threading;
using Protogame;
using Xunit;

namespace Tychaia.Network.Tests
{
    public class BasicTests
    {
        [Fact]
        public void TestConnection()
        {
            var server = new TychaiaServer(9090, 9091);
            var client = new TychaiaClient(9092, 9093);

            client.Connect(new DualIPEndPoint(IPAddress.Loopback, 9090, 9091));

            this.SimulateNetworkCycles(2, server, client);

            var hit = false;
            client.ListenForMessage("hit", (mxc, s) => hit = true);

            server.SendMessage("hit", new byte[0]);

            this.SimulateNetworkCycles(2, server, client);

            Assert.True(hit);
        }

        [Fact]
        public void TestMultipleClients()
        {
            var server = new TychaiaServer(9094, 9095);
            var client1 = new TychaiaClient(9096, 9097);
            var client2 = new TychaiaClient(9098, 9099);

            client1.Connect(new DualIPEndPoint(IPAddress.Loopback, 9094, 9095));
            client2.Connect(new DualIPEndPoint(IPAddress.Loopback, 9094, 9095));

            this.SimulateNetworkCycles(2, server, client1, client2);

            var hit1 = false;
            var hit2 = false;
            client1.ListenForMessage("hit", (mxc, s) => hit1 = true);
            client2.ListenForMessage("hit", (mxc, s) => hit2 = true);

            server.SendMessage("hit", new byte[0]);

            this.SimulateNetworkCycles(1, server, client1, client2);

            Assert.True(hit1);
            Assert.True(hit2);
        }

        private void SimulateNetworkCycles(int cycles, TychaiaServer server, params TychaiaClient[] clients)
        {
            for (var i = 0; i < cycles; i++)
            {
                Thread.Sleep(1000 / 30);

                server.Update();
                foreach (var client in clients)
                {
                    client.Update();
                }
            }
        }
    }
}
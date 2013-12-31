// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System.Net;
using Xunit;

namespace Tychaia.Network.Tests
{
    public class BasicTests
    {
        [Fact]
        public void TestConnection()
        {
            var server = new TychaiaServer(9090);
            var client = new TychaiaClient(9091);

            client.Connect(new IPEndPoint(IPAddress.Loopback, 9090));

            client.Update();
            server.Update();
            client.Update();
            server.Update();

            var hit = false;
            client.ListenForMessage("hit", s => hit = true);

            server.SendMessage("hit", string.Empty);

            server.Update();
            client.Update();

            Assert.True(hit);
        }

        [Fact]
        public void TestMultipleClients()
        {
            var server = new TychaiaServer(9092);
            var client1 = new TychaiaClient(9093);
            var client2 = new TychaiaClient(9094);

            client1.Connect(new IPEndPoint(IPAddress.Loopback, 9092));
            client2.Connect(new IPEndPoint(IPAddress.Loopback, 9092));

            client1.Update();
            client2.Update();
            server.Update();
            client1.Update();
            client2.Update();
            server.Update();

            var hit1 = false;
            var hit2 = false;
            client1.ListenForMessage("hit", s => hit1 = true);
            client2.ListenForMessage("hit", s => hit2 = true);

            server.SendMessage("hit", string.Empty);

            server.Update();
            client1.Update();
            client2.Update();

            Assert.True(hit1);
            Assert.True(hit2);
        }
    }
}
// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Net;
using System.Text;
using System.Threading;
using Protogame;

namespace NetworkTest
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var dispatcher = new MxDispatcher(int.Parse(args[0]), int.Parse(args[0]) + 1);

            dispatcher.Connect(new DualIPEndPoint(IPAddress.Parse(args[1]), int.Parse(args[2]), int.Parse(args[2]) + 1));

            dispatcher.MessageSent += (sender, eventArgs) => Console.WriteLine("message sent " + Encoding.ASCII.GetString(eventArgs.Payload));
            dispatcher.MessageReceived += (sender, eventArgs) => Console.WriteLine("message received " + Encoding.ASCII.GetString(eventArgs.Payload));
            dispatcher.MessageLost += (sender, eventArgs) => Console.WriteLine("message lost " + Encoding.ASCII.GetString(eventArgs.Payload));
            dispatcher.MessageAcknowledged += (sender, eventArgs) => Console.WriteLine("message acknowledged " + Encoding.ASCII.GetString(eventArgs.Payload));

            var array = new[] { "hello", "world", "what", "is", "this", "i", "don't", "even" };
            var counter = 0;
            var accumulator = 0;

            while (true)
            {
                accumulator += 1;

                if (accumulator == 10)
                {
                    foreach (var endpoint in dispatcher.Endpoints)
                    {
                        dispatcher.Send(endpoint, Encoding.ASCII.GetBytes(array[counter]));
                    }

                    counter++;
                    if (counter >= array.Length)
                    {
                        counter = 0;
                    }

                    accumulator = 0;
                }

                dispatcher.Update();

                Thread.Sleep(1000 / 30);
            }
        }
    }
}
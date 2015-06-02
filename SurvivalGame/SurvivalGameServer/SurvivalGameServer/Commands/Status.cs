using Lidgren.Network;
using System;

namespace Mentula.SurvivalGameServer.Commands
{
    public class Status : Command
    {
        private NetServer server;

        public Status(ref NetServer server)
            : base("Status")
        {
            this.server = server;
        }

        public override void Call(string[] args)
        {
            NetPeerStatistics stats = server.Statistics;
            Console.WriteLine("Received Bytes: {0}\nReceived Messages: {1}\nReceived Packets: {2}", stats.ReceivedBytes, stats.ReceivedMessages, stats.ReceivedPackets);
            Console.WriteLine("Sent Bytes: {0}\nSent Messages: {1}\nSent Packets: {2}", stats.SentBytes, stats.SentMessages, stats.SentPackets);
            Console.WriteLine("Bytes In Recycle Pool: {0}\nStorage Bytes Allocated: {1}", stats.BytesInRecyclePool, stats.StorageBytesAllocated);
        }
    }
}
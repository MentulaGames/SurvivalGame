using System;
using NIM = Lidgren.Network.NetIncomingMessage;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer
{
    public static class MentulaExtensions
    {
        public static long GetId(this NIM message)
        {
            return message.SenderConnection.RemoteUniqueIdentifier;
        }

        public static AStar.Node[] PropperClone(this AStar.Node[] arr)
        {
            AStar.Node[] result = new AStar.Node[arr.Length];

            for (int i = 0; i < arr.Length; i++)
            {
                AStar.Node cur = arr[i];
                result[i] = new AStar.Node(cur.Position, cur.GValue, !cur.wall);
            }

            return result;
        }
    }
}

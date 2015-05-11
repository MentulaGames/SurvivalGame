using Lidgren.Network;
using Mentula.General;
using Mentula.General.Res;
using Mentula.Network.Xna;
using System;
using System.Threading;
using NCS = Lidgren.Network.NetConnectionStatus;
using NIMT = Lidgren.Network.NetIncomingMessageType;
using NPConf = Lidgren.Network.NetPeerConfiguration;
using NOM = Lidgren.Network.NetOutgoingMessage;

namespace Mentula.SurvivalGameServer
{
    public class Program
    {
        private static NetServer server;
        private static Map map;

        static void Main(string[] args)
        {
            InitConsole();
            InitServer();
            server.Start();
            server.UPnP.ForwardPort(Ips.PORT, Resources.AppName);
            InitMap();

            double nextSend = NetTime.Now;

            while (!Console.KeyAvailable || Console.ReadLine() != "Exit")
            {
                NetIncomingMessage msg;

                while ((msg = server.ReadMessage()) != null)
                {
                    switch(msg.MessageType)
                    {
                        case(NIMT.DiscoveryRequest):
                            server.SendDiscoveryResponse(null, msg.SenderEndPoint);
                            MentulaExtensions.WriteLine(msg.MessageType, "{0} discovered the service.", msg.SenderEndPoint);
                            break;
                        case(NIMT.VerboseDebugMessage):
                        case(NIMT.DebugMessage):
                        case(NIMT.WarningMessage):
                        case(NIMT.ErrorMessage):
                            MentulaExtensions.WriteLine(msg.MessageType, "{0}", msg.ReadString());
                            break;
                        case(NIMT.StatusChanged):
                            NCS status = (NCS)msg.ReadByte();

                            switch(status)
                            {
                                case(NCS.Connected):
                                    MentulaExtensions.WriteLine(msg.MessageType, "{0} connected!", NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier));
                                    break;
                                case(NCS.Disconnected):
                                    MentulaExtensions.WriteLine(msg.MessageType, "{0} disconnected!", NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier));
                                    break;
                            }

                            break;
                        case(NIMT.Data):
                            IntVector2 chunkPos = msg.ReadVector();
                            map.Generate(chunkPos);
                            map.LoadChunks(chunkPos);

                            NOM nom = server.CreateMessage();
                            nom.Write(map.GetChunks(chunkPos));
                            server.SendMessage(nom, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                            break;
                    }

                    Thread.Sleep(1);
                }
            }

            server.Shutdown(string.Format("{0} Exiting", Resources.AppName));
        }

        private static void InitConsole()
        {
            Console.SetBufferSize(1080, 1920);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Title = string.Format("{0}_Sever", Resources.AppName);
        }

        private static void InitServer()
        {
            NPConf config = new NPConf(Resources.AppName);
            config.EnableMessageType(NIMT.DiscoveryRequest);
            config.Port = Ips.PORT;
            config.EnableUPnP = true;
            server = new NetServer(config);
        }
        
        private static void InitMap()
        {
            map = new Map();
            map.Generate(IntVector2.Zero);
            MentulaExtensions.WriteLine(NIMT.Data, "Generated at: {0}.", IntVector2.Zero);
            map.LoadChunks(IntVector2.Zero);
        }
    }
}
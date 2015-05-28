using Lidgren.Network;
using Mentula.General.Res;
using System;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer.Commands
{
    public class Forward : Command
    {
        private NetServer server;

        public Forward(ref NetServer server)
            : base("Forward Port")
        {
            this.server = server;
        }

        public override void Call(string[] args)
        {
            int port = Ips.PORT;
            if (args.Length > 0) int.TryParse(args[0], out port);

            bool forward = server.UPnP.ForwardPort(port, Resources.AppName);
            MentulaExtensions.WriteLine(forward ? NIMT.DebugMessage : NIMT.WarningMessage, "{0} to forward port: {1}!", forward ? "Succeted" : "Failed", port);
        }
    }
}
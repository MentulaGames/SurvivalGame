using Mentula.SurvivalGameServer;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer.Commands
{
    public class Help : Command
    {
        private CommandHandler Handler;

        public Help(CommandHandler h)
            : base("Help")
        {
            Handler = h;
        }

        public override void Call(string[] args)
        {
            string format = "";
            bool first = true;

            for (int i = 0; i < Handler.Commands.Length; i++)
            {
                format += string.Format("\t{0}{1}.\n", first ? "" : "\t\t", Handler.Commands[i].m_Command);
                first = false;
            }

            NIMT.DebugMessage.WriteLine(format, null);
        }
    }
}
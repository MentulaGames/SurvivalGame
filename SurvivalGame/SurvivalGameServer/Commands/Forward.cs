using Mentula.General.Res;
using System;

namespace Mentula.SurvivalGameServer.Commands
{
    public class Forward : Command
    {
        private Action<int> callback;

        public Forward(Action<int> onForward)
            : base("Forward Port")
        {
            callback = onForward;
        }

        public override void Call(string[] args)
        {
            if (callback != null)
            {
                int port = Ips.PORT;
                if (args.Length > 0) int.TryParse(args[0], out port); 
                callback.Invoke(port);
            }
        }
    }
}
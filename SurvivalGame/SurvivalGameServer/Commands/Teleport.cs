using Mentula.SurvivalGameServer;
using Microsoft.Xna.Framework;
using System;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.Commands
{
    public class Teleport : Command
    {
        private Action<string, Vector2> callback;

        public Teleport(Action<string, Vector2> onTeleport)
            :base("Teleport")
        {
            callback = onTeleport;
        }

        public override void Call(string[] args)
        {
            if (callback != null)
            {
                if (args.Length > 2)
                {
                    float x;
                    float y;

                    if (float.TryParse(args[1], out x) & float.TryParse(args[2], out y)) callback.Invoke(args[0], new Vector2(x, y));
                }
                else NIMT.ErrorMessage.WriteLine("The Teleport command requires a player name and a total position (Teleport bob 12 45)", null);
            }
        }
    }
}
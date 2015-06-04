using System.Net;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer
{
    public delegate bool PlayerListChanged(IPAddress id, string player);
    public delegate bool PlayerBanned(string displayName, IPAddress player);
    public delegate void InfoMessage(string formay, params object[] args);
    public delegate void NIMTMessage(NIMT type, string format, params object[] args);
}
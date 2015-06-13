using Lidgren.Network;
using Mentula.SurvivalGameServer.GUI;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Mentula.SurvivalGameServer
{
    public class Program
    {
        public static bool ServerDown { get { return main.server.Status == NetPeerStatus.NotRunning; } }

        internal static bool Exit;
        internal static Thread updateThread;

        private static form_GUI GUI;
        private static Main main;

        static Program()
        {
            Exit = false;
            updateThread = null;

            GUI = new form_GUI();
            GUI.FormClosed += (obj, e) => Exit = true;

            main = new Main();
        }

        public static void Main(string[] args)
        {
            main.Initialize();
            main.SimpleMessage += (format, arg) => GUI.WriteLine(format, arg);
            main.CustomMessage += (type, format, arg) => GUI.WriteLine(type, format, arg);
            main.TryConnectPlayer += (id, name) => { GUI.AddPlayer(id, name); return true; };
            main.TryRemovePlayer += (id, name) => { GUI.RemovePlayer(id); return true; };

            updateThread = new Thread(StartService);

            updateThread.Start();
            Application.Run(GUI);
        }

        public static void StartService()
        {
            main.Start();

            while(!Exit)
            {
                main.Update();
                GUI.UpdateStats();
                Thread.Sleep(1);
            }

            GUI.UpdateStats(true);
            GUI.ClearPlayers();
            main.Stop();

            DateTime start = DateTime.Now;
            while (!ServerDown)
            {
                TimeSpan diff = DateTime.Now - start;
                if (diff.Seconds > 5) break;
            }

            GUI.WriteLine("Server has stopped.");
        }
    }
}
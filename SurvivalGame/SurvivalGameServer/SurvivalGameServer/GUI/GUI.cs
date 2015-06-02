using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using NIMT = Lidgren.Network.NetIncomingMessageType;

namespace Mentula.SurvivalGameServer.GUI
{
    public partial class form_GUI : Form
    {
        private Dictionary<IPAddress, int> players;

        public form_GUI()
        {
            players = new Dictionary<IPAddress, int>();
            InitializeComponent();
        }

        public void SetState(string state)
        {
            lbl_Status.Text = string.Format("Status: {0}", state);
        }

        public void AddPlayer(IPAddress IP, string name)
        {
            InvokeIfRequired(dGrid_Connections, () => players.Add(IP, dGrid_Connections.Rows.Add(name, IP)));
        }

        public void RemovePlayer(IPAddress IP)
        {
            InvokeIfRequired(dGrid_Connections, () => dGrid_Connections.Rows.RemoveAt(players[IP]));
        }

        public void WriteLine(string format, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(format)) return;

            string line = string.Format("[{0}][Info] {1}\n", string.Format("{0:H:mm:ss}", DateTime.Now), string.Format(format, args));

            InvokeIfRequired(txt_Console, () =>
            {
                txt_Console.SelectionColor = Color.HotPink;
                txt_Console.AppendText(line);
                txt_Console.Find(line);
                txt_Console.ScrollToCaret();
            });

            lbl_LastMessage.Text = line;
        }

        public void WriteLine(NIMT nimt, string format, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(format)) return;
            string mode = "";
            Color color = Color.White;

            switch (nimt)
            {
                case (NIMT.ConnectionApproval):
                    color = Color.Green;
                    mode = "Approval";
                    break;
                case (NIMT.ConnectionLatencyUpdated):
                    mode = "LatencyUpdate";
                    break;
                case (NIMT.Data):
                case (NIMT.UnconnectedData):
                    mode = "Data";
                    break;
                case (NIMT.DebugMessage):
                case (NIMT.VerboseDebugMessage):
                    mode = "Debug";
                    break;
                case (NIMT.DiscoveryRequest):
                case (NIMT.DiscoveryResponse):
                    mode = "Discovery";
                    break;
                case (NIMT.Error):
                case (NIMT.ErrorMessage):
                    color = Color.Red;
                    mode = "Error";
                    break;
                case (NIMT.NatIntroductionSuccess):
                    mode = "NAT";
                    break;
                case (NIMT.Receipt):
                    mode = "Receipt";
                    break;
                case (NIMT.StatusChanged):
                    color = Color.Green;
                    mode = "Status";
                    break;
                case (NIMT.WarningMessage):
                    color = Color.Yellow;
                    mode = "Warning";
                    break;
            }

            string line = string.Format("[{0}][{1}] {2}\n", string.Format("{0:H:mm:ss}", DateTime.Now), mode, string.Format(format, args));

            InvokeIfRequired(txt_Console, () =>
                {
                    txt_Console.SelectionColor = color;
                    txt_Console.AppendText(line);
                    txt_Console.Find(line);
                    txt_Console.ScrollToCaret();
                });
        }

        [DllImport("user32.dll")]
        private static extern int HideCaret(IntPtr hwnd);

        private void InvokeIfRequired(Control control, MethodInvoker action)
        {
            if (control.InvokeRequired) control.Invoke(action);
            else action();
        }

        private void txt_Console_MouseDown(object sender, MouseEventArgs e)
        {
            HideCaret(txt_Console.Handle);
        }

        private void btn_Kill_Click(object sender, EventArgs e)
        {
            if (Program.updateTask != null && Program.updateTask.ThreadState == ThreadState.Running)
            {
                DialogResult result = MessageBox.Show("Are you just you want to kill the server?", "WARNING", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    Program.updateTask.Abort();
                    WriteLine("Killed server!");
                    Close();
                }
            }
        }

        private void btn_netState_Click(object sender, EventArgs e)
        {
            Program.Exit = true;
        }

        private void btn_Restart_Click(object sender, EventArgs e)
        {
            WriteLine(new NotImplementedException().Message);
        }
    }
}

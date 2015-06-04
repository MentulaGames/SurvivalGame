using Mentula.General.Res;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private CPUUsage cpu;

        public form_GUI()
        {
            InitializeComponent();
            WriteFirstLine("Console Created");
            players = new Dictionary<IPAddress, int>();
            cpu = new CPUUsage();
        }

        public void SetState(string state)
        {
            lbl_Status.Text = string.Format("Status: {0}", state);
        }

        public void UpdateStats()
        {
            short value = cpu.GetUsage();

            InvokeIfRequired(lbl_CPU, () =>
                {
                    lbl_CPU.Text = string.Format("CPU Usage: {0}%", value);
                    proBarCPU.Value = (int)value;
                });
        }

        public void AddPlayer(IPAddress IP, string name)
        {
            InvokeIfRequired(dGrid_Connections, () => players.Add(IP, dGrid_Connections.Rows.Add(name, IP)));
        }

        public void RemovePlayer(IPAddress IP)
        {
            InvokeIfRequired(dGrid_Connections, () => dGrid_Connections.Rows.RemoveAt(players[IP]));
        }
        
        public void ClearPlayers()
        {
            InvokeIfRequired(dGrid_Connections, () => dGrid_Connections.Rows.Clear());
        }

        public void WriteLine(string format, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(format)) return;

            string line = string.Format("\n[{0}][Info] {1}", string.Format("{0:H:mm:ss}", DateTime.Now), string.Format(format, args));

            InvokeIfRequired(txt_Console, () =>
                {
                    txt_Console.SelectionColor = Color.DodgerBlue;
                    txt_Console.AppendText(line);
                    txt_Console.Find(line);
                    txt_Console.ScrollToCaret();
                });

            lbl_LastMessage.Text = line.TrimStart('\n');
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

            string line = string.Format("\n[{0}][{1}] {2}", string.Format("{0:H:mm:ss}", DateTime.Now), mode, string.Format(format, args));

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

        private void WriteFirstLine(string format, params object[] args)
        {
            string line = string.Format("[{0}][Info] {1}", string.Format("{0:H:mm:ss}", DateTime.Now), string.Format(format, args));

            txt_Console.SelectionColor = Color.DodgerBlue;
            txt_Console.AppendText(line);
            txt_Console.Find(line);
            txt_Console.ScrollToCaret();
        }

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
            if (Program.updateThread != null && Program.updateThread.ThreadState == System.Threading.ThreadState.Running)
            {
                DialogResult result = MessageBox.Show("Are you just you want to kill the server?", "WARNING", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    Program.updateThread.Abort();
                    WriteLine("Killed server!");
                    Close();
                }
            }
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            Program.Exit = true;
        }

        private void btn_Restart_Click(object sender, EventArgs e)
        {
            WriteLine(new NotImplementedException().Message);
        }
    }
}

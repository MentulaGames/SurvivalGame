namespace Mentula.SurvivalGameServer.GUI
{
    partial class form_GUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_GUI));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lbl_Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbl_LastMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.gBox_Info = new System.Windows.Forms.GroupBox();
            this.lbl_CPU = new System.Windows.Forms.Label();
            this.proBarCPU = new System.Windows.Forms.ProgressBar();
            this.btn_Stop = new System.Windows.Forms.Button();
            this.btn_Restart = new System.Windows.Forms.Button();
            this.btn_Kill = new System.Windows.Forms.Button();
            this.dGrid_Connections = new System.Windows.Forms.DataGridView();
            this.coll_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.coll_Ip = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txt_Console = new System.Windows.Forms.RichTextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.statusStrip.SuspendLayout();
            this.gBox_Info.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGrid_Connections)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lbl_Status,
            this.lbl_LastMessage});
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            // 
            // lbl_Status
            // 
            resources.ApplyResources(this.lbl_Status, "lbl_Status");
            this.lbl_Status.Name = "lbl_Status";
            // 
            // lbl_LastMessage
            // 
            resources.ApplyResources(this.lbl_LastMessage, "lbl_LastMessage");
            this.lbl_LastMessage.Name = "lbl_LastMessage";
            this.lbl_LastMessage.Padding = new System.Windows.Forms.Padding(50, 0, 0, 0);
            // 
            // gBox_Info
            // 
            resources.ApplyResources(this.gBox_Info, "gBox_Info");
            this.gBox_Info.Controls.Add(this.lbl_CPU);
            this.gBox_Info.Controls.Add(this.proBarCPU);
            this.gBox_Info.Controls.Add(this.btn_Stop);
            this.gBox_Info.Controls.Add(this.btn_Restart);
            this.gBox_Info.Controls.Add(this.btn_Kill);
            this.gBox_Info.Name = "gBox_Info";
            this.gBox_Info.TabStop = false;
            // 
            // lbl_CPU
            // 
            resources.ApplyResources(this.lbl_CPU, "lbl_CPU");
            this.lbl_CPU.Name = "lbl_CPU";
            // 
            // proBarCPU
            // 
            resources.ApplyResources(this.proBarCPU, "proBarCPU");
            this.proBarCPU.Name = "proBarCPU";
            // 
            // btn_Stop
            // 
            resources.ApplyResources(this.btn_Stop, "btn_Stop");
            this.btn_Stop.Name = "btn_Stop";
            this.btn_Stop.UseVisualStyleBackColor = true;
            this.btn_Stop.Click += new System.EventHandler(this.btn_Stop_Click);
            // 
            // btn_Restart
            // 
            resources.ApplyResources(this.btn_Restart, "btn_Restart");
            this.btn_Restart.Name = "btn_Restart";
            this.btn_Restart.UseVisualStyleBackColor = true;
            this.btn_Restart.Click += new System.EventHandler(this.btn_Restart_Click);
            // 
            // btn_Kill
            // 
            resources.ApplyResources(this.btn_Kill, "btn_Kill");
            this.btn_Kill.Name = "btn_Kill";
            this.btn_Kill.UseVisualStyleBackColor = true;
            this.btn_Kill.Click += new System.EventHandler(this.btn_Kill_Click);
            // 
            // dGrid_Connections
            // 
            this.dGrid_Connections.AllowUserToAddRows = false;
            this.dGrid_Connections.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.dGrid_Connections, "dGrid_Connections");
            this.dGrid_Connections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGrid_Connections.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.coll_Name,
            this.coll_Ip});
            this.dGrid_Connections.Name = "dGrid_Connections";
            this.dGrid_Connections.ReadOnly = true;
            this.dGrid_Connections.RowHeadersVisible = false;
            // 
            // coll_Name
            // 
            this.coll_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.coll_Name, "coll_Name");
            this.coll_Name.Name = "coll_Name";
            this.coll_Name.ReadOnly = true;
            // 
            // coll_Ip
            // 
            this.coll_Ip.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.coll_Ip, "coll_Ip");
            this.coll_Ip.Name = "coll_Ip";
            this.coll_Ip.ReadOnly = true;
            // 
            // txt_Console
            // 
            resources.ApplyResources(this.txt_Console, "txt_Console");
            this.txt_Console.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txt_Console.ForeColor = System.Drawing.SystemColors.Menu;
            this.txt_Console.Name = "txt_Console";
            this.txt_Console.ReadOnly = true;
            this.txt_Console.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txt_Console_MouseDown);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dGrid_Connections);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txt_Console);
            // 
            // form_GUI
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.gBox_Info);
            this.Controls.Add(this.statusStrip);
            this.Name = "form_GUI";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.gBox_Info.ResumeLayout(false);
            this.gBox_Info.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGrid_Connections)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lbl_Status;
        private System.Windows.Forms.ToolStripStatusLabel lbl_LastMessage;
        private System.Windows.Forms.GroupBox gBox_Info;
        public System.Windows.Forms.Button btn_Kill;
        public System.Windows.Forms.Button btn_Stop;
        public System.Windows.Forms.Button btn_Restart;
        private System.Windows.Forms.DataGridView dGrid_Connections;
        private System.Windows.Forms.RichTextBox txt_Console;
        private System.Windows.Forms.DataGridViewTextBoxColumn coll_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn coll_Ip;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label lbl_CPU;
        private System.Windows.Forms.ProgressBar proBarCPU;
    }
}
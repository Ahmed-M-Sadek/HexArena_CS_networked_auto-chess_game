namespace ASU2019_NetworkedGameWorkshop.view {
    partial class ConnectForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_Host = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.lbl_DeviceIP = new System.Windows.Forms.Label();
            this.lbl_DevicePort = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageHost = new System.Windows.Forms.TabPage();
            this.tabPageConnect = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPageHost.SuspendLayout();
            this.tabPageConnect.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Items.AddRange(new object[] {
            "Device 1",
            "Device 2",
            "Device 3"});
            this.listBox1.Location = new System.Drawing.Point(6, 6);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(274, 124);
            this.listBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(286, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(142, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Refresh";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btn_Host
            // 
            this.btn_Host.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Host.Location = new System.Drawing.Point(165, 52);
            this.btn_Host.Name = "btn_Host";
            this.btn_Host.Size = new System.Drawing.Size(280, 28);
            this.btn_Host.TabIndex = 3;
            this.btn_Host.Text = "Host";
            this.btn_Host.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(286, 136);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(142, 26);
            this.button4.TabIndex = 4;
            this.button4.Text = "Manual Connect";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // lbl_DeviceIP
            // 
            this.lbl_DeviceIP.AutoSize = true;
            this.lbl_DeviceIP.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_DeviceIP.Location = new System.Drawing.Point(160, 10);
            this.lbl_DeviceIP.Name = "lbl_DeviceIP";
            this.lbl_DeviceIP.Size = new System.Drawing.Size(192, 26);
            this.lbl_DeviceIP.TabIndex = 5;
            this.lbl_DeviceIP.Text = "192.168.XXX.XXX";
            // 
            // lbl_DevicePort
            // 
            this.lbl_DevicePort.AutoSize = true;
            this.lbl_DevicePort.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_DevicePort.Location = new System.Drawing.Point(358, 11);
            this.lbl_DevicePort.Name = "lbl_DevicePort";
            this.lbl_DevicePort.Size = new System.Drawing.Size(87, 26);
            this.lbl_DevicePort.TabIndex = 6;
            this.lbl_DevicePort.Text = "XXXXX";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(6, 136);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(274, 26);
            this.textBox1.TabIndex = 7;
            this.textBox1.Text = "temp text ?";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(286, 6);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(142, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Connect";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageHost);
            this.tabControl.Controls.Add(this.tabPageConnect);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.tabControl.Name = "tabControl";
            this.tabControl.Padding = new System.Drawing.Point(10, 5);
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(461, 261);
            this.tabControl.TabIndex = 11;
            // 
            // tabPageHost
            // 
            this.tabPageHost.Controls.Add(this.label1);
            this.tabPageHost.Controls.Add(this.lbl_DeviceIP);
            this.tabPageHost.Controls.Add(this.btn_Host);
            this.tabPageHost.Controls.Add(this.lbl_DevicePort);
            this.tabPageHost.Location = new System.Drawing.Point(4, 26);
            this.tabPageHost.Name = "tabPageHost";
            this.tabPageHost.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPageHost.Size = new System.Drawing.Size(453, 231);
            this.tabPageHost.TabIndex = 0;
            this.tabPageHost.Text = "Host";
            this.tabPageHost.UseVisualStyleBackColor = true;
            // 
            // tabPageConnect
            // 
            this.tabPageConnect.Controls.Add(this.listBox1);
            this.tabPageConnect.Controls.Add(this.button2);
            this.tabPageConnect.Controls.Add(this.button1);
            this.tabPageConnect.Controls.Add(this.button4);
            this.tabPageConnect.Controls.Add(this.textBox1);
            this.tabPageConnect.Location = new System.Drawing.Point(4, 26);
            this.tabPageConnect.Name = "tabPageConnect";
            this.tabPageConnect.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabPageConnect.Size = new System.Drawing.Size(453, 231);
            this.tabPageConnect.TabIndex = 1;
            this.tabPageConnect.Text = "Connect";
            this.tabPageConnect.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 24);
            this.label1.TabIndex = 7;
            this.label1.Text = "Local IP and Port: ";
            // 
            // ConnectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 261);
            this.Controls.Add(this.tabControl);
            this.Name = "ConnectForm";
            this.Text = "ConnectForm";
            this.Load += new System.EventHandler(this.ConnectForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageHost.ResumeLayout(false);
            this.tabPageHost.PerformLayout();
            this.tabPageConnect.ResumeLayout(false);
            this.tabPageConnect.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btn_Host;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label lbl_DeviceIP;
        private System.Windows.Forms.Label lbl_DevicePort;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageHost;
        private System.Windows.Forms.TabPage tabPageConnect;
        private System.Windows.Forms.Label label1;
    }
}
namespace BK390A_Display
{
    partial class FrmMain
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
            this.components = new System.ComponentModel.Container();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.TxtUnit = new System.Windows.Forms.Label();
            this.TxtACDC = new System.Windows.Forms.Label();
            this.TxtMode = new System.Windows.Forms.Label();
            this.TxtOptions = new System.Windows.Forms.Label();
            this.TxtStatus = new System.Windows.Forms.Label();
            this.TxtValue = new System.Windows.Forms.Label();
            this.PnlMain = new System.Windows.Forms.Panel();
            this.MnuPopup = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ItmStartStop = new System.Windows.Forms.ToolStripMenuItem();
            this.ItmComPort = new System.Windows.Forms.ToolStripMenuItem();
            this.ItmExit = new System.Windows.Forms.ToolStripMenuItem();
            this.LblHandle = new System.Windows.Forms.Label();
            this.BtnStart = new System.Windows.Forms.Label();
            this.PnlMain.SuspendLayout();
            this.MnuPopup.SuspendLayout();
            this.SuspendLayout();
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // TxtUnit
            // 
            this.TxtUnit.Font = new System.Drawing.Font("Segoe UI", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TxtUnit.Location = new System.Drawing.Point(436, 112);
            this.TxtUnit.Name = "TxtUnit";
            this.TxtUnit.Size = new System.Drawing.Size(181, 92);
            this.TxtUnit.TabIndex = 2;
            this.TxtUnit.Text = "MHz";
            // 
            // TxtACDC
            // 
            this.TxtACDC.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TxtACDC.ForeColor = System.Drawing.Color.LimeGreen;
            this.TxtACDC.Location = new System.Drawing.Point(440, 58);
            this.TxtACDC.Name = "TxtACDC";
            this.TxtACDC.Size = new System.Drawing.Size(145, 77);
            this.TxtACDC.TabIndex = 3;
            this.TxtACDC.Text = "DC";
            // 
            // TxtMode
            // 
            this.TxtMode.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TxtMode.ForeColor = System.Drawing.Color.LimeGreen;
            this.TxtMode.Location = new System.Drawing.Point(3, 9);
            this.TxtMode.Name = "TxtMode";
            this.TxtMode.Size = new System.Drawing.Size(215, 60);
            this.TxtMode.TabIndex = 4;
            this.TxtMode.Text = "Temperature";
            // 
            // TxtOptions
            // 
            this.TxtOptions.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TxtOptions.ForeColor = System.Drawing.Color.LimeGreen;
            this.TxtOptions.Location = new System.Drawing.Point(395, 14);
            this.TxtOptions.Name = "TxtOptions";
            this.TxtOptions.Size = new System.Drawing.Size(179, 44);
            this.TxtOptions.TabIndex = 5;
            this.TxtOptions.Text = "APO VA Hz Min";
            this.TxtOptions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // TxtStatus
            // 
            this.TxtStatus.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TxtStatus.ForeColor = System.Drawing.Color.LimeGreen;
            this.TxtStatus.Location = new System.Drawing.Point(224, 9);
            this.TxtStatus.Name = "TxtStatus";
            this.TxtStatus.Size = new System.Drawing.Size(155, 60);
            this.TxtStatus.TabIndex = 7;
            this.TxtStatus.Text = "Range";
            this.TxtStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // TxtValue
            // 
            this.TxtValue.CausesValidation = false;
            this.TxtValue.Font = new System.Drawing.Font("Segoe UI", 90F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.TxtValue.Location = new System.Drawing.Point(2, 53);
            this.TxtValue.Name = "TxtValue";
            this.TxtValue.Size = new System.Drawing.Size(440, 158);
            this.TxtValue.TabIndex = 1;
            this.TxtValue.Text = "- 0.000";
            this.TxtValue.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // PnlMain
            // 
            this.PnlMain.BackColor = System.Drawing.Color.Transparent;
            this.PnlMain.CausesValidation = false;
            this.PnlMain.ContextMenuStrip = this.MnuPopup;
            this.PnlMain.Controls.Add(this.LblHandle);
            this.PnlMain.Controls.Add(this.BtnStart);
            this.PnlMain.Controls.Add(this.TxtOptions);
            this.PnlMain.Controls.Add(this.TxtValue);
            this.PnlMain.Controls.Add(this.TxtUnit);
            this.PnlMain.Controls.Add(this.TxtACDC);
            this.PnlMain.Controls.Add(this.TxtMode);
            this.PnlMain.Controls.Add(this.TxtStatus);
            this.PnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PnlMain.Location = new System.Drawing.Point(0, 0);
            this.PnlMain.Name = "PnlMain";
            this.PnlMain.Size = new System.Drawing.Size(626, 218);
            this.PnlMain.TabIndex = 9;
            this.PnlMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnlMain_MouseDown);
            this.PnlMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PnlMain_MouseMove);
            this.PnlMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PnlMain_MouseUp);
            // 
            // MnuPopup
            // 
            this.MnuPopup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ItmStartStop,
            this.ItmComPort,
            this.ItmExit});
            this.MnuPopup.Name = "MnuPopup";
            this.MnuPopup.Size = new System.Drawing.Size(177, 70);
            // 
            // ItmStartStop
            // 
            this.ItmStartStop.Name = "ItmStartStop";
            this.ItmStartStop.Size = new System.Drawing.Size(176, 22);
            this.ItmStartStop.Text = "&Start / Stop";
            this.ItmStartStop.ToolTipText = "Start or Stop the BK390 display.";
            this.ItmStartStop.Click += new System.EventHandler(this.ItmStartStop_Click_1);
            // 
            // ItmComPort
            // 
            this.ItmComPort.Name = "ItmComPort";
            this.ItmComPort.Size = new System.Drawing.Size(176, 22);
            this.ItmComPort.Text = "&About && COM Port";
            this.ItmComPort.ToolTipText = "Brings up the About box which also has the COM port setting.";
            this.ItmComPort.Click += new System.EventHandler(this.ItmComPort_Click);
            // 
            // ItmExit
            // 
            this.ItmExit.Name = "ItmExit";
            this.ItmExit.Size = new System.Drawing.Size(176, 22);
            this.ItmExit.Text = "&Exit";
            this.ItmExit.ToolTipText = "Exit the application.";
            this.ItmExit.Click += new System.EventHandler(this.ItmExit_Click);
            // 
            // LblHandle
            // 
            this.LblHandle.AutoSize = true;
            this.LblHandle.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.LblHandle.ForeColor = System.Drawing.Color.White;
            this.LblHandle.Location = new System.Drawing.Point(594, 184);
            this.LblHandle.Name = "LblHandle";
            this.LblHandle.Size = new System.Drawing.Size(29, 25);
            this.LblHandle.TabIndex = 10;
            this.LblHandle.Text = "◢";
            this.LblHandle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LblHandle_MouseDown);
            this.LblHandle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LblHandle_MouseMove);
            this.LblHandle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LblHandle_MouseUp);
            // 
            // BtnStart
            // 
            this.BtnStart.AutoSize = true;
            this.BtnStart.CausesValidation = false;
            this.BtnStart.Font = new System.Drawing.Font("Segoe UI Black", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.BtnStart.ForeColor = System.Drawing.Color.Orange;
            this.BtnStart.Location = new System.Drawing.Point(587, 4);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(36, 37);
            this.BtnStart.TabIndex = 9;
            this.BtnStart.Text = "≡";
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(626, 218);
            this.ControlBox = false;
            this.Controls.Add(this.PnlMain);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ForeColor = System.Drawing.Color.Yellow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMain";
            this.ShowIcon = false;
            this.Text = "BK390A";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.PnlMain.ResumeLayout(false);
            this.PnlMain.PerformLayout();
            this.MnuPopup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.Label TxtUnit;
        private System.Windows.Forms.Label TxtACDC;
        private System.Windows.Forms.Label TxtMode;
        private System.Windows.Forms.Label TxtOptions;
        private System.Windows.Forms.Label TxtStatus;
        private System.Windows.Forms.Label TxtValue;
        private System.Windows.Forms.Panel PnlMain;
        private System.Windows.Forms.ContextMenuStrip MnuPopup;
        private System.Windows.Forms.ToolStripMenuItem ItmStartStop;
        private System.Windows.Forms.ToolStripMenuItem ItmComPort;
        private System.Windows.Forms.Label BtnStart;
        private System.Windows.Forms.ToolStripMenuItem ItmExit;
        private System.Windows.Forms.Label LblHandle;
    }
}


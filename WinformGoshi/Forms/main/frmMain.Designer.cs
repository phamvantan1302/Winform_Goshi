namespace WinformGoshi.Forms.main
{
    partial class frmMain
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
            this.mnuStrip = new System.Windows.Forms.MenuStrip();
            this.dashboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDBSanXuat = new System.Windows.Forms.ToolStripMenuItem();
            this.dashboardQuảnLýQTMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dashboardToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuStrip
            // 
            this.mnuStrip.AutoSize = false;
            this.mnuStrip.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mnuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dashboardToolStripMenuItem});
            this.mnuStrip.Location = new System.Drawing.Point(0, 0);
            this.mnuStrip.Name = "mnuStrip";
            this.mnuStrip.Size = new System.Drawing.Size(1248, 39);
            this.mnuStrip.TabIndex = 0;
            this.mnuStrip.Text = "menuStrip1";
            // 
            // dashboardToolStripMenuItem
            // 
            this.dashboardToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDBSanXuat,
            this.dashboardQuảnLýQTMToolStripMenuItem,
            this.dashboardToolStripMenuItem1});
            this.dashboardToolStripMenuItem.Name = "dashboardToolStripMenuItem";
            this.dashboardToolStripMenuItem.Size = new System.Drawing.Size(85, 35);
            this.dashboardToolStripMenuItem.Text = "Dashboard";
            // 
            // mnuDBSanXuat
            // 
            this.mnuDBSanXuat.Name = "mnuDBSanXuat";
            this.mnuDBSanXuat.Size = new System.Drawing.Size(331, 22);
            this.mnuDBSanXuat.Text = "Dashboard thông tin sản xuất";
            this.mnuDBSanXuat.Click += new System.EventHandler(this.mnuDBSanXuat_Click);
            // 
            // dashboardQuảnLýQTMToolStripMenuItem
            // 
            this.dashboardQuảnLýQTMToolStripMenuItem.Name = "dashboardQuảnLýQTMToolStripMenuItem";
            this.dashboardQuảnLýQTMToolStripMenuItem.Size = new System.Drawing.Size(331, 22);
            this.dashboardQuảnLýQTMToolStripMenuItem.Text = "Dashboard Quản lý TQM";
            this.dashboardQuảnLýQTMToolStripMenuItem.Click += new System.EventHandler(this.dashboardQuảnLýQTMToolStripMenuItem_Click);
            // 
            // dashboardToolStripMenuItem1
            // 
            this.dashboardToolStripMenuItem1.Name = "dashboardToolStripMenuItem1";
            this.dashboardToolStripMenuItem1.Size = new System.Drawing.Size(331, 22);
            this.dashboardToolStripMenuItem1.Text = "Dashboard Phân tích thời gian dừng chuyền";
            this.dashboardToolStripMenuItem1.Click += new System.EventHandler(this.dashboardToolStripMenuItem1_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1248, 698);
            this.Controls.Add(this.mnuStrip);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mnuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.Text = "GOSHI";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.mnuStrip.ResumeLayout(false);
            this.mnuStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnuStrip;
        private System.Windows.Forms.ToolStripMenuItem dashboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuDBSanXuat;
        private System.Windows.Forms.ToolStripMenuItem dashboardQuảnLýQTMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dashboardToolStripMenuItem1;
    }
}
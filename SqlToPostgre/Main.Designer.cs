namespace SqlToPostgre
{
    partial class Main
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
            this.txtSqlAddress = new System.Windows.Forms.TextBox();
            this.btnCheckSqlConnection = new System.Windows.Forms.Button();
            this.lblSqlAddress = new System.Windows.Forms.Label();
            this.btnConvert = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.lblSqlCatalog = new System.Windows.Forms.Label();
            this.txtSqlCatalog = new System.Windows.Forms.TextBox();
            this.lblSqlPass = new System.Windows.Forms.Label();
            this.txtSqlPass = new System.Windows.Forms.TextBox();
            this.lblSqlUser = new System.Windows.Forms.Label();
            this.txtSqlUser = new System.Windows.Forms.TextBox();
            this.lblSqlProvider = new System.Windows.Forms.Label();
            this.txtSqlProvider = new System.Windows.Forms.TextBox();
            this.btnClearStatus = new System.Windows.Forms.Button();
            this.lblSqlPort = new System.Windows.Forms.Label();
            this.txtSqlPort = new System.Windows.Forms.TextBox();
            this.lblPostgrePort = new System.Windows.Forms.Label();
            this.txtPostgrePort = new System.Windows.Forms.TextBox();
            this.lblPostgreProvider = new System.Windows.Forms.Label();
            this.txtPostgreProvider = new System.Windows.Forms.TextBox();
            this.lblPostgrePass = new System.Windows.Forms.Label();
            this.txtPostgrePass = new System.Windows.Forms.TextBox();
            this.lblPostgreUser = new System.Windows.Forms.Label();
            this.txtPostgreUser = new System.Windows.Forms.TextBox();
            this.lblPostgreCatalog = new System.Windows.Forms.Label();
            this.txtPostgreCatalog = new System.Windows.Forms.TextBox();
            this.lblPostgreAddress = new System.Windows.Forms.Label();
            this.txtPostgreAddress = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnCheckPostgreConnection = new System.Windows.Forms.Button();
            this.btnCancelConvertion = new System.Windows.Forms.Button();
            this.txtFetchRows = new System.Windows.Forms.TextBox();
            this.lblFetchRows = new System.Windows.Forms.Label();
            this.chkAutoScrollStatus = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtSqlAddress
            // 
            this.txtSqlAddress.Location = new System.Drawing.Point(87, 12);
            this.txtSqlAddress.Name = "txtSqlAddress";
            this.txtSqlAddress.Size = new System.Drawing.Size(177, 20);
            this.txtSqlAddress.TabIndex = 0;
            this.txtSqlAddress.Text = "192.168.252.99";
            // 
            // btnCheckSqlConnection
            // 
            this.btnCheckSqlConnection.Location = new System.Drawing.Point(15, 168);
            this.btnCheckSqlConnection.Name = "btnCheckSqlConnection";
            this.btnCheckSqlConnection.Size = new System.Drawing.Size(252, 23);
            this.btnCheckSqlConnection.TabIndex = 1;
            this.btnCheckSqlConnection.Text = "Check SQL connection";
            this.btnCheckSqlConnection.UseVisualStyleBackColor = true;
            this.btnCheckSqlConnection.Click += new System.EventHandler(this.btnCheckSqlConnectionString_Click);
            // 
            // lblSqlAddress
            // 
            this.lblSqlAddress.AutoSize = true;
            this.lblSqlAddress.Location = new System.Drawing.Point(12, 15);
            this.lblSqlAddress.Name = "lblSqlAddress";
            this.lblSqlAddress.Size = new System.Drawing.Size(69, 13);
            this.lblSqlAddress.TabIndex = 2;
            this.lblSqlAddress.Text = "SQL Address";
            // 
            // btnConvert
            // 
            this.btnConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConvert.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnConvert.BackColor = System.Drawing.Color.Transparent;
            this.btnConvert.BackgroundImage = global::SqlToPostgre.Properties.Resources.Convert;
            this.btnConvert.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnConvert.Font = new System.Drawing.Font("Segoe UI Semibold", 18.25F, System.Drawing.FontStyle.Bold);
            this.btnConvert.ForeColor = System.Drawing.Color.Black;
            this.btnConvert.Location = new System.Drawing.Point(674, 12);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(306, 118);
            this.btnConvert.TabIndex = 3;
            this.btnConvert.Text = "Convert";
            this.btnConvert.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnConvert.UseVisualStyleBackColor = false;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStatus.Location = new System.Drawing.Point(0, 245);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtStatus.Size = new System.Drawing.Size(994, 216);
            this.txtStatus.TabIndex = 4;
            // 
            // lblSqlCatalog
            // 
            this.lblSqlCatalog.AutoSize = true;
            this.lblSqlCatalog.Location = new System.Drawing.Point(12, 41);
            this.lblSqlCatalog.Name = "lblSqlCatalog";
            this.lblSqlCatalog.Size = new System.Drawing.Size(67, 13);
            this.lblSqlCatalog.TabIndex = 6;
            this.lblSqlCatalog.Text = "SQL Catalog";
            // 
            // txtSqlCatalog
            // 
            this.txtSqlCatalog.Location = new System.Drawing.Point(87, 38);
            this.txtSqlCatalog.Name = "txtSqlCatalog";
            this.txtSqlCatalog.Size = new System.Drawing.Size(177, 20);
            this.txtSqlCatalog.TabIndex = 5;
            this.txtSqlCatalog.Text = "GPSPROTECTMAIN";
            // 
            // lblSqlPass
            // 
            this.lblSqlPass.AutoSize = true;
            this.lblSqlPass.Location = new System.Drawing.Point(12, 93);
            this.lblSqlPass.Name = "lblSqlPass";
            this.lblSqlPass.Size = new System.Drawing.Size(54, 13);
            this.lblSqlPass.TabIndex = 10;
            this.lblSqlPass.Text = "SQL Pass";
            // 
            // txtSqlPass
            // 
            this.txtSqlPass.Location = new System.Drawing.Point(87, 90);
            this.txtSqlPass.Name = "txtSqlPass";
            this.txtSqlPass.PasswordChar = '*';
            this.txtSqlPass.Size = new System.Drawing.Size(177, 20);
            this.txtSqlPass.TabIndex = 9;
            this.txtSqlPass.Text = "millenium_falcon_1";
            // 
            // lblSqlUser
            // 
            this.lblSqlUser.AutoSize = true;
            this.lblSqlUser.Location = new System.Drawing.Point(12, 67);
            this.lblSqlUser.Name = "lblSqlUser";
            this.lblSqlUser.Size = new System.Drawing.Size(53, 13);
            this.lblSqlUser.TabIndex = 8;
            this.lblSqlUser.Text = "SQL User";
            // 
            // txtSqlUser
            // 
            this.txtSqlUser.Location = new System.Drawing.Point(87, 64);
            this.txtSqlUser.Name = "txtSqlUser";
            this.txtSqlUser.Size = new System.Drawing.Size(177, 20);
            this.txtSqlUser.TabIndex = 7;
            this.txtSqlUser.Text = "sa";
            // 
            // lblSqlProvider
            // 
            this.lblSqlProvider.AutoSize = true;
            this.lblSqlProvider.Location = new System.Drawing.Point(12, 119);
            this.lblSqlProvider.Name = "lblSqlProvider";
            this.lblSqlProvider.Size = new System.Drawing.Size(70, 13);
            this.lblSqlProvider.TabIndex = 12;
            this.lblSqlProvider.Text = "SQL Provider";
            // 
            // txtSqlProvider
            // 
            this.txtSqlProvider.Location = new System.Drawing.Point(87, 116);
            this.txtSqlProvider.Name = "txtSqlProvider";
            this.txtSqlProvider.Size = new System.Drawing.Size(177, 20);
            this.txtSqlProvider.TabIndex = 11;
            this.txtSqlProvider.Text = "System.Data.SqlClient";
            // 
            // btnClearStatus
            // 
            this.btnClearStatus.Location = new System.Drawing.Point(0, 214);
            this.btnClearStatus.Name = "btnClearStatus";
            this.btnClearStatus.Size = new System.Drawing.Size(110, 32);
            this.btnClearStatus.TabIndex = 13;
            this.btnClearStatus.Text = "Clear status";
            this.btnClearStatus.UseVisualStyleBackColor = true;
            this.btnClearStatus.Click += new System.EventHandler(this.btnClearStatus_Click);
            // 
            // lblSqlPort
            // 
            this.lblSqlPort.AutoSize = true;
            this.lblSqlPort.Location = new System.Drawing.Point(12, 145);
            this.lblSqlPort.Name = "lblSqlPort";
            this.lblSqlPort.Size = new System.Drawing.Size(50, 13);
            this.lblSqlPort.TabIndex = 16;
            this.lblSqlPort.Text = "SQL Port";
            // 
            // txtSqlPort
            // 
            this.txtSqlPort.Location = new System.Drawing.Point(87, 142);
            this.txtSqlPort.Name = "txtSqlPort";
            this.txtSqlPort.Size = new System.Drawing.Size(177, 20);
            this.txtSqlPort.TabIndex = 15;
            this.txtSqlPort.Text = "1433";
            // 
            // lblPostgrePort
            // 
            this.lblPostgrePort.AutoSize = true;
            this.lblPostgrePort.Location = new System.Drawing.Point(337, 145);
            this.lblPostgrePort.Name = "lblPostgrePort";
            this.lblPostgrePort.Size = new System.Drawing.Size(65, 13);
            this.lblPostgrePort.TabIndex = 29;
            this.lblPostgrePort.Text = "Postgre Port";
            // 
            // txtPostgrePort
            // 
            this.txtPostgrePort.Location = new System.Drawing.Point(427, 142);
            this.txtPostgrePort.Name = "txtPostgrePort";
            this.txtPostgrePort.Size = new System.Drawing.Size(177, 20);
            this.txtPostgrePort.TabIndex = 28;
            this.txtPostgrePort.Text = "5432";
            // 
            // lblPostgreProvider
            // 
            this.lblPostgreProvider.AutoSize = true;
            this.lblPostgreProvider.Location = new System.Drawing.Point(337, 119);
            this.lblPostgreProvider.Name = "lblPostgreProvider";
            this.lblPostgreProvider.Size = new System.Drawing.Size(85, 13);
            this.lblPostgreProvider.TabIndex = 27;
            this.lblPostgreProvider.Text = "Postgre Provider";
            // 
            // txtPostgreProvider
            // 
            this.txtPostgreProvider.Location = new System.Drawing.Point(427, 116);
            this.txtPostgreProvider.Name = "txtPostgreProvider";
            this.txtPostgreProvider.Size = new System.Drawing.Size(177, 20);
            this.txtPostgreProvider.TabIndex = 26;
            this.txtPostgreProvider.Text = "Npgsql";
            // 
            // lblPostgrePass
            // 
            this.lblPostgrePass.AutoSize = true;
            this.lblPostgrePass.Location = new System.Drawing.Point(337, 93);
            this.lblPostgrePass.Name = "lblPostgrePass";
            this.lblPostgrePass.Size = new System.Drawing.Size(69, 13);
            this.lblPostgrePass.TabIndex = 25;
            this.lblPostgrePass.Text = "Postgre Pass";
            // 
            // txtPostgrePass
            // 
            this.txtPostgrePass.Location = new System.Drawing.Point(427, 90);
            this.txtPostgrePass.Name = "txtPostgrePass";
            this.txtPostgrePass.PasswordChar = '*';
            this.txtPostgrePass.Size = new System.Drawing.Size(177, 20);
            this.txtPostgrePass.TabIndex = 24;
            this.txtPostgrePass.Text = "postgres";
            // 
            // lblPostgreUser
            // 
            this.lblPostgreUser.AutoSize = true;
            this.lblPostgreUser.Location = new System.Drawing.Point(337, 67);
            this.lblPostgreUser.Name = "lblPostgreUser";
            this.lblPostgreUser.Size = new System.Drawing.Size(68, 13);
            this.lblPostgreUser.TabIndex = 23;
            this.lblPostgreUser.Text = "Postgre User";
            // 
            // txtPostgreUser
            // 
            this.txtPostgreUser.Location = new System.Drawing.Point(427, 64);
            this.txtPostgreUser.Name = "txtPostgreUser";
            this.txtPostgreUser.Size = new System.Drawing.Size(177, 20);
            this.txtPostgreUser.TabIndex = 22;
            this.txtPostgreUser.Text = "postgres";
            // 
            // lblPostgreCatalog
            // 
            this.lblPostgreCatalog.AutoSize = true;
            this.lblPostgreCatalog.Location = new System.Drawing.Point(337, 41);
            this.lblPostgreCatalog.Name = "lblPostgreCatalog";
            this.lblPostgreCatalog.Size = new System.Drawing.Size(82, 13);
            this.lblPostgreCatalog.TabIndex = 21;
            this.lblPostgreCatalog.Text = "Postgre Catalog";
            // 
            // txtPostgreCatalog
            // 
            this.txtPostgreCatalog.Location = new System.Drawing.Point(427, 38);
            this.txtPostgreCatalog.Name = "txtPostgreCatalog";
            this.txtPostgreCatalog.Size = new System.Drawing.Size(177, 20);
            this.txtPostgreCatalog.TabIndex = 20;
            this.txtPostgreCatalog.Text = "gpsprotectmain";
            // 
            // lblPostgreAddress
            // 
            this.lblPostgreAddress.AutoSize = true;
            this.lblPostgreAddress.Location = new System.Drawing.Point(337, 15);
            this.lblPostgreAddress.Name = "lblPostgreAddress";
            this.lblPostgreAddress.Size = new System.Drawing.Size(84, 13);
            this.lblPostgreAddress.TabIndex = 19;
            this.lblPostgreAddress.Text = "Postgre Address";
            // 
            // txtPostgreAddress
            // 
            this.txtPostgreAddress.Location = new System.Drawing.Point(427, 12);
            this.txtPostgreAddress.Name = "txtPostgreAddress";
            this.txtPostgreAddress.Size = new System.Drawing.Size(177, 20);
            this.txtPostgreAddress.TabIndex = 17;
            this.txtPostgreAddress.Text = "127.0.0.1";
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(675, 124);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(304, 10);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 30;
            this.progressBar.Visible = false;
            // 
            // btnCheckPostgreConnection
            // 
            this.btnCheckPostgreConnection.Location = new System.Drawing.Point(352, 168);
            this.btnCheckPostgreConnection.Name = "btnCheckPostgreConnection";
            this.btnCheckPostgreConnection.Size = new System.Drawing.Size(252, 23);
            this.btnCheckPostgreConnection.TabIndex = 31;
            this.btnCheckPostgreConnection.Text = "Check Postgre connection";
            this.btnCheckPostgreConnection.UseVisualStyleBackColor = true;
            this.btnCheckPostgreConnection.Click += new System.EventHandler(this.btnCheckPostgreConnection_Click);
            // 
            // btnCancelConvertion
            // 
            this.btnCancelConvertion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelConvertion.Location = new System.Drawing.Point(674, 140);
            this.btnCancelConvertion.Name = "btnCancelConvertion";
            this.btnCancelConvertion.Size = new System.Drawing.Size(306, 23);
            this.btnCancelConvertion.TabIndex = 34;
            this.btnCancelConvertion.Text = "Cancel convertion";
            this.btnCancelConvertion.UseVisualStyleBackColor = true;
            this.btnCancelConvertion.Visible = false;
            this.btnCancelConvertion.Click += new System.EventHandler(this.btnCancelConvertion_Click);
            // 
            // txtFetchRows
            // 
            this.txtFetchRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFetchRows.Location = new System.Drawing.Point(801, 180);
            this.txtFetchRows.Name = "txtFetchRows";
            this.txtFetchRows.Size = new System.Drawing.Size(178, 20);
            this.txtFetchRows.TabIndex = 32;
            this.txtFetchRows.Text = "100000";
            // 
            // lblFetchRows
            // 
            this.lblFetchRows.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFetchRows.AutoSize = true;
            this.lblFetchRows.Location = new System.Drawing.Point(672, 174);
            this.lblFetchRows.Name = "lblFetchRows";
            this.lblFetchRows.Size = new System.Drawing.Size(123, 26);
            this.lblFetchRows.TabIndex = 33;
            this.lblFetchRows.Text = "Number of rows to fetch \r\nfrom SQL table per cycle";
            // 
            // chkAutoScrollStatus
            // 
            this.chkAutoScrollStatus.AutoSize = true;
            this.chkAutoScrollStatus.Location = new System.Drawing.Point(116, 222);
            this.chkAutoScrollStatus.Name = "chkAutoScrollStatus";
            this.chkAutoScrollStatus.Size = new System.Drawing.Size(106, 17);
            this.chkAutoScrollStatus.TabIndex = 35;
            this.chkAutoScrollStatus.Text = "Auto-scroll status";
            this.chkAutoScrollStatus.UseVisualStyleBackColor = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(994, 461);
            this.Controls.Add(this.chkAutoScrollStatus);
            this.Controls.Add(this.btnCancelConvertion);
            this.Controls.Add(this.lblFetchRows);
            this.Controls.Add(this.txtFetchRows);
            this.Controls.Add(this.btnCheckPostgreConnection);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblPostgrePort);
            this.Controls.Add(this.txtPostgrePort);
            this.Controls.Add(this.lblPostgreProvider);
            this.Controls.Add(this.txtPostgreProvider);
            this.Controls.Add(this.lblPostgrePass);
            this.Controls.Add(this.txtPostgrePass);
            this.Controls.Add(this.lblPostgreUser);
            this.Controls.Add(this.txtPostgreUser);
            this.Controls.Add(this.lblPostgreCatalog);
            this.Controls.Add(this.txtPostgreCatalog);
            this.Controls.Add(this.lblPostgreAddress);
            this.Controls.Add(this.txtPostgreAddress);
            this.Controls.Add(this.lblSqlPort);
            this.Controls.Add(this.txtSqlPort);
            this.Controls.Add(this.btnClearStatus);
            this.Controls.Add(this.lblSqlProvider);
            this.Controls.Add(this.txtSqlProvider);
            this.Controls.Add(this.lblSqlPass);
            this.Controls.Add(this.txtSqlPass);
            this.Controls.Add(this.lblSqlUser);
            this.Controls.Add(this.txtSqlUser);
            this.Controls.Add(this.lblSqlCatalog);
            this.Controls.Add(this.txtSqlCatalog);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnConvert);
            this.Controls.Add(this.lblSqlAddress);
            this.Controls.Add(this.btnCheckSqlConnection);
            this.Controls.Add(this.txtSqlAddress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimumSize = new System.Drawing.Size(1010, 500);
            this.Name = "Main";
            this.Text = "Main";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSqlAddress;
        private System.Windows.Forms.Button btnCheckSqlConnection;
        private System.Windows.Forms.Label lblSqlAddress;
        private System.Windows.Forms.Button btnConvert;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label lblSqlCatalog;
        private System.Windows.Forms.TextBox txtSqlCatalog;
        private System.Windows.Forms.Label lblSqlPass;
        private System.Windows.Forms.TextBox txtSqlPass;
        private System.Windows.Forms.Label lblSqlUser;
        private System.Windows.Forms.TextBox txtSqlUser;
        private System.Windows.Forms.Label lblSqlProvider;
        private System.Windows.Forms.TextBox txtSqlProvider;
        private System.Windows.Forms.Button btnClearStatus;
        private System.Windows.Forms.Label lblSqlPort;
        private System.Windows.Forms.TextBox txtSqlPort;
        private System.Windows.Forms.Label lblPostgrePort;
        private System.Windows.Forms.TextBox txtPostgrePort;
        private System.Windows.Forms.Label lblPostgreProvider;
        private System.Windows.Forms.TextBox txtPostgreProvider;
        private System.Windows.Forms.Label lblPostgrePass;
        private System.Windows.Forms.TextBox txtPostgrePass;
        private System.Windows.Forms.Label lblPostgreUser;
        private System.Windows.Forms.TextBox txtPostgreUser;
        private System.Windows.Forms.Label lblPostgreCatalog;
        private System.Windows.Forms.TextBox txtPostgreCatalog;
        private System.Windows.Forms.Label lblPostgreAddress;
        private System.Windows.Forms.TextBox txtPostgreAddress;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnCheckPostgreConnection;
        private System.Windows.Forms.Button btnCancelConvertion;
        private System.Windows.Forms.TextBox txtFetchRows;
        private System.Windows.Forms.Label lblFetchRows;
        private System.Windows.Forms.CheckBox chkAutoScrollStatus;
    }
}


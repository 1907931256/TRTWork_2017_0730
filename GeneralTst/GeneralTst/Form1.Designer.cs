namespace GeneralTst
{
    partial class General
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(General));
            this.ApkFileShow = new System.Windows.Forms.TextBox();
            this.dispalyTxt = new System.Windows.Forms.TextBox();
            this.adbdevices_txt = new System.Windows.Forms.TextBox();
            this.LoadApkFile = new System.Windows.Forms.Button();
            this.installBtn = new System.Windows.Forms.Button();
            this.uninstallBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Wifi_radioBtn = new System.Windows.Forms.RadioButton();
            this.usb_radioBtn = new System.Windows.Forms.RadioButton();
            this.connect = new System.Windows.Forms.Button();
            this.disconnect_btn = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmdSendBtn = new System.Windows.Forms.Button();
            this.cmdComBox = new System.Windows.Forms.ComboBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.start_Apk = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ApkFileShow
            // 
            this.ApkFileShow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ApkFileShow.ForeColor = System.Drawing.SystemColors.Window;
            this.ApkFileShow.Location = new System.Drawing.Point(10, 7);
            this.ApkFileShow.Name = "ApkFileShow";
            this.ApkFileShow.Size = new System.Drawing.Size(847, 21);
            this.ApkFileShow.TabIndex = 0;
            // 
            // dispalyTxt
            // 
            this.dispalyTxt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dispalyTxt.Font = new System.Drawing.Font("宋体", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dispalyTxt.ForeColor = System.Drawing.Color.White;
            this.dispalyTxt.Location = new System.Drawing.Point(12, 44);
            this.dispalyTxt.Multiline = true;
            this.dispalyTxt.Name = "dispalyTxt";
            this.dispalyTxt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.dispalyTxt.Size = new System.Drawing.Size(563, 309);
            this.dispalyTxt.TabIndex = 1;
            this.dispalyTxt.WordWrap = false;
            // 
            // adbdevices_txt
            // 
            this.adbdevices_txt.Location = new System.Drawing.Point(611, 44);
            this.adbdevices_txt.Multiline = true;
            this.adbdevices_txt.Name = "adbdevices_txt";
            this.adbdevices_txt.Size = new System.Drawing.Size(249, 35);
            this.adbdevices_txt.TabIndex = 2;
            // 
            // LoadApkFile
            // 
            this.LoadApkFile.Location = new System.Drawing.Point(611, 94);
            this.LoadApkFile.Name = "LoadApkFile";
            this.LoadApkFile.Size = new System.Drawing.Size(114, 42);
            this.LoadApkFile.TabIndex = 3;
            this.LoadApkFile.Text = "APK路径设置";
            this.LoadApkFile.UseVisualStyleBackColor = true;
            this.LoadApkFile.Click += new System.EventHandler(this.LoadApkFile_Click);
            // 
            // installBtn
            // 
            this.installBtn.Location = new System.Drawing.Point(611, 155);
            this.installBtn.Name = "installBtn";
            this.installBtn.Size = new System.Drawing.Size(114, 42);
            this.installBtn.TabIndex = 3;
            this.installBtn.Text = "安装APK";
            this.installBtn.UseVisualStyleBackColor = true;
            this.installBtn.Click += new System.EventHandler(this.installBtn_Click);
            // 
            // uninstallBtn
            // 
            this.uninstallBtn.Location = new System.Drawing.Point(746, 155);
            this.uninstallBtn.Name = "uninstallBtn";
            this.uninstallBtn.Size = new System.Drawing.Size(114, 42);
            this.uninstallBtn.TabIndex = 3;
            this.uninstallBtn.Text = "卸载APK";
            this.uninstallBtn.UseVisualStyleBackColor = true;
            this.uninstallBtn.Click += new System.EventHandler(this.uninstallBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Wifi_radioBtn);
            this.groupBox1.Controls.Add(this.usb_radioBtn);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(616, 223);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(241, 59);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "连接方式";
            // 
            // Wifi_radioBtn
            // 
            this.Wifi_radioBtn.AutoSize = true;
            this.Wifi_radioBtn.Location = new System.Drawing.Point(166, 34);
            this.Wifi_radioBtn.Name = "Wifi_radioBtn";
            this.Wifi_radioBtn.Size = new System.Drawing.Size(47, 16);
            this.Wifi_radioBtn.TabIndex = 1;
            this.Wifi_radioBtn.TabStop = true;
            this.Wifi_radioBtn.Text = "Wifi";
            this.Wifi_radioBtn.UseVisualStyleBackColor = true;
            // 
            // usb_radioBtn
            // 
            this.usb_radioBtn.AutoSize = true;
            this.usb_radioBtn.Location = new System.Drawing.Point(20, 34);
            this.usb_radioBtn.Name = "usb_radioBtn";
            this.usb_radioBtn.Size = new System.Drawing.Size(41, 16);
            this.usb_radioBtn.TabIndex = 1;
            this.usb_radioBtn.TabStop = true;
            this.usb_radioBtn.Text = "USB";
            this.usb_radioBtn.UseVisualStyleBackColor = true;
            // 
            // connect
            // 
            this.connect.ForeColor = System.Drawing.Color.Black;
            this.connect.Location = new System.Drawing.Point(616, 311);
            this.connect.Name = "connect";
            this.connect.Size = new System.Drawing.Size(114, 42);
            this.connect.TabIndex = 0;
            this.connect.Text = "连接";
            this.connect.UseVisualStyleBackColor = true;
            this.connect.Click += new System.EventHandler(this.connect_Click);
            // 
            // disconnect_btn
            // 
            this.disconnect_btn.Location = new System.Drawing.Point(748, 311);
            this.disconnect_btn.Name = "disconnect_btn";
            this.disconnect_btn.Size = new System.Drawing.Size(114, 42);
            this.disconnect_btn.TabIndex = 3;
            this.disconnect_btn.Text = "断开连接";
            this.disconnect_btn.UseVisualStyleBackColor = true;
            this.disconnect_btn.Click += new System.EventHandler(this.disconnect_btn_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmdSendBtn);
            this.groupBox2.Controls.Add(this.cmdComBox);
            this.groupBox2.ForeColor = System.Drawing.Color.Aqua;
            this.groupBox2.Location = new System.Drawing.Point(12, 373);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(850, 80);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "测试指令";
            // 
            // cmdSendBtn
            // 
            this.cmdSendBtn.BackColor = System.Drawing.Color.Aqua;
            this.cmdSendBtn.ForeColor = System.Drawing.Color.Black;
            this.cmdSendBtn.Location = new System.Drawing.Point(728, 13);
            this.cmdSendBtn.Name = "cmdSendBtn";
            this.cmdSendBtn.Size = new System.Drawing.Size(114, 61);
            this.cmdSendBtn.TabIndex = 1;
            this.cmdSendBtn.Text = "指令发送";
            this.cmdSendBtn.UseVisualStyleBackColor = false;
            this.cmdSendBtn.Click += new System.EventHandler(this.cmdSendBtn_Click);
            // 
            // cmdComBox
            // 
            this.cmdComBox.FormattingEnabled = true;
            this.cmdComBox.Location = new System.Drawing.Point(6, 35);
            this.cmdComBox.Name = "cmdComBox";
            this.cmdComBox.Size = new System.Drawing.Size(697, 20);
            this.cmdComBox.TabIndex = 0;
            this.cmdComBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmdComBox_KeyDown);
            // 
            // textBox4
            // 
            this.textBox4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.textBox4.ForeColor = System.Drawing.Color.White;
            this.textBox4.Location = new System.Drawing.Point(10, 459);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox4.Size = new System.Drawing.Size(705, 151);
            this.textBox4.TabIndex = 1;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(746, 487);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(114, 42);
            this.button7.TabIndex = 3;
            this.button7.Text = "抓取日志并保存E盘";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(746, 555);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(114, 42);
            this.button8.TabIndex = 3;
            this.button8.Text = "停止抓取";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // start_Apk
            // 
            this.start_Apk.Location = new System.Drawing.Point(748, 94);
            this.start_Apk.Name = "start_Apk";
            this.start_Apk.Size = new System.Drawing.Size(114, 42);
            this.start_Apk.TabIndex = 3;
            this.start_Apk.Text = "启动Apk";
            this.start_Apk.UseVisualStyleBackColor = true;
            this.start_Apk.Click += new System.EventHandler(this.start_Apk_Click);
            // 
            // General
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(872, 622);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.connect);
            this.Controls.Add(this.disconnect_btn);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.uninstallBtn);
            this.Controls.Add(this.installBtn);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.start_Apk);
            this.Controls.Add(this.LoadApkFile);
            this.Controls.Add(this.adbdevices_txt);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.dispalyTxt);
            this.Controls.Add(this.ApkFileShow);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "General";
            this.Text = "手机调试工具";
            this.Load += new System.EventHandler(this.General_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.General_Paint);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ApkFileShow;
        private System.Windows.Forms.TextBox dispalyTxt;
        private System.Windows.Forms.TextBox adbdevices_txt;
        private System.Windows.Forms.Button LoadApkFile;
        private System.Windows.Forms.Button installBtn;
        private System.Windows.Forms.Button uninstallBtn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton Wifi_radioBtn;
        private System.Windows.Forms.RadioButton usb_radioBtn;
        private System.Windows.Forms.Button connect;
        private System.Windows.Forms.Button disconnect_btn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdSendBtn;
        private System.Windows.Forms.ComboBox cmdComBox;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button start_Apk;
    }
}


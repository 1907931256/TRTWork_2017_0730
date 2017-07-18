namespace TRTCamera
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtXianShi = new System.Windows.Forms.TextBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.button1 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.button4 = new System.Windows.Forms.Button();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.comboPortName = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.uButton13 = new TRTCamera.UButton();
            this.uCheckBox8 = new TRTCamera.UCheckBox();
            this.uCheckBox7 = new TRTCamera.UCheckBox();
            this.uCheckBox6 = new TRTCamera.UCheckBox();
            this.uCheckBox5 = new TRTCamera.UCheckBox();
            this.uCheckBox4 = new TRTCamera.UCheckBox();
            this.uCheckBox3 = new TRTCamera.UCheckBox();
            this.uCheckBox2 = new TRTCamera.UCheckBox();
            this.uCheckBox1 = new TRTCamera.UCheckBox();
            this.uButton6 = new TRTCamera.UButton();
            this.uButton5 = new TRTCamera.UButton();
            this.uButton4 = new TRTCamera.UButton();
            this.uButton3 = new TRTCamera.UButton();
            this.uButton2 = new TRTCamera.UButton();
            this.uButton1 = new TRTCamera.UButton();
            this.uButton7 = new TRTCamera.UButton();
            this.uButton8 = new TRTCamera.UButton();
            this.uButton9 = new TRTCamera.UButton();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(225, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(276, 33);
            this.label2.TabIndex = 57;
            this.label2.Text = "天瑞通硬件调试工具";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(669, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 55;
            this.label1.Text = "Port name";
            // 
            // txtXianShi
            // 
            this.txtXianShi.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtXianShi.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtXianShi.ForeColor = System.Drawing.Color.White;
            this.txtXianShi.Location = new System.Drawing.Point(2, 49);
            this.txtXianShi.Multiline = true;
            this.txtXianShi.Name = "txtXianShi";
            this.txtXianShi.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtXianShi.Size = new System.Drawing.Size(798, 267);
            this.txtXianShi.TabIndex = 54;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(635, 329);
            this.trackBar1.Maximum = 255;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(166, 45);
            this.trackBar1.TabIndex = 62;
            this.trackBar1.TickFrequency = 20;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(491, 332);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(121, 21);
            this.button1.TabIndex = 61;
            this.button1.Text = "2站后色光源调节";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(429, 333);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(48, 20);
            this.comboBox1.Sorted = true;
            this.comboBox1.TabIndex = 60;
            this.comboBox1.Text = "0";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(241, 406);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(57, 20);
            this.button3.TabIndex = 64;
            this.button3.Text = "2站状态灯";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "红灯",
            "黄灯",
            "绿灯"});
            this.comboBox3.Location = new System.Drawing.Point(170, 407);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(48, 20);
            this.comboBox3.TabIndex = 63;
            this.comboBox3.Text = "红灯";
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(429, 374);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(48, 20);
            this.comboBox2.TabIndex = 60;
            this.comboBox2.Text = "0";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(491, 373);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 21);
            this.button2.TabIndex = 61;
            this.button2.Text = "2站OISX电机";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(635, 370);
            this.trackBar2.Maximum = 255;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(166, 45);
            this.trackBar2.TabIndex = 62;
            this.trackBar2.TickFrequency = 20;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar2_Scroll);
            // 
            // comboBox4
            // 
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Location = new System.Drawing.Point(429, 413);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(48, 20);
            this.comboBox4.TabIndex = 60;
            this.comboBox4.Text = "0";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(491, 412);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(121, 21);
            this.button4.TabIndex = 61;
            this.button4.Text = "2站OISZ电机";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // trackBar3
            // 
            this.trackBar3.Location = new System.Drawing.Point(635, 409);
            this.trackBar3.Maximum = 255;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(166, 45);
            this.trackBar3.TabIndex = 62;
            this.trackBar3.TickFrequency = 20;
            this.trackBar3.Scroll += new System.EventHandler(this.trackBar3_Scroll);
            // 
            // comboPortName
            // 
            this.comboPortName.AutoSize = true;
            this.comboPortName.ForeColor = System.Drawing.Color.White;
            this.comboPortName.Location = new System.Drawing.Point(752, 23);
            this.comboPortName.Name = "comboPortName";
            this.comboPortName.Size = new System.Drawing.Size(41, 12);
            this.comboPortName.TabIndex = 65;
            this.comboPortName.Text = "label3";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(251, 438);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(48, 20);
            this.button6.TabIndex = 67;
            this.button6.Text = "清屏";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // uButton13
            // 
            this.uButton13.Location = new System.Drawing.Point(170, 438);
            this.uButton13.Name = "uButton13";
            this.uButton13.Size = new System.Drawing.Size(48, 20);
            this.uButton13.TabIndex = 66;
            this.uButton13.Text = "2站复位";
            this.uButton13.UseVisualStyleBackColor = true;
            // 
            // uCheckBox8
            // 
            this.uCheckBox8.AutoSize = true;
            this.uCheckBox8.ForeColor = System.Drawing.Color.White;
            this.uCheckBox8.Location = new System.Drawing.Point(429, 460);
            this.uCheckBox8.Name = "uCheckBox8";
            this.uCheckBox8.Size = new System.Drawing.Size(90, 16);
            this.uCheckBox8.TabIndex = 59;
            this.uCheckBox8.Tag = "2站后白靠近";
            this.uCheckBox8.Text = "2站后白远离";
            this.uCheckBox8.UseVisualStyleBackColor = true;
            // 
            // uCheckBox7
            // 
            this.uCheckBox7.AutoSize = true;
            this.uCheckBox7.ForeColor = System.Drawing.Color.White;
            this.uCheckBox7.Location = new System.Drawing.Point(317, 486);
            this.uCheckBox7.Name = "uCheckBox7";
            this.uCheckBox7.Size = new System.Drawing.Size(90, 16);
            this.uCheckBox7.TabIndex = 59;
            this.uCheckBox7.Tag = "2站色卡靠近";
            this.uCheckBox7.Text = "2站色卡远离";
            this.uCheckBox7.UseVisualStyleBackColor = true;
            // 
            // uCheckBox6
            // 
            this.uCheckBox6.AutoSize = true;
            this.uCheckBox6.ForeColor = System.Drawing.Color.White;
            this.uCheckBox6.Location = new System.Drawing.Point(317, 460);
            this.uCheckBox6.Name = "uCheckBox6";
            this.uCheckBox6.Size = new System.Drawing.Size(90, 16);
            this.uCheckBox6.TabIndex = 59;
            this.uCheckBox6.Tag = "2站解析靠近";
            this.uCheckBox6.Text = "2站解析远离";
            this.uCheckBox6.UseVisualStyleBackColor = true;
            // 
            // uCheckBox5
            // 
            this.uCheckBox5.AutoSize = true;
            this.uCheckBox5.ForeColor = System.Drawing.Color.White;
            this.uCheckBox5.Location = new System.Drawing.Point(317, 437);
            this.uCheckBox5.Name = "uCheckBox5";
            this.uCheckBox5.Size = new System.Drawing.Size(90, 16);
            this.uCheckBox5.TabIndex = 59;
            this.uCheckBox5.Tag = "2站增距靠近";
            this.uCheckBox5.Text = "2站增距远离";
            this.uCheckBox5.UseVisualStyleBackColor = true;
            // 
            // uCheckBox4
            // 
            this.uCheckBox4.AutoSize = true;
            this.uCheckBox4.ForeColor = System.Drawing.Color.White;
            this.uCheckBox4.Location = new System.Drawing.Point(317, 409);
            this.uCheckBox4.Name = "uCheckBox4";
            this.uCheckBox4.Size = new System.Drawing.Size(90, 16);
            this.uCheckBox4.TabIndex = 59;
            this.uCheckBox4.Tag = "2站前后靠近";
            this.uCheckBox4.Text = "2站前后远离";
            this.uCheckBox4.UseVisualStyleBackColor = true;
            // 
            // uCheckBox3
            // 
            this.uCheckBox3.AutoSize = true;
            this.uCheckBox3.ForeColor = System.Drawing.Color.White;
            this.uCheckBox3.Location = new System.Drawing.Point(317, 382);
            this.uCheckBox3.Name = "uCheckBox3";
            this.uCheckBox3.Size = new System.Drawing.Size(90, 16);
            this.uCheckBox3.TabIndex = 59;
            this.uCheckBox3.Tag = "2站副摄靠近";
            this.uCheckBox3.Text = "2站副摄远离";
            this.uCheckBox3.UseVisualStyleBackColor = true;
            // 
            // uCheckBox2
            // 
            this.uCheckBox2.AutoSize = true;
            this.uCheckBox2.ForeColor = System.Drawing.Color.White;
            this.uCheckBox2.Location = new System.Drawing.Point(317, 355);
            this.uCheckBox2.Name = "uCheckBox2";
            this.uCheckBox2.Size = new System.Drawing.Size(90, 16);
            this.uCheckBox2.TabIndex = 59;
            this.uCheckBox2.Tag = "2站标靶靠近";
            this.uCheckBox2.Text = "2站标靶远离";
            this.uCheckBox2.UseVisualStyleBackColor = true;
            // 
            // uCheckBox1
            // 
            this.uCheckBox1.AutoSize = true;
            this.uCheckBox1.ForeColor = System.Drawing.Color.White;
            this.uCheckBox1.Location = new System.Drawing.Point(317, 333);
            this.uCheckBox1.Name = "uCheckBox1";
            this.uCheckBox1.Size = new System.Drawing.Size(90, 16);
            this.uCheckBox1.TabIndex = 59;
            this.uCheckBox1.Tag = "2站产品固定";
            this.uCheckBox1.Text = "2站产品松开";
            this.uCheckBox1.UseVisualStyleBackColor = true;
            // 
            // uButton6
            // 
            this.uButton6.Location = new System.Drawing.Point(24, 497);
            this.uButton6.Name = "uButton6";
            this.uButton6.Size = new System.Drawing.Size(128, 27);
            this.uButton6.TabIndex = 58;
            this.uButton6.Text = "2站警告取消检测";
            this.uButton6.UseVisualStyleBackColor = true;
            // 
            // uButton5
            // 
            this.uButton5.Location = new System.Drawing.Point(24, 464);
            this.uButton5.Name = "uButton5";
            this.uButton5.Size = new System.Drawing.Size(128, 27);
            this.uButton5.TabIndex = 58;
            this.uButton5.Text = "2站色卡远离检测";
            this.uButton5.UseVisualStyleBackColor = true;
            // 
            // uButton4
            // 
            this.uButton4.Location = new System.Drawing.Point(24, 431);
            this.uButton4.Name = "uButton4";
            this.uButton4.Size = new System.Drawing.Size(128, 27);
            this.uButton4.TabIndex = 58;
            this.uButton4.Text = "2站解析远离检测";
            this.uButton4.UseVisualStyleBackColor = true;
            // 
            // uButton3
            // 
            this.uButton3.Location = new System.Drawing.Point(24, 398);
            this.uButton3.Name = "uButton3";
            this.uButton3.Size = new System.Drawing.Size(128, 27);
            this.uButton3.TabIndex = 58;
            this.uButton3.Text = "2站标靶下到位检测";
            this.uButton3.UseVisualStyleBackColor = true;
            // 
            // uButton2
            // 
            this.uButton2.Location = new System.Drawing.Point(24, 365);
            this.uButton2.Name = "uButton2";
            this.uButton2.Size = new System.Drawing.Size(128, 27);
            this.uButton2.TabIndex = 58;
            this.uButton2.Text = "2站标靶上到位检测";
            this.uButton2.UseVisualStyleBackColor = true;
            // 
            // uButton1
            // 
            this.uButton1.Location = new System.Drawing.Point(24, 332);
            this.uButton1.Name = "uButton1";
            this.uButton1.Size = new System.Drawing.Size(128, 27);
            this.uButton1.TabIndex = 58;
            this.uButton1.Text = "2站产品到位检测";
            this.uButton1.UseVisualStyleBackColor = true;
            // 
            // uButton7
            // 
            this.uButton7.Location = new System.Drawing.Point(170, 332);
            this.uButton7.Name = "uButton7";
            this.uButton7.Size = new System.Drawing.Size(128, 27);
            this.uButton7.TabIndex = 58;
            this.uButton7.Text = "2站增距远离检测";
            this.uButton7.UseVisualStyleBackColor = true;
            // 
            // uButton8
            // 
            this.uButton8.Location = new System.Drawing.Point(170, 365);
            this.uButton8.Name = "uButton8";
            this.uButton8.Size = new System.Drawing.Size(128, 27);
            this.uButton8.TabIndex = 58;
            this.uButton8.Text = "2站后白远离检测";
            this.uButton8.UseVisualStyleBackColor = true;
            // 
            // uButton9
            // 
            this.uButton9.Location = new System.Drawing.Point(170, 475);
            this.uButton9.Name = "uButton9";
            this.uButton9.Size = new System.Drawing.Size(128, 27);
            this.uButton9.TabIndex = 58;
            this.uButton9.Text = "2站色卡电机回零";
            this.uButton9.UseVisualStyleBackColor = true;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(805, 534);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.uButton13);
            this.Controls.Add(this.comboPortName);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.trackBar3);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.comboBox4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.uCheckBox8);
            this.Controls.Add(this.uCheckBox7);
            this.Controls.Add(this.uCheckBox6);
            this.Controls.Add(this.uCheckBox5);
            this.Controls.Add(this.uCheckBox4);
            this.Controls.Add(this.uCheckBox3);
            this.Controls.Add(this.uCheckBox2);
            this.Controls.Add(this.uCheckBox1);
            this.Controls.Add(this.uButton6);
            this.Controls.Add(this.uButton5);
            this.Controls.Add(this.uButton4);
            this.Controls.Add(this.uButton3);
            this.Controls.Add(this.uButton9);
            this.Controls.Add(this.uButton8);
            this.Controls.Add(this.uButton7);
            this.Controls.Add(this.uButton2);
            this.Controls.Add(this.uButton1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtXianShi);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form2";
            this.Text = "Hardware debugging tool station 2";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form2_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtXianShi;
        private UButton uButton1;
        private UButton uButton2;
        private UButton uButton3;
        private UButton uButton4;
        private UButton uButton5;
        private UButton uButton6;
        private UCheckBox uCheckBox1;
        private UCheckBox uCheckBox2;
        private UCheckBox uCheckBox3;
        private UCheckBox uCheckBox4;
        private UCheckBox uCheckBox5;
        private UCheckBox uCheckBox6;
        private UCheckBox uCheckBox7;
        private UCheckBox uCheckBox8;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.Label comboPortName;
        private System.Windows.Forms.Button button6;
        private UButton uButton13;
        private UButton uButton7;
        private UButton uButton8;
        private UButton uButton9;
    }
}
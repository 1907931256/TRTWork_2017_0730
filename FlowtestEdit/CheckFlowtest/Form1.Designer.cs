namespace CheckFlowtest
{
    partial class Form1
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dispalytxt = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.filePathtxt = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dispalytxt
            // 
            this.dispalytxt.Location = new System.Drawing.Point(8, 133);
            this.dispalytxt.Multiline = true;
            this.dispalytxt.Name = "dispalytxt";
            this.dispalytxt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.dispalytxt.Size = new System.Drawing.Size(480, 383);
            this.dispalytxt.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(22, 55);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(182, 54);
            this.button1.TabIndex = 1;
            this.button1.Text = "文件加载";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // filePathtxt
            // 
            this.filePathtxt.Location = new System.Drawing.Point(8, 12);
            this.filePathtxt.Name = "filePathtxt";
            this.filePathtxt.Size = new System.Drawing.Size(464, 21);
            this.filePathtxt.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(279, 55);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(182, 54);
            this.button2.TabIndex = 1;
            this.button2.Text = "开始检测";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 519);
            this.Controls.Add(this.filePathtxt);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dispalytxt);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox dispalytxt;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox filePathtxt;
        private System.Windows.Forms.Button button2;
    }
}


namespace fastCSharp.demo.tcpPerformanceClient
{
    partial class form
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
            this.timesTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.socketTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.packetTextBox = new System.Windows.Forms.TextBox();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.threadTextBox = new System.Windows.Forms.TextBox();
            this.randomCheckBox = new System.Windows.Forms.CheckBox();
            this.asynchronousCheckBox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.clearMemoryPoolButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // timesTextBox
            // 
            this.timesTextBox.Location = new System.Drawing.Point(45, 4);
            this.timesTextBox.Name = "timesTextBox";
            this.timesTextBox.Size = new System.Drawing.Size(52, 21);
            this.timesTextBox.TabIndex = 0;
            this.timesTextBox.Text = "100";
            this.timesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Times";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(103, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Socket";
            // 
            // socketTextBox
            // 
            this.socketTextBox.Location = new System.Drawing.Point(150, 4);
            this.socketTextBox.Name = "socketTextBox";
            this.socketTextBox.Size = new System.Drawing.Size(37, 21);
            this.socketTextBox.TabIndex = 2;
            this.socketTextBox.Text = "100";
            this.socketTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(291, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Packet";
            // 
            // packetTextBox
            // 
            this.packetTextBox.Location = new System.Drawing.Point(332, 3);
            this.packetTextBox.Name = "packetTextBox";
            this.packetTextBox.Size = new System.Drawing.Size(52, 21);
            this.packetTextBox.TabIndex = 4;
            this.packetTextBox.Text = "5000";
            this.packetTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // messageTextBox
            // 
            this.messageTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.messageTextBox.Location = new System.Drawing.Point(-2, 26);
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.ReadOnly = true;
            this.messageTextBox.Size = new System.Drawing.Size(452, 215);
            this.messageTextBox.TabIndex = 6;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(291, 248);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(51, 23);
            this.startButton.TabIndex = 7;
            this.startButton.Text = "START";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.start);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(348, 247);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(46, 23);
            this.stopButton.TabIndex = 8;
            this.stopButton.Text = "STOP";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stop);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(193, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "Thread";
            // 
            // threadTextBox
            // 
            this.threadTextBox.Location = new System.Drawing.Point(234, 3);
            this.threadTextBox.Name = "threadTextBox";
            this.threadTextBox.Size = new System.Drawing.Size(52, 21);
            this.threadTextBox.TabIndex = 10;
            this.threadTextBox.Text = "1";
            this.threadTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // randomCheckBox
            // 
            this.randomCheckBox.AutoSize = true;
            this.randomCheckBox.Location = new System.Drawing.Point(390, 5);
            this.randomCheckBox.Name = "randomCheckBox";
            this.randomCheckBox.Size = new System.Drawing.Size(60, 16);
            this.randomCheckBox.TabIndex = 12;
            this.randomCheckBox.Text = "随机包";
            this.randomCheckBox.UseVisualStyleBackColor = true;
            // 
            // asynchronousCheckBox
            // 
            this.asynchronousCheckBox.AutoSize = true;
            this.asynchronousCheckBox.Checked = true;
            this.asynchronousCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.asynchronousCheckBox.Location = new System.Drawing.Point(237, 252);
            this.asynchronousCheckBox.Name = "asynchronousCheckBox";
            this.asynchronousCheckBox.Size = new System.Drawing.Size(48, 16);
            this.asynchronousCheckBox.TabIndex = 17;
            this.asynchronousCheckBox.Text = "异步";
            this.asynchronousCheckBox.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(150, 252);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "Port";
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(182, 249);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(49, 21);
            this.portTextBox.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 252);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "IP";
            // 
            // ipTextBox
            // 
            this.ipTextBox.Location = new System.Drawing.Point(23, 249);
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(121, 21);
            this.ipTextBox.TabIndex = 13;
            // 
            // clearMemoryPoolButton
            // 
            this.clearMemoryPoolButton.Location = new System.Drawing.Point(400, 247);
            this.clearMemoryPoolButton.Name = "clearMemoryPoolButton";
            this.clearMemoryPoolButton.Size = new System.Drawing.Size(50, 23);
            this.clearMemoryPoolButton.TabIndex = 18;
            this.clearMemoryPoolButton.Text = "CLEAR";
            this.clearMemoryPoolButton.UseVisualStyleBackColor = true;
            this.clearMemoryPoolButton.Click += new System.EventHandler(this.clearMemoryPoolButton_Click);
            // 
            // form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 273);
            this.Controls.Add(this.clearMemoryPoolButton);
            this.Controls.Add(this.asynchronousCheckBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ipTextBox);
            this.Controls.Add(this.randomCheckBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.threadTextBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.packetTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.socketTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.timesTextBox);
            this.Name = "form";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.close);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox timesTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox socketTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox packetTextBox;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox threadTextBox;
        private System.Windows.Forms.CheckBox randomCheckBox;
        private System.Windows.Forms.CheckBox asynchronousCheckBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.Button clearMemoryPoolButton;
    }
}


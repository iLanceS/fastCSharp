namespace fastCSharp.demo.gifScreen
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
            this.bottomTextBox = new System.Windows.Forms.TextBox();
            this.rightTextBox = new System.Windows.Forms.TextBox();
            this.leftTextBox = new System.Windows.Forms.TextBox();
            this.topTextBox = new System.Windows.Forms.TextBox();
            this.set_left_right_top_bottom_button = new System.Windows.Forms.Button();
            this.set_left_button = new System.Windows.Forms.Button();
            this.set_bottom_button = new System.Windows.Forms.Button();
            this.set_right_button = new System.Windows.Forms.Button();
            this.set_top_button = new System.Windows.Forms.Button();
            this.set_left_top_Button = new System.Windows.Forms.Button();
            this.set_left_bottom_Button = new System.Windows.Forms.Button();
            this.set_right_bottom_Button = new System.Windows.Forms.Button();
            this.set_right_top_Button = new System.Windows.Forms.Button();
            this.dec_left_button = new System.Windows.Forms.Button();
            this.inc_left_button = new System.Windows.Forms.Button();
            this.dec_right_button = new System.Windows.Forms.Button();
            this.inc_right_button = new System.Windows.Forms.Button();
            this.inc_bottom_button = new System.Windows.Forms.Button();
            this.inc_top_button = new System.Windows.Forms.Button();
            this.dec_bottom_button = new System.Windows.Forms.Button();
            this.dec_top_button = new System.Windows.Forms.Button();
            this.incTextBox = new System.Windows.Forms.TextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.fileNameTextBox = new System.Windows.Forms.TextBox();
            this.fileButton = new System.Windows.Forms.Button();
            this.frameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // bottomTextBox
            // 
            this.bottomTextBox.Location = new System.Drawing.Point(82, 92);
            this.bottomTextBox.Name = "bottomTextBox";
            this.bottomTextBox.Size = new System.Drawing.Size(46, 21);
            this.bottomTextBox.TabIndex = 0;
            this.bottomTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // rightTextBox
            // 
            this.rightTextBox.Location = new System.Drawing.Point(134, 65);
            this.rightTextBox.Name = "rightTextBox";
            this.rightTextBox.Size = new System.Drawing.Size(46, 21);
            this.rightTextBox.TabIndex = 1;
            this.rightTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // leftTextBox
            // 
            this.leftTextBox.Location = new System.Drawing.Point(30, 63);
            this.leftTextBox.Name = "leftTextBox";
            this.leftTextBox.Size = new System.Drawing.Size(46, 21);
            this.leftTextBox.TabIndex = 2;
            this.leftTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // topTextBox
            // 
            this.topTextBox.Location = new System.Drawing.Point(82, 38);
            this.topTextBox.Name = "topTextBox";
            this.topTextBox.Size = new System.Drawing.Size(46, 21);
            this.topTextBox.TabIndex = 3;
            this.topTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // set_left_right_top_bottom_button
            // 
            this.set_left_right_top_bottom_button.Location = new System.Drawing.Point(82, 63);
            this.set_left_right_top_bottom_button.Name = "set_left_right_top_bottom_button";
            this.set_left_right_top_bottom_button.Size = new System.Drawing.Size(46, 23);
            this.set_left_right_top_bottom_button.TabIndex = 4;
            this.set_left_right_top_bottom_button.Text = "Reset";
            this.set_left_right_top_bottom_button.UseVisualStyleBackColor = true;
            this.set_left_right_top_bottom_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // set_left_button
            // 
            this.set_left_button.Location = new System.Drawing.Point(1, 63);
            this.set_left_button.Name = "set_left_button";
            this.set_left_button.Size = new System.Drawing.Size(25, 23);
            this.set_left_button.TabIndex = 5;
            this.set_left_button.Text = "左";
            this.set_left_button.UseVisualStyleBackColor = true;
            this.set_left_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // set_bottom_button
            // 
            this.set_bottom_button.Location = new System.Drawing.Point(92, 119);
            this.set_bottom_button.Name = "set_bottom_button";
            this.set_bottom_button.Size = new System.Drawing.Size(26, 23);
            this.set_bottom_button.TabIndex = 6;
            this.set_bottom_button.Text = "下";
            this.set_bottom_button.UseVisualStyleBackColor = true;
            this.set_bottom_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // set_right_button
            // 
            this.set_right_button.Location = new System.Drawing.Point(184, 65);
            this.set_right_button.Name = "set_right_button";
            this.set_right_button.Size = new System.Drawing.Size(27, 23);
            this.set_right_button.TabIndex = 7;
            this.set_right_button.Text = "右";
            this.set_right_button.UseVisualStyleBackColor = true;
            this.set_right_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // set_top_button
            // 
            this.set_top_button.Location = new System.Drawing.Point(92, 9);
            this.set_top_button.Name = "set_top_button";
            this.set_top_button.Size = new System.Drawing.Size(26, 23);
            this.set_top_button.TabIndex = 8;
            this.set_top_button.Text = "上";
            this.set_top_button.UseVisualStyleBackColor = true;
            this.set_top_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // set_left_top_Button
            // 
            this.set_left_top_Button.Location = new System.Drawing.Point(32, 36);
            this.set_left_top_Button.Name = "set_left_top_Button";
            this.set_left_top_Button.Size = new System.Drawing.Size(44, 23);
            this.set_left_top_Button.TabIndex = 9;
            this.set_left_top_Button.Text = "左上";
            this.set_left_top_Button.UseVisualStyleBackColor = true;
            this.set_left_top_Button.Click += new System.EventHandler(this.onClickButton);
            // 
            // set_left_bottom_Button
            // 
            this.set_left_bottom_Button.Location = new System.Drawing.Point(32, 92);
            this.set_left_bottom_Button.Name = "set_left_bottom_Button";
            this.set_left_bottom_Button.Size = new System.Drawing.Size(44, 23);
            this.set_left_bottom_Button.TabIndex = 10;
            this.set_left_bottom_Button.Text = "左下";
            this.set_left_bottom_Button.UseVisualStyleBackColor = true;
            this.set_left_bottom_Button.Click += new System.EventHandler(this.onClickButton);
            // 
            // set_right_bottom_Button
            // 
            this.set_right_bottom_Button.Location = new System.Drawing.Point(134, 92);
            this.set_right_bottom_Button.Name = "set_right_bottom_Button";
            this.set_right_bottom_Button.Size = new System.Drawing.Size(44, 23);
            this.set_right_bottom_Button.TabIndex = 11;
            this.set_right_bottom_Button.Text = "右下";
            this.set_right_bottom_Button.UseVisualStyleBackColor = true;
            this.set_right_bottom_Button.Click += new System.EventHandler(this.onClickButton);
            // 
            // set_right_top_Button
            // 
            this.set_right_top_Button.Location = new System.Drawing.Point(134, 36);
            this.set_right_top_Button.Name = "set_right_top_Button";
            this.set_right_top_Button.Size = new System.Drawing.Size(44, 23);
            this.set_right_top_Button.TabIndex = 12;
            this.set_right_top_Button.Text = "右上";
            this.set_right_top_Button.UseVisualStyleBackColor = true;
            this.set_right_top_Button.Click += new System.EventHandler(this.onClickButton);
            // 
            // dec_left_button
            // 
            this.dec_left_button.Location = new System.Drawing.Point(1, 36);
            this.dec_left_button.Name = "dec_left_button";
            this.dec_left_button.Size = new System.Drawing.Size(25, 23);
            this.dec_left_button.TabIndex = 13;
            this.dec_left_button.Text = "←";
            this.dec_left_button.UseVisualStyleBackColor = true;
            this.dec_left_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // inc_left_button
            // 
            this.inc_left_button.Location = new System.Drawing.Point(1, 92);
            this.inc_left_button.Name = "inc_left_button";
            this.inc_left_button.Size = new System.Drawing.Size(25, 23);
            this.inc_left_button.TabIndex = 14;
            this.inc_left_button.Text = "→";
            this.inc_left_button.UseVisualStyleBackColor = true;
            this.inc_left_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // dec_right_button
            // 
            this.dec_right_button.Location = new System.Drawing.Point(186, 38);
            this.dec_right_button.Name = "dec_right_button";
            this.dec_right_button.Size = new System.Drawing.Size(25, 23);
            this.dec_right_button.TabIndex = 16;
            this.dec_right_button.Text = "←";
            this.dec_right_button.UseVisualStyleBackColor = true;
            this.dec_right_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // inc_right_button
            // 
            this.inc_right_button.Location = new System.Drawing.Point(184, 92);
            this.inc_right_button.Name = "inc_right_button";
            this.inc_right_button.Size = new System.Drawing.Size(25, 23);
            this.inc_right_button.TabIndex = 17;
            this.inc_right_button.Text = "→";
            this.inc_right_button.UseVisualStyleBackColor = true;
            this.inc_right_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // inc_bottom_button
            // 
            this.inc_bottom_button.Location = new System.Drawing.Point(124, 119);
            this.inc_bottom_button.Name = "inc_bottom_button";
            this.inc_bottom_button.Size = new System.Drawing.Size(25, 23);
            this.inc_bottom_button.TabIndex = 18;
            this.inc_bottom_button.Text = "↓";
            this.inc_bottom_button.UseVisualStyleBackColor = true;
            this.inc_bottom_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // inc_top_button
            // 
            this.inc_top_button.Location = new System.Drawing.Point(124, 9);
            this.inc_top_button.Name = "inc_top_button";
            this.inc_top_button.Size = new System.Drawing.Size(25, 23);
            this.inc_top_button.TabIndex = 19;
            this.inc_top_button.Text = "↓";
            this.inc_top_button.UseVisualStyleBackColor = true;
            this.inc_top_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // dec_bottom_button
            // 
            this.dec_bottom_button.Location = new System.Drawing.Point(61, 119);
            this.dec_bottom_button.Name = "dec_bottom_button";
            this.dec_bottom_button.Size = new System.Drawing.Size(25, 23);
            this.dec_bottom_button.TabIndex = 20;
            this.dec_bottom_button.Text = "↑";
            this.dec_bottom_button.UseVisualStyleBackColor = true;
            this.dec_bottom_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // dec_top_button
            // 
            this.dec_top_button.Location = new System.Drawing.Point(61, 9);
            this.dec_top_button.Name = "dec_top_button";
            this.dec_top_button.Size = new System.Drawing.Size(25, 23);
            this.dec_top_button.TabIndex = 21;
            this.dec_top_button.Text = "↑";
            this.dec_top_button.UseVisualStyleBackColor = true;
            this.dec_top_button.Click += new System.EventHandler(this.onClickButton);
            // 
            // incTextBox
            // 
            this.incTextBox.Location = new System.Drawing.Point(167, 7);
            this.incTextBox.Name = "incTextBox";
            this.incTextBox.Size = new System.Drawing.Size(25, 21);
            this.incTextBox.TabIndex = 22;
            this.incTextBox.Text = "1";
            this.incTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(12, 7);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(44, 23);
            this.startButton.TabIndex = 23;
            this.startButton.Text = "开始";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(12, 119);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(44, 23);
            this.stopButton.TabIndex = 24;
            this.stopButton.Text = "停止";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // fileNameTextBox
            // 
            this.fileNameTextBox.Location = new System.Drawing.Point(1, 148);
            this.fileNameTextBox.Name = "fileNameTextBox";
            this.fileNameTextBox.Size = new System.Drawing.Size(160, 21);
            this.fileNameTextBox.TabIndex = 25;
            // 
            // fileButton
            // 
            this.fileButton.Enabled = false;
            this.fileButton.Location = new System.Drawing.Point(167, 148);
            this.fileButton.Name = "fileButton";
            this.fileButton.Size = new System.Drawing.Size(44, 23);
            this.fileButton.TabIndex = 26;
            this.fileButton.Text = "文件";
            this.fileButton.UseVisualStyleBackColor = true;
            this.fileButton.Click += new System.EventHandler(this.fileButton_Click);
            // 
            // frameTextBox
            // 
            this.frameTextBox.Location = new System.Drawing.Point(167, 121);
            this.frameTextBox.Name = "frameTextBox";
            this.frameTextBox.Size = new System.Drawing.Size(25, 21);
            this.frameTextBox.TabIndex = 27;
            this.frameTextBox.Tag = "";
            this.frameTextBox.Text = "24";
            this.frameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(192, 130);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 28;
            this.label1.Text = "帧";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(198, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 29;
            this.label2.Text = "px";
            // 
            // form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 175);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.frameTextBox);
            this.Controls.Add(this.fileButton);
            this.Controls.Add(this.fileNameTextBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.incTextBox);
            this.Controls.Add(this.dec_top_button);
            this.Controls.Add(this.dec_bottom_button);
            this.Controls.Add(this.inc_top_button);
            this.Controls.Add(this.inc_bottom_button);
            this.Controls.Add(this.inc_right_button);
            this.Controls.Add(this.dec_right_button);
            this.Controls.Add(this.inc_left_button);
            this.Controls.Add(this.dec_left_button);
            this.Controls.Add(this.set_right_top_Button);
            this.Controls.Add(this.set_right_bottom_Button);
            this.Controls.Add(this.set_left_bottom_Button);
            this.Controls.Add(this.set_left_top_Button);
            this.Controls.Add(this.set_top_button);
            this.Controls.Add(this.set_right_button);
            this.Controls.Add(this.set_bottom_button);
            this.Controls.Add(this.set_left_button);
            this.Controls.Add(this.set_left_right_top_bottom_button);
            this.Controls.Add(this.topTextBox);
            this.Controls.Add(this.leftTextBox);
            this.Controls.Add(this.rightTextBox);
            this.Controls.Add(this.bottomTextBox);
            this.Name = "form";
            this.Text = "gifScreen - fastCSharp";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.onFormClose);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox bottomTextBox;
        private System.Windows.Forms.TextBox rightTextBox;
        private System.Windows.Forms.TextBox leftTextBox;
        private System.Windows.Forms.TextBox topTextBox;
        private System.Windows.Forms.Button set_left_right_top_bottom_button;
        private System.Windows.Forms.Button set_left_button;
        private System.Windows.Forms.Button set_bottom_button;
        private System.Windows.Forms.Button set_right_button;
        private System.Windows.Forms.Button set_top_button;
        private System.Windows.Forms.Button set_left_top_Button;
        private System.Windows.Forms.Button set_left_bottom_Button;
        private System.Windows.Forms.Button set_right_bottom_Button;
        private System.Windows.Forms.Button set_right_top_Button;
        private System.Windows.Forms.Button dec_left_button;
        private System.Windows.Forms.Button inc_left_button;
        private System.Windows.Forms.Button dec_right_button;
        private System.Windows.Forms.Button inc_right_button;
        private System.Windows.Forms.Button inc_bottom_button;
        private System.Windows.Forms.Button inc_top_button;
        private System.Windows.Forms.Button dec_bottom_button;
        private System.Windows.Forms.Button dec_top_button;
        private System.Windows.Forms.TextBox incTextBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.TextBox fileNameTextBox;
        private System.Windows.Forms.Button fileButton;
        private System.Windows.Forms.TextBox frameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}


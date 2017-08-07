namespace fastCSharp.demo.chatClient
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
            this.userNameTextBox = new System.Windows.Forms.TextBox();
            this.loginButton = new System.Windows.Forms.Button();
            this.logoutButton = new System.Windows.Forms.Button();
            this.userListBox = new System.Windows.Forms.CheckedListBox();
            this.allUserButton = new System.Windows.Forms.Button();
            this.changeUserButton = new System.Windows.Forms.Button();
            this.clearUserButton = new System.Windows.Forms.Button();
            this.sendTextBox = new System.Windows.Forms.TextBox();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.Location = new System.Drawing.Point(406, 304);
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(115, 21);
            this.userNameTextBox.TabIndex = 0;
            // 
            // loginButton
            // 
            this.loginButton.Location = new System.Drawing.Point(527, 304);
            this.loginButton.Name = "loginButton";
            this.loginButton.Size = new System.Drawing.Size(50, 23);
            this.loginButton.TabIndex = 1;
            this.loginButton.Text = "登录";
            this.loginButton.UseVisualStyleBackColor = true;
            this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
            // 
            // logoutButton
            // 
            this.logoutButton.Enabled = false;
            this.logoutButton.Location = new System.Drawing.Point(583, 304);
            this.logoutButton.Name = "logoutButton";
            this.logoutButton.Size = new System.Drawing.Size(47, 23);
            this.logoutButton.TabIndex = 2;
            this.logoutButton.Text = "退出";
            this.logoutButton.UseVisualStyleBackColor = true;
            this.logoutButton.Click += new System.EventHandler(this.logoutButton_Click);
            // 
            // userListBox
            // 
            this.userListBox.CheckOnClick = true;
            this.userListBox.Enabled = false;
            this.userListBox.FormattingEnabled = true;
            this.userListBox.Location = new System.Drawing.Point(12, 35);
            this.userListBox.Name = "userListBox";
            this.userListBox.Size = new System.Drawing.Size(153, 292);
            this.userListBox.TabIndex = 3;
            // 
            // allUserButton
            // 
            this.allUserButton.Enabled = false;
            this.allUserButton.Location = new System.Drawing.Point(12, 6);
            this.allUserButton.Name = "allUserButton";
            this.allUserButton.Size = new System.Drawing.Size(44, 23);
            this.allUserButton.TabIndex = 5;
            this.allUserButton.Text = "全选";
            this.allUserButton.UseVisualStyleBackColor = true;
            this.allUserButton.Click += new System.EventHandler(this.allUserButton_Click);
            // 
            // changeUserButton
            // 
            this.changeUserButton.Enabled = false;
            this.changeUserButton.Location = new System.Drawing.Point(62, 6);
            this.changeUserButton.Name = "changeUserButton";
            this.changeUserButton.Size = new System.Drawing.Size(45, 23);
            this.changeUserButton.TabIndex = 6;
            this.changeUserButton.Text = "反选";
            this.changeUserButton.UseVisualStyleBackColor = true;
            this.changeUserButton.Click += new System.EventHandler(this.changeUserButton_Click);
            // 
            // clearUserButton
            // 
            this.clearUserButton.Enabled = false;
            this.clearUserButton.Location = new System.Drawing.Point(113, 6);
            this.clearUserButton.Name = "clearUserButton";
            this.clearUserButton.Size = new System.Drawing.Size(52, 23);
            this.clearUserButton.TabIndex = 7;
            this.clearUserButton.Text = "清除";
            this.clearUserButton.UseVisualStyleBackColor = true;
            this.clearUserButton.Click += new System.EventHandler(this.clearUserButton_Click);
            // 
            // sendTextBox
            // 
            this.sendTextBox.Enabled = false;
            this.sendTextBox.Location = new System.Drawing.Point(172, 208);
            this.sendTextBox.Multiline = true;
            this.sendTextBox.Name = "sendTextBox";
            this.sendTextBox.Size = new System.Drawing.Size(458, 90);
            this.sendTextBox.TabIndex = 8;
            // 
            // messageTextBox
            // 
            this.messageTextBox.BackColor = System.Drawing.SystemColors.Info;
            this.messageTextBox.Location = new System.Drawing.Point(172, 6);
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.ReadOnly = true;
            this.messageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.messageTextBox.Size = new System.Drawing.Size(458, 196);
            this.messageTextBox.TabIndex = 9;
            // 
            // sendButton
            // 
            this.sendButton.Enabled = false;
            this.sendButton.Location = new System.Drawing.Point(172, 304);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(228, 23);
            this.sendButton.TabIndex = 10;
            this.sendButton.Text = "发送";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 339);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.sendTextBox);
            this.Controls.Add(this.clearUserButton);
            this.Controls.Add(this.changeUserButton);
            this.Controls.Add(this.allUserButton);
            this.Controls.Add(this.userListBox);
            this.Controls.Add(this.logoutButton);
            this.Controls.Add(this.loginButton);
            this.Controls.Add(this.userNameTextBox);
            this.Name = "form";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.onFormClose);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Button loginButton;
        private System.Windows.Forms.Button logoutButton;
        private System.Windows.Forms.CheckedListBox userListBox;
        private System.Windows.Forms.Button allUserButton;
        private System.Windows.Forms.Button changeUserButton;
        private System.Windows.Forms.Button clearUserButton;
        private System.Windows.Forms.TextBox sendTextBox;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.Button sendButton;
    }
}


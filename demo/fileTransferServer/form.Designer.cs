namespace fastCSharp.demo.fileTransferServer
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
            this.label1 = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.readPermissionCheckBox = new System.Windows.Forms.CheckBox();
            this.writePermissionCheckBox = new System.Windows.Forms.CheckBox();
            this.permissionButton = new System.Windows.Forms.Button();
            this.pathBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.pathButton = new System.Windows.Forms.Button();
            this.userComboBox = new System.Windows.Forms.ComboBox();
            this.deletePathButton = new System.Windows.Forms.Button();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.pathCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.allPathButton = new System.Windows.Forms.Button();
            this.changePathButton = new System.Windows.Forms.Button();
            this.clearPathButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.deleteUserButton = new System.Windows.Forms.Button();
            this.backupPathTextBox = new System.Windows.Forms.TextBox();
            this.backupPathButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "用户名";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(235, 0);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.Size = new System.Drawing.Size(144, 21);
            this.passwordTextBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(200, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "密码";
            // 
            // readPermissionCheckBox
            // 
            this.readPermissionCheckBox.AutoSize = true;
            this.readPermissionCheckBox.Checked = true;
            this.readPermissionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.readPermissionCheckBox.Location = new System.Drawing.Point(395, 2);
            this.readPermissionCheckBox.Name = "readPermissionCheckBox";
            this.readPermissionCheckBox.Size = new System.Drawing.Size(48, 16);
            this.readPermissionCheckBox.TabIndex = 5;
            this.readPermissionCheckBox.Text = "读取";
            this.readPermissionCheckBox.UseVisualStyleBackColor = true;
            // 
            // writePermissionCheckBox
            // 
            this.writePermissionCheckBox.AutoSize = true;
            this.writePermissionCheckBox.Checked = true;
            this.writePermissionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.writePermissionCheckBox.Location = new System.Drawing.Point(449, 2);
            this.writePermissionCheckBox.Name = "writePermissionCheckBox";
            this.writePermissionCheckBox.Size = new System.Drawing.Size(48, 16);
            this.writePermissionCheckBox.TabIndex = 6;
            this.writePermissionCheckBox.Text = "写入";
            this.writePermissionCheckBox.UseVisualStyleBackColor = true;
            // 
            // permissionButton
            // 
            this.permissionButton.Location = new System.Drawing.Point(555, 0);
            this.permissionButton.Name = "permissionButton";
            this.permissionButton.Size = new System.Drawing.Size(44, 23);
            this.permissionButton.TabIndex = 7;
            this.permissionButton.Text = "确定";
            this.permissionButton.UseVisualStyleBackColor = true;
            this.permissionButton.Click += new System.EventHandler(this.permissionButton_Click);
            // 
            // pathButton
            // 
            this.pathButton.Location = new System.Drawing.Point(555, 27);
            this.pathButton.Name = "pathButton";
            this.pathButton.Size = new System.Drawing.Size(44, 23);
            this.pathButton.TabIndex = 9;
            this.pathButton.Text = "路径";
            this.pathButton.UseVisualStyleBackColor = true;
            this.pathButton.Click += new System.EventHandler(this.pathButton_Click);
            // 
            // userComboBox
            // 
            this.userComboBox.FormattingEnabled = true;
            this.userComboBox.Location = new System.Drawing.Point(50, 1);
            this.userComboBox.Name = "userComboBox";
            this.userComboBox.Size = new System.Drawing.Size(144, 20);
            this.userComboBox.TabIndex = 10;
            this.userComboBox.SelectedIndexChanged += new System.EventHandler(this.selectUser);
            this.userComboBox.TextChanged += new System.EventHandler(this.changeUser);
            this.userComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.changeUser);
            // 
            // deletePathButton
            // 
            this.deletePathButton.Location = new System.Drawing.Point(175, 79);
            this.deletePathButton.Name = "deletePathButton";
            this.deletePathButton.Size = new System.Drawing.Size(44, 23);
            this.deletePathButton.TabIndex = 12;
            this.deletePathButton.Text = "删除";
            this.deletePathButton.UseVisualStyleBackColor = true;
            this.deletePathButton.Click += new System.EventHandler(this.deletePathButton_Click);
            // 
            // pathTextBox
            // 
            this.pathTextBox.Location = new System.Drawing.Point(5, 27);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(544, 21);
            this.pathTextBox.TabIndex = 13;
            // 
            // pathCheckedListBox
            // 
            this.pathCheckedListBox.CheckOnClick = true;
            this.pathCheckedListBox.FormattingEnabled = true;
            this.pathCheckedListBox.Location = new System.Drawing.Point(5, 103);
            this.pathCheckedListBox.Name = "pathCheckedListBox";
            this.pathCheckedListBox.Size = new System.Drawing.Size(594, 388);
            this.pathCheckedListBox.TabIndex = 14;
            this.pathCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkPath);
            // 
            // allPathButton
            // 
            this.allPathButton.Location = new System.Drawing.Point(5, 79);
            this.allPathButton.Name = "allPathButton";
            this.allPathButton.Size = new System.Drawing.Size(44, 23);
            this.allPathButton.TabIndex = 15;
            this.allPathButton.Text = "全选";
            this.allPathButton.UseVisualStyleBackColor = true;
            this.allPathButton.Click += new System.EventHandler(this.allPathButton_Click);
            // 
            // changePathButton
            // 
            this.changePathButton.Location = new System.Drawing.Point(55, 79);
            this.changePathButton.Name = "changePathButton";
            this.changePathButton.Size = new System.Drawing.Size(44, 23);
            this.changePathButton.TabIndex = 16;
            this.changePathButton.Text = "反选";
            this.changePathButton.UseVisualStyleBackColor = true;
            this.changePathButton.Click += new System.EventHandler(this.changePathButton_Click);
            // 
            // clearPathButton
            // 
            this.clearPathButton.Location = new System.Drawing.Point(105, 79);
            this.clearPathButton.Name = "clearPathButton";
            this.clearPathButton.Size = new System.Drawing.Size(44, 23);
            this.clearPathButton.TabIndex = 17;
            this.clearPathButton.Text = "清空";
            this.clearPathButton.UseVisualStyleBackColor = true;
            this.clearPathButton.Click += new System.EventHandler(this.clearPathButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Location = new System.Drawing.Point(555, 81);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(44, 23);
            this.stopButton.TabIndex = 18;
            this.stopButton.Text = "停止";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.Enabled = false;
            this.startButton.Location = new System.Drawing.Point(505, 81);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(44, 23);
            this.startButton.TabIndex = 19;
            this.startButton.Text = "启动";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // deleteUserButton
            // 
            this.deleteUserButton.Location = new System.Drawing.Point(505, 0);
            this.deleteUserButton.Name = "deleteUserButton";
            this.deleteUserButton.Size = new System.Drawing.Size(44, 23);
            this.deleteUserButton.TabIndex = 20;
            this.deleteUserButton.Text = "删除";
            this.deleteUserButton.UseVisualStyleBackColor = true;
            this.deleteUserButton.Click += new System.EventHandler(this.deleteUserButton_Click);
            // 
            // backupPathTextBox
            // 
            this.backupPathTextBox.Location = new System.Drawing.Point(5, 54);
            this.backupPathTextBox.Name = "backupPathTextBox";
            this.backupPathTextBox.Size = new System.Drawing.Size(544, 21);
            this.backupPathTextBox.TabIndex = 22;
            // 
            // backupPathButton
            // 
            this.backupPathButton.Location = new System.Drawing.Point(555, 54);
            this.backupPathButton.Name = "backupPathButton";
            this.backupPathButton.Size = new System.Drawing.Size(44, 23);
            this.backupPathButton.TabIndex = 21;
            this.backupPathButton.Text = "备份";
            this.backupPathButton.UseVisualStyleBackColor = true;
            this.backupPathButton.Click += new System.EventHandler(this.backupPathButton_Click);
            // 
            // form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 499);
            this.Controls.Add(this.backupPathTextBox);
            this.Controls.Add(this.backupPathButton);
            this.Controls.Add(this.deleteUserButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.clearPathButton);
            this.Controls.Add(this.changePathButton);
            this.Controls.Add(this.allPathButton);
            this.Controls.Add(this.pathCheckedListBox);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.deletePathButton);
            this.Controls.Add(this.userComboBox);
            this.Controls.Add(this.pathButton);
            this.Controls.Add(this.permissionButton);
            this.Controls.Add(this.writePermissionCheckBox);
            this.Controls.Add(this.readPermissionCheckBox);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "form";
            this.Text = "fileTransferServer - fastCSharp";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.close);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox readPermissionCheckBox;
        private System.Windows.Forms.CheckBox writePermissionCheckBox;
        private System.Windows.Forms.Button permissionButton;
        private System.Windows.Forms.FolderBrowserDialog pathBrowserDialog;
        private System.Windows.Forms.Button pathButton;
        private System.Windows.Forms.ComboBox userComboBox;
        private System.Windows.Forms.Button deletePathButton;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.CheckedListBox pathCheckedListBox;
        private System.Windows.Forms.Button allPathButton;
        private System.Windows.Forms.Button changePathButton;
        private System.Windows.Forms.Button clearPathButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button deleteUserButton;
        private System.Windows.Forms.TextBox backupPathTextBox;
        private System.Windows.Forms.Button backupPathButton;
    }
}


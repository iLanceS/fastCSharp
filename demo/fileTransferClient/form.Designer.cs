namespace fastCSharp.demo.fileTransferClient
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
            this.label3 = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.clientButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.removeUserButton = new System.Windows.Forms.Button();
            this.portTextBox = new System.Windows.Forms.TextBox();
            this.hostTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nextServerPathButton = new System.Windows.Forms.Button();
            this.previousServerPathButton = new System.Windows.Forms.Button();
            this.parentServerPathButton = new System.Windows.Forms.Button();
            this.parentPathButton = new System.Windows.Forms.Button();
            this.previousPathButton = new System.Windows.Forms.Button();
            this.nextPathButton = new System.Windows.Forms.Button();
            this.pathComboBox = new System.Windows.Forms.ComboBox();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.pathButton = new System.Windows.Forms.Button();
            this.uploadButton = new System.Windows.Forms.Button();
            this.downloadButton = new System.Windows.Forms.Button();
            this.clientCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.serverCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.allServerListNameButton = new System.Windows.Forms.Button();
            this.changeServerListNameButton = new System.Windows.Forms.Button();
            this.clearServerListNameButton = new System.Windows.Forms.Button();
            this.deleteServerListNameButton = new System.Windows.Forms.Button();
            this.deleteListNameButton = new System.Windows.Forms.Button();
            this.clearListNameButton = new System.Windows.Forms.Button();
            this.changeListNameButton = new System.Windows.Forms.Button();
            this.allListNameButton = new System.Windows.Forms.Button();
            this.refreshServerListNameButton = new System.Windows.Forms.Button();
            this.refreshListNameButton = new System.Windows.Forms.Button();
            this.uploadThreadTextBox = new System.Windows.Forms.TextBox();
            this.downloadThreadTextBox = new System.Windows.Forms.TextBox();
            this.errorTextBox = new System.Windows.Forms.TextBox();
            this.pathCheckBox = new System.Windows.Forms.CheckBox();
            this.userComboBox = new System.Windows.Forms.ComboBox();
            this.serverPermissionComboBox = new System.Windows.Forms.ComboBox();
            this.clearMessageButton = new System.Windows.Forms.Button();
            this.clearErrorButton = new System.Windows.Forms.Button();
            this.serverPathTextBox = new System.Windows.Forms.TextBox();
            this.extensionFilterTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.clientDirectoryCheckBox = new System.Windows.Forms.CheckBox();
            this.serverDirectoryCheckBox = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.autoCheckBox = new System.Windows.Forms.CheckBox();
            this.timeVersionCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // userNameTextBox
            // 
            this.userNameTextBox.Location = new System.Drawing.Point(515, 2);
            this.userNameTextBox.Name = "userNameTextBox";
            this.userNameTextBox.Size = new System.Drawing.Size(114, 21);
            this.userNameTextBox.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(478, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "用户名";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(660, 2);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '*';
            this.passwordTextBox.Size = new System.Drawing.Size(114, 21);
            this.passwordTextBox.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(635, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "密码";
            // 
            // clientButton
            // 
            this.clientButton.Location = new System.Drawing.Point(870, 0);
            this.clientButton.Name = "clientButton";
            this.clientButton.Size = new System.Drawing.Size(41, 23);
            this.clientButton.TabIndex = 9;
            this.clientButton.Text = "连接";
            this.clientButton.UseVisualStyleBackColor = true;
            this.clientButton.Click += new System.EventHandler(this.clientButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Enabled = false;
            this.closeButton.Location = new System.Drawing.Point(823, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(41, 23);
            this.closeButton.TabIndex = 10;
            this.closeButton.Text = "断开";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // removeUserButton
            // 
            this.removeUserButton.Location = new System.Drawing.Point(776, 0);
            this.removeUserButton.Name = "removeUserButton";
            this.removeUserButton.Size = new System.Drawing.Size(41, 23);
            this.removeUserButton.TabIndex = 11;
            this.removeUserButton.Text = "删除";
            this.removeUserButton.UseVisualStyleBackColor = true;
            this.removeUserButton.Click += new System.EventHandler(this.removeUserButton_Click);
            // 
            // portTextBox
            // 
            this.portTextBox.Location = new System.Drawing.Point(421, 2);
            this.portTextBox.Name = "portTextBox";
            this.portTextBox.Size = new System.Drawing.Size(46, 21);
            this.portTextBox.TabIndex = 4;
            // 
            // hostTextBox
            // 
            this.hostTextBox.Location = new System.Drawing.Point(284, 2);
            this.hostTextBox.Name = "hostTextBox";
            this.hostTextBox.Size = new System.Drawing.Size(114, 21);
            this.hostTextBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(404, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(11, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = ":";
            // 
            // nextServerPathButton
            // 
            this.nextServerPathButton.Enabled = false;
            this.nextServerPathButton.Location = new System.Drawing.Point(891, 75);
            this.nextServerPathButton.Name = "nextServerPathButton";
            this.nextServerPathButton.Size = new System.Drawing.Size(20, 23);
            this.nextServerPathButton.TabIndex = 15;
            this.nextServerPathButton.Text = "→";
            this.nextServerPathButton.UseVisualStyleBackColor = true;
            this.nextServerPathButton.Click += new System.EventHandler(this.nextServerPathButton_Click);
            // 
            // previousServerPathButton
            // 
            this.previousServerPathButton.Enabled = false;
            this.previousServerPathButton.Location = new System.Drawing.Point(870, 75);
            this.previousServerPathButton.Name = "previousServerPathButton";
            this.previousServerPathButton.Size = new System.Drawing.Size(20, 23);
            this.previousServerPathButton.TabIndex = 16;
            this.previousServerPathButton.Text = "←";
            this.previousServerPathButton.UseVisualStyleBackColor = true;
            this.previousServerPathButton.Click += new System.EventHandler(this.previousServerPathButton_Click);
            // 
            // parentServerPathButton
            // 
            this.parentServerPathButton.Enabled = false;
            this.parentServerPathButton.Location = new System.Drawing.Point(844, 75);
            this.parentServerPathButton.Name = "parentServerPathButton";
            this.parentServerPathButton.Size = new System.Drawing.Size(20, 23);
            this.parentServerPathButton.TabIndex = 17;
            this.parentServerPathButton.Text = "↑";
            this.parentServerPathButton.UseVisualStyleBackColor = true;
            this.parentServerPathButton.Click += new System.EventHandler(this.parentServerPathButton_Click);
            // 
            // parentPathButton
            // 
            this.parentPathButton.Enabled = false;
            this.parentPathButton.Location = new System.Drawing.Point(378, 75);
            this.parentPathButton.Name = "parentPathButton";
            this.parentPathButton.Size = new System.Drawing.Size(20, 23);
            this.parentPathButton.TabIndex = 20;
            this.parentPathButton.Text = "↑";
            this.parentPathButton.UseVisualStyleBackColor = true;
            this.parentPathButton.Click += new System.EventHandler(this.parentPathButton_Click);
            // 
            // previousPathButton
            // 
            this.previousPathButton.Enabled = false;
            this.previousPathButton.Location = new System.Drawing.Point(404, 75);
            this.previousPathButton.Name = "previousPathButton";
            this.previousPathButton.Size = new System.Drawing.Size(20, 23);
            this.previousPathButton.TabIndex = 19;
            this.previousPathButton.Text = "←";
            this.previousPathButton.UseVisualStyleBackColor = true;
            this.previousPathButton.Click += new System.EventHandler(this.previousPathButton_Click);
            // 
            // nextPathButton
            // 
            this.nextPathButton.Enabled = false;
            this.nextPathButton.Location = new System.Drawing.Point(424, 75);
            this.nextPathButton.Name = "nextPathButton";
            this.nextPathButton.Size = new System.Drawing.Size(20, 23);
            this.nextPathButton.TabIndex = 18;
            this.nextPathButton.Text = "→";
            this.nextPathButton.UseVisualStyleBackColor = true;
            this.nextPathButton.Click += new System.EventHandler(this.nextPathButton_Click);
            // 
            // pathComboBox
            // 
            this.pathComboBox.FormattingEnabled = true;
            this.pathComboBox.Location = new System.Drawing.Point(2, 29);
            this.pathComboBox.Name = "pathComboBox";
            this.pathComboBox.Size = new System.Drawing.Size(438, 20);
            this.pathComboBox.TabIndex = 21;
            this.pathComboBox.SelectedIndexChanged += new System.EventHandler(this.selectPath);
            this.pathComboBox.TextChanged += new System.EventHandler(this.changePath);
            this.pathComboBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.changePath);
            // 
            // pathButton
            // 
            this.pathButton.Location = new System.Drawing.Point(399, 48);
            this.pathButton.Name = "pathButton";
            this.pathButton.Size = new System.Drawing.Size(41, 23);
            this.pathButton.TabIndex = 22;
            this.pathButton.Text = "路径";
            this.pathButton.UseVisualStyleBackColor = true;
            this.pathButton.Click += new System.EventHandler(this.pathButton_Click);
            // 
            // uploadButton
            // 
            this.uploadButton.Enabled = false;
            this.uploadButton.Location = new System.Drawing.Point(447, 157);
            this.uploadButton.Name = "uploadButton";
            this.uploadButton.Size = new System.Drawing.Size(20, 23);
            this.uploadButton.TabIndex = 25;
            this.uploadButton.Text = "→";
            this.uploadButton.UseVisualStyleBackColor = true;
            this.uploadButton.Click += new System.EventHandler(this.uploadButton_Click);
            // 
            // downloadButton
            // 
            this.downloadButton.Enabled = false;
            this.downloadButton.Location = new System.Drawing.Point(447, 336);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(20, 23);
            this.downloadButton.TabIndex = 26;
            this.downloadButton.Text = "←";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // clientCheckedListBox
            // 
            this.clientCheckedListBox.CheckOnClick = true;
            this.clientCheckedListBox.FormattingEnabled = true;
            this.clientCheckedListBox.Location = new System.Drawing.Point(2, 104);
            this.clientCheckedListBox.Name = "clientCheckedListBox";
            this.clientCheckedListBox.Size = new System.Drawing.Size(438, 340);
            this.clientCheckedListBox.TabIndex = 27;
            this.clientCheckedListBox.DoubleClick += new System.EventHandler(this.clientEnter);
            // 
            // serverCheckedListBox
            // 
            this.serverCheckedListBox.CheckOnClick = true;
            this.serverCheckedListBox.FormattingEnabled = true;
            this.serverCheckedListBox.Location = new System.Drawing.Point(473, 104);
            this.serverCheckedListBox.Name = "serverCheckedListBox";
            this.serverCheckedListBox.Size = new System.Drawing.Size(438, 340);
            this.serverCheckedListBox.TabIndex = 28;
            this.serverCheckedListBox.DoubleClick += new System.EventHandler(this.serverEnter);
            // 
            // messageTextBox
            // 
            this.messageTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.messageTextBox.Location = new System.Drawing.Point(2, 451);
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.ReadOnly = true;
            this.messageTextBox.Size = new System.Drawing.Size(453, 78);
            this.messageTextBox.TabIndex = 30;
            // 
            // allServerListNameButton
            // 
            this.allServerListNameButton.Enabled = false;
            this.allServerListNameButton.Location = new System.Drawing.Point(473, 75);
            this.allServerListNameButton.Name = "allServerListNameButton";
            this.allServerListNameButton.Size = new System.Drawing.Size(41, 23);
            this.allServerListNameButton.TabIndex = 31;
            this.allServerListNameButton.Text = "全选";
            this.allServerListNameButton.UseVisualStyleBackColor = true;
            this.allServerListNameButton.Click += new System.EventHandler(this.allServerListNameButton_Click);
            // 
            // changeServerListNameButton
            // 
            this.changeServerListNameButton.Enabled = false;
            this.changeServerListNameButton.Location = new System.Drawing.Point(515, 75);
            this.changeServerListNameButton.Name = "changeServerListNameButton";
            this.changeServerListNameButton.Size = new System.Drawing.Size(41, 23);
            this.changeServerListNameButton.TabIndex = 32;
            this.changeServerListNameButton.Text = "反选";
            this.changeServerListNameButton.UseVisualStyleBackColor = true;
            this.changeServerListNameButton.Click += new System.EventHandler(this.changeServerListNameButton_Click);
            // 
            // clearServerListNameButton
            // 
            this.clearServerListNameButton.Enabled = false;
            this.clearServerListNameButton.Location = new System.Drawing.Point(562, 75);
            this.clearServerListNameButton.Name = "clearServerListNameButton";
            this.clearServerListNameButton.Size = new System.Drawing.Size(41, 23);
            this.clearServerListNameButton.TabIndex = 33;
            this.clearServerListNameButton.Text = "清空";
            this.clearServerListNameButton.UseVisualStyleBackColor = true;
            this.clearServerListNameButton.Click += new System.EventHandler(this.clearServerListNameButton_Click);
            // 
            // deleteServerListNameButton
            // 
            this.deleteServerListNameButton.Enabled = false;
            this.deleteServerListNameButton.Location = new System.Drawing.Point(609, 75);
            this.deleteServerListNameButton.Name = "deleteServerListNameButton";
            this.deleteServerListNameButton.Size = new System.Drawing.Size(41, 23);
            this.deleteServerListNameButton.TabIndex = 34;
            this.deleteServerListNameButton.Text = "删除";
            this.deleteServerListNameButton.UseVisualStyleBackColor = true;
            this.deleteServerListNameButton.Click += new System.EventHandler(this.deleteServerListNameButton_Click);
            // 
            // deleteListNameButton
            // 
            this.deleteListNameButton.Enabled = false;
            this.deleteListNameButton.Location = new System.Drawing.Point(138, 75);
            this.deleteListNameButton.Name = "deleteListNameButton";
            this.deleteListNameButton.Size = new System.Drawing.Size(41, 23);
            this.deleteListNameButton.TabIndex = 38;
            this.deleteListNameButton.Text = "删除";
            this.deleteListNameButton.UseVisualStyleBackColor = true;
            this.deleteListNameButton.Click += new System.EventHandler(this.deleteListNameButton_Click);
            // 
            // clearListNameButton
            // 
            this.clearListNameButton.Enabled = false;
            this.clearListNameButton.Location = new System.Drawing.Point(91, 75);
            this.clearListNameButton.Name = "clearListNameButton";
            this.clearListNameButton.Size = new System.Drawing.Size(41, 23);
            this.clearListNameButton.TabIndex = 37;
            this.clearListNameButton.Text = "清空";
            this.clearListNameButton.UseVisualStyleBackColor = true;
            this.clearListNameButton.Click += new System.EventHandler(this.clearListNameButton_Click);
            // 
            // changeListNameButton
            // 
            this.changeListNameButton.Enabled = false;
            this.changeListNameButton.Location = new System.Drawing.Point(44, 75);
            this.changeListNameButton.Name = "changeListNameButton";
            this.changeListNameButton.Size = new System.Drawing.Size(41, 23);
            this.changeListNameButton.TabIndex = 36;
            this.changeListNameButton.Text = "反选";
            this.changeListNameButton.UseVisualStyleBackColor = true;
            this.changeListNameButton.Click += new System.EventHandler(this.changeListNameButton_Click);
            // 
            // allListNameButton
            // 
            this.allListNameButton.Enabled = false;
            this.allListNameButton.Location = new System.Drawing.Point(2, 75);
            this.allListNameButton.Name = "allListNameButton";
            this.allListNameButton.Size = new System.Drawing.Size(41, 23);
            this.allListNameButton.TabIndex = 35;
            this.allListNameButton.Text = "全选";
            this.allListNameButton.UseVisualStyleBackColor = true;
            this.allListNameButton.Click += new System.EventHandler(this.allListNameButton_Click);
            // 
            // refreshServerListNameButton
            // 
            this.refreshServerListNameButton.Enabled = false;
            this.refreshServerListNameButton.Location = new System.Drawing.Point(797, 75);
            this.refreshServerListNameButton.Name = "refreshServerListNameButton";
            this.refreshServerListNameButton.Size = new System.Drawing.Size(41, 23);
            this.refreshServerListNameButton.TabIndex = 39;
            this.refreshServerListNameButton.Text = "刷新";
            this.refreshServerListNameButton.UseVisualStyleBackColor = true;
            this.refreshServerListNameButton.Click += new System.EventHandler(this.refreshServerListNameButton_Click);
            // 
            // refreshListNameButton
            // 
            this.refreshListNameButton.Enabled = false;
            this.refreshListNameButton.Location = new System.Drawing.Point(331, 75);
            this.refreshListNameButton.Name = "refreshListNameButton";
            this.refreshListNameButton.Size = new System.Drawing.Size(41, 23);
            this.refreshListNameButton.TabIndex = 40;
            this.refreshListNameButton.Text = "刷新";
            this.refreshListNameButton.UseVisualStyleBackColor = true;
            this.refreshListNameButton.Click += new System.EventHandler(this.refreshListNameButton_Click);
            // 
            // uploadThreadTextBox
            // 
            this.uploadThreadTextBox.Location = new System.Drawing.Point(446, 130);
            this.uploadThreadTextBox.Name = "uploadThreadTextBox";
            this.uploadThreadTextBox.Size = new System.Drawing.Size(21, 21);
            this.uploadThreadTextBox.TabIndex = 41;
            this.uploadThreadTextBox.Text = "99";
            this.uploadThreadTextBox.Leave += new System.EventHandler(this.setMaxUploadThreadCount);
            // 
            // downloadThreadTextBox
            // 
            this.downloadThreadTextBox.Location = new System.Drawing.Point(446, 365);
            this.downloadThreadTextBox.Name = "downloadThreadTextBox";
            this.downloadThreadTextBox.Size = new System.Drawing.Size(21, 21);
            this.downloadThreadTextBox.TabIndex = 42;
            this.downloadThreadTextBox.Text = "99";
            this.downloadThreadTextBox.Leave += new System.EventHandler(this.setMaxDownloadThreadCount);
            // 
            // errorTextBox
            // 
            this.errorTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.errorTextBox.Location = new System.Drawing.Point(461, 451);
            this.errorTextBox.Multiline = true;
            this.errorTextBox.Name = "errorTextBox";
            this.errorTextBox.ReadOnly = true;
            this.errorTextBox.Size = new System.Drawing.Size(450, 78);
            this.errorTextBox.TabIndex = 43;
            // 
            // pathCheckBox
            // 
            this.pathCheckBox.AutoSize = true;
            this.pathCheckBox.CheckAlign = System.Drawing.ContentAlignment.TopCenter;
            this.pathCheckBox.Checked = true;
            this.pathCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pathCheckBox.Location = new System.Drawing.Point(440, 245);
            this.pathCheckBox.Name = "pathCheckBox";
            this.pathCheckBox.Size = new System.Drawing.Size(33, 30);
            this.pathCheckBox.TabIndex = 44;
            this.pathCheckBox.Text = "联动";
            this.pathCheckBox.UseVisualStyleBackColor = true;
            // 
            // userComboBox
            // 
            this.userComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.userComboBox.FormattingEnabled = true;
            this.userComboBox.Location = new System.Drawing.Point(2, 2);
            this.userComboBox.Name = "userComboBox";
            this.userComboBox.Size = new System.Drawing.Size(276, 20);
            this.userComboBox.TabIndex = 45;
            this.userComboBox.SelectedIndexChanged += new System.EventHandler(this.selectUser);
            // 
            // serverPermissionComboBox
            // 
            this.serverPermissionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serverPermissionComboBox.FormattingEnabled = true;
            this.serverPermissionComboBox.Location = new System.Drawing.Point(473, 28);
            this.serverPermissionComboBox.Name = "serverPermissionComboBox";
            this.serverPermissionComboBox.Size = new System.Drawing.Size(438, 20);
            this.serverPermissionComboBox.TabIndex = 46;
            this.serverPermissionComboBox.SelectedIndexChanged += new System.EventHandler(this.selectServerPermission);
            // 
            // clearMessageButton
            // 
            this.clearMessageButton.Enabled = false;
            this.clearMessageButton.Location = new System.Drawing.Point(414, 481);
            this.clearMessageButton.Name = "clearMessageButton";
            this.clearMessageButton.Size = new System.Drawing.Size(41, 23);
            this.clearMessageButton.TabIndex = 47;
            this.clearMessageButton.Text = "清除";
            this.clearMessageButton.UseVisualStyleBackColor = true;
            this.clearMessageButton.Click += new System.EventHandler(this.clearMessageButton_Click);
            // 
            // clearErrorButton
            // 
            this.clearErrorButton.Enabled = false;
            this.clearErrorButton.Location = new System.Drawing.Point(870, 481);
            this.clearErrorButton.Name = "clearErrorButton";
            this.clearErrorButton.Size = new System.Drawing.Size(41, 23);
            this.clearErrorButton.TabIndex = 48;
            this.clearErrorButton.Text = "清除";
            this.clearErrorButton.UseVisualStyleBackColor = true;
            this.clearErrorButton.Click += new System.EventHandler(this.clearErrorButton_Click);
            // 
            // serverPathTextBox
            // 
            this.serverPathTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.serverPathTextBox.Location = new System.Drawing.Point(473, 48);
            this.serverPathTextBox.Name = "serverPathTextBox";
            this.serverPathTextBox.ReadOnly = true;
            this.serverPathTextBox.Size = new System.Drawing.Size(438, 21);
            this.serverPathTextBox.TabIndex = 49;
            // 
            // extensionFilterTextBox
            // 
            this.extensionFilterTextBox.Location = new System.Drawing.Point(44, 50);
            this.extensionFilterTextBox.Name = "extensionFilterTextBox";
            this.extensionFilterTextBox.Size = new System.Drawing.Size(135, 21);
            this.extensionFilterTextBox.TabIndex = 50;
            this.extensionFilterTextBox.Leave += new System.EventHandler(this.checkExtensionFilter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(2, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 51;
            this.label1.Text = "扩展名";
            // 
            // clientDirectoryCheckBox
            // 
            this.clientDirectoryCheckBox.AutoSize = true;
            this.clientDirectoryCheckBox.Checked = true;
            this.clientDirectoryCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.clientDirectoryCheckBox.Location = new System.Drawing.Point(185, 79);
            this.clientDirectoryCheckBox.Name = "clientDirectoryCheckBox";
            this.clientDirectoryCheckBox.Size = new System.Drawing.Size(48, 16);
            this.clientDirectoryCheckBox.TabIndex = 52;
            this.clientDirectoryCheckBox.Text = "目录";
            this.clientDirectoryCheckBox.UseVisualStyleBackColor = true;
            this.clientDirectoryCheckBox.CheckedChanged += new System.EventHandler(this.refreshListNameButton_Click);
            // 
            // serverDirectoryCheckBox
            // 
            this.serverDirectoryCheckBox.AutoSize = true;
            this.serverDirectoryCheckBox.Checked = true;
            this.serverDirectoryCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.serverDirectoryCheckBox.Location = new System.Drawing.Point(656, 79);
            this.serverDirectoryCheckBox.Name = "serverDirectoryCheckBox";
            this.serverDirectoryCheckBox.Size = new System.Drawing.Size(48, 16);
            this.serverDirectoryCheckBox.TabIndex = 53;
            this.serverDirectoryCheckBox.Text = "目录";
            this.serverDirectoryCheckBox.UseVisualStyleBackColor = true;
            this.serverDirectoryCheckBox.CheckedChanged += new System.EventHandler(this.refreshServerListNameButton_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(185, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 55;
            this.label5.Text = "例 js/html";
            // 
            // autoCheckBox
            // 
            this.autoCheckBox.AutoSize = true;
            this.autoCheckBox.Checked = true;
            this.autoCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoCheckBox.Location = new System.Drawing.Point(256, 55);
            this.autoCheckBox.Name = "autoCheckBox";
            this.autoCheckBox.Size = new System.Drawing.Size(72, 16);
            this.autoCheckBox.TabIndex = 56;
            this.autoCheckBox.Text = "自动绑定";
            this.autoCheckBox.UseVisualStyleBackColor = true;
            // 
            // timeVersionCheckBox
            // 
            this.timeVersionCheckBox.AutoSize = true;
            this.timeVersionCheckBox.Checked = true;
            this.timeVersionCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.timeVersionCheckBox.Location = new System.Drawing.Point(326, 55);
            this.timeVersionCheckBox.Name = "timeVersionCheckBox";
            this.timeVersionCheckBox.Size = new System.Drawing.Size(72, 16);
            this.timeVersionCheckBox.TabIndex = 57;
            this.timeVersionCheckBox.Text = "时间版本";
            this.timeVersionCheckBox.UseVisualStyleBackColor = true;
            // 
            // form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 531);
            this.Controls.Add(this.timeVersionCheckBox);
            this.Controls.Add(this.autoCheckBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.serverDirectoryCheckBox);
            this.Controls.Add(this.clientDirectoryCheckBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.extensionFilterTextBox);
            this.Controls.Add(this.serverPathTextBox);
            this.Controls.Add(this.clearErrorButton);
            this.Controls.Add(this.clearMessageButton);
            this.Controls.Add(this.serverPermissionComboBox);
            this.Controls.Add(this.userComboBox);
            this.Controls.Add(this.pathCheckBox);
            this.Controls.Add(this.errorTextBox);
            this.Controls.Add(this.downloadThreadTextBox);
            this.Controls.Add(this.uploadThreadTextBox);
            this.Controls.Add(this.refreshListNameButton);
            this.Controls.Add(this.refreshServerListNameButton);
            this.Controls.Add(this.deleteListNameButton);
            this.Controls.Add(this.clearListNameButton);
            this.Controls.Add(this.changeListNameButton);
            this.Controls.Add(this.allListNameButton);
            this.Controls.Add(this.deleteServerListNameButton);
            this.Controls.Add(this.clearServerListNameButton);
            this.Controls.Add(this.changeServerListNameButton);
            this.Controls.Add(this.allServerListNameButton);
            this.Controls.Add(this.messageTextBox);
            this.Controls.Add(this.serverCheckedListBox);
            this.Controls.Add(this.clientCheckedListBox);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.uploadButton);
            this.Controls.Add(this.pathButton);
            this.Controls.Add(this.pathComboBox);
            this.Controls.Add(this.parentPathButton);
            this.Controls.Add(this.previousPathButton);
            this.Controls.Add(this.nextPathButton);
            this.Controls.Add(this.parentServerPathButton);
            this.Controls.Add(this.previousServerPathButton);
            this.Controls.Add(this.nextServerPathButton);
            this.Controls.Add(this.removeUserButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.clientButton);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.userNameTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.portTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.hostTextBox);
            this.Name = "form";
            this.Text = "fileTransferClient - fastCSharp";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.close);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox userNameTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button clientButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button removeUserButton;
        private System.Windows.Forms.TextBox portTextBox;
        private System.Windows.Forms.TextBox hostTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button nextServerPathButton;
        private System.Windows.Forms.Button previousServerPathButton;
        private System.Windows.Forms.Button parentServerPathButton;
        private System.Windows.Forms.Button parentPathButton;
        private System.Windows.Forms.Button previousPathButton;
        private System.Windows.Forms.Button nextPathButton;
        private System.Windows.Forms.ComboBox pathComboBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button pathButton;
        private System.Windows.Forms.Button uploadButton;
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.CheckedListBox clientCheckedListBox;
        private System.Windows.Forms.CheckedListBox serverCheckedListBox;
        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.Button allServerListNameButton;
        private System.Windows.Forms.Button changeServerListNameButton;
        private System.Windows.Forms.Button clearServerListNameButton;
        private System.Windows.Forms.Button deleteServerListNameButton;
        private System.Windows.Forms.Button deleteListNameButton;
        private System.Windows.Forms.Button clearListNameButton;
        private System.Windows.Forms.Button changeListNameButton;
        private System.Windows.Forms.Button allListNameButton;
        private System.Windows.Forms.Button refreshServerListNameButton;
        private System.Windows.Forms.Button refreshListNameButton;
        private System.Windows.Forms.TextBox uploadThreadTextBox;
        private System.Windows.Forms.TextBox downloadThreadTextBox;
        private System.Windows.Forms.TextBox errorTextBox;
        private System.Windows.Forms.CheckBox pathCheckBox;
        private System.Windows.Forms.ComboBox userComboBox;
        private System.Windows.Forms.ComboBox serverPermissionComboBox;
        private System.Windows.Forms.Button clearMessageButton;
        private System.Windows.Forms.Button clearErrorButton;
        private System.Windows.Forms.TextBox serverPathTextBox;
        private System.Windows.Forms.TextBox extensionFilterTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox clientDirectoryCheckBox;
        private System.Windows.Forms.CheckBox serverDirectoryCheckBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox autoCheckBox;
        private System.Windows.Forms.CheckBox timeVersionCheckBox;
    }
}


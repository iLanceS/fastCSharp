namespace fastCSharp.tcpRegister
{
    partial class serviceInstaller
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

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.installer = new System.ServiceProcess.ServiceInstaller();
            installer.Description = "fastCSharp TCP调用动态端口注册";
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller.Password = fastCSharp.config.tcpRegister.Default.Password;
            this.serviceProcessInstaller.Username = fastCSharp.config.tcpRegister.Default.Username;
            // 
            // serviceInstaller1
            // 
            this.installer.ServiceName = fastCSharp.config.tcpRegister.Default.ServiceName;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.installer});
        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller installer;
    }
}
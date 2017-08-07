using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;

namespace fastCSharp.tcpRegister
{
    public partial class service : ServiceBase
    {
        /// <summary>
        /// TCP注册服务
        /// </summary>
        private fastCSharp.tcpRegister.console server;
        public service()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            OnStop();
            (server = new fastCSharp.tcpRegister.console()).Start();
        }
        protected override void OnStop()
        {
            pub.Dispose(ref server);
        }
    }
}

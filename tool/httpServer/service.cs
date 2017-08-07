using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;

namespace fastCSharp.httpServer
{
    public partial class service : ServiceBase
    {
        /// <summary>
        /// HTTP服务
        /// </summary>
        private console server;
        public service()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            OnStop();
            (server = new console()).Start();
        }
        protected override void OnStop()
        {
            pub.Dispose(ref server);
        }
    }
}

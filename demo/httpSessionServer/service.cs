using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

namespace fastCSharp.demo.httpSessionServer
{
    public partial class service : ServiceBase
    {
        /// <summary>
        /// Session服务
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

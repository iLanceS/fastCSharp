using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;


namespace fastCSharp.demo.httpSessionServer
{
    [RunInstaller(true)]
    public partial class serviceInstaller : System.Configuration.Install.Installer
    {
        public serviceInstaller()
        {
            InitializeComponent();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;


namespace fastCSharp.tcpRegister
{
    [RunInstaller(true)]
    public partial class serviceInstaller : System.Configuration.Install.Installer
    {
        public serviceInstaller()
        {
            if (fastCSharp.config.tcpRegister.Default.DependedOn.length() != 0)
            {
                installer.ServicesDependedOn = fastCSharp.config.tcpRegister.Default.DependedOn;
            }
            InitializeComponent();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace fastCSharp.demo.chatClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new form(args.length() == 0 ? null : args[0]));
        }
    }
}

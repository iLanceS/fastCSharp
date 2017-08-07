using System;
using System.Diagnostics;
using Microsoft.Samples.Debugging.MdbgEngine;

namespace fastCSharp.test.mDbgEngine
{
    class Program
    {
        /// <summary>
        /// 利用VS调试工具输出线程堆栈测试 http://blogs.msdn.com/b/jmstall/archive/2005/11/28/snapshot.aspx
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("pid?");
                int pid;
                if (!int.TryParse(Console.ReadLine(), out pid)) return;

                MDbgEngine debugger = new MDbgEngine();

                MDbgProcess proc = null;
                try
                {
                    proc = debugger.Attach(pid);
                    DrainAttach(debugger, proc);

                    MDbgThreadCollection tc = proc.Threads;
                    Console.WriteLine("Attached to pid:{0}", pid);
                    foreach (MDbgThread t in tc)
                    {
                        Console.WriteLine("Callstack for Thread {0}", t.Id.ToString());

                        foreach (MDbgFrame f in t.Frames)
                        {
                            Console.WriteLine("  " + f);
                        }
                    }
                }
                finally
                {
                    if (proc != null) { proc.Detach().WaitOne(); }
                }
            }
            while (true);
        }

        /// <summary>
        /// 附加到进程
        /// </summary>
        /// <param name="debugger"></param>
        /// <param name="proc"></param>
        static void DrainAttach(MDbgEngine debugger, MDbgProcess proc)
        {
            bool fOldStatus = debugger.Options.StopOnNewThread;
            debugger.Options.StopOnNewThread = false; // skip while waiting for AttachComplete

            proc.Go().WaitOne();
            Debug.Assert(proc.StopReason is AttachCompleteStopReason);

            debugger.Options.StopOnNewThread = true; // needed for attach= true; // needed for attach

            // Drain the rest of the thread create events.
            while (proc.CorProcess.HasQueuedCallbacks(null))
            {
                proc.Go().WaitOne();
                Debug.Assert(proc.StopReason is ThreadCreatedStopReason);
            }

            debugger.Options.StopOnNewThread = fOldStatus;
        }
    }
}

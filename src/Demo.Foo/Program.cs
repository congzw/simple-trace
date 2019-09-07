using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Foo
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(() =>
            {
                for (int i = 0; i < 100; i++)
                {
                    LogInfo("Processing " + i);
                    Thread.Sleep(1000);
                }
            });

            LogInfo("App Exit ...");
        }
        
        static void LogInfo(string info)
        {
            var msg = string.Format("[{0}][{1}][{2}] {3} => {4:s}", "SimpleLog", "Info", "Demo.Foo", info, DateTime.Now);
            Console.WriteLine(msg);
            Trace.WriteLine(msg);
        }
    }
}

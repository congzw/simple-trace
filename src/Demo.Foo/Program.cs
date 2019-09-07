using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Demo.Foo
{
    class Program
    {
        private static bool _looping = true;

        static void Main(string[] args)
        {
            LogInfo("Use 'ESC' to Exit App");
            Task.Run(() =>
            {
                while (_looping)
                {
                    LogInfo("...");
                    Thread.Sleep(3000);
                }
            });

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                LogInfo("Use 'ESC' KEY to Exit App");
                _looping = false;
            }
        }
        
        static void LogInfo(string info)
        {
            var msg = string.Format("[{0}][{1}][{2}] {3} => {4:s}", "SimpleLog", "Info", "Demo.Foo", info, DateTime.Now);
            Console.WriteLine(msg);
            Trace.WriteLine(msg);
        }
    }
}

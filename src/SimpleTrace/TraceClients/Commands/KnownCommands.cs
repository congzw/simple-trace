using System.Collections.Generic;
using System.Linq;

namespace SimpleTrace.TraceClients.Commands
{
    public class KnownCommands
    {
        public KnownCommands()
        {
            CommandLogistics = new List<ICommandLogistic>();
        }

        public IList<ICommandLogistic> CommandLogistics { get; set; }

        private readonly object _lock = new object();
        public void Register(ICommandLogistic commandLogistic)
        {
            lock (_lock)
            {
                var any = CommandLogistics.Any(x => x.GetType() == commandLogistic.GetType());
                if (any)
                {
                    return;
                }
                CommandLogistics.Add(commandLogistic);
            }
        }
        
        public static KnownCommands Instance = new KnownCommands();
    }
}
using System;
using SimpleTrace.Common;

namespace SimpleTrace.TraceClients.Commands
{
    public class Command
    {
        public Command(object args, string commandType)
        {
            Args = args;
            ArgsType = args.GetType().Name;
            CommandType = commandType;
            CreateUtc = DateHelper.Instance.GetDateNow();
        }

        public string CommandType { get; set; }
        public string ArgsType { get; set; }
        public DateTime CreateUtc { get; set; }
        public object Args { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleTrace.Common;
using SimpleTrace.TraceClients.ScheduleTasks;

namespace SimpleTrace.TraceClients.Commands
{
    public interface ICommandLogistic
    {
        bool NeedDelay { get; }
        float ProcessSort { get; }
        bool IsForCommand(Command command);
        bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache);
    }

    public abstract class BaseCommandLogistic<T>: ICommandLogistic where T : ICommandLogistic
    {
        protected BaseCommandLogistic(bool needDelay, float processSort)
        {
            NeedDelay = needDelay;
            ProcessSort = processSort;
            CommandType = this.GetType();
        }

        protected Type CommandType { get; set; }
        
        public float ProcessSort { get; }
        public bool IsForCommand(Command command)
        {
            return CommandType.Name == command.CommandType;
        }
        public bool NeedDelay { get; }
        public abstract bool CreateOrUpdate(Command command, IDictionary<string, ClientSpanEntity> clientSpanCache);

        public static Command Create(object args)
        {
            return new Command(args, typeof(T).Name);
        }
    }
}
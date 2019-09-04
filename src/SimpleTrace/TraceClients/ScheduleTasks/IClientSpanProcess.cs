using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleTrace.TraceClients.ScheduleTasks
{
    public interface IClientSpanProcess
    {
        float SortNum { get; set; }
        Task Process(IList<IClientSpan> entities);
    }


    public class ClientSpanProcessRegistry
    {
        public ClientSpanProcessRegistry()
        {
            GetAllProcesses = () => new List<IClientSpanProcess>();
        }

        public Func<IList<IClientSpanProcess>> GetAllProcesses { get; set; }

        //private readonly object _lock = new object();
        //public void Register(ICommandLogistic commandLogistic)
        //{
        //    lock (_lock)
        //    {
        //        var any = CommandLogistics.Any(x => x.GetType() == commandLogistic.GetType());
        //        if (any)
        //        {
        //            return;
        //        }
        //        CommandLogistics.Add(commandLogistic);
        //    }
        //}

        public static ClientSpanProcessRegistry Instance = new ClientSpanProcessRegistry();
    }
}
//using System;
//using System.Collections.Generic;
//using SimpleTrace.Common;

//namespace SimpleTrace.TraceClients.Commands
//{
//    public interface ICommand
//    {
//        float ProcessSort { get; set; }
//        string CommandType { get; set; }
//        DateTime CreateUtc { get; set; }
//        object Args { get; set; }
//        bool CreateOrUpdate(IDictionary<string, ClientSpanEntity> clientSpanCache);
//    }

//    public abstract class BaseCommand: ICommand
//    {
//        protected BaseCommand(object args, float processSort)
//        {
//            Args = args;
//            ProcessSort = processSort;
//            CreateUtc = DateHelper.Instance.GetDateNow();
//            CommandType = this.GetType().Name;
//        }

//        public float ProcessSort { get; set; }
//        public string CommandType { get; set; }
//        public DateTime CreateUtc { get; set; }
//        public object Args { get; set; }
//        public abstract bool CreateOrUpdate(IDictionary<string, ClientSpanEntity> clientSpanCache);
//    }
//}

//namespace SimpleTrace.TraceClients.Commands
//{
//    //可以由实现方自行扩展，预置命令集中在一起方便查看
//    public static class CommandExtensions
//    {
//        //StartSpan
//        private static readonly string startSpan = "StartSpan";
//        public static string StartSpan(this KnownCommands instance)
//        {
//            return startSpan;
//        }
//        public static Command CreateForStartSpan(this CommandFactory instance, ClientSpan args)
//        {
//            return instance.Create(startSpan, args);
//        }
//        public static bool IsStartSpan(this Command instance)
//        {
//            return instance.CommandType == startSpan;
//        }

//        //Log
//        private static readonly string log = "Log";
//        public static string Log(this KnownCommands instance)
//        {
//            return startSpan;
//        }
//        public static Command CreateForLog(this CommandFactory instance, LogArgs args)
//        {
//            return instance.Create(log, args);
//        }

//        //SetTags
//        private static readonly string setTags = "SetTags";
//        public static string SetTags(this KnownCommands instance)
//        {
//            return setTags;
//        }
//        public static Command CreateForSetTags(this CommandFactory instance, SetTagArgs args)
//        {
//            return instance.Create(setTags, args);
//        }


//        //FinishSpan
//        private static readonly string finishSpan = "FinishSpan";
//        public static string FinishSpan(this KnownCommands instance)
//        {
//            return finishSpan;
//        }
//        public static Command CreateForFinishSpan(this CommandFactory instance, FinishSpanArgs args)
//        {
//            return instance.Create(finishSpan, args);
//        }

//        //SaveSpans
//        private static readonly string saveSpans = "SaveSpans";
//        public static string SaveSpans(this KnownCommands instance)
//        {
//            return saveSpans;
//        }
//        public static Command CreateForSaveSpans(this CommandFactory instance, SaveSpansArgs args)
//        {
//            return instance.Create(saveSpans, args);
//        }

//        ////是否需要缓冲处理
//        //public static bool NeedDelay(this Command command)
//        //{
//        //    return command.CommandType != saveSpans;
//        //}
//    }
//}

//// ReSharper disable CheckNamespace

//using System.Collections.Generic;
//using Common;

//namespace SimpleTrace.TraceClients.Commands
//{
//    //可以由实现方自行扩展，预置命令集中在一起方便查看
//    public static class MyCommandExtensions
//    {
//        //StartSpan
//        private static readonly string startSpan = "StartSpan";
//        public static string StartSpan(this KnownCommands instance)
//        {
//            return startSpan;
//        }

//        //Log
//        private static readonly string log = "Log";
//        public static string Log(this KnownCommands instance)
//        {
//            return startSpan;
//        }

//        //SetTags
//        private static readonly string setTags = "SetTags";
//        public static string SetTags(this KnownCommands instance)
//        {
//            return setTags;
//        }


//        //FinishSpan
//        private static readonly string finishSpan = "FinishSpan";
//        public static string FinishSpan(this KnownCommands instance)
//        {
//            return finishSpan;
//        }

//        //SaveSpans
//        private static readonly string saveSpans = "SaveSpans";
//        public static string SaveSpans(this KnownCommands instance)
//        {
//            return saveSpans;
//        }

//        //是否需要缓冲处理
//        public static bool NeedDelay(this ICommand command)
//        {
//            return command.CommandType != saveSpans;
//        }
        
//        public static IEnumerable<T> FilterCommands<T>(IList<object> jsonCommands) where T : ICommand
//        {
//            foreach (var queueInfoCommand in jsonCommands)
//            {
//                if (queueInfoCommand is T theCommand)
//                {
//                    yield return theCommand;
//                }
//                else
//                {
//                    var propName = "CommandType";
//                    var tryGetProperty = queueInfoCommand.TryGetProperty(propName, true, out var propValue);
//                    if (tryGetProperty)
//                    {
//                        if (propValue.ToString() == typeof(T).Name)
//                        {
//                            yield return queueInfoCommand.As<T>();
//                        }
//                    }
//                }
//            }
//        }

//        public static IEnumerable<ICommand> AsCommands(IList<object> jsonCommands)
//        {
//            foreach (var queueInfoCommand in jsonCommands)
//            {
//                if (queueInfoCommand is ICommand theCommand)
//                {
//                    yield return theCommand;
//                }
//                else
//                {
//                    var propName = "CommandType";
//                    var tryGetProperty = queueInfoCommand.TryGetProperty(propName, true, out var propValue);
//                    if (tryGetProperty)
//                    {
//                        if (propValue.ToString() == typeof(T).Name)
//                        {
//                            yield return queueInfoCommand.As<T>();
//                        }
//                    }
//                }
//            }
//        }
//    }
//}

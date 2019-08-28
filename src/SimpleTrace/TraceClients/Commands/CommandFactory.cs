//using System;

//namespace SimpleTrace.TraceClients.Commands
//{
//    public class CommandFactory
//    {
//        public ICommand Create(string commandType, object args)
//        {
//            if (string.IsNullOrWhiteSpace(commandType))
//            {
//                throw new ArgumentNullException(nameof(commandType));
//            }
//            return new Command() { CommandType = commandType, Args = args };
//        }

//        public static CommandFactory Instance = new CommandFactory();
//    }
//}

// ReSharper disable CheckNamespace

namespace SimpleTrace.Common
{
    #region demo for LogWithSimpleEventBus
    
    public static class SimpleEventBusExtensions
    {
        public static string SimpleEventBus = "SimpleEventBus";

        public static ISimpleLogFactory LogWithSimpleEventBus(this ISimpleLogFactory simpleLogFactory)
        {
            var logActions = simpleLogFactory.LogActions;
            
            logActions.SetActions(SimpleEventBus, true, LogMessage);

            var initLog = simpleLogFactory.CreateLogFor("SimpleEventBusExtensions");
            initLog.LogInfo(">>>> LogWithSimpleEventBus");
            return simpleLogFactory;
        }

        private static void LogMessage(LogMessageArgs args)
        {
            var simpleEventBus = SimpleEventBus<AsyncMessageEvent>.Resolve();
            if (args?.Message != null)
            {
                simpleEventBus.Raise(new AsyncMessageEvent(args.Message.ToString()));
            }
        }
    }

    #endregion
}
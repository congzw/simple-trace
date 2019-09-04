// ReSharper disable CheckNamespace
namespace SimpleTrace.TraceClients.Commands
{
    public static class KnownCommandsExtensions
    {
        public static KnownCommands Setup(this KnownCommands knownCommands)
        {
            knownCommands.Register(new StartSpanCommand());
            knownCommands.Register(new LogCommand());
            knownCommands.Register(new SetTagCommand());
            knownCommands.Register(new FinishSpanCommand());
            //batch save
            knownCommands.Register(new SaveSpansCommand());
            return knownCommands;
        }
    }
}

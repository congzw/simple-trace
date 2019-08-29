using SimpleTrace.Common;

namespace Demo.WinApp
{
    public class SimpleLogSingleton<T>
    {
        private ISimpleLog _simpleLog;
        public ISimpleLog Logger => _simpleLog ?? (_simpleLog = SimpleLogFactory.Resolve().GetOrCreateLogFor<T>());

        public static SimpleLogSingleton<T> Instance = new SimpleLogSingleton<T>();
    }
}
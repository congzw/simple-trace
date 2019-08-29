using SimpleTrace.Common;

namespace Demo.WinApp.UI
{
    public class TraceClientsFormCtrl
    {
        public MessageResult CallTraceApi(CallTraceApiArgs args)
        {
            var messageResult = new MessageResult();
            messageResult.Message = "OK";
            messageResult.Success = true;
            return messageResult;
        }
    }

    public class CallTraceApiArgs
    {
        public int Count { get; set; }
        public int Interval { get; set; }
    }
}

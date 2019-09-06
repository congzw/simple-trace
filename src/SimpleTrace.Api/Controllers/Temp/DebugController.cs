using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;
using SimpleTrace.TraceClients.ScheduleTasks;

// ReSharper disable CheckNamespace

namespace SimpleTrace.Api.Controllers
{
    [Route("Api/Debug")]
    public class DebugController : ControllerBase
    {
        [Route("QueueProcessLogs_Get")]
        [HttpGet]
        public Task<IList<CommandQueueProcessLog>> QueueProcessLogs_Get(bool? enabled, int? spanSec)
        {
            var commandQueueProcessLogs = CommandQueueProcessLogs.Instance;
            IList<CommandQueueProcessLog> queueProcessLogs = commandQueueProcessLogs.Items.Select(x => x.Value).ToList();
            return Task.FromResult(queueProcessLogs);
        }

        //use get for easy test only, todo: replace with ui and post
        [Route("QueueProcessLogs_Set")]
        [HttpGet]
        public Task<MessageResult> QueueProcessLogs_Set(bool? enabled, bool? clear)
        {
            var commandQueueProcessLogs = CommandQueueProcessLogs.Instance;
            commandQueueProcessLogs.Enabled = enabled ?? false;
            var needClear = clear ?? false;
            if (needClear)
            {
                commandQueueProcessLogs.Clear();
            }
            var result = string.Format("Set at:{0} enabled:{1}, clear:{2}", DateHelper.Instance.GetDateNow(), commandQueueProcessLogs.Enabled, needClear);

            return MessageResult.Create(true, result).AsTask();
        }
    }
}

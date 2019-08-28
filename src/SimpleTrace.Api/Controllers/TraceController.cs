using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SimpleTrace.TraceClients;
using SimpleTrace.TraceClients.Api;

namespace SimpleTrace.Api.Controllers
{
    [Route("api/trace")]
    [ApiController]
    public class TraceController : ControllerBase, IClientTracerApi
    {
        private readonly IClientTracerApi _clientTracerApi;

        public TraceController(IClientTracerApi clientTracerApi)
        {
            _clientTracerApi = clientTracerApi;
        }

        #region for test only
        
        //for test only
        [Route("GetDate")]
        [HttpGet]
        public Task<DateTime> GetDate()
        {
            return Task.FromResult(DateTime.Now);
        }

        #endregion

        [Route("StartSpan")]
        [HttpPost]
        public Task StartSpan(ClientSpan args)
        {
            return _clientTracerApi.StartSpan(args);
        }

        [Route("Log")]
        [HttpPost]
        public Task Log(LogArgs args)
        {
            return _clientTracerApi.Log(args);
        }

        [Route("SetTags")]
        [HttpPost]
        public Task SetTags(SetTagArgs args)
        {
            return _clientTracerApi.SetTags(args);
        }

        [Route("FinishSpan")]
        [HttpPost]
        public Task FinishSpan(FinishSpanArgs args)
        {
            return _clientTracerApi.FinishSpan(args);
        }

        [Route("SaveSpans")]
        [HttpPost]
        public Task SaveSpans(SaveSpansArgs args)
        {
            return _clientTracerApi.SaveSpans(args);
        }

        [Route("GetQueueInfo")]
        [HttpGet]
        public Task<QueueInfo> GetQueueInfo([FromQuery]GetQueueInfoArgs args)
        {
            return _clientTracerApi.GetQueueInfo(args);
        }
    }
}

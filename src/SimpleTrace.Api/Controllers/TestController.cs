using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CommonTrace.Api.Controllers
{
    //for test only
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [Route("GetDate")]
        [HttpGet]
        public Task<DateTime> GetDate()
        {
            return Task.FromResult(DateTime.Now);
        }

        [Route("AppendFile")]
        [HttpGet]
        public Task<bool> AppendFile(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                var testFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");
                var line = string.Format("{0:yyyy-MM-dd HH:mm:ss:fff} => {1}", DateTime.Now, message);
                System.IO.File.AppendAllLinesAsync(testFilePath, new[] { line }, Encoding.UTF8, CancellationToken.None);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}

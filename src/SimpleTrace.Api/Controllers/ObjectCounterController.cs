using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNetCore.Mvc;

namespace SimpleTrace.Api.Controllers
{
    [Route("api/oc")]
    public class ObjectCountController : ControllerBase, IDisposable
    {
        private readonly ISimpleJson _simpleJson;
        private readonly ISimpleJsonFile _simpleJsonFile;
        private readonly ISimpleLogFactory _simpleLogFactory;
        private readonly SimpleLogFactory _simpleLogFactory2;

        public ObjectCountController(ISimpleJson simpleJson, ISimpleJsonFile simpleJsonFile, ISimpleLogFactory simpleLogFactory, SimpleLogFactory simpleLogFactory2)
        {
            this.ReportAdd();
            _simpleJson = simpleJson;
            _simpleJsonFile = simpleJsonFile;
            _simpleLogFactory = simpleLogFactory;
            _simpleLogFactory2 = simpleLogFactory2;
        }

        [Route("GetInfos")]
        [HttpGet]
        public Task<IList<ObjectCountInfo>> GetInfos()
        {
            var simpleJson = SimpleJson.Resolve();
            var simpleJsonFile = SimpleJson.ResolveSimpleJsonFile();
            var simpleLogFactory = SimpleLogFactory.Resolve();

            var objectCountInfos = ObjectCountInfo.Create(ObjectCounter.Instance);
            return Task.FromResult(objectCountInfos);
        }

        public void Dispose()
        {
            ////for test only
            //if (DateTime.Now.Second % 2 == 0)
            //{
            //    this.ReportDelete();
            //}
            this.ReportDelete();
        }
    }

    public class ObjectCountInfo
    {
        public string Name { get; set; }
        public int Count { get; set; }

        public static IList<ObjectCountInfo> Create(ObjectCounter counter)
        {
            var infos = new List<ObjectCountInfo>();

            foreach (var item in counter.Items)
            {
                var theType = item.Key;
                var info = new ObjectCountInfo();
                info.Name = theType.FullName;
                info.Count = item.Value;
                infos.Add(info);
            }
            return infos;
        }
    }
}

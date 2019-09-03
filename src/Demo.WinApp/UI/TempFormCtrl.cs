using System;
using System.Collections.Generic;
using Common;

namespace Demo.WinApp.UI
{
    public class TempFormCtrl
    {
        public TempFormCtrl()
        {
            Log = SimpleLogSingleton<TempFormCtrl>.Instance.Logger;
        }

        public void TestMyDictionary()
        {

            var myDic = new MyDictionary<string>();
            myDic.Add("A", "foo");
            myDic.Add("B", "bar");
            Log.LogInfo(myDic.ToJson(false));


            var myDic2 = new MyDictionary<object>();
            myDic2.Add("A", "foo");
            myDic2.Add("B", true);
            myDic2.Add("C", 1);


            var mySpan = new MySpan();
            mySpan.Tags = myDic2;
            mySpan.Logs = myDic2;

            Log.LogInfo(mySpan.ToJson(false));
        }
        
        public ISimpleLog Log { get; set; }
    }

    public class MyDictionary<T> : Dictionary<string, T>
    {
        public MyDictionary() : base(StringComparer.OrdinalIgnoreCase)
        {
        }
    }

    public class MySpan
    {
        public MySpan()
        {
            Bags = new MyDictionary<string>();
            Tags = new MyDictionary<object>();
            Logs = new MyDictionary<object>();
            Items = new MyDictionary<object>();
        }

        public string Id { get; set; }
        public MyDictionary<string> Bags { get; set; }
        public MyDictionary<object> Tags { get; set; }
        public MyDictionary<object> Logs { get; set; }
        public MyDictionary<object> Nulls { get; set; }
        public IDictionary<string, object> Items { get; set; }
    }

}

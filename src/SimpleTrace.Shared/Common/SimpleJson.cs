using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpleTrace.Common
{
    public interface ISimpleJson
    {
        string SerializeObject(object value, bool indented);
        T DeserializeObject<T>(string json);
        object DeserializeObject(string json, object defaultValue);
    }

    public interface ISimpleJsonFile
    {
        Task<IList<T>> ReadFile<T>(string filePath);
        Task SaveFile<T>(string filePath, IList<T> list, bool indented);
    }

    public class SimpleJson : ISimpleJson, ISimpleJsonFile
    {
        private readonly AsyncFile _asyncFile = AsyncFile.Instance;

        public string SerializeObject(object value, bool indented)
        {
            return JsonConvert.SerializeObject(value, indented ? Formatting.Indented : Formatting.None);
        }

        public T DeserializeObject<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(json);
        }

        public object DeserializeObject(string json, object defaultValue)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return defaultValue;
            }
            return JsonConvert.DeserializeObject(json);
        }

        public async Task<IList<T>> ReadFile<T>(string filePath)
        {
            var list = new List<T>();
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return list;
            }

            var json = await _asyncFile.ReadAllText(filePath).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(json))
            {
                return list;
            }
            list = DeserializeObject<List<T>>(json);
            return list;
        }

        public Task SaveFile<T>(string filePath, IList<T> list, bool indented)
        {
            if (string.IsNullOrWhiteSpace(filePath) || list == null)
            {
                return Task.FromResult(0);
            }
            
            var json = SerializeObject(list, indented);
            return _asyncFile.WriteAllText(filePath, json);
        }

        #region for di extensions

        private static readonly Lazy<SimpleJson> Instance = new Lazy<SimpleJson>(() => new SimpleJson());
        public static Func<ISimpleJson> Resolve { get; set; } = () => Instance.Value;
        public static Func<ISimpleJsonFile> ResolveSimpleJsonFile { get; set; } = () => Instance.Value;

        #endregion
    }

    public static class SimpleJsonExtensions
    {
        public static async Task<T> ReadFileAsSingle<T>(this ISimpleJsonFile simpleJsonFile, string filePath)
        {
            var list = await simpleJsonFile.ReadFile<T>(filePath).ConfigureAwait(false);
            if (list == null || list.Count == 0)
            {
                return default(T);
            }
            return list.SingleOrDefault();
        }

        public static Task SaveFileAsSingle<T>(this ISimpleJsonFile simpleJsonFile, string filePath, T model, bool indented)
        {
            if (model == null)
            {
                return Task.FromResult(0);
            }
            return simpleJsonFile.SaveFile(filePath, new T[] { model }, indented);
        }

        public static async Task AppendFile<T>(this ISimpleJsonFile simpleJsonFile, string filePath, IList<T> list, bool indented)
        {
            if (string.IsNullOrWhiteSpace(filePath) || list == null || list.Count == 0)
            {
                return;
            }
            var saveItems = new List<T>();
            var currents = await simpleJsonFile.ReadFile<T>(filePath).ConfigureAwait(false);
            if (currents != null)
            {
                saveItems.AddRange(currents);
            }
            saveItems.AddRange(list);
            await simpleJsonFile.SaveFile(filePath, saveItems, indented).ConfigureAwait(false);
        }

        public static string ToJson(this object value, bool indented)
        {
            return SimpleJson.Resolve().SerializeObject(value, indented);
        }

        public static object FromJson(this string content, object defaultValue)
        {
            return SimpleJson.Resolve().DeserializeObject(content, defaultValue);
        }

        public static T FromJson<T>(this string content, T defaultValue)
        {
            var instance = SimpleJson.Resolve().DeserializeObject(content, defaultValue);
            return (T)instance;
        }

        public static T As<T>(this object instance, bool throwEx = true)
        {
            if (instance == null)
            {
                return default(T);
            }

            //JObject is JToken?
            //if (instance is JObject jObject)
            //{
            //    return jObject.ToObject<T>();
            //}

            if (instance is JToken token)
            {
                return token.ToObject<T>();
            }

            if (instance is T theValue)
            {
                return theValue;
            }

            if (!throwEx)
            {
                return default(T);
            }
            throw new InvalidOperationException(string.Format("object can not cast from {0} to {1}", instance.GetType().Name, typeof(T).Name));
        }
        public static IEnumerable<T> As<T>(this IEnumerable<object> instances, bool throwEx = true)
        {
            foreach (var instance in instances)
            {
                yield return instance.As<T>(throwEx);
            }
        }

        public static bool TryGetProperty(this object instance, string propName, bool ignoreCase, out object propValue)
        {
            propValue = null;
            if (instance == null)
            {
                return false;
            }

            //JArray
            //JObject
            if (instance is JObject jObject)
            {
                var dic = (IDictionary<string, JToken>)jObject;
                var tryGetProperty = dic.TryGetValue(propName, ignoreCase, out propValue);
                return tryGetProperty;
            }

            var dic2 = MyModelHelper.GetKeyValueDictionary(instance);
            var tryGetProperty2 = dic2.TryGetValue(propName, ignoreCase, out propValue);
            return tryGetProperty2;
        }
        
        public static bool TryGetValue<T>(this IDictionary<string, T> dic, string propName, bool ignoreCase, out object propValue)
        {
            propValue = null;
            if (dic == null)
            {
                return false;
            }

            var theKey = dic.Keys.SingleOrDefault(x => x.Equals(propName));
            if (theKey == null)
            {
                if (ignoreCase)
                {
                    theKey = dic.Keys.SingleOrDefault(x => x.Equals(propName, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (theKey == null)
            {
                return false;
            }
            propValue = dic[theKey];
            return true;
        }
    }
}

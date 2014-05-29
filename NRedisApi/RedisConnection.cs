using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace NRedisApi
{
    /// <summary>
    /// Deprecated initial experiment connecting to and performing operations against StackExchange.Redis.
    /// Included for now as an example of alternative to Fluent API currently being developed
    /// </summary>
    public class RedisConnection : IRedisConnection
    {

        private readonly IDatabase _redis;

        /// <summary>
        /// Constructor that receives IDatabase instance via constructor injection
        /// </summary>
        /// <param name="redis">IDatabase instance</param>
        internal RedisConnection(IDatabase redis)
        {
            _redis = redis;
        }

        /// <summary>
        /// Gets Redis String (data structure) deserialised from JSON to T by URN key
        /// </summary>
        /// <typeparam name="T">Type to deserialise to</typeparam>
        /// <param name="key">Redis URN</param>
        /// <returns></returns>
        public T GetValue<T>(string key)
        {
            return JsonConvert.DeserializeObject<T>(_redis.StringGet(key));
        }

        /// <summary>
        /// Sets Redis String (data structure) of type T, serialised to JSON
        /// </summary>
        /// <typeparam name="T">Type of object being stored</typeparam>
        /// <param name="key">Redis URN</param>
        /// <param name="value">instance of T to be serialised and set as Redis string</param>
        public void Save<T>(string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            _redis.StringSet(key, json);
        }

        /// <summary>
        /// Sets Redis Hash (data structure) item of type T, serialised to JSON, to create a collection of serialised .Net objects using Hash-Field name to identify objects and value
        /// to store JSON representation of object. 
        /// Creates Redis Hash-Field name using string array of unique identifier fields' values (similar to composite primary key in SQL) via Reflection.
        /// Its highly weakly-typed nature and potential for confusion and/or typos with array of strings is in part what Fluent API approach is intended to mitigate.
        /// </summary>
        /// <typeparam name="T">Type of object being stored</typeparam>
        /// <param name="key">Redis Hash URN</param>
        /// <param name="value">instance of T to be serialised and set as Redis Hash-Field value</param>
        /// <param name="uniqueIdentifierFieldNames">string array of field names the values of which will be used to generate Hash-Field names to ID each object in collection</param>
        public void SaveToHash<T>(string key, T value, string[] uniqueIdentifierFieldNames)
        {
            var json = JsonConvert.SerializeObject(value);
            var type = typeof(T);
            var properties = type.GetProperties();
            var sb = new StringBuilder();
            foreach (var property in properties.Where(p => uniqueIdentifierFieldNames.Contains(p.Name)))
            {
                if (sb.Length > 0)
                    sb.Append("_");
                sb.Append(property.GetValue(value, null));
            }

            _redis.HashSet(key, sb.ToString(), json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="uniqueIdentifierFieldNames"></param>
        /// <param name="uniqueIdentifierFieldNamesAndValues"></param>
        /// <returns></returns>
        public T FindFromHash<T>(string key, string[] uniqueIdentifierFieldNames, Tuple<string, string>[] uniqueIdentifierFieldNamesAndValues)
        {
            var storedObject = default(T);
            var type = typeof(T);
            var properties = type.GetProperties();
            var sb = new StringBuilder();
            foreach (var firstOrDefault in properties.Where(p => uniqueIdentifierFieldNames.Contains(p.Name)).Select(localProperty => uniqueIdentifierFieldNamesAndValues.FirstOrDefault(v => v.Item1.Equals(localProperty.Name))).Where(firstOrDefault => firstOrDefault != null))
            {
                if (sb.Length > 0)
                    sb.Append("_");
                sb.Append(firstOrDefault.Item2);
            }
            var hashFieldName = sb.ToString();

            if (_redis.HashExists(key, hashFieldName))
                storedObject = JsonConvert.DeserializeObject<T>(_redis.HashGet(key, hashFieldName));

            return storedObject;
        }


    }
}
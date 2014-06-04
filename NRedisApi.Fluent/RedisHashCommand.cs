using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace NRedisApi.Fluent
{
    /// <summary>
    /// Non-generic (i.e. untyped command) RedisHashCommand
    /// </summary>
    public class RedisHashCommand : IRedisHashCommand
    {
        private readonly IDatabase _redis;
        private string _urn;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="redis"></param>
        internal RedisHashCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IRedisHashCommand<T> As<T>()
        {
            IRedisHashCommand<T> hashCmd = new RedisHashCommand<T>(_redis);
            hashCmd.Urn(_urn);
            return hashCmd;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        public IRedisHashCommand Urn(string urn)
        {
            _urn = urn;
            return this;
        }
    }

    /// <summary>
    /// Generically typed RedisHashCommand with complete set of Hash functionality available
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedisHashCommand<T> : IRedisHashCommand<T>
    {
        private readonly IDatabase _redis;
        private IEnumerable<PropertyInfo> _uniqueIdProperties;
        private IDictionary<string, string> _uidFieldsAndValues; 
        private string _urn;

        /// <summary>
        /// internal constructor so can only be invoked from IRedisCommaand, not directly
        /// </summary>
        /// <param name="redis"></param>
        internal RedisHashCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
            _uidFieldsAndValues = null;
            //_uniqueIdProperties = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            if (_uniqueIdProperties == null || _uidFieldsAndValues == null || string.IsNullOrEmpty(_urn))
                throw new RedisCommandConfigurationException(
                    @"You must define a URN for the Hash, at least one unique ID field and a provide a value for each of these Unique ID fields 
                      from the object you wish to retrieve to access Hash values");

            var fieldName = GetFieldNameForStoredInstance();
            return JsonConvert.DeserializeObject<T>(_redis.HashGet(_urn, fieldName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Set(T value)
        {
            if (_uniqueIdProperties == null || string.IsNullOrEmpty(_urn))
                throw new RedisCommandConfigurationException("You must define a URN for the Hash and at least one unique ID field to store Hash values");
            var json = JsonConvert.SerializeObject(value);

            var fieldName = SetFieldName(value);
            _redis.HashSet(_urn, fieldName, json);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public void Set(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                Set(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Remove()
        {
            if (_uniqueIdProperties == null || _uidFieldsAndValues == null)
                throw new RedisCommandConfigurationException("You must define at least one unique ID field and provide that field's value for the object you wish to remove to delete a hash entry");

            var fieldName = GetFieldNameForStoredInstance();
            _redis.HashDelete(_urn, fieldName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public IRedisHashCommand<T> UniqueIdFieldName(string fieldName)
        {
            //create list if null
            if (_uniqueIdProperties == null)
                _uniqueIdProperties = new List<PropertyInfo>();

            //cast IEnumerable<PropertyInfo> to IList<PropertyInfo> & make sure that property exists in type.
            //if not property exists throw Config exception, else add PropertyInfo to collection, order alphabetically by name and store as Enumerable again
            IList<PropertyInfo> uidPropertyList = _uniqueIdProperties.ToList();
            var pi = typeof(T).GetProperty(fieldName);
            if (pi == null)
                throw new RedisCommandConfigurationException(string.Format(@"Property '{0}' does not exist for {1} so cannot be set as a UniqueID property for this type.", fieldName, typeof(T).Name));

            uidPropertyList.Add(pi);
            var orderedUidPropertyList = uidPropertyList.OrderBy(o => o.Name);
            _uniqueIdProperties = orderedUidPropertyList.AsEnumerable();

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uidNameValuePairs"></param>
        /// <returns></returns>
        public IRedisHashCommand<T> UniqueIdFieldValues(IDictionary<string, string> uidNameValuePairs)
        {
            if(!uidNameValuePairs.All(nvp => _uniqueIdProperties.Any(uid => uid.Name.Equals(nvp.Key))))
                throw new RedisCommandConfigurationException("The Properties and Values supplied to find haah value do not match the set UniqueID properties.");
            _uidFieldsAndValues = uidNameValuePairs;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        public IRedisHashCommand<T> Urn(string urn)
        {
            _urn = urn;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetFieldNameForStoredInstance()
        {
            //create fieldName
            var sb = new StringBuilder();
            foreach (var property in _uniqueIdProperties)
            {
                if (sb.Length > 0)
                    sb.Append("_");
                sb.Append(_uidFieldsAndValues[property.Name]);
            }
            return sb.ToString();
        }

        private string SetFieldName(T value)
        {
            //create fieldName
            var sb = new StringBuilder();
            foreach (var property in _uniqueIdProperties)
            {
                if (sb.Length > 0)
                    sb.Append("_");
                sb.Append(property.GetValue(value, null));
            }
            return sb.ToString();
        }
    }
}

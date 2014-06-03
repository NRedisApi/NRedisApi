using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace NRedisApi.Fluent
{
    public class RedisHashCommand : IRedisHashCommand
    {
        private readonly IDatabase _redis;
        private string _urn;

        internal RedisHashCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
        }

        public IRedisHashCommand<T> As<T>()
        {
            IRedisHashCommand<T> hashCmd = new RedisHashCommand<T>(_redis);
            hashCmd.Urn(_urn);
            return hashCmd;
        }

        public IRedisHashCommand Urn(string urn)
        {
            _urn = urn;
            return this;
        }
    }

    public class RedisHashCommand<T> : IRedisHashCommand<T>
    {
        private readonly IDatabase _redis;
        private IEnumerable<string> _uniqueIdFields;
        //private IEnumerable<PropertyInfo> _uniqueIdProperties;
        private IDictionary<string, string> _uidFieldsAndValues; 
        private string _urn;

        internal RedisHashCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
            _uniqueIdFields = null;
            _uidFieldsAndValues = null;
            //_uniqueIdProperties = null;
        }

        public T Get()
        {
            if (_uniqueIdFields == null || _uidFieldsAndValues == null)
                throw new RedisSetupIncompleteException("You must define at least one unique ID field and provide that field's value for the object you wish to retrieve to access Hash values");

            var fieldName = GetFieldNameForStoredInstance();
            return JsonConvert.DeserializeObject<T>(_redis.HashGet(_urn, fieldName));
        }

        public void Set(T value)
        {
            if (_uniqueIdFields == null)
                throw new RedisSetupIncompleteException("You must define at least one unique ID field to store Hash values");
            var json = JsonConvert.SerializeObject(value);

            var fieldName = SetFieldName(value);
            _redis.HashSet(_urn, fieldName, json);
        }

        public void Set(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                Set(value);
            }
        }

        public void Remove()
        {
            if (_uniqueIdFields == null || _uidFieldsAndValues == null)
                throw new RedisSetupIncompleteException("You must define at least one unique ID field and provide that field's value for the object you wish to remove to delete a hash entry");

            var fieldName = GetFieldNameForStoredInstance();
            _redis.HashDelete(_urn, fieldName);
        }

        public IRedisHashCommand<T> UniqueIdFieldName(string fieldName)
        {
            if (_uniqueIdFields == null)
                _uniqueIdFields = new List<string>().AsEnumerable();

            //if (_uniqueIdProperties == null)
            //    _uniqueIdProperties = new List<PropertyInfo>().AsEnumerable();

            var uidList = _uniqueIdFields.ToList();
            uidList.Add(fieldName);

            //var uidPropertyList = _uniqueIdProperties.ToList();
            //var pi = typeof(T).GetProperty(fieldName);
            //if (pi == null)
            //    throw new RedisSetupIncompleteException(string.Format(@"Property '{0}' does not exist for {1} so cannot be set as a UniqueID property for this type.", fieldName, typeof(T).Name));

            //uidPropertyList.Add(pi);
            //var orderedUidPropertyList = uidPropertyList.OrderBy(o => o.Name);
            //_uniqueIdProperties = orderedUidPropertyList.AsEnumerable();

                

            var orderedUidList = uidList.OrderBy(o => o[0]);
            _uniqueIdFields = orderedUidList.AsEnumerable();
            return this;
        }

        public IRedisHashCommand<T> UniqueIdFieldValues(IDictionary<string, string> uidNameValuePairs)
        {
            _uidFieldsAndValues = uidNameValuePairs;
            return this;
        }

        public IRedisHashCommand<T> Urn(string urn)
        {
            _urn = urn;
            return this;
        }

        private string GetFieldNameForStoredInstance()
        {
            //create fieldName
            var type = typeof(T);
            var properties = type.GetProperties();
            var sb = new StringBuilder();
            foreach (var property in properties.Where(p => _uniqueIdFields.Contains(p.Name)))
            {
                if (sb.Length > 0)
                    sb.Append("_");
                sb.Append(_uidFieldsAndValues[property.Name]);
            }
            var fieldName = sb.ToString();
            return fieldName;
        }

        private string SetFieldName(T value)
        {
            //create fieldName
            var type = typeof(T);
            var properties = type.GetProperties();
            var sb = new StringBuilder();
            foreach (var property in properties.Where(p => _uniqueIdFields.Contains(p.Name)))
            {
                if (sb.Length > 0)
                    sb.Append("_");
                sb.Append(property.GetValue(value, null));
            }
            var fieldName = sb.ToString();
            return fieldName;
        }
    }
}

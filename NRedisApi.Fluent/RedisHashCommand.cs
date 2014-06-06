using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace NRedisApi.Fluent
{
    /// <summary>
    /// Non-generic (i.e. untyped command) RedisHashCommand
    /// </summary>
    public class RedisHashCommand : RedisCommandBase, IRedisHashCommand
    {
        /// <summary>
        /// internal constructor so object may only be instantiated via Fluent config not directly
        /// </summary>
        /// <param name="redis"></param>
        /// <param name="jsonSerializerSettings"></param>
        internal RedisHashCommand(IDatabase redis, JsonSerializerSettings jsonSerializerSettings) : base(redis, jsonSerializerSettings)
        {
        }

        /// <summary>
        /// returns a typed redis hash command
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IRedisHashCommand of T</returns>
        public IRedisHashCommand<T> AsType<T>()
        {
            return new RedisHashCommand<T>(Redis, JsonSerializerSettings).SetUrn(Urn);
        }

        /// <summary>
        /// Sets _urn for command
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        public new IRedisHashCommand SetUrn(string urn)
        {
            Urn = urn;
            return this;
        }
    }

    /// <summary>
    /// Generically typed RedisHashCommand with complete set of Hash functionality available
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedisHashCommand<T> : RedisCommandBase, IRedisHashCommand<T>
    {
        private IEnumerable<PropertyInfo> _uniqueIdProperties;
        private IDictionary<string, string> _uidFieldsAndValues; 

        /// <summary>
        /// internal constructor so can only be invoked from IRedisCommaand, not directly
        /// </summary>
        /// <param name="redis"></param>
        /// <param name="jsonSerializerSettings"></param>
        internal RedisHashCommand(IDatabase redis, JsonSerializerSettings jsonSerializerSettings) : base(redis, jsonSerializerSettings)
        {
            _uidFieldsAndValues = null;
            _uniqueIdProperties = null;
        }

        /// <summary>
        /// returns a count of all elements contained in a Hash
        /// </summary>
        /// <returns></returns>
        public long Count()
        {
            if (string.IsNullOrEmpty(Urn))
                throw new RedisCommandConfigurationException(@"You must define a URN for the Hash to count the number of elements it contains.");
            return Redis.HashLength(Urn);
        }

        /// <summary>
        /// retrieves a single instance of T using uid properties and values. If either or _urn are not set config exception is thrown
        /// </summary>
        /// <returns>rehydrated instance of T from Hash at _urn and hash field as generated from uid fields and values</returns>
        public T Get()
        {
            if (_uniqueIdProperties == null || _uidFieldsAndValues == null || string.IsNullOrEmpty(Urn))
                throw new RedisCommandConfigurationException(
                    @"You must define a URN for the Hash, at least one unique ID field and a provide a value for each of these Unique ID fields 
                      from the object you wish to retrieve to access Hash values");

            var fieldName = GetFieldNameForStoredInstance();
            var returnValue = default(T);

            if (Redis.HashExists(Urn, fieldName))
                returnValue = JsonConvert.DeserializeObject<T>(Redis.HashGet(Urn, fieldName));   

            return returnValue;
        }

        public IEnumerable<T> GetAll()
        {
            if (string.IsNullOrEmpty(Urn))
                throw new RedisCommandConfigurationException(@"You must define a URN for the Hash to retrieve that Hash.");

            //try to deserialiase all elements in Hash to T so they can be included in the IEnumerable<T> that is returned. All elements that are
            //of type T all will be returned as part of the GetAll (values) collection whereas any that are not will be discarded.
            //NOTE: planned implementation of mini-schema that stores a Hash's config - uid fields and fully qualified .Net tyoe name of the Hash's .As<T> in an
            //item within a Set having the relevant URN as key 
            return Redis.HashGetAll(Urn).Where(ValueIsTypeT).Select(hashEntry => JsonConvert.DeserializeObject<T>(hashEntry.Value));
        }

        private bool ValueIsTypeT(HashEntry hashEntry)
        {
            var isT = true;

            var tryObject = JObject.Parse(hashEntry.Value);

            var typeProperties = typeof (T).GetProperties();
            if(tryObject.Children().Count() != typeProperties.Count() || typeProperties.Any(typeProperty => tryObject[typeProperty.Name] == null))
            {
                isT = false;
            }

            return isT;
        }

        /// <summary>
        /// Sets an item of T into the Hash specified by the RedisHashCommand's _urn
        /// If unique id properties or urn are not set a config exception is thrown
        /// </summary>
        /// <param name="value"></param>
        public void Set(T value)
        {
            if (_uniqueIdProperties == null || string.IsNullOrEmpty(Urn))
                throw new RedisCommandConfigurationException("You must define a URN for the Hash and at least one unique ID field to store Hash values");
            var json = JsonConvert.SerializeObject(value);

            var fieldName = SetFieldName(value);
            Redis.HashSet(Urn, fieldName, json);
        }

        /// <summary>
        /// Sets a collection of T by passing eack item into the singluar set command
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
        /// Removes an item of T from the Hash specified by the RedisHashCommand's _urn and identified within the Hash by rhe uid values.
        /// If unique id properties, uid values or urn are not set a config exception is thrown
        /// </summary>
        public void Remove()
        {
            if (_uniqueIdProperties == null || _uidFieldsAndValues == null)
                throw new RedisCommandConfigurationException("You must define at least one unique ID field and provide that field's value for the object you wish to remove to delete a hash entry");

            var fieldName = GetFieldNameForStoredInstance();
            Redis.HashDelete(Urn, fieldName);
        }

        /// <summary>
        /// Adds a unique ID field to the command for use in generating a key
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
        /// Sets RedisHashCommand's uid values for assembling a key to find a stored item via
        /// If ALL uid properties are not provided with matching value config exception is thrown
        /// </summary>
        /// <param name="uidNameValuePairs"></param>
        /// <returns></returns>
        public IRedisHashCommand<T> UniqueIdFieldValues(IDictionary<string, string> uidNameValuePairs)
        {
            if(!uidNameValuePairs.Any(nvp => _uniqueIdProperties.All(uid => uid.Name.Equals(nvp.Key))) || uidNameValuePairs.Values.Any(string.IsNullOrEmpty))
                throw new RedisCommandConfigurationException("The Properties and Values supplied to find hash value do not match the set UniqueID properties or are incomplete.");
            _uidFieldsAndValues = uidNameValuePairs;
            return this;
        }

        /// <summary>
        /// Sets _urn
        /// </summary>
        /// <param name="urn"></param>
        /// <returns></returns>
        public new IRedisHashCommand<T> SetUrn(string urn)
        {
            base.SetUrn(urn);
            return this;
        }

        /// <summary>
        /// converts instance of T's uid property values into key for retrieving value from Hash storage
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

        /// <summary>
        /// creates key from value being stored using uid properties configured for command
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

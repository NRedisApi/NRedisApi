using System;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace NRedisApi.Fluent
{
    /// <summary>
    /// untyped (non-generic) RedisStringCommand
    /// </summary>
    public class RedisStringCommand : IRedisStringCommand
    {
        private readonly IDatabase _redis;
        private string _urn;
        private TimeSpan? _timeSpanUntilExpiration;

        /// <summary>
        /// internal constructor prevents direct instantiation 
        /// </summary>
        /// <param name="redis">IDatabase instance</param>
        internal RedisStringCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
            _timeSpanUntilExpiration = null;
        }

        /// <summary>
        /// Sets the .Net type that this string will be serialised to/from
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>IRedisStringCommand of T populated with any properties already set</returns>
        public IRedisStringCommand<T> As<T>()
        {
            IRedisStringCommand<T> stringCmd = new RedisStringCommand<T>(_redis);
            stringCmd
                .Urn(_urn)
                .Expires(_timeSpanUntilExpiration);
            return stringCmd;
        }

        /// <summary>
        /// Sets the urn that is used to identify and retrieve this command's value
        /// </summary>
        /// <param name="urn">urn string</param>
        /// <returns>IRedisStringCommand this</returns>
        public IRedisStringCommand Urn(string urn)
        {
            _urn = urn;
            return this;
        }

        /// <summary>
        /// Sets a TimeSpan that defines how long the item stored by this command will be cached if Redis is being used as a cache
        /// </summary>
        /// <param name="timeSpanUntilExpiration">TimeSpan defining time until expiration</param>
        /// <returns>IRedisStringCommand this</returns>
        public IRedisStringCommand Expires(TimeSpan? timeSpanUntilExpiration)
        {
            _timeSpanUntilExpiration = timeSpanUntilExpiration;
            return this;
        }
    }

    /// <summary>
    /// Generic RedisStringCommand. Can perform operations as type is known for serialisation/deserialisation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedisStringCommand<T> : IRedisStringCommand<T>
    {
        private readonly IDatabase _redis;
        private string _urn;
        private TimeSpan? _timeSpanUntilExpiration;

        /// <summary>
        /// internal constructor prevents direct instantiation 
        /// </summary>
        /// <param name="redis">IDatabase instance</param>
        internal RedisStringCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
            _timeSpanUntilExpiration = null;
        }

        /// <summary>
        /// retrieves an item associated with the URN. Throws config exception if urn has not been assigned
        /// </summary>
        /// <returns>T item</returns>
        public T Get()
        {
            if (string.IsNullOrEmpty(_urn))
                throw new RedisCommandConfigurationException("You must define a URN for a String before attempting to retrieve its value");
            return JsonConvert.DeserializeObject<T>(_redis.StringGet(_urn));
        }

        /// <summary>
        /// stores an item associated with the URN. Throws config exception if urn has not been assigned
        /// </summary>
        /// <param name="value">object of type T to be serialised and stored</param>
        public void Set(T value)
        {
            if (string.IsNullOrEmpty(_urn))
                throw new RedisCommandConfigurationException("You must define a URN for a String before attempting to store its value");
            var json = JsonConvert.SerializeObject(value);
            if (_timeSpanUntilExpiration == null)
                _redis.StringSet(_urn, json);
            else
                _redis.StringSet(_urn, json, _timeSpanUntilExpiration.Value);
        }

        /// <summary>
        /// removes the item associated with the URN. Throws config exception if urn has not been assigned
        /// </summary>
        public void Remove()
        {
            if (string.IsNullOrEmpty(_urn))
                throw new RedisCommandConfigurationException("You must define a URN for a String before attempting to remove its value");
            _redis.KeyDelete(_urn);
        }

        /// <summary>
        /// Sets a TimeSpan that defines how long the item stored by this command will be cached if Redis is being used as a cache
        /// </summary>
        /// <param name="timeSpanUntilExpiration">TimeSpan defining time until expiration</param>
        /// <returns>IRedisStringCommand this</returns>
        public IRedisStringCommand<T> Expires(TimeSpan? timeSpanUntilExpiration)
        {
            _timeSpanUntilExpiration = timeSpanUntilExpiration;
            return this;
        }

        /// <summary>
        /// Sets the urn that is used to identify and retrieve this command's value
        /// </summary>
        /// <param name="urn">urn string</param>
        /// <returns>IRedisStringCommand this</returns>
        public IRedisStringCommand<T> Urn(string urn)
        {
            _urn = urn;
            return this;
        }
    }
}

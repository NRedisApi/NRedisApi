using System;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace NRedisApi.Fluent
{
    public class RedisStringCommand : IRedisStringCommand
    {
        private readonly IDatabase _redis;
        private string _urn;
        private TimeSpan? _timeSpanUntilExpiration;

        internal RedisStringCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
            _timeSpanUntilExpiration = null;
        }

        public IRedisStringCommand<T> As<T>()
        {
            IRedisStringCommand<T> stringCmd = new RedisStringCommand<T>(_redis);
            stringCmd
                .Urn(_urn)
                .Expires(_timeSpanUntilExpiration);
            return stringCmd;
        }

        public IRedisStringCommand Urn(string urn)
        {
            _urn = urn;
            return this;
        }

        public IRedisStringCommand Expires(TimeSpan? timeSpanUntilExpiration)
        {
            _timeSpanUntilExpiration = timeSpanUntilExpiration;
            return this;
        }
    }

    public class RedisStringCommand<T> : IRedisStringCommand<T>
    {
        private readonly IDatabase _redis;
        private string _urn;
        private TimeSpan? _timeSpanUntilExpiration;

        internal RedisStringCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
            _timeSpanUntilExpiration = null;
        }

        public T Get()
        {
            return JsonConvert.DeserializeObject<T>(_redis.StringGet(_urn));
        }

        public void Set(T value)
        {
            var json = JsonConvert.SerializeObject(value);
            if (_timeSpanUntilExpiration == null)
                _redis.StringSet(_urn, json);
            else
                _redis.StringSet(_urn, json, _timeSpanUntilExpiration.Value);
        }

        public void Remove()
        {
            _redis.KeyDelete(_urn);
        }

        public IRedisStringCommand<T> Expires(TimeSpan? timeSpanUntilExpiration)
        {
            _timeSpanUntilExpiration = timeSpanUntilExpiration;
            return this;
        }

        public IRedisStringCommand<T> Urn(string urn)
        {
            _urn = urn;
            return this;
        }
    }
}

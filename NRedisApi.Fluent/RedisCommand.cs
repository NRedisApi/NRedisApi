using System;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace NRedisApi.Fluent
{
    public class RedisCommand : IRedisCommand
    {
        protected string _urn;
        private readonly IDatabase _redis;
        protected TimeSpan? _timeSpanUntilExpiration;

        public RedisCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
            _timeSpanUntilExpiration = null;
        }

        public IRedisCommand Expires(TimeSpan? timeSpanUntilExpiration)
        {
            _timeSpanUntilExpiration = timeSpanUntilExpiration;
            return this;
        }

        public IRedisCommand Urn(string urn)
        {
            _urn = urn;
            return this;
        }

        public IRedisCommand<T> As<T>()
        {
            IRedisCommand<T> cmd = new RedisCommand<T>(_redis);
            cmd
                .Urn(_urn)
                .Expires(_timeSpanUntilExpiration);
            return cmd;
        }

        public IRedisStringCommand RedisString()
        {
            IRedisStringCommand cmd = new RedisStringCommand(_redis);
            cmd
                .Urn(_urn)
                .Expires(_timeSpanUntilExpiration);
            return cmd;
        }
    }

    public class RedisCommand<T> : IRedisCommand<T>
    {
        protected string _urn;
        private readonly IDatabase _redis;
        protected TimeSpan? _timeSpanUntilExpiration;

        internal RedisCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
            _timeSpanUntilExpiration = null;
        }

        public IRedisCommand<T> Expires(TimeSpan? secondsUntilExpiration)
        {
            _timeSpanUntilExpiration = secondsUntilExpiration;
            return this;
        }

        public IRedisCommand<T> Urn(string urn)
        {
            _urn = urn;
            return this;
        }

        public IRedisStringCommand<T> RedisString()
        {
            IRedisStringCommand<T> stringCmd = new RedisStringCommand<T>(_redis);
            stringCmd
                .Urn(_urn)
                .Expires(_timeSpanUntilExpiration);
            return stringCmd;
        }

    }


}
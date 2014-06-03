using StackExchange.Redis;

namespace NRedisApi.Fluent
{
    public class RedisCommand : IRedisCommand
    {
        protected string _urn;
        private readonly IDatabase _redis;
        private long? _secondsUntilExpiration;

        public RedisCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
            _secondsUntilExpiration = null;
        }

        public IRedisCommand Expires(long? secondsUntilExpiration)
        {
            throw new System.NotImplementedException();
        }

        public IRedisCommand Urn(string urn)
        {
            throw new System.NotImplementedException();
        }

        public IRedisCommand<T> As<T>()
        {
            throw new System.NotImplementedException();
        }

        public IStringRedisCommand RedisString()
        {
            throw new System.NotImplementedException();
        }
    }
}
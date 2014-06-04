using StackExchange.Redis;

namespace NRedisApi.Fluent
{
    public class RedisCommand : IRedisCommand
    {
        private readonly IDatabase _redis;
        private string _urn;

        public RedisCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
        }

        public IRedisCommand<T> As<T>()
        {
            IRedisCommand<T> cmd = new RedisCommand<T>(_redis);
            cmd
                .Urn(_urn);
            return cmd;
        }

        public IRedisCommand Urn(string urn)
        {
            _urn = urn;
            return this;
        }

        public IRedisStringCommand RedisString()
        {
            IRedisStringCommand cmd = new RedisStringCommand(_redis);
            cmd
                .Urn(_urn);
            return cmd;
        }

        public IRedisHashCommand RedisHash()
        {
            IRedisHashCommand cmd = new RedisHashCommand(_redis);
            cmd
                .Urn(_urn);
            return cmd;
        }

        public void Dispose()
        {

        }
    }

    public class RedisCommand<T> : IRedisCommand<T> 
    {
        private readonly IDatabase _redis;
        private string _urn;

        internal RedisCommand(IDatabase redis)
        {
            _redis = redis;
            _urn = string.Empty;
        }

        public IRedisStringCommand<T> RedisString()
        {
            IRedisStringCommand<T> stringCmd = new RedisStringCommand<T>(_redis);
            stringCmd
                .Urn(_urn);
            return stringCmd;
        }

        public IRedisHashCommand<T> RedisHash()
        {
            IRedisHashCommand<T> hashCmd = new RedisHashCommand<T>(_redis);
            hashCmd
                .Urn(_urn);
            return hashCmd;
        }

        public IRedisCommand<T> Urn(string urn)
        {
            _urn = urn;
            return this;
        }
    }
}
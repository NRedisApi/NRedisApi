using Newtonsoft.Json;
using StackExchange.Redis;

namespace NRedisApi.Fluent
{
    public class RedisCommand : RedisCommandBase, IRedisCommand
    {

        public RedisCommand(IDatabase redis, JsonSerializerSettings jsonSerializerSettings) : base(redis, jsonSerializerSettings)
        {
        }

        public IRedisCommand<T> AsType<T>()
        {
            return new RedisCommand<T>(Redis, JsonSerializerSettings).SetUrn(Urn);
        }

        public new IRedisCommand SetUrn(string urn)
        {
            base.SetUrn(urn);
            return this;
        }

        public IRedisStringCommand RedisString()
        {
            return new RedisStringCommand(Redis, JsonSerializerSettings).SetUrn(Urn);
        }

        public IRedisHashCommand RedisHash()
        {
            return new RedisHashCommand(Redis, JsonSerializerSettings).SetUrn(Urn);
        }

    }

    public class RedisCommand<T> : RedisCommandBase, IRedisCommand<T> 
    {
        internal RedisCommand(IDatabase redis, JsonSerializerSettings jsonSerializerSettings) : base(redis, jsonSerializerSettings)
        {

        }

        public IRedisStringCommand<T> RedisString()
        {
            return new RedisStringCommand<T>(Redis, JsonSerializerSettings).SetUrn(Urn);
        }

        public IRedisHashCommand<T> RedisHash()
        {
            return new RedisHashCommand<T>(Redis, JsonSerializerSettings).SetUrn(Urn);
        }

        public new IRedisCommand<T> SetUrn(string urn)
        {
            Urn = urn;
            return this;
        }
    }
}
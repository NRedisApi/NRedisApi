using Newtonsoft.Json;
using StackExchange.Redis;

namespace NRedisApi.Fluent
{
    public abstract class RedisCommandBase
    {
        protected readonly JsonSerializerSettings JsonSerializerSettings;
        protected string Urn;
        protected readonly IDatabase Redis;

        protected RedisCommandBase(IDatabase redis, JsonSerializerSettings jsonSerializerSettings)
        {
            Redis = redis;
            JsonSerializerSettings = jsonSerializerSettings;
            JsonSerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
        }

        protected RedisCommandBase(IDatabase redis)
        {
            Redis = redis;
            JsonSerializerSettings = new JsonSerializerSettings{ TypeNameHandling = TypeNameHandling.Objects };
        }

        public virtual void SetUrn(string urn)
        {
            Urn = urn;
        }
    }
}
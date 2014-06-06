using Newtonsoft.Json;
using StackExchange.Redis;
using StructureMap;
using StructureMap.Configuration.DSL;
namespace NRedisApi
{
    public class RedisRegistry : Registry
    {
        public RedisRegistry()
        {
            var config = new ConfigurationOptions
            {
                EndPoints = { { "localhost", 6379 } }
            };

            var jsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };

            ForSingletonOf<ConnectionMultiplexer>().Use(ConnectionMultiplexer.Connect(config));
            For<IRedisConnectionFactory>().Singleton().Use(() => new RedisConnectionFactory(ObjectFactory.GetInstance<ConnectionMultiplexer>(), config, jsonSettings));
        }
    }
}

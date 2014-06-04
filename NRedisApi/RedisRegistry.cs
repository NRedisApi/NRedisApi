using StackExchange.Redis;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace NRedisApi
{
    public class RedisRegistry : Registry
    {
        public RedisRegistry()
        {
            ForSingletonOf<ConnectionMultiplexer>().Use(ConnectionMultiplexer.Connect("localhost"));
            For<IRedisConnectionFactory>().Singleton().Use(() => new RedisConnectionFactory(ObjectFactory.GetInstance<ConnectionMultiplexer>()));
        }
    }
}

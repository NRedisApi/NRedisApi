using NRedisApi.Fluent;
using StackExchange.Redis;

namespace NRedisApi
{
    public interface IRedisConnectionFactory
    {
       // ConfigurationOptions Configuration { get; }
        IRedisCommand GetConnection();
        //void SetConfiguration(ConfigurationOptions configuration);
    }
}
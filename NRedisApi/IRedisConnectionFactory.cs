using NRedisApi.Fluent;

namespace NRedisApi
{
    public interface IRedisConnectionFactory
    {
        IRedisCommand GetConnection();
    }
}
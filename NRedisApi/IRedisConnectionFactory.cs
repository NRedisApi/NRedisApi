namespace NRedisApi
{
    public interface IRedisConnectionFactory
    {
        RedisConnection GetConnection();
    }
}
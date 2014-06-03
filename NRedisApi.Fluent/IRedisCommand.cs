namespace NRedisApi.Fluent
{
    public interface IRedisCommand 
    {
        IRedisStringCommand RedisString();
        IRedisHashCommand RedisHash();
        IRedisCommand<T> As<T>();
        IRedisCommand Urn(string urn);
    }

    public interface IRedisCommand<T> 
    {
        IRedisStringCommand<T> RedisString();
        IRedisHashCommand<T> RedisHash();
        IRedisCommand<T> Urn(string urn);
    }
}

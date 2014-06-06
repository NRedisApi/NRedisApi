namespace NRedisApi.Fluent
{
    public interface IRedisCommand 
    {
        IRedisStringCommand RedisString();
        IRedisHashCommand RedisHash();
        IRedisCommand<T> AsType<T>();
        IRedisCommand SetUrn(string urn);
    }

    public interface IRedisCommand<T> 
    {
        IRedisStringCommand<T> RedisString();
        IRedisHashCommand<T> RedisHash();
        IRedisCommand<T> SetUrn(string urn);
    }
}

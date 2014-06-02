namespace NRedisApi.Fluent
{
    public interface IRedisStringOperations
    {
        void Remove();
    }

    public interface IRedisCommand
    {
        IRedisCommand Expires(long seconds);
        IRedisCommand Urn(string urn);
        IRedisCommand<T> As<T>();
        IStringRedisCommand RedisString();
    }

    public interface IStringRedisCommand : IRedisCommand, IRedisStringOperations
    {
    }

    public interface IRedisCommand<T>
    {
        IRedisCommand<T> Expires(long seconds);
        IRedisCommand<T> Urn(string urn);
        IStringRedisCommand<T> RedisString();
        T Get();
        void Set();
    }

    public interface IStringRedisCommand<T> : IRedisCommand<T>, IRedisStringOperations
    {
    }
}

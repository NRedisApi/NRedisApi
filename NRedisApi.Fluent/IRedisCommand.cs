namespace NRedisApi.Fluent
{
    public interface IRedisStringOperations
    {
        void Remove();
    }

    public interface IRedisCommand
    {
        IRedisCommand Expires(long? secondsUntilExpiration);
        IRedisCommand Urn(string urn);
        IRedisCommand<T> As<T>();
        IStringRedisCommand RedisString();
    }

    public interface IStringRedisCommand : IRedisCommand, IRedisStringOperations
    {
        new IStringRedisCommand<T> As<T>();
    }

    public interface IRedisCommand<T>
    {
        IRedisCommand<T> Expires(long? secondsUntilExpiration);
        IRedisCommand<T> Urn(string urn);
        IStringRedisCommand<T> RedisString();
        T Get();
        void Set(T value);
    }

    public interface IStringRedisCommand<T> : IRedisCommand<T>, IRedisStringOperations
    {
    }
}

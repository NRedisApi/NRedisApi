using System;

namespace NRedisApi.Fluent
{
    public interface IRedisStringOperations<T>
    {
        T Get();
        void Set(T value);
        void Remove();
    }

    public interface IRedisCommand
    {
        IRedisCommand Expires(TimeSpan? secondsUntilExpiration);
        IRedisCommand Urn(string urn);
        IRedisCommand<T> As<T>();
        IRedisStringCommand RedisString();
    }

    public interface IRedisStringCommand : IRedisCommand
    {
        new IRedisStringCommand<T> As<T>();
    }

    public interface IRedisCommand<T>
    {
        IRedisCommand<T> Expires(TimeSpan? secondsUntilExpiration);
        IRedisCommand<T> Urn(string urn);
        IRedisStringCommand<T> RedisString();
    }

    public interface IRedisStringCommand<T> : IRedisCommand<T>, IRedisStringOperations<T>
    {
    }


}

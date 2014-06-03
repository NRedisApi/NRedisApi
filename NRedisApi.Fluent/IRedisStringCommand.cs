using System;

namespace NRedisApi.Fluent
{
    public interface IRedisStringCommand
    {
        IRedisStringCommand Expires(TimeSpan? secondsUntilExpiration);
        IRedisStringCommand Urn(string urn);
        IRedisStringCommand<T> As<T>();
    }

    public interface IRedisStringCommand<T>
    {
        T Get();
        void Set(T value);
        void Remove();
        IRedisStringCommand<T> Expires(TimeSpan? secondsUntilExpiration);
        IRedisStringCommand<T> Urn(string urn);
    }
}
using System;
using Newtonsoft.Json;

namespace NRedisApi.Fluent
{
    public interface IRedisStringCommand
    {
        IRedisStringCommand SetExpires(TimeSpan? secondsUntilExpiration);
        IRedisStringCommand SetUrn(string urn);
        IRedisStringCommand<T> AsType<T>();
    }

    public interface IRedisStringCommand<T>
    {
        T Get();
        void Set(T value);
        void Remove();
        IRedisStringCommand<T> SetExpires(TimeSpan? secondsUntilExpiration);
        IRedisStringCommand<T> SetUrn(string urn);
    }
}
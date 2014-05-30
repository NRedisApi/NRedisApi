using System.Collections.Generic;

namespace NRedisApi.Fluent
{
    public interface IRedisOperation
    {
        RedisOperation Urn(string urn);
        RedisOperation Hash();
        RedisOperation String();
        RedisOperation List();
        RedisOperation Set();
        RedisOperation SortedSet();
        RedisOperation<T> AsType<T>();
    }

    public interface IRedisOperation<T> : IRedisOperation
    {
        IEnumerable<T> GetCollection();
        T Get();
        void Store(T value);
    }
}